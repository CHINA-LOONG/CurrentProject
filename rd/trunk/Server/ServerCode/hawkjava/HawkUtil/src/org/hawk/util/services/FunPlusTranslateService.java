package org.hawk.util.services;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

import org.apache.http.HttpResponse;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
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
import org.hawk.util.HawkTickable;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class FunPlusTranslateService extends HawkTickable {

	public static class TranslateRequest {
		// in
		public String sourceText;
		public String sourceLang;
		public String[] targetLangArray;
		// out
		public String[] transTextArray;
	}

	private static final Logger logger = LoggerFactory.getLogger("FunPlus");

	/**
	 * 是否通过cache调用翻译
	 */
	public static boolean fromCache;

	/**
	 * cache列表
	 */
	public static List<String> cacheList;
	private Lock cacheListLock;

	/**
	 * 异步httpClient
	 */
	CloseableHttpAsyncClient httpClient;

	/**
	 * cache刷新周期（毫秒）
	 */
	public static final int REFRESH_PERIOD = 600000;

	/**
	 * 上次更新时间
	 */
	private Long lastUpdateTimestamp;

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

		fromCache = false;
		cacheList = new ArrayList<String>();
		cacheListLock = new ReentrantLock();

		refreshCache();
		lastUpdateTimestamp = HawkTime.getCalendar().getTimeInMillis();

		if (HawkApp.getInstance() != null) {
			HawkApp.getInstance().addTickable(this);
		}
	}

	/**
	 * 翻译对外接口
	 */
	public String translate(TranslateRequest request) {
		final CountDownLatch latch = new CountDownLatch(1);
		final String[] transText = {null};
		final HttpPost httpPost = new HttpPost("TODO");
		//httpPost.addHeader(header);

		httpClient.execute(httpPost, new FutureCallback<HttpResponse>() {

			@Override
			public void completed(HttpResponse response) {
				latch.countDown();
				try {
					String responseString = EntityUtils.toString(response.getEntity());
					// TODO
					transText[0] = responseString;
				} catch(Exception e) {
					HawkException.catchException(e);
				}
			}

			@Override
			public void failed(Exception e) {
				latch.countDown();
				HawkLog.logPrintln(String.format("FunPlus translate failed : %s", httpPost.getRequestLine()));
				HawkException.catchException(e);
			}

			@Override
			public void cancelled() {
				latch.countDown();
				HawkLog.logPrintln(String.format("FunPlus translate cancelled : %s", httpPost.getRequestLine()));
			}

		});

		try {
			latch.await();
		} catch(Exception e) {
			HawkException.catchException(e);
		}

		request.transTextArray = transText;
		return transText[0];
	}

	/**
	 * 批量翻译对外接口
	 */
	public void translateBatch(TranslateRequest[] requestArray) {
		int total = 0;
		for (TranslateRequest request : requestArray) {
			total += request.targetLangArray.length;
		}
		final CountDownLatch latch = new CountDownLatch(total);
		final String[][] transText = new String[requestArray.length][];

		for (int i = 0; i < requestArray.length; ++i) {
			final int reqIndex = i;
			TranslateRequest request = requestArray[reqIndex];
			transText[reqIndex] = new String[request.targetLangArray.length];

			for (int j = 0; j < request.targetLangArray.length; ++j) {
				final int langIndex = j;
				final HttpPost httpPost = new HttpPost("TODO");

				httpClient.execute(httpPost, new FutureCallback<HttpResponse>() {

					@Override
					public void completed(HttpResponse response) {
						latch.countDown();
						try {
							String responseString = EntityUtils.toString(response.getEntity());
							// TODO
							transText[reqIndex][langIndex] = responseString;
						} catch(Exception e) {
							HawkException.catchException(e);
						}
					}

					@Override
					public void failed(Exception e) {
						latch.countDown();
						HawkLog.logPrintln(String.format("FunPlus translate failed : %s", httpPost.getRequestLine()));
						HawkException.catchException(e);
					}

					@Override
					public void cancelled() {
						latch.countDown();
						HawkLog.logPrintln(String.format("FunPlus translate cancelled : %s", httpPost.getRequestLine()));
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
			requestArray[i].transTextArray = transText[i];
		}
	}

	/**
	 * 更新缓存列表
	 */
	private void refreshCache() {
		List<String> newCacheList = null;

		final CountDownLatch latch = new CountDownLatch(1);
		final HttpGet httpGet = new HttpGet("TODO");

		httpClient.execute(httpGet, new FutureCallback<HttpResponse>() {

			@Override
			public void completed(HttpResponse response) {
				latch.countDown();
				try {
					String responseString = EntityUtils.toString(response.getEntity());
					// TODO
					//newCacheList;
				} catch(Exception e) {
					HawkException.catchException(e);
				}
			}

			@Override
			public void failed(Exception e) {
				latch.countDown();
				HawkLog.logPrintln(String.format("FunPlus update cache failed : %s", httpGet.getRequestLine()));
				HawkException.catchException(e);
			}

			@Override
			public void cancelled() {
				latch.countDown();
				HawkLog.logPrintln(String.format("FunPlus update cache cancelled : %s", httpGet.getRequestLine()));
			}

		});

		try {
			latch.await();
		} catch(Exception e) {
			HawkException.catchException(e);
		}

		cacheListLock.lock();
		try {
			cacheList = newCacheList;
		} finally {
			cacheListLock.unlock();
		}
	}

	@Override
	public void onTick() {
		long curTimestamp = HawkTime.getCalendar().getTimeInMillis();
		if (curTimestamp - lastUpdateTimestamp >= REFRESH_PERIOD) {
			lastUpdateTimestamp = curTimestamp;
			refreshCache();
		}
	}

	@Override
	public String getName() {
		return this.getClass().getSimpleName();
	}

	@Override
	public  void finalize() {
		try {
			httpClient.close();
		} catch (IOException e) {
			HawkException.catchException(e);
		}
	}
}
