package org.hawk.util.services;

import java.io.IOException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.http.HttpResponse;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpUriRequest;
import org.apache.http.concurrent.FutureCallback;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.nio.client.CloseableHttpAsyncClient;
import org.apache.http.impl.nio.client.HttpAsyncClients;
import org.apache.http.impl.nio.conn.PoolingNHttpClientConnectionManager;
import org.apache.http.impl.nio.reactor.DefaultConnectingIOReactor;
import org.apache.http.nio.reactor.ConnectingIOReactor;
import org.apache.http.util.EntityUtils;
import org.hawk.app.HawkApp;
import org.hawk.cryption.HawkBase64;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.util.HawkJsonUtil;
import org.hawk.util.HawkTickable;
import org.hawk.util.HawkURL;

/**
 * 趣加推送Caffeine
 * 
 * @author walker
 */
public class FunPlusPushService extends HawkTickable {
	/**
	 * 异步httpClient
	 */
	private CloseableHttpAsyncClient httpClient;
	private String authentication;
	private String gameId;

	/**
	 * 实例对象
	 */
	private static FunPlusPushService instance = null;
	public static FunPlusPushService getInstance() {
		if (instance == null) {
			instance = new FunPlusPushService();
		}
		return instance;
	}

	public FunPlusPushService() {
	}

	/**
	 * 初始化趣加推送服务
	 */
	public boolean install(String gameId, String gameKey) {
		this.gameId = gameId;

		// 初始化httpAsyncClient
		try {
			ConnectingIOReactor ioReactor = new DefaultConnectingIOReactor();
			PoolingNHttpClientConnectionManager cm = new PoolingNHttpClientConnectionManager(ioReactor);
			cm.setMaxTotal(100);
			httpClient = HttpAsyncClients.custom().setConnectionManager(cm).build();
			httpClient.start();
		} catch (Exception e) {
			HawkException.catchException(e);
		}

		// 计算Basic access authentication
		byte[] digest = new StringBuilder().append(gameId).append(":").append(gameKey).toString().getBytes();
		this.authentication = "Basic ".concat(HawkBase64.encode(digest));

		if (HawkApp.getInstance() != null) {
			HawkApp.getInstance().addTickable(this);
		}
		return true;
	}

	/**
	 * 推送默认格式消息，全部用户
	 */
	public boolean pushSimple(String msg) {
		pushSimple(msg, null);
		return true;
	}

	/**
	 * 推送默认格式消息，部分用户
	 */
	public boolean pushSimple(String msg, List<Integer> funplusIdList) {
		final HttpUriRequest httpRequest = genHttpRequest(msg, funplusIdList);
		final String pushMsg = msg;
		httpClient.execute(httpRequest, new FutureCallback<HttpResponse>() {

			@Override
			public void completed(HttpResponse response) {
				HawkLog.logPrintln(String.format("Funplus push completed : \"%s\" %s", pushMsg, httpRequest.getRequestLine() + "->" + response.getStatusLine()));
				try {
					String responseString = EntityUtils.toString(response.getEntity());
					HawkLog.logPrintln(responseString);
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}

			@Override
			public void failed(Exception e) {
				HawkLog.logPrintln(String.format("Funplus push failed : \"%s\" %s", pushMsg, httpRequest.getRequestLine()));
				HawkException.catchException(e);
			}

			@Override
			public void cancelled() {
				HawkLog.logPrintln(String.format("Funplus push cancelled : \"%s\" %s", pushMsg, httpRequest.getRequestLine()));
			}

		});
		return true;
	}

	/**
	 * 生成推送http请求
	 */
	private HttpUriRequest genHttpRequest(String msg, List<Integer> funplusIdList) {
		Map<String, Object> dataMap = new HashMap<String, Object>();
		dataMap.put(HawkURL.urlEncodeRFC3986("game_id", false), HawkURL.urlEncodeRFC3986(this.gameId, false));
		dataMap.put(HawkURL.urlEncodeRFC3986("message", false), HawkURL.urlEncodeRFC3986(msg, false));
		if (funplusIdList != null) {
			dataMap.put(HawkURL.urlEncodeRFC3986("funplus_ids", false), funplusIdList);
		}

		final HttpPost httpPost = new HttpPost("http://caffeine-api.funplusgame.com/push/to_all");
		httpPost.setHeader("Accept", "application/json;charset=UTF-8");
		httpPost.addHeader("Content-type", "application/json;charset=UTF-8");
		httpPost.addHeader("Authorization", this.authentication);
		try {
			StringEntity jsonEntity = new StringEntity(HawkJsonUtil.getJsonInstance().toJson(dataMap));
			httpPost.setEntity(jsonEntity);
		} catch (Exception e) {
			HawkException.catchException(e);
		}

		return httpPost;
	}

	/**
	 * 帧更新
	 */
	@Override
	public void onTick() {
	}

	/**
	 * 停止服务
	 */
	public void stop() {
		try {
			httpClient.close();
		} catch (IOException e) {
			HawkException.catchException(e);
		}
	}

	@Override
	public void finalize() {
		stop();
	}
}
