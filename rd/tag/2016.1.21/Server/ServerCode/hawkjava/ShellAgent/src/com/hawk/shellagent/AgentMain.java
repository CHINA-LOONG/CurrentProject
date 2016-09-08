package com.hawk.shellagent;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

public class AgentMain {
	/**
	 * 系统token校验
	 */
	private static String httpToken = "";
	/**
	 * 系统用户
	 */
	private static String httpUser = "";
	
	public static void setToken(String token) {
		httpToken = token.trim();
	}
	
	public static void setUser(String user) {
		httpUser = user.trim();
	}
	
	public static String getUser() {
		return httpUser;
	}
	
	public static boolean checkToken(String token) {
		if (httpToken != null && httpToken.length() > 0) {
			if (token == null || !token.equals(httpToken)) {
				throw new RuntimeException("http token check failed.");
			}
		}
		return true;
	}
	
	/**
	 * 主函数
	 * 
	 * @param args
	 */
	public static void main(String[] args) {
		try {
			// 退出构造装载
			ShutDownHook.install();

			// 打印系统信息
			HawkOSOperator.printOsEnv();
			
			// 创建并初始化服务
			if (AgentServices.getInstance().init(System.getProperty("user.dir") + "/cfg/config.xml")) {
				// 启动服务器
				AgentServices.getInstance().run();

			} else {
				HawkLog.errPrintln("Agent Init Failed.");
			}

			// 退出
			HawkLog.logPrintln("Agent Exit.");
			System.exit(0);

		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
