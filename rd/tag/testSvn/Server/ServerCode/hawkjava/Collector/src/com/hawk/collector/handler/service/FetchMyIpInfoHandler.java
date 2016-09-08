package com.hawk.collector.handler.service;

import java.io.IOException;
import java.util.Map;

import org.hawk.os.HawkException;
import org.hawk.util.HawkHttpParams;

import com.google.gson.JsonObject;
import com.hawk.collector.Collector;
import com.hawk.collector.http.CollectorHttpServer;
import com.hawk.collector.zmq.CollectorZmqServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

/**
 * 获取服务器信息
 * 
 * @author hawk
 */
public class FetchMyIpInfoHandler implements HttpHandler {
	/**
	 * 格式: 
	 */
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		try {
			Map<String, String> params = HawkHttpParams.parseHttpParam(httpExchange);
			Collector.checkToken(params.get("token"));
			String remoteIp = httpExchange.getRemoteAddress().getAddress().getHostAddress();
			JsonObject jsonObject = new JsonObject();
			jsonObject.addProperty("myIp", remoteIp);
			jsonObject.addProperty("zmqPort", CollectorZmqServer.getInstance().getPort());
			CollectorHttpServer.response(httpExchange, jsonObject.toString());
		} catch (Exception e) {
			HawkException.catchException(e);
			CollectorHttpServer.response(httpExchange, HawkException.formatStackMsg(e));
		}
	}
}
