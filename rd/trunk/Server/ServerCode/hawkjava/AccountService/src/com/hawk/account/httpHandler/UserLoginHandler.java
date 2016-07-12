package com.hawk.account.httpHandler;

import java.io.IOException;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import org.hawk.util.services.HawkCdkService;
import com.hawk.account.http.AccountHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class UserLoginHandler implements HttpHandler{

	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		int status = HawkCdkService.CDK_PARAM_ERROR;
		JSONObject jsonObject = new JSONObject();
		List<String> genCdks = new LinkedList<String>();

		

		AccountHttpServer.response(httpExchange, jsonObject);
	}
}
