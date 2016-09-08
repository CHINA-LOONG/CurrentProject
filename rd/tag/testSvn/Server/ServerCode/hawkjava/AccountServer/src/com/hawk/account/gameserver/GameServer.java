package com.hawk.account.gameserver;

import org.hawk.os.HawkTime;

public class GameServer{
	/**
	 * 服务器状态
	 */
	public static int SERVER_STATE_NEW = 1;
	public static int SERVER_STATE_FULL = 2;
	public static int SERVER_STATE_HOT = 3;
	public static int SERVER_STATE_MAINTAIN = 4;
	public static int SERVER_STATE_UNKNOW = 5;

	/**
	 * 未指定服务器
	 */
	static int UNKONW_AREA = 0;

	public int index;
	public int area;
	public String name;
	public String hostIp;
	public int port;
	public int state;
	public long heartBeatTime;

	public GameServer() {
		heartBeatTime = HawkTime.getMillisecond();
		name = "";
		hostIp = "";
		port = 0;
		state = GameServer.SERVER_STATE_MAINTAIN;
		index = 0;
		area = 0;
	}

	public int getIndex() {
		return index;
	}

	public void setIndex(int index) {
		this.index = index;
	}

	public int getArea() {
		return area;
	}

	public void setArea(int area) {
		this.area = area;
	}

	public long getHeartBeatTime() {
		return heartBeatTime;
	}

	public void setHeartBeatTime(long heartBeatTime) {
		this.heartBeatTime = heartBeatTime;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getHostIp() {
		return hostIp;
	}

	public void setHostIp(String hostIp) {
		this.hostIp = hostIp;
	}

	public int getPort() {
		return port;
	}

	public void setPort(int port) {
		this.port = port;
	}

	public int getState() {
		return state;
	}

	public void setState(int state) {
		this.state = state;
	}
}