package org.hawk.dbmerge;

import org.hawk.db.mysql.HawkMysqlSession;
import org.hawk.log.HawkLog;
import org.hawk.thread.HawkTask;

/**
 * 合服之前 单服的服务器信息
 * 
 * @author xulinqs
 * 
 */
public class ServerInfo {
	/**
	 * 服务器编号
	 */
	private int serverId;
	/**
	 * 数据库链接地址
	 */
	private String dbUrl;
	/**
	 * 数据库用户名
	 */
	private String dbUserName;
	/**
	 * 数据库密码
	 */
	private String dbPassword;
	/**
	 * 数据库会话
	 */
	private HawkMysqlSession session;

	public ServerInfo(int serverId, String dbUrl, String dbUserName, String dbPassword) {
		this.serverId = serverId;
		this.dbUrl = dbUrl;
		this.dbUserName = dbUserName;
		this.dbPassword = dbPassword;
	}

	public boolean connect() {
		session = new HawkMysqlSession();
		return session.init(dbUrl, dbUserName, dbPassword, 1);
	}

	public int getServerId() {
		return serverId;
	}

	public HawkMysqlSession getSession() {
		return session;
	}

	@Override
	public String toString() {
		return String.format("serverId : %d, dbUrl : %s, dbUserName : %s, dbPassWord : %s", serverId, dbUrl, dbUserName, dbPassword);
	}

	public void executeSql(final String sql) {
		session.executeSql(sql);
	}

	public void executeInsert(final String insertSql, final String tableName) {
		MergeMain.threadPool.addTask(new HawkTask(true) {
			@Override
			protected int run() {
				HawkLog.logPrintln("sql quence begin , table name : " + tableName);
				if (session.executeSql(insertSql) <= 0) {
					MergeMain.writeFile(insertSql, MergeMain.APP_DIR + "/sql/" + tableName + ".exception");
				}
				HawkLog.logPrintln("sql quence complete , table name : " + tableName);
				return 0;
			}
		}, tableName.hashCode() % MergeMain.threadPool.getThreadNum(), false);
	}
}
