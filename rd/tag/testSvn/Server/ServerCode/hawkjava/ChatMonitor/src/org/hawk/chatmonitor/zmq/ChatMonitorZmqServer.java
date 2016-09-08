package org.hawk.chatmonitor.zmq;

import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;

import net.sf.json.JSONObject;

import org.hawk.chatmonitor.ChatMonitorServices;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.thread.HawkThreadPool;
import org.hawk.util.HawkTickable;
import org.hawk.zmq.HawkZmq;
import org.hawk.zmq.HawkZmqManager;

public class ChatMonitorZmqServer extends HawkTickable {
	/**
	 * 服务地址
	 */
	private String zmqAddr;
	/**
	 * 服务zmq
	 */
	private HawkZmq serviceZmq;
	/**
	 * 接收数据缓冲区
	 */
	private byte[] bytes = null;
	/**
	 * 待发送的聊天项
	 */
	BlockingQueue<ChatSendItem> waitingChatItems;
	/**
	 * 线程池
	 */
	private HawkThreadPool threadPool;
	/**
	 * 数据库管理器单例对象
	 */
	static ChatMonitorZmqServer instance;

	/**
	 * 获取数据库管理器单例对象
	 * 
	 * @return
	 */
	public static ChatMonitorZmqServer getInstance() {
		if (instance == null) {
			instance = new ChatMonitorZmqServer();
		}
		return instance;
	}

	/**
	 * 函数
	 */
	private ChatMonitorZmqServer() {
		waitingChatItems = new LinkedBlockingQueue<ChatSendItem>();
	}

	/**
	 * 获取名字
	 */
	@Override
	public String getName() {
		return this.getClass().getSimpleName();
	}
	
	/**
	 * 开启服务
	 */
	public boolean setup(String addr) {
		// 创建通用缓冲区
		bytes = new byte[16 * 1024 * 1024];
		ChatMonitorServices.getInstance().addTickable(this);

		this.zmqAddr = addr;
		serviceZmq = HawkZmqManager.getInstance().createZmq(HawkZmq.ZmqType.ROUTER);
		if (!serviceZmq.bind(addr)) {
			HawkLog.logPrintln("Service Zmq Bind Failed......");
			return false;
		}
		
		return true;
	}

	/**
	 * 停止服务
	 */
	public void stop() {
		try {
			if (serviceZmq != null) {
				HawkZmqManager.getInstance().closeZmq(serviceZmq);
				serviceZmq = null;
			}
			
			if (threadPool != null) {
				threadPool.close(true);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
	
	/**
	 * 获取服务地址
	 * 
	 * @return
	 */
	public String getAddr() {
		return zmqAddr;
	}
	
	/**
	 * 帧更新事件
	 */
	@Override
	public void onTick() {
		while (serviceZmq.pollEvent(HawkZmq.HZMQ_EVENT_READ, 0) > 0) {
			int recvSize = serviceZmq.recv(bytes, 0);
			if (recvSize > 0) {
				String identify = new String(bytes, 0, recvSize);
				if (serviceZmq.isWaitRecv()) {
					recvSize = serviceZmq.recv(bytes, 0);
					if (recvSize > 0) {
						String data = new String(bytes, 0, recvSize);
						onServerData(identify, data);
					}
				}
			}
		}
		
		while (waitingChatItems.size() > 0) {
			ChatSendItem chatSendItem = waitingChatItems.poll();
			serviceZmq.send(chatSendItem.identify.getBytes(), HawkZmq.HZMQ_SNDMORE);
			serviceZmq.send(chatSendItem.msg.getBytes(), 0);
		}
	}

	/**
	 * 游戏服务器发送过来消息
	 * 
	 * @param identify
	 * @param data
	 */
	private void onServerData(String identify, String data) {
		try {
			HawkLog.logPrintln(String.format("[%s]: %s", identify, data));
			
			JSONObject jsonObject = JSONObject.fromObject(data);
			ChatMonitorServices.getInstance().addServerChatData(identify, jsonObject);
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
	
	/**
	 * 通知游戏服务器聊天信息
	 * 
	 * @param puid
	 * @param msg
	 */
	public void notifyServerInfo(String identify, String msg) {
		try {
			waitingChatItems.add(new ChatSendItem(identify, msg));
			
			JSONObject jsonObject = new JSONObject();
			jsonObject.put("playerId", 0);
			jsonObject.put("playerName", "");
			jsonObject.put("chatMsg", msg);
			jsonObject.put("transFlag", 0);
			ChatMonitorServices.getInstance().addServerChatData(identify, jsonObject);
			
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
