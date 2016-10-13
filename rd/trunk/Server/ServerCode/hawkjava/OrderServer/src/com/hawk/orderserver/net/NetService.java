package com.hawk.orderserver.net;

import java.util.Map;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.LinkedBlockingQueue;

import net.sf.json.JSONObject;

import org.hawk.log.HawkLog;
import org.hawk.net.mq.HawkMQServer;
import org.hawk.net.mq.HawkMQSession;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hawk.zmq.HawkZmq;
import org.hawk.zmq.HawkZmqManager;

import com.hawk.orderserver.entify.QueueInfo;

public class NetService extends HawkMQServer {
	/**
	 * zmq服务通讯
	 */
	private HawkZmq serverZmq = null;
	/**
	 * 协议接收队列
	 */
	BlockingQueue<QueueInfo> recvQueue;
	/**
	 * 会话列表
	 */
	Map<String, BlockingQueue<HawkMQSession>> identifySession;
	
	/**
	 * 单例对象
	 */
	static NetService instance;

	/**
	 * 获取实例
	 */
	public static NetService getInstance() {
		if (instance == null) {
			instance = new NetService();
		}
		return instance;
	}
	
	/**
	 * 构造函数
	 */
	public NetService() {
		recvQueue = new LinkedBlockingQueue<QueueInfo>();
		identifySession = new ConcurrentHashMap<String, BlockingQueue<HawkMQSession>>();
	}
	
