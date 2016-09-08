package com.hawk.opsagent;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

/**
 * @author hawk
 */
public class ShutDownHook {
	/**
	 * 安装钩子
	 */
	public static void install() {
		if (HawkOSOperator.isWindowsOS() && !OpsAgentServices.getInstance().isDebugMode()) {
			Thread thread = new Thread(new Thread() {
				public void run() {
					try {
						System.in.read();
						HawkLog.logPrintln("OpsAgent Kill Shutdown.");
						OpsAgentServices.getInstance().breakLoop();
					} catch (Exception e) {
						HawkException.catchException(e);
					}
				}
			});
			thread.setName("WinConsole");
			thread.start();
		}

		// 添加关闭钩子
		Runtime.getRuntime().addShutdownHook(new Thread() {
			public void run() {
				HawkLog.logPrintln("OpsAgent Kill Shutdown.");
				OpsAgentServices.getInstance().breakLoop();
			}
		});
	}
}
