package com.hawk.orderserver.http;

import java.io.IOException;
import java.util.Map;
import java.util.UUID;

import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.client.HttpClient;
import org.apache.http.client.config.RequestConfig;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.utils.URIBuilder;
import org.apache.http.impl.client.HttpClients;
import org.hawk.os.HawkOSOperator;
import org.hawk.util.HawkHttpParams;

public class TestBuyHttpHandler  implements HttpHandler {
	
	private static final String serverQuery = "uid=%s&status=%d&ts=%d&tid=%s&type=%s&app_data=%s";
	
	public void handle(HttpExchange httpExchange) throws IOException {
		Map<String, String> params = HawkHttpParams.parseHttpParam(httpExchange);
		if (params.containsKey("product_id") && params.containsKey("through_cargo") && params.containsKey("puid")) {
			
			String appData = "{\"product_id\": \""+ params.get("product_id") + "\",\"through_cargo\": \""+ params.get("through_cargo") +"\"}";
			String queryParam = String.format(serverQuery, params.get("puid"), 1, 1398134578, UUID.randomUUID(), "googleplayiap", appData);

			RequestConfig requestConfig = RequestConfig.custom()
					.setConnectTimeout(500)
					.setSocketTimeout(500)
					.build();
			HttpClient httpClient = HttpClients.custom()
					.setDefaultRequestConfig(requestConfig)
					.build();

			try {
				URIBuilder uriBuilder = new URIBuilder("http://127.0.0.1:9600");
				uriBuilder.setPath("/callback");
				uriBuilder.setCustomQuery(queryParam);
				HttpGet httpGet  = new HttpGet(uriBuilder.build());
				HttpResponse httpResponse = httpClient.execute(httpGet);
				int status =  httpResponse.getStatusLine().getStatusCode();
				if (status == HttpStatus.SC_OK) {

				}
			} catch (Exception e) {
				
			}
		}
		
		HawkOSOperator.sendHttpResponse(httpExchange, "OK");
	}
}
