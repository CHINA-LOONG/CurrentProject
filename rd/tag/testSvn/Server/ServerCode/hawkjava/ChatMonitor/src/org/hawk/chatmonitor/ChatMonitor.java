package org.hawk.chatmonitor;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

/**
 * 数据收集服务器
 * 
 * @author hawk
 */
public class ChatMonitor {
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
			if (ChatMonitorServices.getInstance().init(System.getProperty("user.dir") + "/cfg/config.xml")) {
				// 启动服务器
				ChatMonitorServices.getInstance().run();

			} else {
				HawkLog.errPrintln("ChatMonitor Init Failed.");
			}

			// 退出
			HawkLog.logPrintln("ChatMonitor Exit.");
			System.exit(0);

		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
