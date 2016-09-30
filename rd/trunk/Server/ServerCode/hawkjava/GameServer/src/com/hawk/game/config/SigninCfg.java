package com.hawk.game.config;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

@HawkConfigManager.CsvResource(file = "staticData/signin.csv", struct = "list")
public class SigninCfg extends HawkConfigBase {

	protected final int month;
	protected final String col1;
	protected final String col2;
	protected final String col3;
	protected final String col4;
	protected final String col5;
	protected final String col6;

	// global
	protected static List<List<RewardCfg>> monthRewardList = new ArrayList<List<RewardCfg>>(Arrays.asList(
			new ArrayList<RewardCfg>(),  //1
			new ArrayList<RewardCfg>(),  //2
			new ArrayList<RewardCfg>(),  //3
			new ArrayList<RewardCfg>(),  //4
			new ArrayList<RewardCfg>(),  //5
			new ArrayList<RewardCfg>(),  //6
			new ArrayList<RewardCfg>(),  //7
			new ArrayList<RewardCfg>(),  //8
			new ArrayList<RewardCfg>(),  //9
			new ArrayList<RewardCfg>(),  //10
			new ArrayList<RewardCfg>(),  //11
			new ArrayList<RewardCfg>()  //12
			));

	public static RewardCfg getReward(int month, int day) {
		if (month >= 1 && month <= 12) {
			List<RewardCfg> rewardList = monthRewardList.get(month - 1);
			if (day >= 1 && day <= rewardList.size()) {
				return monthRewardList.get(month - 1).get(day - 1);
			}
		}
		return null;
	}

	public SigninCfg() {
		month = 0;
		col1 = col2 = col3 = col4 = col5 = col6 = "";
	}

	@Override
	protected boolean assemble() {
		if (month < 1 || month > 12) {
			return false;
		}
		return true;
	}

	@Override
	protected boolean checkValid() {
		// 检测奖励是否存在，并建立引用
		if (false == checkReward(col1)
				|| false == checkReward(col2)
				|| false == checkReward(col3)
				|| false == checkReward(col4)
				|| false == checkReward(col5)
				|| false == checkReward(col6)) {
			return false;
		}
		return true;
	}

	private boolean checkReward(String rewardId) {
		if (rewardId != null && rewardId.equals("") == false) {
			RewardCfg rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardId);
			if (null == rewardCfg) {
				HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", col1));
				return false;
			}
			monthRewardList.get(month - 1).add(rewardCfg);
		}
		return true;
	}

	public int getMonth() {
		return month;
	}

}
