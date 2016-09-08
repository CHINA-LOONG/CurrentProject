package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/recharge.csv", struct = "map")
public class RechargeCfg extends HawkConfigBase{
	/**
	 * 级别
	 */
	@Id
	protected final String id;
	/**
	 * 经验
	 */
	protected final int gold;
	/**
	 * 容量
	 */
	protected final float gift;
	
	public RechargeCfg(){
		this.id = "";
		this.gold = 0;
		this.gift = 0;
	}

	public String getId() {
		return id;
	}

	public int getGold() {
		return gold;
	}

	public float getGift() {
		return gift;
	}
}
