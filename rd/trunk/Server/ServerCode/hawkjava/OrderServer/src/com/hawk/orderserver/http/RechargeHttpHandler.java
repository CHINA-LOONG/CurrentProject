package com.hawk.orderserver.http;

import java.io.IOException;
import java.sql.ResultSet;
import java.sql.Statement;
import java.util.Map;
import java.util.UUID;
import java.util.Map.Entry;

import net.sf.json.JSONObject;

import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

import com.hawk.orderserver.db.DBManager;
import com.hawk.orderserver.service.OrderManager;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

/**
 * @author hawk
 */
public class RechargeHttpHandler implements HttpHandler {
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		String result = "failed";
		try {
			Map<String, String> params = HawkOSOperator.parseHttpParam(httpExchange);
			
			if (!params.containsKey("suuid")) {
				try {
					String sql = String.format("SELECT suuid FROM orders WHERE game = \"%s\" AND platform = \"%s\" AND serverid = %s ORDER BY id DESC LIMIT 1",
							params.get("game"), params.get("platform"), params.get("serverId"));
					
					Statement statement = DBManager.getInstance().createStatement(OrderManager.getInstance().getOrderDatabase());
					ResultSet resultSet = statement.executeQuery(sql);
					if (resultSet.next()) {
						params.put("suuid", resultSet.getString(1));
					}
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
			
			if (!params.containsKey("puid")) {
				try {
					String sql = String.format("SELECT puid FROM orders WHERE game = \"%s\" AND platform = \"%s\" AND serverid = %s AND playerid = %s ORDER BY id DESC LIMIT 1",
							params.get("game"), params.get("platform"), params.get("serverId"), params.get("playerId"));
					
					Statement statement = DBManager.getInstance().createStatement(OrderManager.getInstance().getOrderDatabase());
					ResultSet resultSet = statement.executeQuery(sql);
					if (resultSet.next()) {
						params.put("puid", resultSet.getString(1));
					} else {
						params.put("puid", UUID.randomUUID().toString());
					}
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
			
			if (!params.containsKey("device")) {
				try {
					String sql = String.format("SELECT device FROM orders WHERE game = \"%s\" AND platform = \"%s\" AND serverid = %s AND playerid = %s ORDER BY id DESC LIMIT 1",
							params.get("game"), params.get("platform"), params.get("serverId"), params.get("playerId"));
					
					Statement statement = DBManager.getInstance().createStatement(OrderManager.getInstance().getOrderDatabase());
					ResultSet resultSet = statement.executeQuery(sql);
					if (resultSet.next()) {
						params.put("device", resultSet.getString(1));
					} else {
						params.put("device", UUID.randomUUID().toString());
					}
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
			
			if (!params.containsKey("channel")) {
				int pos = params.get("puid").indexOf("_");
				if (pos > 0) {
					params.put("channel", params.get("puid").substring(0, pos).toLowerCase());
				}
			}
			
			if (!params.containsKey("goodsCount")) {
				params.put("goodsCount", "1");
			}
			
			if (!params.containsKey("currency")) {
				params.put("currency", "rmb");
			}
			
			JSONObject jsonObject = new JSONObject();
			for (Entry<String, String> entry : params.entrySet()) {
				jsonObject.put(entry.getKey(), entry.getValue());
			}

			//OrderInfo orderInfo = OrderManager.getInstance().generateOrder(jsonObject);
			//if (orderInfo != null) {
		//		result = orderInfo.toJson().toString();
		//	}
		} catch (Exception e) {
			HawkException.catchException(e);
		} finally {
			HawkOSOperator.sendHttpResponse(httpExchange, result);
		}
	}
}
