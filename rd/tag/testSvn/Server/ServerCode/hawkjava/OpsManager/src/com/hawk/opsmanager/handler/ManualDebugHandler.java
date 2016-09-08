package com.hawk.opsmanager.handler;

import java.io.BufferedReader;
import java.io.InputStreamReader;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

public class ManualDebugHandler extends Thread {
	@Override
	public void run() {
		BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
		// Shell操作
		do {
			try {
				System.out.print("\r\n>");
				String line = br.readLine().trim();
				if (line.equals("exit")) {
					break;
				}
				// 执行命令
				String result = OpsCommandHandler.onCommand(line);
				HawkLog.logPrintln(result);
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		} while (true);
	}
}
