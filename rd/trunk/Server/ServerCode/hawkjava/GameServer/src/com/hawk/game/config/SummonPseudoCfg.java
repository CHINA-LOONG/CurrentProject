package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

@HawkConfigManager.CsvResource(file = "staticData/summonPseudo.csv", struct = "map")
public class SummonPseudoCfg extends HawkConfigBase {

	@Id
	protected final int id;
	protected final int times;
	protected final String reward;

	// client only
	protected final String comments = null;

	// assemble
	protected RewardCfg rewardCfg;

	public SummonPseudoCfg() {
		id = 0;
		times = 0;
		reward = "";
	}

	@Override
	protected boolean assemble() {
		return true;
	}

	@Override
	protected boolean checkValid() {
		// 检测奖励是否存在，并建立引用
		rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, reward);
		if (null == rewardCfg) {
			HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", reward));
			return false;
		}

		return true;
	}

	public int getId() {
		return id;
	}

	public int getTimes() {
		return times;
	}

	public RewardCfg getReward() {
		return rewardCfg;
	}

}
