package com.hawk.orderserver;

import java.util.HashSet;
import java.util.Set;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;

import net.sf.json.JSONObject;

import org.apache.commons.configuration.XMLConfiguration;
import org.hawk.db.cache.HawkMemCacheDB;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.util.HawkTickable;
import org.hawk.util.services.HawkOrderService;

import com.hawk.orderserver.db.DBManager;
import com.hawk.orderserver.entify.QueueInfo;
import com.hawk.orderserver.http.CallbackHttpServer;
import com.hawk.orderserver.net.NetService;
import com.hawk.orderserver.service.OrderFetcher;
import com.hawk.orderserver.service.OrderManager;

/**
 * 收集服务
 * 
 * @author hawk
 */
public class OrderServices {
	/**
	 * 服务器是否运行中
	 */
	volatile boolean running;
	/**
	 * 订单通知周期
	 */
	int notifyPeriod = 10000;
	/**
	 * 缓存超时时间
	 */
	int expireSeconds = 86400;
	/**
	 * 缓存服务器
	 */
	HawkMemCacheDB memCacheDB = null;
	/**
	 * 可更新列表
	 */
	Set<HawkTickable> tickableSet;
	/**
	 * 信息发送队列
	 */
	BlockingQueue<QueueInfo> sendQueue;
	/**
	 * 信息接收队列
	 */
	BlockingQueue<QueueInfo> recvQueue;
	
	/**
	 * 密匙
	 */
	String funplusKey;
	
	/**
	 * 单例对象
	 */
	static OrderServices instance;

	/**
	 * 获取实例
	 */
	public static OrderServices getInstance() {
		if (instance == null) {
			instance = new OrderServices();
		}
		return instance;
	}

	/**
	 * 默认构造函数
	 */
	private OrderServices() {
		running = true;
		tickableSet = new HashSet<HawkTickable>();
		sendQueue = new LinkedBlockingQueue<QueueInfo>();
		recvQueue = new LinkedBlockingQueue<QueueInfo>();
	}

	/**
	 * 是否运行中
	 * 
	 * @return
	 */
	public boolean isRunning() {
		return running;
	}

	/**
	 * 退出服务主循环
	 */
	public void breakLoop() {
		running = false;
	}

	/**
	 * 获取出错重发间隔(ms)
	 * 
	 * @return
	 */
	public int getNotifyPeriod() {
		return notifyPeriod;
	}

	/**
	 * 获取缓存过期时间
	 * 
	 * @return
	 */
	public int getExpireSeconds() {
		return expireSeconds;
	}

	public String getFunplusKey() {
		return funplusKey;
	}

