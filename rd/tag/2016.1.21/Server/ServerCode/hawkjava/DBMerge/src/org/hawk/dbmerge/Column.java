package org.hawk.dbmerge;

/**
 * 抽象的列
 * 
 * @author xulinqs
 * 
 */
public class Column {
	/**
	 * 特殊列类型
	 * 
	 * @author xulinqs
	 */
	public static enum SpecialColType {
		NONE, 
		ARRAY, 
		JSON
	}

	/**
	 * 指向的table
	 */
	public Table table;
	/**
	 * 名字
	 */
	public String name;
	/**
	 * 依赖的列
	 */
	public Column dependentColumn;
	/**
	 * 类型
	 */
	public String colType;
	/**
	 * 分隔符
	 */
	public String split;

	/**
	 * 判定是否依赖表
	 * 
	 * @param tableName
	 * @return
	 */
	public boolean isDependOn(String tableName) {
		if (dependentColumn == null) {
			return false;
		}
		return dependentColumn.isDependOn(tableName) || dependentColumn.table.name.equals(tableName);
	}
}
