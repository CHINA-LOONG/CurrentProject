package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

import com.hawk.game.util.GsConst;

@HawkConfigManager.CsvResource(file = "staticData/sociatyQuest.csv", struct = "map")
public class SociatyQuestCfg extends HawkConfigBase{
	/**
	 * 配置id
	 */
	@Id
	protected final int id;
	
	/**
	 * 名称
	 */
	protected final String name;
	
	/**
	 * 图标
	 */
	protected final String icon;
	
	/**
	 * 目标类型
	 */
	protected final int goalType;
	
	/**
	 * 目标参数
	 */
	protected final String goalParam;
	
	/**
	 * 目标值
	 */
	protected final int goalCount;
	
	/**
	 * 奖励
	 */
	protected final String rewardId;

	public SociatyQuestCfg(){
		id = 0;
		name = null;
		icon = null;
		goalType = 0;
		goalParam = null;
		goalCount = 0;
		rewardId = null;
	}

	public int getId() {
		return id;
	}

	public String getName() {
		return name;
	}

	public String getIcon() {
		return icon;
	}
	
	public int getGoalType() {
		return goalType;
	}

	public String getGoalParam() {
		return goalParam;
	}

	public int getGoalCount() {
		return goalCount;
	}

	public String getRewardId() {
		return rewardId;
	}

	@Override
	protected boolean assemble() {	
		return true;
	}
	
	@Override
	protected boolean checkValid(){
		if (HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardId) == null) {
			return false;
		}
		
		if (goalType == GsConst.AllianceQuestType.INSTANCE_QUEST && HawkConfigManager.getInstance().getConfigByKey(InstanceCfg.class, goalParam) == null) {
			return false;
		}
		
		return true;
	}
}
