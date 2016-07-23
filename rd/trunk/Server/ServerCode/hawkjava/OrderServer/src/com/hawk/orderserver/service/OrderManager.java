package com.hawk.orderserver.service;

import java.sql.ResultSet;
import java.sql.Statement;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.LinkedBlockingQueue;

import net.sf.json.JSONObject;

import org.hawk.log.HawkLog;
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
import com.hawk.orderserver.entify.OrderInfo;

public class OrderManager extends HawkTickable {
	/**
	 * 订单发货发货日志对象
	 */
	static Logger logger = LoggerFactory.getLogger("PayDeliver");

	/**
	 * 回调信息状态
	 */
	static int CALLBACK_STATE_NOTFOUND = 1;
	static int CALLBACK_STATE_NOTIFY = 2;

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
	BlockingQueue<OrderInfo> orderQueue;
	/**
	 * 订单信息映射表
	 */
	Map<String, OrderInfo> orderInfoMap;
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
		orderQueue = new LinkedBlockingQueue<OrderInfo>();
		orderInfoMap = new ConcurrentHashMap<String, OrderInfo>();
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
		loadUndeliverOrders();
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
	 * 加钻未发货的订单(status: 1 | 2)
	 */
	private void loadUndeliverOrders() {
		// 从缓存服务器没有获取到, 从stub拉取信息并缓存到memcache
		HawkLog.logPrintln("Load Undeliver Orders......");
		Statement statement = null;
		try {
			String sql = "SELECT id, myOrder, pfOrder, suuid, game, platform, serverId, channel, playerId, puid, device, goodsId, goodsCount, orderMoney, payMoney, currency, userData, status " + 
					"FROM orders WHERE (status = 1 OR status = 2) AND payMoney > 0 AND LENGTH(pfOrder) > 0";
			
			statement = DBManager.getInstance().createStatement(orderDatabase);
			ResultSet resultSet = statement.executeQuery(sql);
			while (resultSet.next()) {
				int column = 0;
				OrderInfo orderInfo = new OrderInfo();
				orderInfo.setId(resultSet.getInt(++column));
				orderInfo.setMyOrder(resultSet.getString(++column));
				orderInfo.setPfOrder(resultSet.getString(++column));
				orderInfo.setSuuid(resultSet.getString(++column));
				orderInfo.setGame(resultSet.getString(++column));
				orderInfo.setPlatform(resultSet.getString(++column));
				orderInfo.setServerId(resultSet.getInt(++column));
				orderInfo.setChannel(resultSet.getString(++column));
				orderInfo.setPlayerId(resultSet.getInt(++column));
				orderInfo.setPuid(resultSet.getString(++column));
				orderInfo.setDevice(resultSet.getString(++column));
				orderInfo.setGoodsId(resultSet.getString(++column));
				orderInfo.setGoodsCount(resultSet.getInt(++column));
				orderInfo.setOrderMoney(resultSet.getInt(++column));
				orderInfo.setPayMoney(resultSet.getInt(++column));
				orderInfo.setCurrency(resultSet.getString(++column));
				orderInfo.setUserData(resultSet.getString(++column));
				orderInfo.setStatus(resultSet.getInt(++column));

				// 缓存到memcache
				HawkLog.logPrintln(orderInfo.toJson().toString());
				OrderServices.getInstance().memcacheSetString(orderInfo.getMyOrder(), orderInfo.toJson().toString());

				// 添加到订单队列等待发货
				orderQueue.add(orderInfo);
				orderInfoMap.put(orderInfo.getMyOrder(), orderInfo);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		} finally {
			try {
				if (statement != null) {
					statement.close();
				}
			} catch (Exception e) {
			}
		}
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
			String sql = String.format("INSERT INTO callback(myOrder, pfOrder, payMoney, payPf, userData, status, date, createTime, updateTime) " +
					"VALUES('%s', '%s', %d, '%s', '%s', %d, '%s', '%s', '%s')", 
					callbackInfo.getMyOrder(), callbackInfo.getPfOrder(), callbackInfo.getPayMoney(),
					callbackInfo.getPayPf(), callbackInfo.getUserData(), callbackInfo.getStatus(), 
					HawkTime.getDateString(), HawkTime.getTimeString(), HawkTime.getTimeString());

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
	 * 更新订单状态
	 * 
	 * @param orderInfo
	 * @return
	 */
	public boolean updateOrderStatus(OrderInfo orderInfo) {
		try {
			// 存储到memcache缓存服务器
			OrderServices.getInstance().memcacheSetString(orderInfo.getMyOrder(), orderInfo.toJson().toString());

			// 数据落地到mysql
			String sql = String.format("UPDATE orders SET status = %d WHERE myOrder = '%s'", orderInfo.getStatus(), orderInfo.getMyOrder());
			if (orderInfo.getStatus() == ORDER_STATE_CALLBACK) {
				// 更新支付回调发送过来的信息
				sql = String.format("UPDATE orders SET pfOrder = '%s', payMoney = %d, payPf = '%s', userData = '%s', status = %d WHERE myOrder = '%s'", 
						orderInfo.getPfOrder(), orderInfo.getPayMoney(), orderInfo.getPayPf(), orderInfo.getUserData(), 
						orderInfo.getStatus(), orderInfo.getMyOrder());
				
			} else if (orderInfo.getStatus() == ORDER_STATE_DELIVER) {
				// 更新游戏服务器加钻信息
				sql = String.format("UPDATE orders SET status = %d, addGold = %d, giftGold = %d WHERE myOrder = '%s'", 
						orderInfo.getStatus(), orderInfo.getAddGold(), orderInfo.getGiftGold(), orderInfo.getMyOrder());
				
			}
			// 线程模式落地
			executeSql(sql, true);
			return true;
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return false;
	}

	/**
	 * 获取订单信息
	 * 
	 * @param order
	 * @return
	 */
	public OrderInfo getOrderInfo(String order) {
		// 当前内存订单队列
		if (orderInfoMap.containsKey(order)) {
			return orderInfoMap.get(order);
		}

		// 在缓存中寻找订单信息
		String value = OrderServices.getInstance().memcacheGetString(order);
		if (value != null) {
			try {
				OrderInfo orderInfo = new OrderInfo();
				JSONObject jsonObject = JSONObject.fromObject(value);
				if (orderInfo.fromJson(jsonObject) && orderInfo.isValid()) {
					return orderInfo;
				}
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}

		// 从缓存服务器没有获取到, 从stub拉取信息并缓存到memcache
		Statement statement = null;
		try {
			String sql = String.format("SELECT id, myOrder, pfOrder, suuid, game, platform, serverId, channel, playerId, puid, device, "
					+ "goodsId, goodsCount, orderMoney, payMoney, currency, payPf, userData, status FROM orders WHERE myOrder = '%s'", order);
			
			statement = DBManager.getInstance().createStatement(orderDatabase);
			ResultSet resultSet = statement.executeQuery(sql);
			if (resultSet.next()) {
				int column = 0;
				OrderInfo orderInfo = new OrderInfo();
				orderInfo.setId(resultSet.getInt(++column));
				orderInfo.setMyOrder(resultSet.getString(++column));
				orderInfo.setPfOrder(resultSet.getString(++column));
				orderInfo.setSuuid(resultSet.getString(++column));
				orderInfo.setGame(resultSet.getString(++column));
				orderInfo.setPlatform(resultSet.getString(++column));
				orderInfo.setServerId(resultSet.getInt(++column));
				orderInfo.setChannel(resultSet.getString(++column));
				orderInfo.setPlayerId(resultSet.getInt(++column));
				orderInfo.setPuid(resultSet.getString(++column));
				orderInfo.setDevice(resultSet.getString(++column));
				orderInfo.setGoodsId(resultSet.getString(++column));
				orderInfo.setGoodsCount(resultSet.getInt(++column));
				orderInfo.setOrderMoney(resultSet.getInt(++column));
				orderInfo.setPayMoney(resultSet.getInt(++column));
				orderInfo.setCurrency(resultSet.getString(++column));
				orderInfo.setPayPf(resultSet.getString(++column));
				orderInfo.setUserData(resultSet.getString(++column));
				orderInfo.setStatus(resultSet.getInt(++column));

				// 缓存到memcache
				OrderServices.getInstance().memcacheSetString(orderInfo.getMyOrder(), orderInfo.toJson().toString());
				return orderInfo;
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		} finally {
			try {
				if (statement != null) {
					statement.close();
				}
			} catch (Exception e) {
			}
		}
		return null;
	}

	/**
	 * 生成订单
	 * 
	 * @param args
	 * @return
	 */
	public OrderInfo generateOrder(JSONObject args) {
		try {
			// 反序列化参数
			OrderInfo orderInfo = new OrderInfo();
			orderInfo.fromJson(args);

			// 检查必要参数
			if (args.containsKey("suuid") && 
				args.containsKey("game") && 
				args.containsKey("platform") && 
				args.containsKey("serverId") && 
				args.containsKey("channel") && 
				args.containsKey("playerId") && 
				args.containsKey("puid") && 
				args.containsKey("device") && 
				args.containsKey("goodsId") && 
				args.containsKey("goodsCount") && 
				args.containsKey("orderMoney") && 
				args.containsKey("currency")) {
				// 生成自身的订单号
				String myOrder = UUID.randomUUID().toString().toLowerCase().replace("-", "");
				orderInfo.setMyOrder(myOrder);

				// 生成的订单记录到缓存
				OrderServices.getInstance().memcacheSetString(orderInfo.getMyOrder(), orderInfo.toJson().toString());

				// 写入到数据库
				String sql = String.format("INSERT INTO " + 
						"orders(myOrder, suuid, game, platform, serverId, channel, playerId, puid, device, goodsId, goodsCount, orderMoney, currency, date, createTime, updateTime) " + 
						"VALUES('%s', '%s', '%s', '%s', %d, '%s', %d, '%s', '%s', '%s', %d, %d, '%s', '%s', '%s', '%s')", 
						orderInfo.getMyOrder(), orderInfo.getSuuid(), orderInfo.getGame(), orderInfo.getPlatform(), 
						orderInfo.getServerId(), orderInfo.getChannel(), orderInfo.getPlayerId(), orderInfo.getPuid(), 
						orderInfo.getDevice(), orderInfo.getGoodsId(), orderInfo.getGoodsCount(), orderInfo.getOrderMoney(), 
						orderInfo.getCurrency(), HawkTime.getDateString(), HawkTime.getTimeString(), HawkTime.getTimeString());

				// 线程模式落地
				executeSql(sql, false);

				// 日志记录
				logger.info(String.format("OrderGenerate: %s", orderInfo.toJson()));
				
				// 订单测试
				try {
					if (args.containsKey("callback")) {
						int callback = args.getInt("callback");
						if (callback == 1) {
							String pfOrder = UUID.randomUUID().toString().toLowerCase().replace("-", "");
							sql = String.format("INSERT INTO callback(myOrder, pfOrder, payMoney, payPf, date, createTime) values('%s', '%s', %d, '%s', CURDATE(), NOW())",
									orderInfo.getMyOrder(), pfOrder, orderInfo.getOrderMoney(), orderInfo.getChannel());
							
							executeSql(sql, false);
						}
					}
				} catch (Exception e) {
					HawkException.catchException(e);
				}
				
				return orderInfo;
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return null;
	}
	
	/**
	 * 通知订单平台回调
	 * 
	 * @param orderInfo
	 */
	public boolean onCallbackNotify(CallbackInfo callbackInfo) {
		OrderInfo orderInfo = getOrderInfo(callbackInfo.getMyOrder());
		if (orderInfo != null && orderInfo.isValid()) {
			// 更新回调状态
			callbackInfo.setStatus(CALLBACK_STATE_NOTIFY);
			updateCallbackStatus(callbackInfo);
			logger.info(String.format("OrderCallback: %s", callbackInfo.toJson()));

			// 设置订单属性
			orderInfo.setNotifyTime(0);
			orderInfo.setPayPf(callbackInfo.getPayPf());
			orderInfo.setPfOrder(callbackInfo.getPfOrder());
			orderInfo.setPayMoney(callbackInfo.getPayMoney());
			orderInfo.setUserData(callbackInfo.getUserData());
			orderInfo.setStatus(ORDER_STATE_CALLBACK);

			// 更新订单状态
			updateOrderStatus(orderInfo);

			// 添加到订单队列
			orderQueue.add(orderInfo);
			orderInfoMap.put(orderInfo.getMyOrder(), orderInfo);
			return true;
		} else {
			// 更新回调状态
			callbackInfo.setStatus(CALLBACK_STATE_NOTFOUND);
			updateCallbackStatus(callbackInfo);
			// 日志记录
			logger.error(String.format("InvalidOrder: %s", callbackInfo.toJson()));
		}
		return false;
	}

	/**
	 * 通知订单发货完成
	 * 
	 * @param orderInfo
	 * @param status
	 *            (0: 成功, 负数: 错误码)
	 */
	public void onDeliverNotify(String order, int status, int addGole, int giftGold) {
		OrderInfo orderInfo = getOrderInfo(order);
		if (orderInfo != null && status <= 0 && orderInfo.getStatus() == ORDER_STATE_NOTIFY) {
			int lastStatus = orderInfo.getStatus();
			if (status == 0) {
				orderInfo.setStatus(ORDER_STATE_DELIVER);
				orderInfo.setAddGold(addGole);
				orderInfo.setGiftGold(giftGold);
			} else {
				orderInfo.setStatus(status);
			}

			if (lastStatus != orderInfo.getStatus()) {
				updateOrderStatus(orderInfo);
			}
		}

		// 日志记录
		logger.info(String.format("OrderDeliver: %s, Status: %d", orderInfo.toJson(), status));
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
			OrderInfo orderInfo = orderQueue.peek();

			// 已发货的订单从队列清理(不再添加进来即可)
			// 按情况应该不会出现这个
			if (orderInfo.getStatus() == ORDER_STATE_DELIVER) {
				orderQueue.remove(orderInfo);
				orderInfoMap.remove(orderInfo.getMyOrder());
				continue;
			}

			// 最近时间的订单未到发货时间
			if (orderInfo.getNotifyTime() > currTime) {
				break;
			}

			// 添加到队尾以便重发
			orderInfo.setNotifyTime(currTime + OrderServices.getInstance().getNotifyPeriod());
			// 从队列头部移动到到队尾部
			orderQueue.poll();
			orderQueue.add(orderInfo);

			try {
				// 指定发送服务器
				String identify = orderInfo.getSuuid();
				if (identify == null || identify.length() <= 0) {
					identify = String.format("%s.%s.%d", orderInfo.getGame(), orderInfo.getPlatform(), orderInfo.getServerId());
				}
				JsonObject jsonObject = orderInfo.toJson();
				jsonObject.addProperty("action", HawkOrderService.ACTION_ORDER_DELIVER_REQUEST);
				OrderServices.getInstance().sendNotify(identify, jsonObject.toString());

				// 更新订单状态
				if (orderInfo.getStatus() != ORDER_STATE_NOTIFY) {
					orderInfo.setStatus(ORDER_STATE_NOTIFY);
					updateOrderStatus(orderInfo);
				}

				// 记录通知发货信息
				logger.info(String.format("OrderNotify: %s", orderInfo.toJson()));
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
	}
}
