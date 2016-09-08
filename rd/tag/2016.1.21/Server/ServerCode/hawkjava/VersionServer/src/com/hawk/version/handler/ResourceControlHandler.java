package com.hawk.version.handler;

import java.io.IOException;
import java.util.Map;

import org.hawk.os.HawkException;
import org.hawk.util.HawkHttpParams;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import com.hawk.version.VersionServices;
import com.hawk.version.entity.Version;
import com.hawk.version.http.VersionHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class ResourceControlHandler implements HttpHandler{
	
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		
		try {
			int status = VersionHttpServer.VERSION_STATUS_OK;
			JSONObject jsonObject = new JSONObject();
			Map<String, String> params = HawkHttpParams.parseHttpParam(httpExchange);
			
			do {
				// 平台错误
				if (params.get("channel").equals(VersionServices.getInstance().getChanel()) == false) {
					status = VersionHttpServer.VERSION_STATUS_CHANNEL_ERROR;
					break;
				}
				
				// 小版本更新
				Map<Integer, Version> versions = VersionServices.getInstance().getPlatformVersions(params.get("platform"));
				if (versions == null) {
					status = VersionHttpServer.VERSION_STATUS_PLATFORM_ERROR;
					break;
				}
			
				if (VersionServices.getInstance().getCurrentVersion(params.get("platform")) == null) {
					status = VersionHttpServer.VERSION_STATUS_PLATFORM_ERROR;
					break;
				}
				
				Version version = versions.get(Integer.valueOf(params.get("vid")));
				if (version == null) {
					status = VersionHttpServer.VERSION_STATUS_ERROR;
					break;
				}

				// 更新版本
				if (VersionServices.getInstance().getCurrentVersion(params.get("platform")) > version.getVersion()) {
					status = VersionHttpServer.VERSION_STATUS_VERSION_TIMEOUT;
					break;
				}
				
				boolean beginAdd = false;
				JSONArray jsonArray = new JSONArray();
				for (Version element : versions.values()) {
					if (beginAdd == false && element.getId() == Integer.valueOf(params.get("vid"))) {
						beginAdd = true;
						continue;
					}
					
					if (beginAdd) {	
						JSONObject versionObject = new JSONObject();
						versionObject.put("vid", String.valueOf(element.getId()));
						versionObject.put("resourceName", element.getName());
						versionObject.put("resourceSize", String.valueOf(element.getSize()));
						jsonArray.add(versionObject);
					}
				}
				
				if (beginAdd == false) {
					status = VersionHttpServer.VERSION_STATUS_ERROR;
				}
				else
				{
					jsonObject.put("resources", jsonArray);	
					jsonObject.put("resourceServer", VersionServices.getInstance().getResourceAddress());
				}
				
			} while (false);

			jsonObject.put("status", String.valueOf(status));		
			VersionHttpServer.response(httpExchange, jsonObject);
			
		} catch (Exception e) {
			HawkException.catchException(e);
			VersionHttpServer.response(httpExchange, HawkException.formatStackMsg(e));
		}
		
		
		
		
	}
}
