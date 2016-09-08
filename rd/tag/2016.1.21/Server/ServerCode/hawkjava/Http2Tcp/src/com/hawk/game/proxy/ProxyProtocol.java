package com.hawk.game.proxy;

import org.hawk.net.protocol.HawkProtocol;

public class ProxyProtocol {
	/**
	 * 发送协议模式
	 */
	public static int SEND_MODE = 1;
	/**
	 * 接收协议模式
	 */
	public static int RECV_MODE = 2;
	/**
	 * 响应模式
	 */
	public static int RESP_MODE = 3;
	
	/**
	 * 协议类型
	 */
	private int type = 0;
	/**
	 * 协议状态, 主要用来回复客户端协议
	 */
	private int status = 0;
	/**
	 * 会话校验码
	 */
	private String token = null;
	/**
	 * 协议数据
	 */
	private HawkProtocol[] protocols = null;

	/**
	 * 构造
	 * 
	 * @param type
	 * @param bytes
	 */
	public ProxyProtocol(int type, String token, HawkProtocol[] protocols) {
		this.type = type;
		this.token = token;
		this.protocols = protocols;
	}

	/**
	 * 协议是否有效
	 * 
	 * @return
	 */
	public boolean isValid() {
		return type == SEND_MODE || type == RECV_MODE || type == RESP_MODE;
	}

	/**
	 * 获取类型
	 * 
	 * @return
	 */
	public int getType() {
		return type;
	}

	/**
	 * 设置类型
	 * 
	 * @param type
	 */
	public ProxyProtocol setType(int type) {
		this.type = type;
		return this;
	}

	/**
	 * 获取状态码
	 * 
	 * @return
	 */
	public int getStatus() {
		return status;
	}

	/**
	 * 设置状态码
	 * 
	 * @param status
	 */
	public void setStatus(int status) {
		this.status = status;
	}
	
	/**
	 * 获取协议对应令牌
	 * 
	 * @return
	 */
	public String getToken() {
		return token;
	}

	/**
	 * 设置协议令牌
	 * 
	 * @param token
	 */
	public ProxyProtocol setToken(String token) {
		this.token = token;
		return this;
	}

	/**
	 * 获取协议数据
	 * 
	 * @return
	 */
	public HawkProtocol[] getProtocols() {
		return protocols;
	}

	/**
	 * 设置协议数据
	 * 
	 * @param data
	 */
	public ProxyProtocol setProtocols(HawkProtocol[] protocols) {
		this.protocols = protocols;
		return this;
	}
}
