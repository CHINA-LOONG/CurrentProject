package com.hawk.opsagent.handler;

import org.hawk.cryption.HawkBase64;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkOSOperator;
import org.hawk.shell.HawkShellExecutor;
import org.hawk.util.HawkCmdParams;

public class SystemCommandHandler {
	public static String executeShellCommand(String cmd, long timeout) {
		String result = HawkShellExecutor.execute("sh -c " + cmd, timeout);
		HawkLog.logPrintln("shell command: " + cmd + "\r\n" + result + "\r\n");
		return result;
	}
	
	public static String onCommand(HawkCmdParams params) {
		// 系统shell命令执行
		if (params.cmd.equals("sh")) {
			long timeout = 0;
			if (params.existParam("timeout")) {
				timeout = Long.valueOf(params.getParam("timeout"));
			}
			
			// 执行shell命令
			if (params.existParam("cmd")) {
				return executeShellCommand(params.getParam("cmd"), timeout);
			} 
			
			// 执行脚本文件
			if (params.existParam("script")) {
				String script = HawkOSOperator.getWorkPath() + "script/" + params.getParam("script");
				if (!script.endsWith(".sh")) {
					script += ".sh";
				}
				
				if (HawkOSOperator.existFile(script)) {
					String cmd = String.format("sh '%s'", script);
					String result = HawkShellExecutor.execute(cmd, timeout);
					HawkLog.logPrintln("shell command: " + cmd + "\r\n" + result + "\r\n");
					return result;
				}
			}
		} 

		// 更新本地shell脚本
		if (params.cmd.equals("update") && params.existParam("script") && params.existParam("data")) {
			String script = HawkOSOperator.getWorkPath() + "script/" + params.getParam("script");
			if (!script.endsWith(".sh")) {
				script += ".sh";
			}
			HawkOSOperator.makeSureFileName(script);
			String data = params.getParam("data");
			String content = new String(HawkBase64.decode(data));
			if (params.existParam("nonbase64")) {
				content = data;
			}
			HawkLog.logPrintln("update script: " + params.getParam("script"));
			HawkLog.logPrintln(content);
			
			// 保存脚本文件
			if (!HawkOSOperator.saveAsFile(content, script)) {
				return "failed";
			}
			// 修改格式
			String cmd = String.format("dos2unix '%s'", script);
			HawkShellExecutor.execute(cmd, 0);
			return "success";
		}
		return null;
	}
}
