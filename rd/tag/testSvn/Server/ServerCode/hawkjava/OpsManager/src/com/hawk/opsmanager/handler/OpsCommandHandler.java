package com.hawk.opsmanager.handler;

import net.sf.json.JSONArray;

import org.hawk.cryption.HawkBase64;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.util.HawkCmdParams;

import com.hawk.opsmanager.OpsServices;

public class OpsCommandHandler {
	public static String onCommand(String cmd) {
		JSONArray jsonArray = new JSONArray();
		try {
			HawkCmdParams params = HawkCmdParams.valueOf(cmd);
			if (params != null && params.existParam("game")) {
				// 先处理参数
				if (updateParams(params)) {
					HawkLog.logPrintln(params.toString());
					
					String game = params.getParam("game");
					String platformServerIds = null;
					if (params.existParam("serverids")) {
						platformServerIds = params.getParam("serverids");
					}
					
					long timeout = 300000;
					if (params.existParam("timeout")) {
						timeout = Long.valueOf(params.getParam("timeout"));
					}
					
					jsonArray = OpsServices.getInstance().doRequest(params, game, platformServerIds, timeout);
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return jsonArray.toString();
	}

	private static boolean updateParams(HawkCmdParams params) {
		if (params.cmd.equals("update") && params.existParam("args")) {
			String script = HawkOSOperator.getWorkPath() + "script/" + params.getParam("args");
			if (!script.endsWith(".sh")) {
				script += ".sh";
			}
			
			if (!HawkOSOperator.existFile(script)) {
				HawkLog.logPrintln("Script Not Found: " + script);
				return false;
			}
			
			try {
				String content = HawkOSOperator.readTextFile(script);
				String fileName = params.getParam("args");
				params.getParams().remove("args");
				params.getParams().put("script", fileName);
				params.getParams().put("data", HawkBase64.encode(content.getBytes()));
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
		return true;
	}
}
 