	/**
	 * 初始化网络服务
	 * 
	 * @param addr
	 * @return
	 */
	public boolean init(String addr) {
		try {
			if (addr.indexOf("tcp://") >= 0) {
				serverZmq = HawkZmqManager.getInstance().createZmq(HawkZmq.ZmqType.ROUTER);
				if (!serverZmq.bind(addr)) {
					HawkZmqManager.getInstance().closeZmq(serverZmq);
					serverZmq = null;
					return false;
				}
				HawkLog.logPrintln("Init PayZmq Success: " + addr);
			} else {
				String items[] = addr.split(":");
				int port = Integer.valueOf(items[1]);
				if (!super.init(port, 4)) {
					HawkLog.logPrintln("Init MQServer Failed: " + addr);
					return false;
				}
				HawkLog.logPrintln("Init MQServer Success: " + addr);
			}
			return true;
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return false;
	}
	
	/**
	 * 接收网络请求
	 * 
	 * @param infoQueue
	 * @return
	 */
	public boolean updateRecvQueue(BlockingQueue<QueueInfo> infoQueue) {
		try {
			// 更新zmq消息并接收到队列
			if (serverZmq != null) {
				updateZmqEvent();
			}

			// 写出队列
			synchronized (recvQueue) {
				infoQueue.addAll(recvQueue);
				recvQueue.clear();
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return infoQueue.size() > 0;
	}
	
	/**
	 * zmq对象接收数据通知
	 * 
	 * @param infoQueue
	 */
	private void updateZmqEvent() {
		while (serverZmq.pollEvent(HawkZmq.HZMQ_EVENT_READ, 0) > 0) {
			try {
				byte[] bytes = new byte[4096];
				int size = serverZmq.recv(bytes, 0);
				if (size > 0) {
					String identify = new String(bytes, 0, size);
					if (serverZmq.isWaitRecv()) {
						size = serverZmq.recv(bytes, 0);
						if (size > 0) {
							String data = new String(bytes, 0, size, "UTF-8");
							HawkLog.logPrintln(String.format("[%s] : %s", identify, data));
							synchronized (recvQueue) {
								recvQueue.add(new QueueInfo(identify, data));
							}
						} else {
							HawkLog.logPrintln(String.format("payZmq recv request info failed, identify: " + identify));
						}
					} else {
						HawkLog.logPrintln(String.format("payZmq wait recv flag incorrect, identify: " + identify));
					}
				} else {
					HawkLog.logPrintln(String.format("payZmq recv identify failed"));
				}
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
	}
	
	/**
	 * 写出发送队列所以内容
	 * 
	 * @param infoQueue
	 */
	public void flushSendQueue(BlockingQueue<QueueInfo> infoQueue) {
		while (infoQueue.size() > 0) {
			QueueInfo info = infoQueue.poll();
			sendNotify(info.identify, info.data);
		}
	}
	
	/**
	 * 网络发送请求
	 * 
	 * @param identify
	 * @param data
	 * @return
	 */
	private boolean sendNotify(String identify, String data) {
		try {
			if (serverZmq != null) {
				// 指定管道
				if (!serverZmq.send(identify.getBytes(), HawkZmq.HZMQ_SNDMORE)) {
					HawkLog.logPrintln("send identify failed, identify: " + identify);
				}
				
				// 发送数据
				if (!serverZmq.send(data.getBytes(), 0)) {
					HawkLog.logPrintln("send notify failed, data: " + data);
				}
				
			} else {
				BlockingQueue<HawkMQSession> sessionQueue = identifySession.get(identify);
				if (sessionQueue != null) {
					// 发送给相同标识的所有会话对象
					synchronized (sessionQueue) {
						for (HawkMQSession mqSession : sessionQueue) {
							mqSession.sendProtocol(HawkProtocol.valueOf(0, data.getBytes()));
						}
					}
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return true;
	}
	
	/**
	 * 会话开启回调
	 * 
	 * @param session
	 */
	@Override
	protected boolean onSessionOpened(HawkMQSession session) {
		HawkLog.logPrintln("order session connect, ipaddr: " + session.getIpAddr());
		return super.onSessionOpened(session);
	}

	/**
	 * 会话协议回调, 由IO线程直接调用, 非线程安全
	 * 
	 * @param session
	 * @param protocol
	 * @return
	 */
	@Override
	protected boolean onSessionProtocol(HawkMQSession session, HawkProtocol protocol) {
		try {
			// 基础校验
			if (protocol.getType() != 0 || protocol.getSize() <= 0) {
				return false;
			}
			
			// 解开协议数据
			String data = new String(protocol.getOctets().getBuffer().array(), 0, protocol.getSize());
			if (session.getIdentify() == null || session.getIdentify().length() <= 0) {
				// 未标识的会话, 第一个协议必须标识自己
				JSONObject jsonObject = JSONObject.fromObject(data);
				if (!jsonObject.containsKey("identify")) {
					return false;
				}
				
				String identify = jsonObject.getString("identify");
				session.setIdentify(identify);
				
				BlockingQueue<HawkMQSession> sessionQueue = null; 
				synchronized (identifySession) {
					// 判定会话标识列表是否存在, 不存在即创建
					if (!identifySession.containsKey(identify)) {
						identifySession.put(identify, new LinkedBlockingQueue<HawkMQSession>());
					}
					sessionQueue = identifySession.get(identify);
				}
				
				// 添加到相同的标识会话队列
				synchronized (sessionQueue) {
					if (!sessionQueue.contains(session)) {
						sessionQueue.add(session);
						HawkLog.logPrintln("order session identify, ipaddr: " + session.getIpAddr() + ", identify: " + identify);
					}
				}
			} else {
				synchronized (recvQueue) {
					recvQueue.add(new QueueInfo(session.getIdentify(), data));	
				}
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return super.onSessionProtocol(session, protocol);
	}
	
	/**
	 * 会话关闭回调
	 * 
	 * @param session
	 */
	@Override
	protected boolean onSessionClosed(HawkMQSession session) {
		// 会话关闭
		String identify = session.getIdentify();
		if (identify != null && identifySession.containsKey(identify)) {
			if (identifySession.get(identify).remove(session)) {
				HawkLog.logPrintln("order session disconnect, ipaddr: " + session.getIpAddr() + ", identify: " + identify);
			}
		} else {
			HawkLog.logPrintln("order session disconnect, ipaddr: " + session.getIpAddr());
		}
		return super.onSessionClosed(session);
	}
}
