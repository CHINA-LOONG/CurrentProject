package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/playerAttr.csv", struct = "map")
public class PlayerAttrCfg extends HawkConfigBase {
	/**
	 * 等级
	 */
	@Id
	final protected int level ;
	/**
	 * 升到下一级需要经验
	 */
	final protected int exp;
	/**
	 * 体力上限
	 */
	final protected int fatigue;
	/**
	 * 升到本级奖励体力
	 */
	final protected int fatigueReward;

	public PlayerAttrCfg() {
		level = 0;
		exp = 0;
		fatigue = 0;
		fatigueReward = 0;
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

	public int getFatigueReward() {
		return fatigueReward;
	}

	@Override
	protected boolean checkValid() {
		return level != 0 && exp != 0 && fatigue != 0;
	}
}
