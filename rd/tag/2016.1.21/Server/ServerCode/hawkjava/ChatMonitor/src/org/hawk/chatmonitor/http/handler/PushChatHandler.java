package org.hawk.chatmonitor.http.handler;

import java.io.IOException;
import java.util.Map;

import org.hawk.chatmonitor.http.ChatMonitorHttpServer;
import org.hawk.chatmonitor.zmq.ChatMonitorZmqServer;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

/**
 * 
 * @author hawk
 */
public class PushChatHandler implements HttpHandler {
	/**
	 * 格式: identify=%s&msg=%s
	 */
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		try {
			Map<String, String> params = HawkOSOperator.parseHttpParam(httpExchange);
			if (params != null && params.containsKey("identify") && params.containsKey("msg")) {
				ChatMonitorZmqServer.getInstance().notifyServerInfo(params.get("identify"), params.get("msg"));
				
				ChatMonitorHttpServer.response(httpExchange, "ok");
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
