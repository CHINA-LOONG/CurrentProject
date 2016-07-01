package com.hawk.game.tcp;

import java.util.HashSet;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;

import org.apache.mina.util.ConcurrentHashSet;
import org.hawk.app.HawkApp;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;

import com.hawk.game.proxy.ProxyServer;

/**
 * 对内的tcp连接管理器
 * 
 * @author hawk
 */
public class ClientManager {
	/**
	 * 全局实例对象
	 */
	private static ClientManager instance;

	/**
	 * 获取全局实例对象
	 * 
	 * @return
	 */
	public static ClientManager getInstance() {
		if (instance == null) {
			instance = new ClientManager();
		}
		return instance;
	}

	/**
	 * 活跃检测时间
	 */
	private long activeCheckTime;

	/**
	 * 活跃信息
	 */
	private Set<String> activeTokens;

	/**
	 * 所有tcp连接
	 */
	private Map<String, ClientSession> tcpClients;

	/**
	 * 构造
	 */
	private ClientManager() {
		this.activeCheckTime = HawkTime.getMillisecond();
		this.activeTokens = new ConcurrentHashSet<String>();
		this.tcpClients = new ConcurrentHashMap<String, ClientSession>();
		this.activeTokens.addAll(tcpClients.keySet());
	}

	/**
	 * 创建一个tcp连接
	 * 
	 * @return
	 */
	public ClientSession createTcpClient() {
		try {
			ClientSession client = new ClientSession();
			if (client.connect(ProxyServer.getInstance().getServerIp(), ProxyServer.getInstance().getServerPort(), ProxyServer.getInstance().getConnTimeout())) {
				tcpClients.put(client.getToken(), client);
				return getTcpClient(client.getToken());
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}

		if (HawkApp.getInstance().getAppCfg().isDebug()) {
			HawkLog.logPrintln(String.format("session connect failed, tcpserver: %s:%d", ProxyServer.getInstance().getServerIp(), ProxyServer.getInstance().getServerPort()));
		}
		return null;
	}

	/**
	 * 获取指定的tcp连接
	 * 
	 * @param token
	 * @return
	 */
	public ClientSession getTcpClient(String token) {
		activeTokens.remove(token);
		return tcpClients.get(token);
	}

	/**
	 * 移除指定的tcp连接
	 * 
	 * @param token
	 * @return
	 */
	public void removeTcpClient(String token) {
		tcpClients.remove(token);
	}

	/**
	 * 帧更新
	 */
	public void onTick() {
		if (HawkTime.getMillisecond() - activeCheckTime >= ProxyServer.getInstance().getIdleTimeout()) {
			Set<String> remainTokens = new HashSet<String>();
			remainTokens.addAll(activeTokens);
			activeTokens.clear();
			for (String token : remainTokens) {
				removeTcpClient(token);
			}
			activeTokens.addAll(tcpClients.keySet());
			activeCheckTime = HawkTime.getMillisecond();
		}
	}
}
