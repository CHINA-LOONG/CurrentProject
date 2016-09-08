package com.hawk.account.httpHandler;

import java.io.IOException;
import java.sql.ResultSet;
import java.sql.Statement;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.util.HawkHttpParams;

import com.google.gson.JsonObject;
import com.hawk.account.AccountServices;
import com.hawk.account.db.DBManager;
import com.hawk.account.http.AccountHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class FetchGameServerInfoHandler implements HttpHandler{

	private static class RoleServerInfo{
		public int server;
		public String nickname;
		public int level;

		public RoleServerInfo(int server, int level, String nickname){
			this.server = server;
			this.nickname = nickname;
			this.level = level;
		}

		@Override
		public String toString() {
			return super.toString() + " " + server + " "  + nickname + " "  + level;
		}
	}

	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		try {
			HawkLog.logPrintln("fetch Account gameserver");
			Map<String, String> params = HawkHttpParams.parseHttpParam(httpExchange);

			JsonObject serverListInfo = new JsonObject();
			serverListInfo.add("servers", AccountServices.getInstance().getServerInfo());

			List<RoleServerInfo> roleServerInfos = fetchGameServer(params.get("puid"));
			if (roleServerInfos != null) {
				JsonObject rolesInfo = new JsonObject();
				for (RoleServerInfo server : roleServerInfos) {
					JsonObject roleInfo = new JsonObject();
					roleInfo.addProperty("nickname", server.nickname);
					roleInfo.addProperty("level", server.level);
					rolesInfo.add(String.valueOf(server.server), roleInfo);
				}
				serverListInfo.add("roles", rolesInfo);
			}

			HawkLog.logPrintln("fetch Account gameserver response: " + serverListInfo.toString());
			AccountHttpServer.response(httpExchange, serverListInfo.toString());
		} catch (Exception e) {
			HawkException.catchException(e);
			AccountHttpServer.response(httpExchange, null);
		}
	}

	public static List<RoleServerInfo> fetchGameServer(String puid) {
		if (puid == null || "".equals(puid) || "null".equals("puid")) {
			return null;
		}

		Statement statement = null;
		try {
			String sql = String.format("select server, level, nickname from role where puid=\"%s\"", puid);
			HawkLog.logPrintln(sql);
			statement = DBManager.getInstance().createStatement("account");
			ResultSet resultSet = statement.executeQuery(sql);
			List<RoleServerInfo> roleServerInfos = new LinkedList<>();
			while (resultSet.next()) {
				RoleServerInfo roleServerInfo = new RoleServerInfo(resultSet.getInt(1), resultSet.getInt(2), resultSet.getString(3));
				roleServerInfos.add(roleServerInfo);
			}

			return roleServerInfos;
		} catch (Exception e) {
			HawkException.catchException(e);
		} finally {
			try {
				if (statement != null) {
					statement.close();
				}
			} catch (Exception e) {
			}
		}
		return null;
	}
}
