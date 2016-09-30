package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/pvpStageReward.csv", struct = "map")
public class PVPStageRewardCfg extends HawkConfigBase{
	/**
	 * grade
	 */
	@Id
	final protected int grade;
	
	/**
	 * 奖励
	 */
	final protected String reward1;
	
	/**
	 * 界面显示奖励
	 */
	final protected String reward2;
	
	public PVPStageRewardCfg() {
		grade = 0;
		reward1 = null;
		reward2 = null;
	}
	
	public int getGrade() {
		return grade;
	}

	public String getReward1() {
		return reward1;
	}

	public String getReward2() {
		return reward2;
	}

	@Override
	protected boolean checkValid() {
		if (HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, reward1) == null) {
			return false;
		}
		
		if (HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, reward2) == null) {
			return false;
		}
		
		return true;
	}
}
