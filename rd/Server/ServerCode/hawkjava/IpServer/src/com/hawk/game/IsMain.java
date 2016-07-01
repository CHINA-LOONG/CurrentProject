package com.hawk.game;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

/**
 * gameserver入口
 * 
 * @author hawk
 * 
 */
public class IsMain {
	public static void main(String[] args) {
		try {
			// 打印启动参数
			for (int i = 0; i < args.length; i++) {
				HawkLog.logPrintln(args[i]);
			}

			// 创建应用
			IsApp app = new IsApp();
			if (app.init("cfg/gs.cfg")) {
				app.run();
			}
			
			// 退出
			HawkLog.logPrintln("ipserver exit");
			System.exit(0);

		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
