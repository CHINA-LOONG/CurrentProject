package org.hawk.chatmonitor.http;

import java.io.OutputStream;
import java.net.BindException;
import java.net.InetSocketAddress;
import java.util.concurrent.Executors;

import org.hawk.chatmonitor.http.handler.FetchChatHandler;
import org.hawk.chatmonitor.http.handler.PushChatHandler;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpServer;

/**
 * 收集的HTTP服务
 * 
 * @author hawk
 */
public class ChatMonitorHttpServer {
	/**
	 * 服务器对象
	 */
	private HttpServer httpServer = null;
	/**
	 * 数据库管理器单例对象
	 */
	static ChatMonitorHttpServer instance;

	/**
	 * 获取数据库管理器单例对象
	 * 
	 * @return
	 */
	public static ChatMonitorHttpServer getInstance() {
		if (instance == null) {
			instance = new ChatMonitorHttpServer();
		}
		return instance;
	}

	/**
	 * 函数
	 */
	private ChatMonitorHttpServer() {
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
				HawkLog.logPrintln("ChatMonitor Http Server [" + addr + ":" + port + "] Start OK.");
				return true;
			}
		} catch (BindException e) {
			HawkException.catchException(e);
			HawkLog.logPrintln("ChatMonitor Http Server Bind Failed, Address: " + addr + ":" + port);
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
			httpServer.createContext("/fetch_chat", new FetchChatHandler());
			httpServer.createContext("/push_chat", new PushChatHandler());
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
