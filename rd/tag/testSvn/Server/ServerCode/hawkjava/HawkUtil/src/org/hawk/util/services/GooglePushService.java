package org.hawk.util.services;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentLinkedQueue;

import org.hawk.app.HawkApp;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.util.HawkTickable;

import com.google.android.gcm.server.Message;
import com.google.android.gcm.server.MulticastResult;
import com.google.android.gcm.server.Notification;
import com.google.android.gcm.server.Result;
import com.google.android.gcm.server.Sender;

/**
 * 谷歌推送GCM
 * 
 * @author walker
 */
public class GooglePushService extends HawkTickable {

	public static class PushMsg {
		public Message gcmMsg;
		String to;
		List<Integer> playerIdList;
	}

	private Sender service;
	private ConcurrentLinkedQueue<PushMsg> pushQueue;
	private ConcurrentHashMap<Integer, String> playerTokenMap;
	private ConcurrentHashMap<String, Integer> tokenPlayerMap;

	/**
	 * 实例对象
	 */
	private static GooglePushService instance = null;
	public static GooglePushService getInstance() {
		if (instance == null) {
			instance = new GooglePushService();
		}
		return instance;
	}

	public GooglePushService() {
		pushQueue = new ConcurrentLinkedQueue<>();
		playerTokenMap = new ConcurrentHashMap<>();
		tokenPlayerMap = new ConcurrentHashMap<>();
	}

	/**
	 * 初始化谷歌推送服务
	 */
	public boolean install(String apiKey, Map<Integer, String> playerTokenMap) {
		for (Entry<Integer, String> entry : playerTokenMap.entrySet()) {
			this.playerTokenMap.put(entry.getKey(), entry.getValue());
			this.tokenPlayerMap.put(entry.getValue(), entry.getKey());
		}
		this.service = new Sender(apiKey);

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
		// TODO 更多参数
		Notification gcmNoti = new Notification.Builder("myicon")
		.title("Notice!")
		.body(msg)
		.build();

		Message gcmMsg = new Message.Builder()
		.notification(gcmNoti)
		.build();

		push(gcmMsg);
		return true;
	}

	/**
	 * 推送默认格式消息，部分用户
	 */
	public boolean pushSimple(String msg, List<Integer> playerIdList) {
		// TODO 更多参数
		Notification gcmNoti = new Notification.Builder("myicon")
		.title("Notice!")
		.body(msg)
		.build();

		Message gcmMsg = new Message.Builder()
		.notification(gcmNoti)
		.build();

		push(gcmMsg, playerIdList);
		return true;
	}

	/**
	 * 推送默认格式消息，主题
	 */
	public boolean pushSimpleTopic(String msg, String topic) {
		// TODO 更多参数
		Notification gcmNoti = new Notification.Builder("myicon")
		.title("Notice!")
		.body(msg)
		.build();

		Message gcmMsg = new Message.Builder()
		.notification(gcmNoti)
		.build();

		push(gcmMsg, topic);
		return true;
	}

	/**
	 * 推送自定义格式消息，全部用户
	 */
	public boolean push(Message gcmMsg) {
		PushMsg push = new PushMsg();
		push.gcmMsg = gcmMsg;
		pushQueue.offer(push);
		return true;
	}

	/**
	 * 推送自定义格式消息，部分用户
	 */
	public boolean push(Message gcmMsg, List<Integer> playerIdList) {
		PushMsg push = new PushMsg();
		push.gcmMsg = gcmMsg;
		push.playerIdList = playerIdList;
		pushQueue.offer(push);
		return true;
	}

	/**
	 * 推送自定义格式消息，主题
	 */
	public boolean push(Message gcmMsg, String topic) {
		PushMsg push = new PushMsg();
		push.gcmMsg = gcmMsg;
		push.to = "/topics/".concat(topic);
		pushQueue.offer(push);
		return true;
	}

	/**
	 * 处理结果
	 */
	private void handleResult(Result result, String to) {
		if (result.getMessageId() == null) {
			HawkLog.errPrintln(String.format("Google push error: %s", result.getErrorCodeName()));
		} else {
			String canonicalRegistrationId = result.getCanonicalRegistrationId();
			if (canonicalRegistrationId != null) {
				Integer inactivePlayerId = tokenPlayerMap.remove(to);
				if (inactivePlayerId != null) {
					playerTokenMap.put(inactivePlayerId, canonicalRegistrationId);
					tokenPlayerMap.put(canonicalRegistrationId, inactivePlayerId);
				}

				// TODO 修改数据库
			}
		}
	}

	/**
	 * 处理广播结果
	 */
	private void handleMulticastResult(MulticastResult multiResult, List<String> regIds) {
		if (multiResult.getFailure() != 0 || multiResult.getCanonicalIds() != 0) {
			List<Result> resultList = multiResult.getResults();
			int size = resultList.size();
			for (int i = 0; i < size; ++i) {
				handleResult(resultList.get(i), regIds.get(i));
			}
		}
	}

	/**
	 * 执行推送
	 */
	private void pushInternal(PushMsg msg) {
		try {
			if (msg.to != null) {
				// 推送给主题
				Result result = service.send(msg.gcmMsg, msg.to, 1);
				handleResult(result, msg.to);
			} else if (msg.playerIdList == null) {
				// 推送给全部用户
				// TODO 改用topic做法。否则注意请求的大小限制
				List<String> regIds = new ArrayList<String>(playerTokenMap.values());
				MulticastResult result = service.send(msg.gcmMsg, regIds, 1);
				handleMulticastResult(result, regIds);
			} else {
				// 推送给指定用户组
				// TODO 改用topic做法。否则注意请求的大小限制
				List<String> regIds = new ArrayList<String>();
				for (Integer playerId : msg.playerIdList) {
					String token = playerTokenMap.get(playerId);
					if (token != null) {
						regIds.add(token);
					}
				}
				MulticastResult result = service.send(msg.gcmMsg, regIds, 1);
				handleMulticastResult(result, regIds);
			}
		} catch (IOException e) {
			HawkException.catchException(e);
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
	}

}
