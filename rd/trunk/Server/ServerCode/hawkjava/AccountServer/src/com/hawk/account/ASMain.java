package com.hawk.account;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

public class ASMain {
	public static void main(String[] args) {
		try {
			// 参数列表
			for (int i = 0; i < args.length; i++) {
				HawkLog.logPrintln(args[i]);
			}

			// 创建并初始化服务
			if (AccountServices.getInstance().init(System.getProperty("user.dir") + "/cfg/config.xml")) {
				// 启动服务器
				AccountServices.getInstance().run();
			} else {
				HawkLog.errPrintln("Account Server Init Failed.");
			}
			
			// 退出
			HawkLog.logPrintln("account server exit");
			System.exit(0);

		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
