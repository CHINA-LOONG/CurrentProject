package com.hawk.version.handler;

import java.io.IOException;
import java.util.Map;

import net.sf.json.JSONObject;

import com.hawk.version.VersionServices;
import com.hawk.version.http.VersionHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class VersionControlHandler implements HttpHandler {

	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		int status = VersionHttpServer.VERSION_STATUS_OK;	
		Map<String, String> params = VersionHttpServer.parseHttpParam(httpExchange);
		JSONObject jsonObject = new JSONObject();		
		jsonObject.put("versionCode", String.valueOf(VersionServices.getInstance().getVersionCode()));
		jsonObject.put("status", String.valueOf(status));		
		VersionHttpServer.response(httpExchange, jsonObject);
	}
}
