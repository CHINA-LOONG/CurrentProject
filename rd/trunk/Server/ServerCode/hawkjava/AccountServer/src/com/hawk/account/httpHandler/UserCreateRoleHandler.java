package com.hawk.account.httpHandler;

import java.io.IOException;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.util.HawkHttpParams;

import com.hawk.account.AccountServices;
import com.hawk.account.db.DBManager;
import com.hawk.account.gameserver.GameServer;
import com.hawk.account.http.AccountHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class UserCreateRoleHandler implements HttpHandler{
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
			GameServer gameServer = AccountServices.getInstance().getGameServer(Integer.valueOf(params.get("server")));
			if (gameServer != null) {
				String value = String.format("\"%s\", \"%s\", \"%s\", %d, \"%s\", \"%s\", %d, \"%s\", %d", 
						params.get("game"), params.get("platform"), params.get("channel"), Integer.valueOf(params.get("server")), gameServer.getName(),
						params.get("puid"), Integer.valueOf(params.get("playerid")), params.get("nickname"), 1);

				HawkLog.logPrintln("report_createRole: " + value);

				// 插入
				String sql  = String.format("INSERT INTO role(game, platform, channel, server, serverName, puid, playerid, nickname, level) VALUES(%s);", value);
				HawkLog.logPrintln("report_createRole: " + sql);
				DBManager.getInstance().executeSql("account", sql);
			}

		}
	}
}
