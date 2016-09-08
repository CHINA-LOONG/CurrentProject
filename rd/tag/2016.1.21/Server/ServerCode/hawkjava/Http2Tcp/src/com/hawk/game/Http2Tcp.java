package com.hawk.game;

import org.hawk.app.HawkApp;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.xid.HawkXID;

import com.hawk.game.proxy.ProxyServer;

public class Http2Tcp {
	/**
	 * 程序入扣
	 * 
	 * @param args
	 */
	public static void main(String[] args) {
		try {
			// 初始化应用对象
			new HawkApp(HawkXID.valueOf(0)) {
			};

			// 退出构造装载
			ShutDownHook.install();

			// 打印系统信息
			HawkOSOperator.printOsEnv();

			if (!ProxyServer.getInstance().init(System.getProperty("user.dir") + "/cfg/config.xml")) {
				HawkLog.logPrintln("http2tcp proxy init failed");
				return;
			}
			ProxyServer.getInstance().run();

		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
