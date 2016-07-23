package com.hawk.orderserver.http;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.net.URLEncoder;
import java.io.InputStream;
import java.net.URLDecoder;
import java.net.URLEncoder;
import java.util.HashMap;
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

public class TestBuyHttpHandler  implements HttpHandler {
	
	private static final String serverQuery = "uid=%s&status=%d&ts=%d&tid=%s&type=%s&app_data=%s";
	
	public void handle(HttpExchange httpExchange) throws IOException {
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
		
			Map<String, String> param = new HashMap<String, String>();
			String[] kvPairs = uriQuery.split("&");
			for (String kv : kvPairs) {
				// param maybe empty string, use -1
				String[] pair = kv.split("=", -1);
				if (pair.length != 2) {
					continue;
				}
				param.put(pair[0], pair[1]);
			}

			if (param.containsKey("product_id") && param.containsKey("through_cargo") && param.containsKey("puid")) {

				String appData = "{\"product_id\": \""+ param.get("product_id") + "\",\"through_cargo\": \""+ param.get("through_cargo") +"\"}";
				String queryParam = String.format(serverQuery, param.get("puid"), 1, 1398134578, UUID.randomUUID(), "googleplayiap", appData);

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
		}
		
		HawkOSOperator.sendHttpResponse(httpExchange, "OK");
	}
}
