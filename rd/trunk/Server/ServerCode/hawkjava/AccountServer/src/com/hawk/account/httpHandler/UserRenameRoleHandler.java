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

public class UserRenameRoleHandler implements HttpHandler{
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
				String value = String.format("\"%s\", \"%s\", \"%s\", %d, \"%s\", \"%s\", %d, \"%s\"", 
						params.get("game"), params.get("platform"), params.get("channel"), Integer.valueOf(params.get("server")), gameServer.getName(),
						params.get("puid"), Integer.valueOf(params.get("playerid")), params.get("nickname"));

				HawkLog.logPrintln("report_renameRole: " + value);

				// 更新
				String sql = String.format("UPDATE role SET nickname = %s WHERE game = \"%s\" AND channel = \"%s\" AND platform = \"%s\" AND server = %d AND puid = \"%s\" AND playerid = %d", 
						params.get("nickname"), params.get("game"), params.get("channel"), params.get("platform"), 
						Integer.valueOf(params.get("server")), params.get("puid"), Integer.valueOf(params.get("playerid")));

				HawkLog.logPrintln("report_renameRole: " + sql);
				DBManager.getInstance().executeSql("account", sql);
			}
		}
	}
}
