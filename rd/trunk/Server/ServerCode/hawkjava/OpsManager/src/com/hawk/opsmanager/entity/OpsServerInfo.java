package com.hawk.opsmanager.entity;

import org.hawk.os.HawkTime;
import org.hawk.util.services.HawkOpsService;
import org.hawk.util.services.helper.HawkOpsServerInfo;

public class OpsServerInfo {
	/**
	 * 代理标识
	 */
	private String agentIdentify;
	/**
	 * 运维服务器信息
	 */
	private HawkOpsServerInfo serverInfo;
	/**
	 * 服务器更新事件
	 */
	private long updateTime;

	public OpsServerInfo() {
		this.updateTime = HawkTime.getMillisecond();
	}

	public String getAgentIdentify() {
		return agentIdentify;
	}

	public void setAgentIdentify(String agentIdentify) {
		this.agentIdentify = agentIdentify;
	}

	public HawkOpsServerInfo getServerInfo() {
		return serverInfo;
	}

	public void setServerInfo(HawkOpsServerInfo serverInfo) {
		this.serverInfo = serverInfo;
	}

	public long getUpdateTime() {
		return updateTime;
	}

	public void setUpdateTime(long updateTime) {
		this.updateTime = updateTime;
	}
	
	public boolean isOnline() {
		return this.updateTime + HawkOpsService.SYNC_PERIOD * 3 > HawkTime.getMillisecond();
	}

	public void update(String agentIdentify, HawkOpsServerInfo info) {
		this.updateTime = HawkTime.getMillisecond();
		this.agentIdentify = agentIdentify;
		if (this.serverInfo.hasChanged(info)) {
			this.serverInfo = info;
		}
	}
}
