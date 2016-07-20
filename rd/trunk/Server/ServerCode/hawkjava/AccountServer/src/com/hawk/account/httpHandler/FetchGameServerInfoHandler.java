package com.hawk.account.httpHandler;

import java.io.IOException;
import java.sql.ResultSet;
import java.sql.Statement;
import java.util.HashMap;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import com.hawk.account.AccountServices;
import com.hawk.account.AccountServices.GameServer;
import com.hawk.account.db.DBManager;
import com.hawk.account.http.AccountHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class FetchGameServerInfoHandler implements HttpHandler{
	
	private static class RoleServerInfo{
		public String server;
		public String nickname;
		public int level;
		
		public RoleServerInfo(String server, int level, String nickname){
			this.server = server;
			this.nickname = nickname;
			this.level = level;
		}		
		
		@Override
		public String toString() {
			// TODO Auto-generated method stub
			return super.toString() + " " + server + " "  + nickname + " "  + level;
		}
	}
	
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		try {
			
			HawkLog.logPrintln("fetch Account gameserver");
			Map<String, String> params = AccountHttpServer.parseHttpParam(httpExchange);
			Map<String, RoleServerInfo> roleServerInfos = fetchGameServer(params.get("puid"));			
			
			JsonArray jsonArray = new JsonArray();
			for (Map.Entry<String, GameServer> entry : AccountServices.getInstance().getGameServer(params.get("platform"), params.get("channel")).entrySet()) {
				JsonObject jsonObject = new JsonObject();
				jsonObject.addProperty("server", entry.getValue().server);
				jsonObject.addProperty("hostIp", entry.getValue().hostIp);
				jsonObject.addProperty("port",entry.getValue().port);	
				RoleServerInfo roleServerInfo = roleServerInfos.get(entry.getValue().server);
				if (roleServerInfo != null) {
					JsonObject roleInfo = new JsonObject();
					roleInfo.addProperty("nickname", roleServerInfo.nickname);
					roleInfo.addProperty("level", roleServerInfo.level);
					jsonObject.add("role", roleInfo);
				}				
				jsonArray.add(jsonObject);
			}						
			
			HawkLog.logPrintln("fetch Account gameserver response: " + jsonArray.toString());
			AccountHttpServer.response(httpExchange, jsonArray.toString());
		} catch (Exception e) {
			HawkException.catchException(e);
			AccountHttpServer.response(httpExchange, null);
		}
	}
	
	public static Map<String, RoleServerInfo> fetchGameServer(String puid) {
		Statement statement = null;
		try {
			String sql = String.format("select server, level, nickname from role where puid='%s'", puid);
			HawkLog.logPrintln(sql);
			statement = DBManager.getInstance().createStatement("account");
			ResultSet resultSet = statement.executeQuery(sql);
			Map<String, RoleServerInfo> roleServerInfos = new HashMap<String, RoleServerInfo>();
			while (resultSet.next()) {
				RoleServerInfo roleServerInfo = new RoleServerInfo(
						resultSet.getString(1), 
						resultSet.getInt(2), 
						resultSet.getString(3));
				roleServerInfos.put(roleServerInfo.server, roleServerInfo);
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
