package org.hawk.dbmerge;

import java.sql.ResultSet;
import java.sql.ResultSetMetaData;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Types;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.hawk.db.mysql.HawkMysqlSession;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

/**
 * 抽象的表
 * 
 * @author xulinqs
 * 
 */
public class Table {
	/**
	 * 查询sql定义
	 */
	private static String QUERY_MAXID_SQL = "select max(%s) from %s;";
	private static String QUERY_ID_SQL = "select %s from %s order by %s;";

	public String name = "";

	public String pkName = "";

	public String pkType = "";

	private Column pkColumn;

	private boolean complete;

	public List<Column> needCheckColumns = new LinkedList<Column>();

	private Map<Long, Long> exchangeMap = new HashMap<Long, Long>();

	/**
	 * 字符串比较
	 * 
	 * @param s1
	 * @param s2
	 * @return
	 */
	private boolean equalsIngoreCase(String s1, String s2) {
		return s1.toLowerCase().equals(s2.toLowerCase());
	}

	/**
	 * 转义字符串
	 * 
	 * @param value
	 * @return
	 */
	private String transfer(String value) {
		if (value.indexOf("\\") >= 0) {
			value = value.replace("\\", "\\\\");
		}

		if (value.indexOf("'") >= 0) {
			value = value.replace("'", "\\'");
		}

		if (value.indexOf("\"") >= 0) {
			value = value.replace("\"", "\\\"");
		}
		return value;
	}

	public Column createPKColumn(String table, String column) {
		pkColumn = new Column();
		pkColumn.table = this;
		pkColumn.name = column;
		return pkColumn;
	}

	public Column getPkColumn() {
		return pkColumn;
	}

	public boolean isComplete() {
		return complete;
	}

	public void addDependencyColumn(String colName, Column depCol) {
		for (Column column : needCheckColumns) {
			if (column.name.equals(colName)) {
				return;
			}
		}

		Column column = new Column();
		column.table = this;
		if (colName.indexOf(":") >= 0) {
			String items[] = colName.split(":");
			column.name = items[0];
			String sv[] = items[1].split("_");
			column.colType = sv[0];
			column.split = sv[1];
		} else {
			column.name = colName;
		}
		column.dependentColumn = depCol;
		needCheckColumns.add(column);
	}

	/**
	 * 依赖的表是否已经建立完成
	 * 
	 * @return
	 */
	public boolean checkDependency() {
		if (needCheckColumns.size() == 0) {
			return true;
		}

		boolean dependComplete = true;
		for (Column column : needCheckColumns) {
			if (!column.dependentColumn.table.isComplete()) {
				dependComplete = false;
				break;
			}
		}

		return dependComplete;
	}

	private Column getDepColumn(String colName) {
		for (Column col : this.needCheckColumns) {
			if (col.name.toLowerCase().equals(colName.toLowerCase())) {
				return col.dependentColumn;
			}
		}
		return null;
	}

	/**
	 * 根据老KEY获得新KEY
	 * 
	 * @param old
	 * @return
	 */
	public Long getNewValue(Long old) {
		return exchangeMap.get(old);
	}