	/**
	 * 定时更新
	 */
	private void onTick() {
		for (HawkTickable tick : tickableSet) {
			try {
				tick.onTick();
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
	}

	/**
	 * 添加可更新对象列表
	 * 
	 * @param tickable
	 */
	public void addTickable(HawkTickable tickable) {
		tickableSet.add(tickable);
	}

	/**
	 * 使用配置文件初始化
	 * 
	 * @param cfgFile
	 * @return
	 */
	public boolean init(String cfgFile) {
		// 添加库加载目录
		HawkOSOperator.installLibPath();
		try {
			// 加载配置文件
			XMLConfiguration conf = new XMLConfiguration(cfgFile);

			// 订单网络服务组件
			String addr = conf.getString("basic.addr");
			if (!NetService.getInstance().init(addr)) {
				return false;
			}

			// 失败延时时间配置
			if (conf.containsKey("basic.notifyPeriod")) {
				notifyPeriod = conf.getInt("basic.notifyPeriod");
			}

			// 连接缓存服务器
			if (conf.containsKey("memcached.addr")) {
				memCacheDB = new HawkMemCacheDB();
				if (memCacheDB.initAsMemCached(conf.getString("memcached.addr"), conf.getInt("memcached.timeout"))) {
					HawkLog.logPrintln("Init Memcached Success: " + conf.getString("memcached.addr"));
				} else {
					HawkLog.logPrintln("Init Memcached Failed: " + conf.getString("memcached.addr"));
					return false;
				}

				if (conf.containsKey("memcached.expireSeconds")) {
					expireSeconds = conf.getInt("memcached.expireSeconds");
				}
			}

			// 获取趣加的密匙
			if (conf.containsKey("key.secret")) {
				funplusKey = conf.getString("key.secret");
			}
			
			// 初始化http的回调服务器
			if (conf.containsKey("httpserver.addr")) {
				if (CallbackHttpServer.getInstance().setup(conf.getString("httpserver.addr"), conf.getInt("httpserver.port"), conf.getInt("httpserver.pool"))) {
					HawkLog.logPrintln("Setup Callback HttpServer Success: " + conf.getString("httpserver.addr") + ":" + conf.getInt("httpserver.port"));
				} else {
					HawkLog.logPrintln("Setup Callback HttpServer Failed: " + conf.getString("httpserver.addr") + ":" + conf.getInt("httpserver.port"));
					return false;
				}
			}
			
			// 初始化订单数据库连接
			String orderDatabase = conf.getString("database.dbName");

			// 连接订单数据库
			if (DBManager.getInstance().init(conf.getString("database.dbHost"), conf.getString("database.userName"), 
					conf.getString("database.passWord"), conf.getInt("database.poolSize"))) {
				HawkLog.logPrintln("Init DBManager Success: " + conf.getString("database.dbHost"));
			} else {
				HawkLog.logPrintln("Init DBManager Failed: " + conf.getString("database.dbHost"));
				return false;
			}
			
			// 创建默认会话
			if (DBManager.getInstance().createDbSession(orderDatabase) == null) {
				HawkLog.logPrintln("Connect OrderFetcher Database Failed: " + orderDatabase);
				return false;
			} else {
				HawkLog.logPrintln("Connect OrderFetcher Database Success: " + orderDatabase);
			}
			
			// 初始化订单管理器
			OrderManager.getInstance().init(orderDatabase);

			// 初始化订单拉取器
			if (!OrderFetcher.getInstance().init(orderDatabase)) {
				HawkLog.logPrintln("Init OrderFetcher Failed");
				return false;
			}
			return true;
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return false;
	}

	/**
	 * 设置缓存数据
	 * 
	 * @param key
	 * @param value
	 * @param expireSeconds
	 */
	public boolean memcacheSetString(String key, String value) {
		try {
			// 存储到memcache缓存服务器
			if (memCacheDB != null) {
				// 过期时间暂时不支持
				// memCacheDB.setString(key, value, expireSeconds);
				memCacheDB.setString(key, value);
				return true;
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return false;
	}

	/**
	 * 获取缓存数据
	 * 
	 * @param key
	 */
	public String memcacheGetString(String key) {
		try {
			// 存储到memcache缓存服务器
			if (memCacheDB != null) {
				return memCacheDB.getString(key);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return "";
	}

	/**
	 * 发送数据给服务器
	 * 
	 * @param identify
	 * @param data
	 * @return
	 */
	public void sendNotify(String identify, String data) {
		sendQueue.add(new QueueInfo(identify, data));
	}
	
	/**
	 * 运行服务器
	 */
	public void run() {
		HawkLog.logPrintln("MainLoop Running OK.");
		while (running) {
			try {
				// 内部更新
				onTick();
				
				// 检测网络请求
				recvQueue.clear();
				if (NetService.getInstance().updateRecvQueue(recvQueue)) {
					while (recvQueue.size() > 0) {
						QueueInfo info = recvQueue.poll();
						onRequest(info.identify, JSONObject.fromObject(info.data));
					}
				}

				// 发送队列消息
				NetService.getInstance().flushSendQueue(sendQueue);
				
				// 空闲控制帧
				HawkOSOperator.sleep();
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}

		// 关闭订单管理器
		OrderManager.getInstance().close();
	}

	/**
	 * 请求处理
	 * 
	 * @param identify
	 * @param requestInfo
	 */
	private void onRequest(String identify, JSONObject requestInfo) {
		try {
			int action = requestInfo.getInt("action");
			// 响应支付发货
			if (action == HawkOrderService.ACTION_ORDER_DELIVER_RESPONSE) {
				// 通知订单发货响应
				OrderManager.getInstance().onDeliverNotify(requestInfo.getString("order"), 
						requestInfo.getInt("status"), requestInfo.getInt("addGold"), requestInfo.getInt("giftGold"));
			}
			
			// 心跳
			else if (action == HawkOrderService.ACTION_ORDER_HEART_BEAT) {
				HawkLog.logPrintln(String.format("HeartBeat: %s", requestInfo.toString()));
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
