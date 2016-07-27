package com.hawk.version.http;

import java.net.BindException;
import java.net.InetSocketAddress;
import java.util.concurrent.Executors;

import net.sf.json.JSONObject;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.hawk.version.VersionServices;
import com.hawk.version.handler.ResourceControlHandler;
import com.hawk.version.handler.VersionControlHandler;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpServer;

public class VersionHttpServer {
	/**
	 * 前端访问状态码
	 */
	public static final int VERSION_STATUS_OK = 0; // 
	public static final int VERSION_STATUS_VERSION_NONEXIST = 1; // 版本不存在
	public static final int VERSION_STATUS_VERSION_TIMEOUT = 2; // 版本过期
	public static final int VERSION_STATUS_RESOURCE_NONEXIST = 3; // 小版本不存在
	public static final int VERSION_STATUS_PLATFORM_ERROR = 4; // 平台错误
	public static final int VERSION_STATUS_CHANEL_ERROR = 5; // 渠道错误
	public static final int VERSION_STATUS_ERROR = 6; // 服务器错误
	
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
				httpServer.setExecutor(Executors.newFixedThreadPool(pool));
				httpServer.createContext("/version_control", new VersionControlHandler());
				httpServer.createContext("/resource_control", new ResourceControlHandler());
				httpServer.start();
				HawkLog.logPrintln("Version Http Server [" + addr + ":" + port + "] Start OK.");
			}
		} catch (BindException e) {
			HawkException.catchException(e);
			HawkLog.logPrintln("Version Http Server Bind Failed, Address: " + addr + ":" + port);
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
	
	/**
	 * 主循环函数
	 */
	public void run() {
		while (true) {
			try {
				if (!VersionServices.getInstance().tick()) {
					HawkLog.logPrintln("version Service Exit.");
					System.exit(0);
				}

				Thread.sleep(50);

			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
	}
}
