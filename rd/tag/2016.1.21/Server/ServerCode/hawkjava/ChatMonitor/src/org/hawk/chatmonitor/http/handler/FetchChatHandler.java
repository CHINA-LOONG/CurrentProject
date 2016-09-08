package org.hawk.chatmonitor.http.handler;

import java.io.IOException;
import java.util.Map;

import net.sf.json.JSONArray;

import org.hawk.chatmonitor.ChatMonitorServices;
import org.hawk.chatmonitor.http.ChatMonitorHttpServer;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

/**
 * 
 * @author hawk
 */
public class FetchChatHandler implements HttpHandler {
	/**
	 * 格式: identify=%s&seconds=%d&count=%d
	 */
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		try {
			Map<String, String> params = HawkOSOperator.parseHttpParam(httpExchange);
			if (params != null && params.containsKey("identify") && params.containsKey("count")) {
				int minChatId = params.containsKey("minChatId") ? Integer.valueOf(params.get("minChatId")) : 0;
				JSONArray chatArray = ChatMonitorServices.getInstance().getServerChatData(params.get("identify"), 
						minChatId, Integer.valueOf(params.get("count")));
				
				if (chatArray == null) {
					chatArray = new JSONArray();
				}
				
				// 响应http请求
				ChatMonitorHttpServer.response(httpExchange, chatArray.toString());
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			
			ChatMonitorHttpServer.response(httpExchange, HawkException.formatStackMsg(e));
		}
	}
}
