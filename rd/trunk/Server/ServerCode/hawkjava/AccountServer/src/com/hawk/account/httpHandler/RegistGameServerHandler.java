package com.hawk.account.httpHandler;

import java.io.IOException;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.hawk.account.AccountServices;
import com.hawk.account.http.AccountHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class RegistGameServerHandler implements HttpHandler{
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		try {
			Map<String, String> params = AccountHttpServer.parseHttpParam(httpExchange);
			doReport(params);
		} catch (Exception e) {
			HawkException.catchException(e);
		} finally {
			AccountHttpServer.response(httpExchange, null);
		}
	}
	
	public static void doReport(Map<String, String> params) throws Exception {
		if (params != null) {
			if (params.get("server") != null && params.get("ip") != null && params.get("port") != null) {
				AccountServices.getInstance().addGameServer(new AccountServices.GameServer(params.get("server"), params.get("ip"), Integer.valueOf(params.get("port"))));
			}
		}
	}
}
