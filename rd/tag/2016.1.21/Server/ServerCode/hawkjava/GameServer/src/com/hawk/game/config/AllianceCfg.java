package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/allianceAttr.csv", struct = "map")
public class AllianceCfg extends HawkConfigBase{
	/**
	 * 级别
	 */
	@Id
	protected final int level;
	/**
	 * 经验
	 */
	protected final int exp;
	/**
	 * 容量
	 */
	protected final int pop;
	
	public AllianceCfg(){
		this.level = 0;
		this.exp = 0;
		this.pop = 0;
	}

	public int getLevel() {
		return level;
	}

	public int getExp() {
		return exp;
	}

	public int getPop() {
		return pop;
	}
	
}
