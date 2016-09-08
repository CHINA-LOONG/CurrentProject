package com.hawk.opsmanager.http;

import java.io.OutputStream;
import java.net.BindException;
import java.net.InetSocketAddress;
import java.util.concurrent.Executors;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.hawk.opsmanager.http.handler.FetchServerListHandler;
import com.hawk.opsmanager.http.handler.OpsHttpHandler;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpServer;

/**
 * 
 * @author hawk
 */
public class OpsHttpServer {
	/**
	 * 服务器对象
	 */
	private HttpServer httpServer = null;
	/**
	 * 数据库管理器单例对象
	 */
	static OpsHttpServer instance;

	/**
	 * 获取数据库管理器单例对象
	 * 
	 * @return
	 */
	public static OpsHttpServer getInstance() {
		if (instance == null) {
			instance = new OpsHttpServer();
		}
		return instance;
	}

	/**
	 * 函数
	 */
	private OpsHttpServer() {
	}

	/**
	 * 开启服务
	 */
	public boolean setup(String addr, int port, int pool) {
		try {
			if (addr != null && addr.length() > 0) {
				httpServer = HttpServer.create(new InetSocketAddress(addr, port), 100);
				httpServer.setExecutor(Executors.newFixedThreadPool(pool));

				installContext();

				httpServer.start();
				HawkLog.logPrintln("Ops Http Server [" + addr + ":" + port + "] Start OK.");
				return true;
			}
		} catch (BindException e) {
			HawkException.catchException(e);
			HawkLog.logPrintln("Ops Http Server Bind Failed, Address: " + addr + ":" + port);
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return false;
	}

	/**
	 * 创建http上下文
	 * 
	 * @return
	 */
	private boolean installContext() {
		try {			
			// 运维接口
			httpServer.createContext("/ops", new OpsHttpHandler());
			
			// 信息接口
			httpServer.createContext("/serverlist", new FetchServerListHandler());
			return true;
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return false;
	}

	/**
	 * 停止服务器
	 */
	public void stop() {
		try {
			if (httpServer != null) {
				httpServer.stop(0);
				httpServer = null;
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * http请求回应内容
	 * 
	 * @param httpExchange
	 * @param response
	 */
	public static void response(HttpExchange httpExchange, String response) {
		try {
			OutputStream responseBody = httpExchange.getResponseBody();
			if (response != null && response.length() > 0) {
				byte[] bytes = response.getBytes("UTF-8");
				httpExchange.sendResponseHeaders(200, bytes.length);
				responseBody.write(bytes);
			} else {
				httpExchange.sendResponseHeaders(200, 0);
			}
			responseBody.close();
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
