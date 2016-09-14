package com.hawk.account.httpHandler;

import java.io.IOException;
import java.util.List;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.util.HawkHttpParams;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import com.hawk.account.http.AccountHttpServer;
import com.hawk.account.roleServeInfo.RoleServerInfo;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class FetchAllPlayerHandler implements HttpHandler{
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		try {
			HawkLog.logPrintln("fetch Account roles");
			Map<String, String> params = HawkHttpParams.parseHttpParam(httpExchange);

			JsonArray rolesListInfo = new JsonArray();
			
			List<RoleServerInfo> roleServerInfos = FetchGameServerInfoHandler.fetchGameServer(params.get("puid"));
			if (roleServerInfos != null) {
				for (RoleServerInfo server : roleServerInfos) {
					JsonObject roleInfo = new JsonObject();
					roleInfo.addProperty("nickname", server.nickname);
					roleInfo.addProperty("level", server.level);
					roleInfo.addProperty("server", server.server);
					rolesListInfo.add(roleInfo);
				}
			}

			HawkLog.logPrintln("fetch Account roles response: " + rolesListInfo.toString());
			AccountHttpServer.response(httpExchange, rolesListInfo.toString());
		} catch (Exception e) {
			
		}
	}
}
