package com.hawk.game.config;

import java.util.ArrayList;
import java.util.List;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;

@HawkConfigManager.CsvResource(file = "xml/instanceReward.csv", struct = "map")
public class InstanceRewardCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final float expCoef;
	protected final float goldCoef;
	protected final int rewardId;
	protected final int sweepRewardId;
	protected final int star1RewardGroupId;
	protected final int star2RewardGroupId;
	protected final int star3RewardGroupId;

	// assemble
	protected RewardCfg rewardCfg;
	protected RewardCfg sweepRewardCfg;
	protected List<RewardGroupCfg> starRewardCfgList;

	public InstanceRewardCfg() {
		id = "";
		expCoef = 1.0f;
		goldCoef = 1.0f;
		rewardId = 0;
		star1RewardGroupId = 0;
		star2RewardGroupId = 0;
		star3RewardGroupId = 0;
		sweepRewardId = 0;

		starRewardCfgList = new ArrayList<RewardGroupCfg>();
	}

	@Override
	protected boolean assemble() {
		return true;
	}

	@Override
	protected boolean checkValid() {
		// 检测奖励是否存在，并建立引用
		rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardId);
		if (null == rewardCfg) {
			return false;
		}

		sweepRewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, sweepRewardId);
		if (null == sweepRewardCfg) {
			return false;
		}

		RewardGroupCfg rewardGroupCfg = HawkConfigManager.getInstance().getConfigByKey(RewardGroupCfg.class, star1RewardGroupId);
		if (null == rewardGroupCfg) {
			return false;
		}
		starRewardCfgList.add(rewardGroupCfg);

		rewardGroupCfg = HawkConfigManager.getInstance().getConfigByKey(RewardGroupCfg.class, star2RewardGroupId);
		if (null == rewardGroupCfg) {
			return false;
		}
		starRewardCfgList.add(rewardGroupCfg);

		rewardGroupCfg = HawkConfigManager.getInstance().getConfigByKey(RewardGroupCfg.class, star3RewardGroupId);
		if (null == rewardGroupCfg) {
			return false;
		}
		starRewardCfgList.add(rewardGroupCfg);
		
		return true;
	}

	public String getInstanceId() {
		return id;
	}

	public float getExpCoef() {
		return expCoef;
	}
	
	public float getGoldCoef() {
		return goldCoef;
	}
	
	public RewardCfg getReward() {
		return rewardCfg;
	}

	public RewardCfg getSweepReward() {
		return sweepRewardCfg;
	}

	public RewardGroupCfg getStarRewardGroup(int starCount) {
		int index = starCount - 1;
		if (index > 0 && index < starRewardCfgList.size()) {
			return starRewardCfgList.get(index);
		}
		return null;
	}
}
