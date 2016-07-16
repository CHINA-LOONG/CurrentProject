package com.hawk.collector.handler.report;

import java.io.IOException;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.hawk.collector.Collector;
import com.hawk.collector.CollectorServices;
import com.hawk.collector.http.CollectorHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class ReportAccountServerHandler implements HttpHandler{

	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		try {
			Map<String, String> params = CollectorHttpServer.parseHttpParam(httpExchange);
			Collector.checkToken(params.get("token"));
			String remoteIp = httpExchange.getRemoteAddress().getAddress().getHostAddress();
			doReport(params, remoteIp);
		} catch (Exception e) {
			HawkException.catchException(e);
		} finally {
			CollectorHttpServer.response(httpExchange, null);
		}
	}
	
	public static void doReport(Map<String, String> params, String remoteIp) throws Exception {
		if (params != null) {
			if (params.containsKey("ip") && ((String)params.get("ip")).length() > 0) {
				remoteIp = (String)params.get("ip");
			}
			
			String value = String.format("'%s', '%s', '%s', '%s', '%s', %s, %s, '%s', '%s'", 
					params.get("game"), params.get("platform"), params.get("channel"), params.get("server"), 
					remoteIp, params.get("zmq_port"), params.get("http_port"), 
					params.get("dburl"), params.get("dbuser"), params.get("dbpwd"));
			
			HawkLog.logPrintln("report_server: " + value);
			
			// 添加账号服务器
			CollectorServices.getInstance().addAccountServer(new CollectorServices.AccountServer(params.get("game"), params.get("server"), params.get("platform"), 
															params.get("channel"), params.get("ip"), Integer.valueOf(params.get("http_port")),Integer.valueOf(params.get("zmq_port"))));
		}
	}
}
