package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

@HawkConfigManager.CsvResource(file = "staticData/instanceChapter.csv", struct = "map")
public class InstanceChapterCfg extends HawkConfigBase {

	@Id
	protected final int chapter;
	protected final String name;
	protected final int normalLevel;
	protected final int hardLevel;
	protected final String normalReward;
	protected final String hardReward;

	// client only
	protected final String normalDrop = null;
	protected final String hardDrop = null;

	// assemble
	protected int[] difficultyLevelList;
	protected RewardCfg[] difficultyRewardList;

	public InstanceChapterCfg() {
		chapter = 0;
		name = "";
		normalLevel = hardLevel = 0;
		normalReward = hardReward = "";
	}

	@Override
	protected boolean assemble() {
		// 2种难度
		difficultyLevelList = new int[] {normalLevel, hardLevel};
		difficultyRewardList = new RewardCfg[] {null, null};

		return true;
	}

	@Override
	protected boolean checkValid() {
		// 检测奖励是否存在，并建立引用
		int index = 0;
		if (normalReward != "") {
			RewardCfg rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, normalReward);
			if (rewardCfg == null) {
				HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", normalReward));
				return false;
			}
			difficultyRewardList[index++] = rewardCfg;
		}

		if (hardReward != "") {
			RewardCfg rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, hardReward);
			if (rewardCfg == null) {
				HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", hardReward));
				return false;
			}
			difficultyRewardList[index++] = rewardCfg;
		}

		return true;
	}

	public int getChapter() {
		return chapter;
	}

	public String getName() {
		return name;
	}

	public int getLevelByDifficulty(int difficulty) {
		if (difficulty >= difficultyLevelList.length) {
			return Integer.MAX_VALUE;
		}
		return difficultyLevelList[difficulty];
	}

	public RewardCfg getRewardByDifficulty(int difficulty) {
		if (difficulty >= difficultyRewardList.length) {
			return null;
		}
		return difficultyRewardList[difficulty];
	}

}