	/**
	 * 预先构建主键映射关系
	 * 
	 * @param srcSession
	 * @param dstSession
	 */
	public void genPkRef(HawkMysqlSession srcSession, HawkMysqlSession dstSession) {
		HawkLog.logPrintln("build pk mapping： " + name);
		if (pkType.equals("assign")) {
			return;
		}

		long beginId = 0;
		Statement dstStatement = dstSession.createStatement();
		try {
			ResultSet resultSet = dstStatement.executeQuery(String.format(QUERY_MAXID_SQL, pkName, name));
			if (resultSet.next()) {
				beginId = resultSet.getLong(1);
			}
		} catch (SQLException e) {
			HawkException.catchException(e);
			throw new RuntimeException(e);
		}

		Statement srcStatement = srcSession.createStatement();
		try {
			ResultSet resultSet = srcStatement.executeQuery(String.format(QUERY_ID_SQL, pkName, name, pkName));
			while (resultSet.next()) {
				Long old = resultSet.getLong(1);
				Long newValue = Long.valueOf(++beginId);
				exchangeMap.put(old, newValue);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			throw new RuntimeException(e);
		}

		StringBuilder info = new StringBuilder(4 * 1024 * 1024);
		for (Map.Entry<Long, Long> entry : this.exchangeMap.entrySet()) {
			info.append(entry.getKey()).append(",").append(entry.getValue()).append("|\n");
		}
		MergeMain.writeFile(info.toString(), MergeMain.APP_DIR + "/sql/" + name + ".map");
	}

	public void doMerge(ServerInfo src, ServerInfo dst) {
		String sql = "select %s.* from %s;";
		StringBuilder sb = new StringBuilder(MergeMain.sqlPacketSize);
		sb.append("insert into ").append(name).append(" values ");

		Statement srcStatement = src.getSession().createStatement();
		try {
			ResultSet resultSet = srcStatement.executeQuery(String.format(sql, name, name));
			// 可用于获取关于 ResultSet 对象中列的类型和属性信息的对象
			ResultSetMetaData rmsds = resultSet.getMetaData();
			// 返回此 ResultSet 对象中的列数。
			int count = rmsds.getColumnCount();
			while (resultSet.next()) {
				sb.append("(");
				for (int i = 1; i <= count; i++) {
					String colName = rmsds.getColumnName(i);
					if (equalsIngoreCase(pkName, colName) && !this.pkType.equals("assign")) {
						long oldValue = resultSet.getLong(i);
						if (i != count) {
							sb.append(getNewValue(oldValue)).append(",");
						} else {
							sb.append(getNewValue(oldValue));
						}
					} else {
						int type = rmsds.getColumnType(i);
						if (Types.VARCHAR == type) {
							String old = resultSet.getString(i);
							Column col = getDepColumn(colName);
							if (col != null && col.colType != null) {
								String newStr = "";
								if (col.colType.equals("array")) {
									String ss[] = old.split(col.split);
									for (int j = 0; j < ss.length; j++) {
										newStr += col.dependentColumn.table.getNewValue(Long.valueOf(ss[j]));
										if (j < ss.length - 1) {
											newStr += col.split;
										}
									}
								}
								sb.append("'" + newStr + "'");
							} else {
								if (old != null) {
									sb.append("'" + transfer(old) + "'");
								} else {
									sb.append("'" + old + "'");
								}
							}
						} else if (Types.TIMESTAMP == type) {
							String date = resultSet.getString(i);
							if (date != null) {
								sb.append("'" + date + "'");
							} else {
								sb.append("null");
							}
						} else if (Types.DATE == type) {
							String date = resultSet.getString(i);
							if (date != null) {
								sb.append("'" + date + "'");
							} else {
								sb.append("null");
							}
						} else if (Types.BLOB == type) {
							sb.append("'" + resultSet.getBlob(i) + "'");
						} else if (Types.INTEGER == type) {
							int old = resultSet.getInt(i);
							if (old <= 0) {
								sb.append(old);
							} else {
								Column depCol = getDepColumn(colName);
								if (depCol != null) {
									sb.append(depCol.table.getNewValue(Long.valueOf(old)));
								} else {
									sb.append(old);
								}
							}
						} else if (Types.BIGINT == type) {
							long old = resultSet.getLong(i);
							if (old == 0) {
								sb.append(old);
							} else {
								Column depCol = getDepColumn(colName);
								if (depCol != null) {
									sb.append(depCol.table.getNewValue(old));
								} else {
									sb.append(old);
								}
							}
						} else if (Types.LONGVARBINARY == type) {
							sb.append("'" + new String(resultSet.getBytes(i)) + "'");
						}
						if (i != count) {
							sb.append(",");
						}
					}
				}

				if (sb.length() > (sb.capacity() - 1024)) {
					sb.append(");");
					String insertSql = sb.toString();
					MergeMain.writeFile(insertSql, MergeMain.APP_DIR + "/sql/" + name);
					dst.executeInsert(insertSql, name);
					sb = new StringBuilder(MergeMain.sqlPacketSize);
					sb.append("insert into ").append(name).append(" values ");
				} else {
					sb.append("),");
				}
			}

			// 有自己添加的数据
			if (sb.charAt(sb.length() - 1) == ',') {
				sb.setCharAt(sb.length() - 1, ';');
				String insertSql = sb.toString();
				MergeMain.writeFile(insertSql, MergeMain.APP_DIR + "/sql/" + name);
				dst.executeInsert(insertSql, name);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			throw new RuntimeException(e);
		}
	}
}
