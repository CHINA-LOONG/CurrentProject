package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/instanceDrop.csv", struct = "map")
public class InstanceDropCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final String rewardId;
	
	// assemble
	protected RewardCfg rewardCfg;

	public InstanceDropCfg() {
		id = "";
		rewardId = "";
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
		return true;
	}
	
	public String getInstanceMonsterId() {
		return id;
	}
	
	public RewardCfg getReward() {
		return rewardCfg;
	}
	
}
