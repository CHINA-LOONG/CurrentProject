package com.hawk.collector.handler.service;

import java.io.IOException;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.google.gson.JsonObject;
import com.hawk.collector.Collector;
import com.hawk.collector.CollectorServices;
import com.hawk.collector.CollectorServices.AccountServer;
import com.hawk.collector.http.CollectorHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class FetchAccountServerInfoHandler implements HttpHandler {
	/**
	 * 格式: 
	 */
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		try {
			HawkLog.logPrintln("fetch Collector accountserver");
			
			Map<String, String> params = CollectorHttpServer.parseHttpParam(httpExchange);
			Collector.checkToken(params.get("token"));
			AccountServer accountServer = CollectorServices.getInstance().GetAccountServer(params.get("game"), params.get("platform"), params.get("channel"), params.get("server"));
			if (accountServer != null) {
				JsonObject jsonObject = new JsonObject();
				jsonObject.addProperty("hostIp", accountServer.ipAddr);
				if (params.get("server") == null) {
					jsonObject.addProperty("httpPort", accountServer.httpPort);
				}
				else
				{
					jsonObject.addProperty("zmqPort", accountServer.zmqPort);
				}
				HawkLog.logPrintln(jsonObject.toString());
				HawkLog.logPrintln("fetch Collector accountserver :" + jsonObject.toString());
				CollectorHttpServer.response(httpExchange, jsonObject.toString());
			}
			else {
				CollectorHttpServer.response(httpExchange, "");
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			CollectorHttpServer.response(httpExchange, HawkException.formatStackMsg(e));
		}
	}
}