package com.hawk.opsagent.handler;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkOSOperator;
import org.hawk.shell.HawkShellExecutor;
import org.hawk.util.HawkCmdParams;
import org.hawk.util.services.helper.HawkOpsServerInfo;

public class ServerCommandHandler {
	public static String onCommand(HawkOpsServerInfo serverInfo, HawkCmdParams params) {
		String script = HawkOSOperator.getWorkPath() + "script/" + params.cmd;
		if (!script.endsWith(".sh")) {
			script += ".sh";
		}
		
		if (!HawkOSOperator.existFile(script)) {
			return "script not found";
		}
		
		String dbHost = "";
		String dbName = "";
		String dbPort = "3306";
		
		String dbUrl = serverInfo.getDbUrl().replace("jdbc:mysql://", "");
		int pos = dbUrl.indexOf("/");
		if (pos > 0) {
			dbHost = dbUrl.substring(0, pos);
			dbName = dbUrl.substring(pos + 1);
			dbPort = "3306";
			pos = dbHost.indexOf(":");
			if (pos > 0) {
				dbPort = dbHost.substring(pos + 1);
				dbHost = dbHost.substring(0, pos);
			}
		}
		
		String cmd = String.format("sh %s %s %s %s %s %s %s %s %s %s", script, 
				serverInfo.getPort(), serverInfo.getScriptPort(), 
				dbHost, dbPort, dbName, serverInfo.getDbUser(), serverInfo.getDbPwd(), 
				serverInfo.getWorkPath(), serverInfo.getPid());
		
		// 携带参数
		if (params.existParam("args")) {
			cmd += " ";
			cmd += params.getParam("args");
		}
		
		long timeout = 0;
		if (params.existParam("timeout")) {
			timeout = Long.valueOf(params.getParam("timeout"));
		}
		
		HawkLog.logPrintln("shell command: " + cmd + "\r\n");
		String result = HawkShellExecutor.execute(cmd, timeout);
		HawkLog.logPrintln("result: " + result + "\r\n");
		return result;
	}
}
