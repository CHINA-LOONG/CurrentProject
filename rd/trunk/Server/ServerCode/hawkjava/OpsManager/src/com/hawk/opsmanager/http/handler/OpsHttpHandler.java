package com.hawk.opsmanager.http.handler;

import java.io.IOException;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

import com.hawk.opsmanager.handler.OpsCommandHandler;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

/**
 * @author hawk
 */
public class OpsHttpHandler implements HttpHandler {
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		try {
			String contentBody = "";
			Map<String, String> params = HawkOSOperator.parseHttpParam(httpExchange);
			if (params.containsKey("params")) {
				contentBody = params.get("params");
			} else {
				contentBody = HawkOSOperator.readRequestBody(httpExchange);
			}
			
			HawkLog.logPrintln("HttpRequest: " + contentBody);
			// 直接调用命令
			String result = OpsCommandHandler.onCommand(contentBody);
			HawkOSOperator.sendHttpResponse(httpExchange, result);
		} catch (Exception e) {
			HawkException.catchException(e);
			HawkOSOperator.sendHttpResponse(httpExchange, HawkException.formatStackMsg(e));
		}
	}
}
