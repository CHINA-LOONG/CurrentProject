package com.hawk.account;

import org.hawk.log.HawkLog;

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
		Runtime.getRuntime().addShutdownHook(new Thread() {
			public void run() {
				HawkLog.logPrintln("CdkServer Kill Shutdown.");
				AccountServices.getInstance().stop();
			}
		});
	}
}
