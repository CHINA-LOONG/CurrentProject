package com.hawk.version.handler;

import java.io.IOException;
import java.util.List;
import java.util.Map;

import org.hawk.util.HawkHttpParams;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import com.hawk.version.VersionServices;
import com.hawk.version.entity.VersionEntity;
import com.hawk.version.http.VersionHttpServer;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;

public class ResourceControlHandler implements HttpHandler{
	
	@Override
	public void handle(HttpExchange httpExchange) throws IOException {
		int status = VersionHttpServer.VERSION_STATUS_ERROR;
		JSONObject jsonObject = new JSONObject();
		Map<String, String> params = HawkHttpParams.parseHttpParam(httpExchange);
		
		List<VersionEntity> currentVersions = VersionServices.getInstance().getEntityList(Integer.valueOf(params.get("versionCode")));
		if (currentVersions != null) {
			boolean beginAdd = false;
			JSONArray jsonArray = new JSONArray();
			for (VersionEntity element : currentVersions) {
				if (beginAdd == false && element.getId() == Integer.valueOf(params.get("resourceId"))) {
					beginAdd = true;
					continue;
				}
				
				if (beginAdd) {	
					JSONObject versionObject = new JSONObject();
					versionObject.put("resourceId", String.valueOf(element.getId()));
					versionObject.put("resourceName", element.getName());
					jsonArray.add(versionObject);
				}
			}
			
			if (beginAdd == false) {
				status = VersionHttpServer.VERSION_STATUS_RESOURCE_NONEXIST;
			}
			else
			{
				jsonObject.put("resources", jsonArray);	
				jsonObject.put("resourceServer", VersionServices.getInstance().getResourceServerAddress());
				status = VersionHttpServer.VERSION_STATUS_OK;
			}	
		}
		else {
			status = VersionHttpServer.VERSION_STATUS_VERSION_NONEXIST;
		}
		
		jsonObject.put("status", String.valueOf(status));		
		VersionHttpServer.response(httpExchange, jsonObject);
	}
}
