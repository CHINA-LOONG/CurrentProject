package org.hawk.util.services;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Iterator;
import java.util.Map.Entry;
import java.util.SortedMap;
import java.util.TreeMap;
import java.util.concurrent.CountDownLatch;

import javax.crypto.Mac;
import javax.crypto.SecretKey;
import javax.crypto.spec.SecretKeySpec;

import org.apache.http.HttpResponse;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpUriRequest;
import org.apache.http.concurrent.FutureCallback;
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
import org.hawk.os.HawkTime;
import org.hawk.util.HawkJsonUtil;
import org.hawk.util.HawkTickable;
import org.hawk.util.HawkURL;

import com.google.gson.reflect.TypeToken;

/**
 * 趣加翻译客户端
 * 
 * @author walker
 */
public class FunPlusTranslateService extends HawkTickable {

	public static class Translation {
		// in
		public String sourceText;
		public String sourceLang;
		public String[] targetLangArray;
		public String textType;
		public String profanity;
		// out
		public String[] transTextArray;
	}

	@SuppressWarnings("unused")
	private static class TranslateResponse {
		private static class Data {
			public String source;
			public String target;
			public String sourceText;
			public String targetText;
		}

		public int errorCode;
		public String errorMessage; 
		public Data translation;
	}

	/**
	 * 异步httpClient
	 */
	private CloseableHttpAsyncClient httpClient;
	private String gameId;
	private String gameKey;

	/**
	 * 实例对象
	 */
	private static FunPlusTranslateService instance = null;
	public static FunPlusTranslateService getInstance() {
		if (instance == null) {
			instance = new FunPlusTranslateService();
		}
		return instance;
	}

	private FunPlusTranslateService() {
	}

	/**
	 * 初始化趣加翻译服务
	 */
	public boolean install(String gameId, String gameKey) {
		this.gameId = gameId;
		this.gameKey = gameKey;

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

		if (HawkApp.getInstance() != null) {
			HawkApp.getInstance().addTickable(this);
		}
		return true;
	}

	/**
	 * 翻译对外接口
	 * 
	 *	@return 如果翻译失败，译文设为null
	 */
	public String translate(Translation trans) {
		final CountDownLatch latch = new CountDownLatch(1);
		final String[] transText = {null};

		final HttpUriRequest httpRequest = genHttpRequest(trans.sourceText, trans.sourceLang, trans.targetLangArray[0], trans.profanity, trans.textType);
		final String sourceText = trans.sourceText;
		// 异步请求
		httpClient.execute(httpRequest, new FutureCallback<HttpResponse>() {

			@Override
			public void completed(HttpResponse response) {
				try {
					String responseString = EntityUtils.toString(response.getEntity());
					TranslateResponse transResponse = HawkJsonUtil.getJsonInstance().fromJson(responseString, new TypeToken<TranslateResponse>() {}.getType());
					if (transResponse.errorCode == 0) {
						transText[0] = transResponse.translation.targetText;
					} else {
						HawkLog.errPrintln(String.format("Funplus translate error : %d %s \"%s\" %s", transResponse.errorCode, transResponse.errorMessage, sourceText, httpRequest.getRequestLine()));
					}
				} catch(Exception e) {
					HawkException.catchException(e);
				} finally {
					latch.countDown();
				}
			}

			@Override
			public void failed(Exception e) {
				HawkLog.logPrintln(String.format("Funplus translate failed : \"%s\" %s", sourceText, httpRequest.getRequestLine()));
				HawkException.catchException(e);
				latch.countDown();
			}

			@Override
			public void cancelled() {
				HawkLog.logPrintln(String.format("Funplus translate cancelled : \"%s\" %s", sourceText, httpRequest.getRequestLine()));
				latch.countDown();
			}

		});

		try {
			latch.await();
		} catch(Exception e) {
			HawkException.catchException(e);
		}

		trans.transTextArray = transText;
		return transText[0];
	}

