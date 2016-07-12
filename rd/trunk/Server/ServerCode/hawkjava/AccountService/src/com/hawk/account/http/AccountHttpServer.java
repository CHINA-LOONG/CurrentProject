package com.hawk.account.http;

import java.net.BindException;
import java.net.InetSocketAddress;
import java.util.concurrent.Executors;

import net.sf.json.JSONObject;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.hawk.account.httpHandler.UserLoginHandler;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpServer;

public class AccountHttpServer {
	
	/**
	 * 账号服务参数串格式
	 */
	private static final String login_account	 = "/login?token=%s"; 
	
	/**
	 * 服务器对象
	 */
	private HttpServer httpServer = null;

	/**
	 * 开启服务
	 */
	public void setup(String addr, int port, int pool) {
		try {
			if (addr != null && addr.length() > 0) {
				httpServer = HttpServer.create(new InetSocketAddress(addr, port), 0);				
				// TODO: 暂时不支持多线程(若以后性能不足, 及时修改)
				httpServer.setExecutor(Executors.newFixedThreadPool(1));

				httpServer.createContext("/login_account", new UserLoginHandler());

				httpServer.start();
				HawkLog.logPrintln("Cdk Http Server [" + addr + ":" + port + "] Start OK.");
			}
		} catch (BindException e) {
			HawkException.catchException(e);
			HawkLog.logPrintln("Cdk Http Server Bind Failed, Address: " + addr + ":" + port);
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * http请求回应内容
	 * 
	 * @param httpExchange
	 * @param jsonObject
	 */
	public static void response(HttpExchange httpExchange, JSONObject jsonObject) {
		response(httpExchange, jsonObject.toString());
	}

	/**
	 * http请求回应内容
	 * 
	 * @param httpExchange
	 * @param response
	 */
	public static void response(HttpExchange httpExchange, String response) {
		try {
			if (response != null && response.length() > 0) {
				final byte[] bytes = response.getBytes("UTF-8");
				httpExchange.sendResponseHeaders(200, bytes.length);
				httpExchange.getResponseBody().write(bytes);
				httpExchange.getResponseBody().close();
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
	
}
