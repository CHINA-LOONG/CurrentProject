package com.hawk.game;

import org.hawk.log.HawkLog;

import com.hawk.game.proxy.ProxyServer;

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
				HawkLog.logPrintln("http2tcp proxy shutdown");
				ProxyServer.getInstance().stop();
			}
		});
	}
}
