package org.hawk.util;

import java.io.ByteArrayOutputStream;
import java.io.InputStream;
import java.net.URLDecoder;
import java.util.HashMap;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.sun.net.httpserver.HttpExchange;

/**
 * Http params
 * 
 * @author walker
 */
public class HawkHttpParams {

	/**
	 * 解析http请求的参数
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
					uriQuery = URLDecoder.decode(uriQuery, "UTF-8");
				} finally {
					in.close();
				}
			}
			else
			{
				uriQuery = httpExchange.getRequestURI().getQuery();
			}
			
			if (uriQuery != null && uriQuery.length() > 0) {
				HawkLog.logPrintln("UriQuery: " + uriPath + "?" + uriQuery);

				if (uriQuery != null) {
					String[] querys = uriQuery.split("&");
					for (String query : querys) {
						// param maybe empty string, use -1
						String[] pair = query.split("=", -1);
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
}
