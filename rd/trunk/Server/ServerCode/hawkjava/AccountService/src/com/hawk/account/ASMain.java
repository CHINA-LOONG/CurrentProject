package com.hawk.account;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

public class ASMain {
	public static void main(String[] args) {
		try {
			// ��ӡ��������
			for (int i = 0; i < args.length; i++) {
				HawkLog.logPrintln(args[i]);
			}

			// �˳�
			HawkLog.logPrintln("gameserver exit");
			System.exit(0);

		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
