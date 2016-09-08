package org.hawk.security;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.security.SecureRandom;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;

import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;

import org.apache.http.HttpHost;
import org.apache.http.config.Registry;
import org.apache.http.config.RegistryBuilder;
import org.apache.http.conn.socket.ConnectionSocketFactory;
import org.apache.http.protocol.HttpContext;
import org.hawk.os.HawkException;

// TODO httpClient 从3升级到4, 此类待修改
public class HawkHttpsSocketFactory implements ConnectionSocketFactory {
	/**
	 * ssl上下文
	 */
	private SSLContext sslContext = null;

	/**
	 * 创建ssl上下文环境
	 * 
	 * @return
	 */
	private SSLContext createSSLContext() {
		SSLContext sslContext = null;
		try {
			sslContext = SSLContext.getInstance("SSL");
			sslContext.init(null, new TrustManager[] { new X509TrustManager() {
				@Override
				public void checkClientTrusted(X509Certificate[] chain, String authType) throws CertificateException {
				}

				@Override
				public void checkServerTrusted(X509Certificate[] chain, String authType) throws CertificateException {
				}

				@Override
				public X509Certificate[] getAcceptedIssuers() {
					return new X509Certificate[] {};
				}
			} }, new SecureRandom());
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return sslContext;
	}

	/**
	 * 获取ssl上下文环境
	 * 
	 * @return
	 */
	private SSLContext getSSLContext() {
		if (null == this.sslContext) {
			this.sslContext = createSSLContext();
		}
		return this.sslContext;
	}

	/**
	 * 装载https的支持
	 */
	public static boolean install() {
		Registry<ConnectionSocketFactory> registry = RegistryBuilder.<ConnectionSocketFactory>create()
				.register("https", new HawkHttpsSocketFactory())
				.build();

		return true;
	}

	@Override
	public Socket connectSocket(int connectTimeout, Socket socket, HttpHost host, InetSocketAddress remoteAddress, InetSocketAddress localAddress, HttpContext context) throws IOException {
		return null;
	}

	@Override
	public Socket createSocket(HttpContext context) throws IOException {
		return null;
	}

//	/**
//	 * 创建ssl上下文socket
//	 */
//	@Override
//	public Socket createSocket(String host, int port) throws UnknownHostException, IOException {
//		return getSSLContext().getSocketFactory().createSocket(host, port);
//	}
//
//	/**
//	 * 创建ssl上下文socket
//	 */
//	@Override
//	public Socket createSocket(String host, int port, InetAddress clientHost, int clientPort) throws IOException, UnknownHostException {
//		return getSSLContext().getSocketFactory().createSocket(host, port, clientHost, clientPort);
//	}
//
//	/**
//	 * 创建ssl上下文socket
//	 */
//	@Override
//	public Socket createSocket(String host, int port, InetAddress localAddress, int localPort, HttpConnectionParams params) throws IOException, UnknownHostException, ConnectTimeoutException {
//		if (params == null) {
//			throw new IllegalArgumentException("https parameters cannot be null");
//		}
//
//		int timeout = params.getConnectionTimeout();
//		SocketFactory socketfactory = getSSLContext().getSocketFactory();
//		if (timeout == 0) {
//			return socketfactory.createSocket(host, port, localAddress, localPort);
//		} else {
//			Socket socket = socketfactory.createSocket();
//			SocketAddress localaddr = new InetSocketAddress(localAddress, localPort);
//			SocketAddress remoteaddr = new InetSocketAddress(host, port);
//			socket.bind(localaddr);
//			socket.connect(remoteaddr, timeout);
//			return socket;
//		}
//	}
//
//	/**
//	 * 装载https的支持
//	 */
//	public static boolean install() {
//		try {
//			Protocol https = new Protocol("https", new HawkHttpsSocketFactory(), 443);
//			Protocol.registerProtocol("https", https);
//			return true;
//		} catch (Exception e) {
//			HawkException.catchException(e);
//		}
//		return false;
//	}
}
