package com.hawk.opsmanager.http.handler;

import java.io.IOException;
import java.util.List;
import java.util.Map;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

import com.hawk.opsmanager.OpsServices;
import com.hawk.opsmanager.entity.OpsServerInfo;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

/**
 * @author hawk
 */
public class FetchServerListHandler implements HttpHandler {
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		try {
			JSONArray jsonArray = new JSONArray();
			Map<String, String> params = HawkOSOperator.parseHttpParam(httpExchange);
			if (params.containsKey("game")) {
				List<OpsServerInfo> opsServerInfos = OpsServices.getInstance().getServerInfo(params.get("game"));
				for (OpsServerInfo opsServerInfo : opsServerInfos) {
					JSONObject jsonObject = JSONObject.fromObject(opsServerInfo.getServerInfo().toString());
					jsonArray.add(jsonObject);
				}
			}
			HawkOSOperator.sendHttpResponse(httpExchange, jsonArray.toString());
		} catch (Exception e) {
			HawkException.catchException(e);
			HawkOSOperator.sendHttpResponse(httpExchange, HawkException.formatStackMsg(e));
		}
	}
}
