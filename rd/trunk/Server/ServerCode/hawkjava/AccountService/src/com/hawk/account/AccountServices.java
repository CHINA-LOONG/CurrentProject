package com.hawk.account;

public class AccountServices {	
	/**
	 * 运行状态
	 */
	volatile boolean running = true;
	
	/**
	 * 单例实例对象
	 */
	private static AccountServices instance = null;
	
	/**
	 * 获取实例对象
	 * 
	 * @return
	 */
	public static AccountServices getInstance() {
		if (instance == null) {
			instance = new AccountServices();
		}
		return instance;
	}

	/**
	 * 停止
	 */
	public void stop() {
		running = false;
	}
}
