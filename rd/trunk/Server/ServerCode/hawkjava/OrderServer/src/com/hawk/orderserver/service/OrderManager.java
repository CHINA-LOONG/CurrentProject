package com.hawk.orderserver.service;


import java.util.Map;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.LinkedBlockingQueue;

import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;
import org.hawk.thread.HawkTask;
import org.hawk.thread.HawkThreadPool;
import org.hawk.util.HawkTickable;
import org.hawk.util.services.HawkOrderService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.google.gson.JsonObject;
import com.hawk.orderserver.OrderServices;
import com.hawk.orderserver.db.DBManager;
import com.hawk.orderserver.entify.CallbackInfo;

public class OrderManager extends HawkTickable {
	/**
	 * 订单发货发货日志对象
	 */
	static Logger logger = LoggerFactory.getLogger("PayDeliver");

	
	/**
	 * 订单回调状态(支付回调, 通知发货, 发货完成)
	 */
	static int ORDER_STATE_CALLBACK = 1;
	static int ORDER_STATE_NOTIFY = 2;
	static int ORDER_STATE_DELIVER = 3;

	/**
	 * 订单数据库名
	 */
	private String orderDatabase = "";
	/**
	 * 订单队列
	 */
	BlockingQueue<CallbackInfo> orderQueue;
	/**
	 * 订单信息映射表
	 */
	Map<String, CallbackInfo> orderInfoMap;
	/**
	 * stub数据库落地线程
	 */
	private HawkThreadPool threadPool;
	/**
	 * 单例对象
	 */
	static OrderManager instance;

	/**
	 * 获取实例
	 */
	public static OrderManager getInstance() {
		if (instance == null) {
			instance = new OrderManager();
		}
		return instance;
	}

	/**
	 * 隐藏构造函数
	 */
	private OrderManager() {
		orderQueue = new LinkedBlockingQueue<CallbackInfo>();
		orderInfoMap = new ConcurrentHashMap<String, CallbackInfo>();
		OrderServices.getInstance().addTickable(this);
	}

	/**
	 * 初始化
	 * 
	 * @return
	 */
	public boolean init(String orderDatabase) {
		this.orderDatabase = orderDatabase;
		threadPool = new HawkThreadPool("OrderManager");
		threadPool.initPool(2, true);
		return true;
	}

	/**
	 * 关闭管理器, 等待线程结束
	 * 
	 * @return
	 */
	public boolean close() {
		if (threadPool != null) {
			threadPool.close(true);
			threadPool = null;
		}
		return true;
	}

	/**
	 * 获取订单数据库名
	 * 
	 * @return
	 */
	public String getOrderDatabase() {
		return orderDatabase;
	}
	
	/**
	 * 执行sql语句
	 * 
	 * @param sql
	 * @return
	 */
	public boolean executeSql(final String sql, boolean async) {
		if (!async || threadPool == null) {
			DBManager.getInstance().executeSql(orderDatabase, sql);
			return true;
		}

		// 按照操作的表进行线程划分
		int threadIdx = sql.toLowerCase().indexOf("orders") >= 0 ? 0 : 1;

		// 添加到线程池
		threadPool.addTask(new HawkTask(true) {
			@Override
			protected int run() {
				DBManager.getInstance().executeSql(orderDatabase, sql);
				return 0;
			};
		}, threadIdx, false);

		return true;
	}

	/**
	 * 添加保存回调信息
	 * 
	 * @param callbackInfo
	 * @return
	 */
	public boolean addCallbackInfo(CallbackInfo callbackInfo) {
		try {
			// 数据落地到mysql
			String sql = String.format("INSERT INTO callback(tid, uid, product_id, through_cargo, type, status, ts, createTime) " +
					"VALUES(\"%s\", \"%s\", \"%s\", \"%s\", \"%s\", %d, \"%s\", \"%s\")", 
					callbackInfo.getTid(), callbackInfo.getUid(), callbackInfo.getProduct_id(),
					callbackInfo.getThrough_cargo(), callbackInfo.getType(), ORDER_STATE_CALLBACK, 
					HawkTime.getTimeString(HawkTime.getCalendar((long)callbackInfo.getTs() * 1000)), HawkTime.getTimeString());

			// 线程模式落地
			executeSql(sql, true);
			return true;
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return false;
	}

	/**
	 * 更新订单状态
	 * 
	 * @param callbackInfo
	 * @return
	 */
	public boolean updateCallbackStatus(CallbackInfo callbackInfo) {
		try {
			// 数据落地到mysql
			String sql = String.format("UPDATE callback SET status = %d WHERE id = %d;", callbackInfo.getStatus(), callbackInfo.getId());
			executeSql(sql, true);
			return true;
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return false;
	}


	/**
	 * 通知订单平台回调
	 * 
	 * @param orderInfo
	 */
	public boolean onCallbackNotify(CallbackInfo callbackInfo) {
		// 添加到订单队列
		callbackInfo.setNotifyTime(0);
		orderQueue.add(callbackInfo);
		orderInfoMap.put(callbackInfo.getTid(), callbackInfo);
		return true;
	}

	/**
	 * 通知订单发货完成
	 * 
	 * @param orderInfo
	 * @param status
	 *            (0: 成功, 负数: 错误码)
	 */
	public void onDeliverNotify(String order, int status, int addGole, int giftGold) {
		CallbackInfo callbackInfo = orderInfoMap.get(order);
		if (callbackInfo != null && status <= 0 && callbackInfo.getStatus() == ORDER_STATE_NOTIFY) {
			int lastStatus = callbackInfo.getStatus();
			if (status == 0) {
				callbackInfo.setStatus(ORDER_STATE_DELIVER);
			} else {
				callbackInfo.setStatus(status);
			}

			if (lastStatus != callbackInfo.getStatus()) {
				updateCallbackStatus(callbackInfo);
			}
		}

		// 日志记录
		logger.info(String.format("OrderDeliver: %s, Status: %d", callbackInfo.toJson(), status));
	}

	/**
	 * 每帧更新
	 * 
	 */
	@Override
	public void onTick() {
		// 订单队列发货
		long currTime = HawkTime.getMillisecond();
		while (orderQueue.size() > 0) {
			CallbackInfo callbackInfo = orderQueue.peek();

			// 已发货的订单从队列清理(不再添加进来即可)
			// 按情况应该不会出现这个
			if (callbackInfo.getStatus() == ORDER_STATE_DELIVER) {
				orderQueue.remove(callbackInfo);
				orderInfoMap.remove(callbackInfo.getTid());
				continue;
			}

			// 最近时间的订单未到发货时间
			if (callbackInfo.getNotifyTime() > currTime) {
				break;
			}

			// 添加到队尾以便重发
			callbackInfo.setNotifyTime(currTime + OrderServices.getInstance().getNotifyPeriod());
			// 从队列头部移动到到队尾部
			orderQueue.poll();
			orderQueue.add(callbackInfo);

			try {
				// 指定发送服务器
				String identify = callbackInfo.getThrough_cargo();
				JsonObject jsonObject = callbackInfo.toJson();
				jsonObject.addProperty("action", HawkOrderService.ACTION_ORDER_DELIVER_REQUEST);
				OrderServices.getInstance().sendNotify(identify, jsonObject.toString());

				// 更新订单状态
				if (callbackInfo.getStatus() != ORDER_STATE_NOTIFY) {
					callbackInfo.setStatus(ORDER_STATE_NOTIFY);
					updateCallbackStatus(callbackInfo);
				}

				// 记录通知发货信息
				logger.info(String.format("CallbackNotify: %s", callbackInfo.toJson()));
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
	}
}
