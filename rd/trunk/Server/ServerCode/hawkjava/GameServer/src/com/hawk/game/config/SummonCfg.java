package com.hawk.game.config;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

@HawkConfigManager.CsvResource(file = "staticData/summon.csv", struct = "list")
public class SummonCfg extends HawkConfigBase {

	@Id
	protected final int id;
	protected final String recharge;
	protected final int type;
	protected final String reward;
	protected final String rewardGuarantee;

	// assemble
	protected int minRecharge;
	protected int maxRecharge;
	protected RewardCfg rewardCfg;
	protected RewardCfg guaranteeRewardCfg;

	// global
	protected static Map<Integer, List<SummonCfg>> summonMap = new HashMap<>();

	public static Map<Integer, List<SummonCfg>> getSummonMap() {
		return Collections.unmodifiableMap(summonMap);
	}

	public static SummonCfg getSummonCfg(int type, int recharge) {
		List<SummonCfg> summonList = summonMap.get(type);
		if (null != summonList) {
			for (int i = 0; i < summonList.size(); ++i) {
				SummonCfg cfg = summonList.get(i);
				if (cfg.isInRechargeRange(recharge)) {
					return cfg;
				}
			}
		}
		return null;
	}

	public SummonCfg() {
		id = 0;
		type = 0;
		recharge = "";
		reward = "";
		rewardGuarantee = "";
	}

	@Override
	protected boolean assemble() {
		String[] rechargeRange = recharge.split(",");
		if (rechargeRange.length != 2) {
			return false;
		}
		minRecharge = Integer.parseInt(rechargeRange[0]);
		maxRecharge = Integer.parseInt(rechargeRange[1]);

		List<SummonCfg> summonList = summonMap.get(type);
		if (null == summonList) {
			summonList = new ArrayList<> ();
			summonMap.put(type, summonList);
		}
		summonList.add(this);

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

		if (null != rewardGuarantee && false == rewardGuarantee.equals("")) {
			guaranteeRewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardGuarantee);
			if (null == guaranteeRewardCfg) {
				HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", rewardGuarantee));
				return false;
			}
		}

		return true;
	}

	public int getId() {
		return id;
	}

	public int getType() {
		return type;
	}

	public RewardCfg getReward() {
		return rewardCfg;
	}

	public RewardCfg getGuaranteeReward() {
		return guaranteeRewardCfg;
	}

	public boolean isInRechargeRange(int recharge) {
		return (recharge >= minRecharge && recharge <= maxRecharge) ? true : false;
	}
}
