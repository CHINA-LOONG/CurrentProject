package com.hawk.shellagent;

import org.apache.commons.configuration.XMLConfiguration;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import com.hawk.shellagent.http.AgentHttpServer;

/**
 * 收集服务
 * 
 * @author hawk
 */
public class AgentServices {
	/**
	 * 服务器是否允许中
	 */
	volatile boolean running;
	/**
	 * 单例对象
	 */
	static AgentServices instance;

	/**
	 * 获取实例
	 */
	public static AgentServices getInstance() {
		if (instance == null) {
			instance = new AgentServices();
		}
		return instance;
	}

	/**
	 * 默认构造函数
	 */
	private AgentServices() {
		// 初始化变量
		running = true;
	}

	/**
	 * 是否运行中
	 * 
	 * @return
	 */
	public boolean isRunning() {
		return running;
	}

	/**
	 * 退出服务主循环
	 */
	public void breakLoop() {
		running = false;
	}

	/**
	 * 定时更新
	 */
	private void onTick() {
	}

	/**
	 * 使用配置文件初始化
	 * 
	 * @param cfgFile
	 * @return
	 */
	public boolean init(String cfgFile) {
		boolean initOK = true;
		try {
			// 加载配置文件
			XMLConfiguration conf = new XMLConfiguration(System.getProperty("user.dir") + "/cfg/config.xml");

			// 初始化http服务器
			initOK &= AgentHttpServer.getInstance().setup(conf.getString("httpserver.addr"), conf.getInt("httpserver.port"), conf.getInt("httpserver.pool"));
			if (initOK) {
				HawkLog.logPrintln("Setup HttpServer Success, " + conf.getString("httpserver.addr") + ":" + conf.getInt("httpserver.port"));
			} else {
				HawkLog.logPrintln("Setup HttpServer Failed, " + conf.getString("httpserver.addr") + ":" + conf.getInt("httpserver.port"));
			}

			// 设置校验码
			if (conf.containsKey("httpserver.token")) {
				AgentMain.setToken(conf.getString("httpserver.token"));
			}
			
			// 设置用户
			if (conf.containsKey("httpserver.user")) {
				AgentMain.setUser(conf.getString("httpserver.user"));
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			initOK = false;
		}

		return initOK;
	}

	/**
	 * 运行服务器
	 */
	public void run() {
		HawkLog.logPrintln("MainLoop Running OK.");
		while (running) {
			try {
				onTick();

				HawkOSOperator.sleep();
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}

		AgentHttpServer.getInstance().stop();
	}
}
