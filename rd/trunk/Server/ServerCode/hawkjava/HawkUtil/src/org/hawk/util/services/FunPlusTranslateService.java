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

import org.apache.commons.codec.binary.Base64;
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

	public static final String FUNPLUS_APP_ID = "1013";
	public static final String FUNPLUS_KEY = "aacbb2be28236338a3cb61d610a76f9e";

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
	CloseableHttpAsyncClient httpClient;

	private static FunPlusTranslateService instance = null;
	public static FunPlusTranslateService getInstance() {
		if (instance == null) {
			instance = new FunPlusTranslateService();
		}
		return instance;
	}

	private FunPlusTranslateService() {
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
	}

	/**
	 * 生成翻译http请求
	 */
	public HttpUriRequest genHttpRequest(String q, String source, String target, String profanity, String textType) {
		SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'");
		String timeStamp = sdf.format(HawkTime.getCalendar().getTime());
		// ----------------------------------------------------------------------------------------------
		String queryString = "";
		SortedMap<String, String> paramMap = new TreeMap<String, String>();
		paramMap.put(HawkURL.urlEncodeRFC3986("appId", false), HawkURL.urlEncodeRFC3986(FUNPLUS_APP_ID, false));
		paramMap.put(HawkURL.urlEncodeRFC3986("q", false), HawkURL.urlEncodeRFC3986(q, false));
		paramMap.put(HawkURL.urlEncodeRFC3986("source", false), HawkURL.urlEncodeRFC3986(source, false));
		paramMap.put(HawkURL.urlEncodeRFC3986("target", false), HawkURL.urlEncodeRFC3986(target, false));
		paramMap.put(HawkURL.urlEncodeRFC3986("timeStamp", false), HawkURL.urlEncodeRFC3986(timeStamp, false));
		paramMap.put(HawkURL.urlEncodeRFC3986("profanity", false), HawkURL.urlEncodeRFC3986(profanity, false));
		paramMap.put(HawkURL.urlEncodeRFC3986("textType", false), HawkURL.urlEncodeRFC3986(textType, false));

		Iterator<Entry<String, String>> iterator = paramMap.entrySet().iterator();
		while (iterator.hasNext()) {
			Entry<String, String> pair = iterator.next();
			queryString += pair.getKey();
			queryString += "=";
			queryString += pair.getValue();
			if (iterator.hasNext()) {
				queryString += "&";
			}
		}

		// -----------------------------------------------------------------------------------------------
		String signString =
			"GET\n" +
			"translate.funplusgame.com\n" +
			"/api/v2/translate\n" +
			queryString;

		// -----------------------------------------------------------------------------------------------
		String authentication = "";
		try {
			byte[] keyByte = FUNPLUS_KEY.getBytes("UTF-8");
			byte[] dataByte = signString.getBytes("UTF-8");
			SecretKey secret = new SecretKeySpec(keyByte, "HMACSHA256");
			Mac mac = Mac.getInstance(secret.getAlgorithm());
			mac.init(secret);
			byte[] digest = mac.doFinal(dataByte);

			Base64 base64 = new Base64();
			authentication = base64.encodeAsString(digest);
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

	/**
	 * 翻译对外接口
	 * 
	 *	@return 如果翻译失败，译文设为null
	 */
	public String translate(Translation trans) {
		final CountDownLatch latch = new CountDownLatch(1);
		final String[] transText = {null};

		final HttpUriRequest httpRequest = genHttpRequest(trans.sourceText, trans.sourceLang, trans.targetLangArray[0], trans.profanity, trans.textType);
		// for log
		final String sourceText = trans.sourceText;
//		HawkLog.logPrintln(String.format("FunPlus translate send : \"%s\" %s", sourceText, httpRequest.getRequestLine()));
		// 异步请求
		httpClient.execute(httpRequest, new FutureCallback<HttpResponse>() {

			@Override
			public void completed(HttpResponse response) {
//				HawkLog.logPrintln(String.format("FunPlus translate complete : \"%s\" %s", sourceText, httpRequest.getRequestLine()));
				try {
					String responseString = EntityUtils.toString(response.getEntity());

					TranslateResponse transResponse = HawkJsonUtil.getJsonInstance().fromJson(responseString, new TypeToken<TranslateResponse>() {}.getType());
					if (transResponse.errorCode == 0) {
						transText[0] = transResponse.translation.targetText;
					} else {
						HawkLog.errPrintln(String.format("FunPlus translate error : %d %s \"%s\" %s", transResponse.errorCode, transResponse.errorMessage, sourceText, httpRequest.getRequestLine()));
					}
				} catch(Exception e) {
					HawkException.catchException(e);
				} finally {
					latch.countDown();
				}
			}

			@Override
			public void failed(Exception e) {
				HawkLog.logPrintln(String.format("FunPlus translate failed : \"%s\" %s", sourceText, httpRequest.getRequestLine()));
				HawkException.catchException(e);
				latch.countDown();
			}

			@Override
			public void cancelled() {
				HawkLog.logPrintln(String.format("FunPlus translate cancelled : \"%s\" %s", sourceText, httpRequest.getRequestLine()));
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
		HawkLog.logPrintln(String.format("FunPlus translate batch count : %d", total));

		// TODO 多线程可见性是否有问题
		final String[][] transText = new String[transArray.length][];

		for (int i = 0; i < transArray.length; ++i) {
			final int reqIndex = i;
			Translation trans = transArray[reqIndex];
			transText[reqIndex] = new String[trans.targetLangArray.length];

			for (int j = 0; j < trans.targetLangArray.length; ++j) {
				final int langIndex = j;
				final HttpUriRequest httpRequest = genHttpRequest(trans.sourceText, trans.sourceLang, trans.targetLangArray[langIndex], trans.profanity, trans.textType);
				// for log
				final String sourceText = trans.sourceText;
//				HawkLog.logPrintln(String.format("FunPlus translate send : \"%s\" %s", sourceText, httpRequest.getRequestLine()));
				// 异步请求
				httpClient.execute(httpRequest, new FutureCallback<HttpResponse>() {

					@Override
					public void completed(HttpResponse response) {

						try {
							String responseString = EntityUtils.toString(response.getEntity());

							TranslateResponse transResponse = HawkJsonUtil.getJsonInstance().fromJson(responseString, new TypeToken<TranslateResponse>() {}.getType());
							if (transResponse.errorCode == 0) {
								transText[reqIndex][langIndex] = transResponse.translation.targetText;
								//HawkLog.logPrintln(String.format("FunPlus translate succ : \"%s\"  \"%s\" %s", sourceText, transResponse.translation.targetText, httpRequest.getRequestLine()));
							} else {
								HawkLog.errPrintln(String.format("FunPlus translate error : %d %s \"%s\" %s", transResponse.errorCode, transResponse.errorMessage, sourceText, httpRequest.getRequestLine()));
							}
						} catch(Exception e) {
							HawkException.catchException(e);
						} finally {
							latch.countDown();
						}
					}

					@Override
					public void failed(Exception e) {
						HawkLog.logPrintln(String.format("FunPlus translate failed : \"%s\" %s", sourceText, httpRequest.getRequestLine()));
						HawkException.catchException(e);
						latch.countDown();
					}

					@Override
					public void cancelled() {
						HawkLog.logPrintln(String.format("FunPlus translate cancelled : \"%s\" %s", sourceText, httpRequest.getRequestLine()));
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

		HawkLog.logPrintln(String.format("FunPlus translate task complete"));

	}

	@Override
	public void onTick() {
	}

	@Override
	public String getName() {
		return this.getClass().getSimpleName();
	}

	@Override
	public void finalize() {
		try {
			httpClient.close();
		} catch (IOException e) {
			HawkException.catchException(e);
		}
	}
}
