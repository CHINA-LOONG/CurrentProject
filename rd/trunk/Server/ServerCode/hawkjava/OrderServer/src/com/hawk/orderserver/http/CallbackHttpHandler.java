package com.hawk.orderserver.http;

import java.io.IOException;
import java.util.Map;

import net.sf.json.JSONObject;

import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.util.HawkHttpParams;
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
			Map<String, String> params = HawkHttpParams.parseHttpParam(httpExchange);
			if (params.containsKey("app_data")) {
				JSONObject appData = JSONObject.fromObject(params.get("app_data")); 
				for (Object element : appData.keySet()) {
					String key = (String) element;
					params.put(key, appData.getString(key));
				}
			}
			
			logger.info(params.toString());
			
			CallbackInfo callbackInfo = new CallbackInfo();
			if (callbackInfo.fromMap(params)) {
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
