package com.hawk.account.http;

import java.io.ByteArrayOutputStream;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.BindException;
import java.net.InetSocketAddress;
import java.net.URLDecoder;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.Executors;

import net.sf.json.JSONObject;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.hawk.account.httpHandler.FetchGameServerInfoHandler;
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
	 * 解析http请求的参数
	 * 
	 * @param uriQuery
	 * @return
	 */
	public static Map<String, String> parseHttpParam(HttpExchange httpExchange) {
		Map<String, String> paramMap = new HashMap<String, String>();
		try {
			String uriPath = httpExchange.getRequestURI().getPath();			
			String uriQuery = null;

			if (httpExchange.getRequestMethod().toLowerCase().equals("post")) {
				InputStream in = httpExchange.getRequestBody();
				try {
				    ByteArrayOutputStream out = new ByteArrayOutputStream();
				    byte buf[] = new byte[4096];
				    for (int n = in.read(buf); n > 0; n = in.read(buf)) {
				        out.write(buf, 0, n);
				    }
				    uriQuery = new String(out.toByteArray(), "UTF-8");
				} finally {
				    in.close();
				}
			}
			else
			{
				uriQuery = httpExchange.getRequestURI().getQuery();
			}
			if (uriQuery != null && uriQuery.length() > 0) {
				uriQuery = URLDecoder.decode(uriQuery, "UTF-8");
				HawkLog.logPrintln("UriQuery: " + uriPath + "?" + uriQuery);

				if (uriQuery != null) {
					String[] querys = uriQuery.split("&");
					for (String query : querys) {
						String[] pair = query.split("=");
						if (pair.length == 2) {
							paramMap.put(pair[0], pair[1]);
						}
					}
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return paramMap;

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
