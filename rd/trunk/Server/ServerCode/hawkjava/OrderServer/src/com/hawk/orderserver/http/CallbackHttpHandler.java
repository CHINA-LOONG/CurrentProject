package com.hawk.orderserver.http;

import java.io.IOException;

import net.sf.json.JSONObject;

import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.google.gson.JsonObject;
import com.hawk.orderserver.entify.CallbackInfo;
import com.hawk.orderserver.service.OrderManager;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

/**
 * @author hawk
 */
public class CallbackHttpHandler implements HttpHandler {
	/**
	 * 订单发货发货日志对象
	 */
	static Logger logger = LoggerFactory.getLogger("PayDeliver");
	
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		JsonObject retStatus = new JsonObject();
		retStatus.addProperty("status", -1);
		try {
			String uriQuery = httpExchange.getRequestURI().getQuery();
			logger.info("Callback: " + uriQuery);
			
			JsonObject jsonObject = new JsonObject();
			String[] params = uriQuery.split("&");
			for (String item : params) {
				// param maybe empty string, use -1
				String[] pair = item.split("=", -1);
				if (pair.length == 2) {
					if (pair[0].equals("app_data")) {
						JSONObject appData = JSONObject.fromObject(pair[1]); 
						for (Object element : appData.keySet()) {
							String key = (String) element;
							jsonObject.addProperty(key, appData.getString(key));
						}
					}
					else
					{
						jsonObject.addProperty(pair[0], pair[1]);
					}
				}
			}

			CallbackInfo callbackInfo = new CallbackInfo();
			if (callbackInfo.fromJson(jsonObject)) {
				OrderManager.getInstance().addCallbackInfo(callbackInfo);
				retStatus.addProperty("status", 0);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		} finally {
			HawkOSOperator.sendHttpResponse(httpExchange, retStatus.toString());
		}
	}
}
