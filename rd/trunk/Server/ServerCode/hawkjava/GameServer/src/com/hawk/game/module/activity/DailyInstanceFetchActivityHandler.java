package com.hawk.game.module.activity;

import java.io.IOException;
import java.util.Map;

import org.hawk.os.HawkException;
import org.hawk.util.HawkHttpParams;

import com.google.gson.JsonObject;
import com.hawk.game.GsApp;
import com.hawk.game.ServerData;
import com.hawk.game.module.activity.http.ActivityHttpServer;
import com.hawk.game.player.Player;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class DailyInstanceFetchActivityHandler implements HttpHandler{
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {	
		try {
			Map<String, String> params = HawkHttpParams.parseHttpParam(httpExchange);
			int status = -1;
			JsonObject result = new JsonObject();
			
			do {
 				if (!params.containsKey("pid") || !params.containsKey("sid")) {
					break;
				}
				
				int playerId = ServerData.getInstance().getPlayerIdByPuid(params.get("pid"));
				if (playerId == 0) {
					break;
				}
				
				Player player = GsApp.getInstance().queryPlayer(playerId);
				if (player == null || player.getPlayerData() == null || player.getPlayerData().getStatisticsEntity() == null) {
					break;
				}
				
				status = 0;
				//result.addProperty("count", player.getPlayerData().getStatisticsEntity().getInstanceAllCountDaily());
				
			} while (false);

			result.addProperty("status", status);
			ActivityHttpServer.response(httpExchange, result.toString());
		} catch (Exception e) {
			HawkException.catchException(e);
			ActivityHttpServer.response(httpExchange, null);
		}
	}
}
