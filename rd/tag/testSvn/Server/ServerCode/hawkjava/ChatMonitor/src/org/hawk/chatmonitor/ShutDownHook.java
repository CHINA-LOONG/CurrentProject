package org.hawk.chatmonitor;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

/**
 * 关闭�?��钩子
 * 
 * @author hawk
 */
public class ShutDownHook {
	/**
	 * 安装钩子
	 */
	public static void install() {
		if (HawkOSOperator.isWindowsOS()) {
			Thread thread = new Thread(new Thread() {
				public void run() {
					try {
						System.in.read();
						HawkLog.logPrintln("ChatMonitor Kill Shutdown.");
						ChatMonitorServices.getInstance().breakLoop();
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
				HawkLog.logPrintln("ChatMonitor Kill Shutdown.");
				ChatMonitorServices.getInstance().breakLoop();
			}
		});
	}
}
