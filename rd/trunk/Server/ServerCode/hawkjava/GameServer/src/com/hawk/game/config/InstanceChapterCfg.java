package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

@HawkConfigManager.CsvResource(file = "staticData/instanceChapter.csv", struct = "map")
public class InstanceChapterCfg extends HawkConfigBase {

	@Id
	protected final int chapter;
	protected final String name;
	protected final int level1;
	protected final int level2;
	protected final String reward1;
	protected final String reward2;

	// client only
	protected final String drop1 = null;
	protected final String drop2 = null;

	// assemble
	protected int[] difficultyLevelList;
	protected RewardCfg[] difficultyRewardList;

	public InstanceChapterCfg() {
		chapter = 0;
		name = "";
		level1 = level2 = 0;
		reward1 = reward2 = "";
	}

	@Override
	protected boolean assemble() {
		// 2种难度
		difficultyLevelList = new int[] {level1, level2};
		difficultyRewardList = new RewardCfg[] {null, null};

		return true;
	}

	@Override
	protected boolean checkValid() {
		// 检测奖励是否存在，并建立引用
		int index = 0;
		if (reward1 != "") {
			RewardCfg rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, reward1);
			if (rewardCfg == null) {
				HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", reward1));
				return false;
			}
			difficultyRewardList[index++] = rewardCfg;
		}

		if (reward2 != "") {
			RewardCfg rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, reward2);
			if (rewardCfg == null) {
				HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", reward2));
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
