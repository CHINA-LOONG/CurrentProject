package com.hawk.orderserver.http;

import java.io.IOException;
import java.util.Map;

import net.sf.json.JSONObject;

import org.hawk.cryption.HawkMd5;
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
		retStatus.addProperty("status", "ERROR");
		retStatus.addProperty("reason", "REASON");
		
		logger.info("有回调");
		
		
		try {
			logger.info("11111");
			Map<String, String> params = HawkHttpParams.parseHttpParam(httpExchange);
			logger.info("22222");
			logger.info(params.toString());
			// 验证
			String token_all_Content = params.get("app_data")
									 + params.get("appid")
									 + params.get("currency")
									 + params.get("rmoney")
									 + params.get("status")
									 + params.get("test_mode")
									 + params.get("tid")
									 + params.get("ts")
									 + params.get("type")
									 + params.get("uid")
									 + params.get("user_age_min")
									 + params.get("user_country")
									 + params.get("user_locale")
									 + params.get("vcurrency")
									 + params.get("vcurrency_key")
									 + "cunf-)&^#9#)0=-wj3up52hf!#&&sc+r-0suz+*_73m_uwx669";
			
			String token_all = HawkMd5.makeMD5(token_all_Content);
			if (token_all.equals(params.get("token_all"))) {
				logger.info("验证通过");
			}
			else {
				logger.info("验证未通过");
			}
			
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
				if (OrderManager.getInstance().addCallbackInfo(callbackInfo)) {
					retStatus.addProperty("status", "OK");
					retStatus.addProperty("reason", "no reason");
				}
				else {
					retStatus.addProperty("status", "ERROR");
					retStatus.addProperty("reason", "repeat");
				}
			}
		} catch (Exception e) {
			logger.info(e.getMessage());
			HawkException.catchException(e);
		} finally {
			HawkOSOperator.sendHttpResponse(httpExchange, retStatus.toString());
		}
	}
}
