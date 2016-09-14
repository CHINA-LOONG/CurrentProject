package com.hawk.account.httpHandler;

import java.io.IOException;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.util.HawkHttpParams;

import com.google.gson.JsonObject;
import com.hawk.account.AccountServices;
import com.hawk.account.gameserver.GameServer;
import com.hawk.account.http.AccountHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class FetchScriptAddressHandler implements HttpHandler{
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		JsonObject scriptAddr = new JsonObject();
		try {
			Map<String, String> params = HawkHttpParams.parseHttpParam(httpExchange);
			if (params.containsKey("serverId") && AccountServices.getInstance().getGameServer(Integer.valueOf(params.get("serverId"))) != null) {		
				GameServer gameServer = AccountServices.getInstance().getGameServer(Integer.valueOf(params.get("serverId")));		
				scriptAddr.addProperty("scriptAddr", String.format("%s:%d/GM", gameServer.getHostIp(), gameServer.getScriptPort()));
				
				HawkLog.logPrintln("fetch script addr response: " + scriptAddr.toString());
			}	
		} catch (Exception e) {
			HawkException.catchException(e);
		} finally{
			AccountHttpServer.response(httpExchange, scriptAddr.toString());
		}
	}
}
