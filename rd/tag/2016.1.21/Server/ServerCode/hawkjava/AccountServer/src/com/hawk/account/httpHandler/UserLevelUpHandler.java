package com.hawk.account.httpHandler;

import java.io.IOException;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.util.HawkHttpParams;

import com.hawk.account.db.DBManager;
import com.hawk.account.http.AccountHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class UserLevelUpHandler implements HttpHandler{
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
			String value = String.format("\"%s\", \"%s\", \"%s\", \"%s\", \"%s\", %d, %d", 
					params.get("game"), params.get("platform"), params.get("channel"), params.get("server"), 
					params.get("puid"), Integer.valueOf(params.get("playerid")), Integer.valueOf(params.get("level")));

			HawkLog.logPrintln("report_createRole: " + value);

			// 更新
			String sql = String.format("UPDATE role SET level = %d WHERE game = \"%s\" AND channel = \"%s\" AND platform = \"%s\" AND server = %d AND puid = \"%s\" AND playerid = %d", 
					Integer.valueOf(params.get("level")), params.get("game"), params.get("channel"), params.get("platform"), 
					Integer.valueOf(params.get("server")), params.get("puid"), Integer.valueOf(params.get("playerid")));

			HawkLog.logPrintln("report_UpdateLevel: " + sql);
			DBManager.getInstance().executeSql("account", sql);
		}
	}
}
