package com.hawk.account;

public class AccountServices {	
	/**
	 * ����״̬
	 */
	volatile boolean running = true;
	
	/**
	 * ����ʵ������
	 */
	private static AccountServices instance = null;
	
	/**
	 * ��ȡʵ������
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
	 * ֹͣ
	 */
	public void stop() {
		running = false;
	}
}
