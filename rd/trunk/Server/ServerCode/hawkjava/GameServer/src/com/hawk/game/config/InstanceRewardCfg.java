package com.hawk.game.config;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;
import org.hawk.log.HawkLog;

@HawkConfigManager.CsvResource(file = "staticData/instanceReward.csv", struct = "map")
public class InstanceRewardCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final String rewardId;
	protected final String sweepRewardId;

	// assemble
	protected RewardCfg rewardCfg;
	protected RewardCfg sweepRewardCfg;

	public InstanceRewardCfg() {
		id = "";
		rewardId = "";
		sweepRewardId = "";
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
			HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", rewardId));
			return false;
		}

		sweepRewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, sweepRewardId);
		if (null == sweepRewardCfg) {
			HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", sweepRewardId));
			return false;
		}

		return true;
	}

	public String getInstanceId() {
		return id;
	}

	public RewardCfg getReward() {
		return rewardCfg;
	}

	public RewardCfg getSweepReward() {
		return sweepRewardCfg;
	}
}