	/**
	 * 批量翻译对外接口
	 * 
	 *	@return 如果翻译失败，译文设为null
	 */
	public void translateBatch(Translation[] transArray) {
		int total = 0;
		for (Translation trans : transArray) {
			total += trans.targetLangArray.length;
		}
		final CountDownLatch latch = new CountDownLatch(total);
		HawkLog.logPrintln(String.format("Funplus translate batch count : %d", total));

		// TODO 多线程可见性是否有问题
		final String[][] transText = new String[transArray.length][];

		for (int i = 0; i < transArray.length; ++i) {
			final int reqIndex = i;
			Translation trans = transArray[reqIndex];
			transText[reqIndex] = new String[trans.targetLangArray.length];

			for (int j = 0; j < trans.targetLangArray.length; ++j) {
				final int langIndex = j;
				final HttpUriRequest httpRequest = genHttpRequest(trans.sourceText, trans.sourceLang, trans.targetLangArray[langIndex], trans.profanity, trans.textType);
				final String sourceText = trans.sourceText;
				// 异步请求
				httpClient.execute(httpRequest, new FutureCallback<HttpResponse>() {

					@Override
					public void completed(HttpResponse response) {
						try {
							String responseString = EntityUtils.toString(response.getEntity());
							TranslateResponse transResponse = HawkJsonUtil.getJsonInstance().fromJson(responseString, new TypeToken<TranslateResponse>() {}.getType());
							if (transResponse.errorCode == 0) {
								transText[reqIndex][langIndex] = transResponse.translation.targetText;
								//HawkLog.logPrintln(String.format("Funplus translate succ : \"%s\"  \"%s\" %s", sourceText, transResponse.translation.targetText, httpRequest.getRequestLine()));
							} else {
								HawkLog.errPrintln(String.format("Funpus translate error : %d %s \"%s\" %s", transResponse.errorCode, transResponse.errorMessage, sourceText, httpRequest.getRequestLine()));
							}
						} catch(Exception e) {
							HawkException.catchException(e);
						} finally {
							latch.countDown();
						}
					}

					@Override
					public void failed(Exception e) {
						HawkLog.logPrintln(String.format("Funplus translate failed : \"%s\" %s", sourceText, httpRequest.getRequestLine()));
						HawkException.catchException(e);
						latch.countDown();
					}

					@Override
					public void cancelled() {
						HawkLog.logPrintln(String.format("Funplus translate cancelled : \"%s\" %s", sourceText, httpRequest.getRequestLine()));
						latch.countDown();
					}

				});
			}
		}

		try {
			latch.await();
		} catch(Exception e) {
			HawkException.catchException(e);
		}

		for (int i = 0; i < transText.length; ++i) {
			transArray[i].transTextArray = transText[i];
		}

		HawkLog.logPrintln(String.format("Funplus translate task complete"));
	}

	/**
	 * 生成翻译http请求
	 */
	private HttpUriRequest genHttpRequest(String q, String source, String target, String profanity, String textType) {
		SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'");
		String timeStamp = sdf.format(HawkTime.getCalendar().getTime());

		// ----------------------------------------------------------------------------------------------
		String queryString = "";
		SortedMap<String, String> paramMap = new TreeMap<String, String>();
		paramMap.put(HawkURL.urlEncodeRFC3986("appId", false), HawkURL.urlEncodeRFC3986(this.gameId, false));
		paramMap.put(HawkURL.urlEncodeRFC3986("q", false), HawkURL.urlEncodeRFC3986(q, false));
		paramMap.put(HawkURL.urlEncodeRFC3986("source", false), HawkURL.urlEncodeRFC3986(source, false));
		paramMap.put(HawkURL.urlEncodeRFC3986("target", false), HawkURL.urlEncodeRFC3986(target, false));
		paramMap.put(HawkURL.urlEncodeRFC3986("timeStamp", false), HawkURL.urlEncodeRFC3986(timeStamp, false));
		paramMap.put(HawkURL.urlEncodeRFC3986("profanity", false), HawkURL.urlEncodeRFC3986(profanity, false));
		paramMap.put(HawkURL.urlEncodeRFC3986("textType", false), HawkURL.urlEncodeRFC3986(textType, false));

		StringBuilder queryBuilder = new StringBuilder();
		Iterator<Entry<String, String>> iterator = paramMap.entrySet().iterator();
		while (iterator.hasNext()) {
			Entry<String, String> pair = iterator.next();
			queryBuilder.append(pair.getKey());
			queryBuilder.append("=");
			queryBuilder.append(pair.getValue());
			if (iterator.hasNext()) {
				queryBuilder.append("&");
			}
		}

		queryString = queryBuilder.toString();

		// -----------------------------------------------------------------------------------------------
		String signString = new StringBuilder()
			.append("GET\n")
			.append("translate.funplusgame.com\n")
			.append("/api/v2/translate\n")
			.append(queryString)
			.toString();

		// -----------------------------------------------------------------------------------------------
		String authentication = "";
		try {
			byte[] keyByte = this.gameKey.getBytes("UTF-8");
			byte[] dataByte = signString.getBytes("UTF-8");
			SecretKey secret = new SecretKeySpec(keyByte, "HMACSHA256");
			Mac mac = Mac.getInstance(secret.getAlgorithm());
			mac.init(secret);
			byte[] digest = mac.doFinal(dataByte);

			authentication = HawkBase64.encode(digest);
		} catch (Exception e) {
				HawkException.catchException(e);
		}

		// --------------------------------------------------------------------------------------------------
		String uri = "http://translate.funplusgame.com/api/v2/translate?" + queryString;
		final HttpGet httpGet = new HttpGet(uri);
		httpGet.addHeader("Accept", "application/json;charset=UTF-8");
		httpGet.addHeader("Authorization", authentication);

		return httpGet;
	}

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
