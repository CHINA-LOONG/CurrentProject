package com.hawk.version;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.KVResource(file = "cfg/vs.cfg")
public class VsConfig extends HawkConfigBase{
	/**
	 * 是否允许控制台打印
	 */
	protected final boolean console;
	/**
	 * 调试模式
	 */
	protected final boolean isDebug;
	/**
	 * 网络接收器端口
	 */
	protected final int acceptorPort;
	/**
	 * 数据库连接hbm配置
	 */
	protected final String dbHbmXml;
	/**
	 * 数据库连接地址
	 */
	protected final String dbConnUrl;
	/**
	 * 数据库连接用户名
	 */
	protected final String dbUserName;
	/**
	 * 数据库连接密码
	 */
	protected final String dbPassWord;
	/**
	 * 数据库实体包路径
	 */
	protected final String entityPackages;

	public VsConfig() {
		console = true;
		isDebug = true;
		acceptorPort = 0;
		dbHbmXml = null;
		dbConnUrl = null;
		dbUserName = null;
		dbPassWord = null;
		entityPackages = null;
	}

	public boolean isConsole() {
		return console;
	}

	public boolean isDebug() {
		return isDebug;
	}

	public int getAcceptorPort() {
		return acceptorPort;
	}

	public String getDbHbmXml() {
		return dbHbmXml;
	}

	public String getDbConnUrl() {
		return dbConnUrl;
	}

	public String getDbUserName() {
		return dbUserName;
	}

	public String getDbPassWord() {
		return dbPassWord;
	}

	public String getEntityPackages() {
		return entityPackages;
	}
	
}
