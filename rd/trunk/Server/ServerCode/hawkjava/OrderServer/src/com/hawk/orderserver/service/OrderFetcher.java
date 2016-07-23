package com.hawk.orderserver.service;

import java.sql.ResultSet;
import java.sql.Statement;

import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;

import com.hawk.orderserver.OrderServices;
import com.hawk.orderserver.db.DBManager;
import com.hawk.orderserver.entify.CallbackInfo;

public class OrderFetcher extends Thread {
	/**
	 * 订单数据库名
	 */
	private String orderDatabase = "";
	/**
	 * 获取的最大订单号
	 */
	private int maxOrderId = 0;
	/**
	 * 单例对象
	 */
	static OrderFetcher instance;

	/**
	 * 获取实例
	 */
	public static OrderFetcher getInstance() {
		if (instance == null) {
			instance = new OrderFetcher();
		}
		return instance;
	}

	/**
	 * 隐藏构造函数
	 */
	private OrderFetcher() {
	}

	/**
	 * 初始化订单拉取器
	 * 
	 * @return
	 */
	public boolean init(String orderDatabase) {
		// 获取初始未发货订单
		this.orderDatabase = orderDatabase;
		fetchLatestOrders();

		// 开启线程
		this.start();

		return true;
	}

	/**
	 * 拉取最新订单信息
	 * 
	 * @return
	 */
	private int fetchLatestOrders() {
		int fetchCount = 0;
		Statement statement = null;
		try {
			String sql = String.format("SELECT id, myOrder, pfOrder, payMoney, payPf, userData, status FROM callback WHERE status <= 0 AND id > %d", maxOrderId);
			statement = DBManager.getInstance().createStatement(orderDatabase);
			ResultSet resultSet = statement.executeQuery(sql);
			while (resultSet.next()) {
				int column = 0;
				CallbackInfo callbackInfo = new CallbackInfo();
				callbackInfo.setId(resultSet.getInt(++column));
				callbackInfo.setMyOrder(resultSet.getString(++column));
				callbackInfo.setPfOrder(resultSet.getString(++column));
				callbackInfo.setPayMoney(resultSet.getInt(++column));
				callbackInfo.setPayPf(resultSet.getString(++column));
				callbackInfo.setUserData(resultSet.getString(++column));
				callbackInfo.setStatus(resultSet.getInt(++column));

				// 通知从数据库读到新回调订单
				OrderManager.getInstance().onCallbackNotify(callbackInfo);
				maxOrderId = Math.max(maxOrderId, callbackInfo.getId());
				fetchCount++;
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
		return fetchCount;
	}

	@Override
	public void run() {
		while (OrderServices.getInstance().isRunning()) {
			try {
				int fetchCount = fetchLatestOrders();
				if (fetchCount <= 0) {
					HawkOSOperator.osSleep(200);
				}
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}
	}
}
