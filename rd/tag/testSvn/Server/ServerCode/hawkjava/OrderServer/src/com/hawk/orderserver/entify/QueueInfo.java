package com.hawk.orderserver.entify;

public class QueueInfo {
	/**
	 * 标识符
	 */
	public String identify;
	/**
	 * 数据体
	 */
	public String data;
	/**
	 * 构造函数
	 * 
	 * @param identify
	 * @param data
	 */
	public QueueInfo(String identify, String data) {
		this.identify = identify;
		this.data = data;
	}
}
