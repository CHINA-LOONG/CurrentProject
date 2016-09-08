package com.hawk.version;

import org.hawk.log.HawkLog;

import com.hawk.version.VersionServices;;
/**
 * 关闭退出钩子
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
				VersionServices.getInstance().stop();
			}
		});
	}
}
