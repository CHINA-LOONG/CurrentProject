package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "xml/playerAttr.csv", struct = "map")
public class PlayerAttrCfg extends HawkConfigBase {
	/**
	 * 配置id
	 */
	@Id
	final protected int level ;
	/**
	 * 1角色2怪物
	 */
	final protected int exp;
	/**
	 * 类型
	 */
	final protected int fatigue;
	
	public PlayerAttrCfg() {
		level = 0;
		exp = 0;
		fatigue = 0;
	}
	
	public int getLevel() {
		return level;
	}

	public int getExp() {
		return exp;
	}

	public int getFatigue() {
		return fatigue;
	}
	
	@Override
	protected boolean checkValid() {
		return level != 0 && exp != 0 && fatigue != 0;
	}
}
