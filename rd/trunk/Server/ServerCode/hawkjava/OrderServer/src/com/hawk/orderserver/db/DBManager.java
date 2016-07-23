package com.hawk.orderserver.db;

import java.sql.SQLException;
import java.sql.Statement;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

import org.hawk.db.mysql.HawkMysqlSession;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.hawk.orderserver.OrderServices;

/**
 * 数据库管理器
 * 
 * @author hawk
 */
public class DBManager {
	/**
	 * 数据库主机地址
	 */
	private String dbHost;
	/**
	 * 数据库用户名
	 */
	private String dbUser;
	/**
	 * 数据库登陆密码
	 */
	private String dbPwd;
	/**
	 * 连接池大小
	 */
	private int poolSize;
	/**
	 * 数据库会话表
	 */
	Map<String, HawkMysqlSession> dbSessions;

	/**
	 * 数据库管理器单例对象
	 */
	static DBManager instance;

	/**
	 * 获取数据库管理器单例对象
	 * 
	 * @return
	 */
	public static DBManager getInstance() {
		if (instance == null) {
			instance = new DBManager();
		}
		return instance;
	}

	/**
	 * 函数
	 */
	private DBManager() {
		poolSize = 1;
		dbSessions = new ConcurrentHashMap<String, HawkMysqlSession>();
	}

	/**
	 * 初始化数据库连接
	 * 
	 * @param dbHost
	 * @param dbUser
	 * @param dbPwd
	 * @return
	 */
	public boolean init(String dbHost, String dbUser, String dbPwd, int poolSize) {
		this.dbHost = dbHost;
		this.dbUser = dbUser;
		this.dbPwd = dbPwd;
		this.poolSize = Math.max(1, poolSize);
		try {
			// 加载驱动程序
			try {
				Class.forName("com.mysql.jdbc.Driver");
			} catch (ClassNotFoundException e) {
				HawkException.catchException(e);
				return false;
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}
		return true;
	}

	/**
	 * 获取db的url地址
	 * 
	 * @param dbName
	 * @return
	 */
	public String getDbUrl(String dbName) {
		return String.format("jdbc:mysql://%s/%s?useUnicode=true&amp;characterEncoding=UTF-8", dbHost, dbName);
	}

	/**
	 * 获取数据库会话
	 * 
	 * @param dbName
	 * @return
	 */
	public HawkMysqlSession getDbSession(String dbName) {
		return dbSessions.get(dbName);
	}

	/**
	 * 进行连接
	 * 
	 * @return
	 */
	public HawkMysqlSession createDbSession(String dbName) {
		try {
			HawkMysqlSession session = new HawkMysqlSession();
			if (session.init(getDbUrl(dbName), dbUser, dbPwd, poolSize)) {
				dbSessions.put(dbName, session);
				OrderServices.getInstance().addTickable(session);

				HawkLog.logPrintln(String.format("create dbsession success, dbHost: %s, dbUser: %s, dbPwd: %s, poolSize: %d", getDbUrl(dbName), dbUser, dbPwd, poolSize));

				return session;
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}

		HawkLog.logPrintln(String.format("create dbsession failed, dbHost: %s, dbUser: %s, dbPwd: %s, poolSize: %d", getDbUrl(dbName), dbUser, dbPwd, poolSize));
		return null;
	}

	/**
	 * 创建statement
	 * 
	 * @return
	 */
	public Statement createStatement(String dbName) {
		HawkMysqlSession session = getDbSession(dbName);
		if (session != null) {
			return session.createStatement();
		} else {
			HawkLog.errPrintln("database session null: " + dbName);
		}
		return null;
	}

	/**
	 * 直接执行sql语句
	 * 
	 * @param dbName
	 * @param sql
	 * @return 影响的行数
	 * @throws SQLException
	 */
	public int executeSql(String dbName, String sql) {
		HawkMysqlSession session = getDbSession(dbName);
		if (session != null) {
			return session.executeSql(sql);
		} else {
			HawkLog.errPrintln("database session null: " + dbName);
		}
		return 0;
	}
}
