package com.hawk.shellagent.handler;

import java.io.IOException;
import java.util.Map;

import org.hawk.cryption.HawkBase64;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.script.HawkScriptManager;
import org.hawk.shell.HawkShellExecutor;

import com.hawk.shellagent.AgentMain;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class ExecuteShellHandler implements HttpHandler {

	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		Map<String, String> httpParams = HawkOSOperator.parseHttpParam(httpExchange);
		AgentMain.checkToken(httpParams.get("token"));

		String user = httpParams.get("user");
		String params = httpParams.get("params");
		if (AgentMain.getUser().length() > 0 && (user == null || !user.equals(AgentMain.getUser()))) {
			HawkScriptManager.sendResponse(httpExchange, "user is forbidden: " + user);
			httpExchange.close();
			return;
		}

		String result = onShellCommand(HawkScriptManager.paramsToMap(params));
		if (result != null) {
			HawkScriptManager.sendResponse(httpExchange, result);
		}
	}

	/**
	 * 执行shell命令
	 * 
	 * @param paramsMap
	 * @return
	 */
	public static String onShellCommand(Map<String, String> paramsMap) {
		if (paramsMap != null && paramsMap.containsKey("cmd")) {
			long timeout = -1;
			try {
				String urlCmd = paramsMap.get("cmd").replace('_', '/').replace('-', '=');
				String cmd = new String(HawkBase64.decode(urlCmd));
				if (paramsMap.containsKey("timeout")) {
					timeout = Integer.valueOf(paramsMap.get("timeout"));
				}
				
				if (cmd != null && cmd.length() > 0) {
					String result = HawkShellExecutor.execute(cmd, timeout);
					HawkLog.logPrintln("shell command: " + cmd + "\r\n" + result);
					return result;
				}
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
		return null;
	}
}
