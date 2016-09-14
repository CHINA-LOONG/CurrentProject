package com.hawk.account.http;

import java.io.OutputStream;
import java.net.BindException;
import java.net.InetSocketAddress;
import java.util.concurrent.Executors;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.hawk.account.httpHandler.FetchAllPlayerHandler;
import com.hawk.account.httpHandler.FetchGameServerInfoHandler;
import com.hawk.account.httpHandler.FetchScriptAddressHandler;
import com.hawk.account.httpHandler.HeartBeatHandler;
import com.hawk.account.httpHandler.RegistGameServerHandler;
import com.hawk.account.httpHandler.UnRegistGameServerHandler;
import com.hawk.account.httpHandler.UserCreateRoleHandler;
import com.hawk.account.httpHandler.UserLevelUpHandler;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpServer;

public class AccountHttpServer {

	/**
	 * 前端访问状态码
	 */
	public static final int ACCOUNT_STATUS_OK = 0; // 账号登陆成功
	public static final int ACCOUNT_STATUS_NONEXIST = 1; // 账号不存在
	public static final int ACCOUNT_STATUS_ERROR = 2; // 服务器错误

	/**
	 * 服务器对象
	 */
	private HttpServer httpServer = null;

	/**
	 * 数据库管理器单例对象
	 */
	static AccountHttpServer instance;

	/**
	 * 获取数据库管理器单例对象
	 * 
	 * @return
	 */
	public static AccountHttpServer getInstance() {
		if (instance == null) {
			instance = new AccountHttpServer();
		}
		return instance;
	}

	/**
	 * 开启服务
	 */
	public boolean setup(String addr, int port, int pool) {
		try {
			if (addr != null && addr.length() > 0) {
				httpServer = HttpServer.create(new InetSocketAddress(addr, port), 0);
				// TODO: 暂时不支持多线程(若以后性能不足, 及时修改)
				httpServer.setExecutor(Executors.newFixedThreadPool(1));
				httpServer.createContext("/report_roleCreate", new UserCreateRoleHandler());
				httpServer.createContext("/report_LevelUp", new UserLevelUpHandler());
				httpServer.createContext("/regist_gameserver", new RegistGameServerHandler());
				httpServer.createContext("/unregist_gameserver", new UnRegistGameServerHandler());
				httpServer.createContext("/fetch_gameServer", new FetchGameServerInfoHandler());
				httpServer.createContext("/heartBeat", new HeartBeatHandler());
				httpServer.createContext("/fetchAllPlayer", new FetchAllPlayerHandler());
				httpServer.createContext("/fetchScriptAddr", new FetchScriptAddressHandler());
				httpServer.start();
				HawkLog.logPrintln("Account Http Server [" + addr + ":" + port + "] Start OK.");
			}
		} catch (BindException e) {
			HawkException.catchException(e);
			HawkLog.logPrintln("Account Http Server Bind Failed, Address: " + addr + ":" + port);
			return false;
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		return true;
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
