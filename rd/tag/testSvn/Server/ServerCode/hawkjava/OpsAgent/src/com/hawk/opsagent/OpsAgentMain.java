package com.hawk.opsagent;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

/**
 * 
 * @author hawk
 */
public class OpsAgentMain {
	/**
	 * 主函数
	 * 
	 * @param args
	 */
	public static void main(String[] args) {
		try {
			// 打印系统信息
			HawkOSOperator.printOsEnv();

			// 创建并初始化服务
			if (OpsAgentServices.getInstance().init(System.getProperty("user.dir") + "/cfg/config.xml")) {
				// 退出构造装载
				ShutDownHook.install();
				// 启动服务器
				OpsAgentServices.getInstance().run();

			} else {
				HawkLog.errPrintln("OpsAgent Init Failed.");
			}

			// 退出
			HawkLog.logPrintln("OpsAgent Exit.");
			System.exit(0);

		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
