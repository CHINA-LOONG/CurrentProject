package com.hawk.account.httpHandler;

import java.io.IOException;
import java.util.Map;

import org.hawk.os.HawkException;
import org.hawk.util.HawkHttpParams;

import com.hawk.account.AccountServices;
import com.hawk.account.http.AccountHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class UnRegistGameServerHandler implements HttpHandler{
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		try {
			Map<String, String> params = HawkHttpParams.parseHttpParam(httpExchange);
			doReport(params);
		} catch (Exception e) {
			HawkException.catchException(e);
		} finally {
			AccountHttpServer.response(httpExchange, null);
		}
	}

	public static void doReport(Map<String, String> params) throws Exception {
		if (params != null) {
			if (params.get("server") != null) {
				AccountServices.getInstance().unregistGameServer(Integer.valueOf(params.get("server")));
			}
		}
	}
}

