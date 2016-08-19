package org.hawk.util.services;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentLinkedQueue;

import org.hawk.app.HawkApp;
import org.hawk.util.HawkTickable;

import com.notnoop.apns.APNS;
import com.notnoop.apns.ApnsService;

/**
 * 苹果推送APNs
 * 
 * @author walker
 */
public class ApplePushService extends HawkTickable {

	public static class PushMsg {
		public String payload;
		List<Integer> playerIdList;
	}

	private ApnsService service;
	private ConcurrentLinkedQueue<PushMsg> pushQueue;
	private ConcurrentHashMap<Integer, String> playerTokenMap;
	private ConcurrentHashMap<String, Integer> tokenPlayerMap;

	/**
	 * 实例对象
	 */
	private static ApplePushService instance = null;
	public static ApplePushService getInstance() {
		if (instance == null) {
			instance = new ApplePushService();
		}
		return instance;
	}

	public ApplePushService() {
		pushQueue = new ConcurrentLinkedQueue<>();
		playerTokenMap = new ConcurrentHashMap<>();
		tokenPlayerMap = new ConcurrentHashMap<>();
	}

	/**
	 * 初始化苹果推送服务
	 */
	public boolean install(String certFile, String pwd, Map<Integer, String> playerTokenMap) {
		for (Entry<Integer, String> entry : playerTokenMap.entrySet()) {
			this.playerTokenMap.put(entry.getKey(), entry.getValue());
			this.tokenPlayerMap.put(entry.getValue(), entry.getKey());
		}

		this.service = 
				APNS.newService()
				.withCert(certFile, pwd)
				.withSandboxDestination()
				.build();
		this.service.start();

		if (HawkApp.getInstance() != null) {
			HawkApp.getInstance().addTickable(this);
		}

		return true;
	}

	/**
	 * 更新玩家deviceToken
	 */
	public void updateToken(int playerId, String token) {
		playerTokenMap.put(playerId, token);
		tokenPlayerMap.put(token, playerId);
	}

	/**
	 * 推送默认格式消息，全部用户
	 */
	public boolean pushSimple(String msg) {
		String payload = APNS.newPayload().alertBody(msg).build();
		push(payload);
		return true;
	}

	/**
	 * 推送默认格式消息，部分用户
	 */
	public boolean pushSimple(String msg, List<Integer> playerIdList) {
		String payload = APNS.newPayload().alertBody(msg).build();
		push(payload, playerIdList);
		return true;
	}

	/**
	 * 推送自定义格式消息，全部用户
	 */
	public boolean push(String payload) {
		PushMsg push = new PushMsg();
		push.payload = payload;
		pushQueue.offer(push);
		return true;
	}

	/**
	 * 推送自定义格式消息，部分用户
	 */
	public boolean push(String payload, List<Integer> playerIdList) {
		PushMsg push = new PushMsg();
		push.payload = payload;
		push.playerIdList = playerIdList;
		pushQueue.offer(push);
		return true;
	}

	/**
	 * 检查并去除失效的推送目标
	 */
	public List<Integer> checkFeedback() {
		List<Integer> inactivePlayerList = new ArrayList<Integer>();
		Map<String, Date> inactiveDeviceMap = service.getInactiveDevices();

		for (String deviceToken : inactiveDeviceMap.keySet()) {
			Integer inactivePlayerId = tokenPlayerMap.remove(deviceToken);
			if (inactivePlayerId != null) {
				playerTokenMap.remove(inactivePlayerId);
			}
			inactivePlayerList.add(inactivePlayerId);
		}

		if (false == inactivePlayerList.isEmpty()) {
			return inactivePlayerList;
		}
		return null;
	}

	/**
	 * 执行推送
	 */
	private void pushInternal(PushMsg msg) {
		if (msg.playerIdList == null) {
			// 推送给全部用户
			service.push(playerTokenMap.values(), msg.payload);
		} else {
			// 推送给指定用户组
			for (Integer playerId : msg.playerIdList) {
				String token = playerTokenMap.get(playerId);
				if (token != null) {
					service.push(token, msg.payload);
				}
			}
		}
	}

	/**
	 * 帧更新
	 */
	@Override
	public void onTick() {
		// 每帧处理一个推送
		PushMsg msg = pushQueue.poll();
		if (msg != null) {
			pushInternal(msg);
		}
	}

	/**
	 * 停止服务，把当前消息全部推送
	 */
	public void stop() {
		PushMsg msg;
		while ((msg = pushQueue.poll()) != null) {
			pushInternal(msg);
		}
		service.stop();
	}

}
