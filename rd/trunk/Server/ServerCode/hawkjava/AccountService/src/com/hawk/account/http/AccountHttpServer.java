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
	 * �˺ŷ����������ʽ
	 */
	private static final String login_account	 = "/login?token=%s"; 
	
	/**
	 * ����������
	 */
	private HttpServer httpServer = null;

	/**
	 * ��������
	 */
	public void setup(String addr, int port, int pool) {
		try {
			if (addr != null && addr.length() > 0) {
				httpServer = HttpServer.create(new InetSocketAddress(addr, port), 0);				
				// TODO: ��ʱ��֧�ֶ��߳�(���Ժ����ܲ���, ��ʱ�޸�)
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
	 * http�����Ӧ����
	 * 
	 * @param httpExchange
	 * @param jsonObject
	 */
	public static void response(HttpExchange httpExchange, JSONObject jsonObject) {
		response(httpExchange, jsonObject.toString());
	}

	/**
	 * http�����Ӧ����
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
