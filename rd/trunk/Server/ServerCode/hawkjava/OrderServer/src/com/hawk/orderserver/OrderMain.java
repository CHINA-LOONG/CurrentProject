package com.hawk.orderserver;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

/**
 * 支付发货服务器
 * 
 * @author hawk
 */
public class OrderMain {
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
			if (OrderServices.getInstance().init(System.getProperty("user.dir") + "/cfg/config.xml")) {
				// 启动服务器
				OrderServices.getInstance().run();

			} else {
				HawkLog.errPrintln("OrderServer Init Failed.");
			}

			// 退出
			HawkLog.logPrintln("OrderServer Exit.");
			System.exit(0);

		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
