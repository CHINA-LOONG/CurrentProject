package com.hawk.game;

import org.hawk.app.HawkAppCfg;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.KVResource(file = "cfg/gs.cfg")
public class IsConfig extends HawkAppCfg {
	/**
	 * 服务地址
	 */
	protected final String addr;
	/**
	 * 全局静态对象
	 */
	private static IsConfig instance = null;

	/**
	 * 获取全局静态对象
	 * 
	 * @return
	 */
	public static IsConfig getInstance() {
		return instance;
	}

	public IsConfig() {
		instance = this;
		addr = "";
	}
	
	public String getAddr() {
		return addr;
	}
}
