package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

import com.hawk.game.util.GsConst;
import com.hawk.game.util.InstanceUtil;

@HawkConfigManager.CsvResource(file = "staticData/instanceEntry.csv", struct = "map")
public class InstanceEntryCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final String name;
	protected final int chapter;
	protected final int index;
	protected final int type;
	protected final int difficulty;
	protected final int fatigue;
	protected final int count;
	protected final String reward;

	// client only
	protected final String enemy1 = null;
	protected final String enemy2 = null;
	protected final String enemy3 = null;
	protected final String enemy4 = null;
	protected final String enemy5 = null;
	protected final String enemy6 = null;
	protected final String desc= null;
	protected final int bp = 0;
	protected final String bgzhuangshi = null;
	protected final String instanceSpell = null;

	// assemble
	protected InstanceChapterCfg chapterCfg;
	protected RewardCfg rewardCfg;

	public InstanceEntryCfg() {
		id = "";
		name = "";
		chapter = 0;
		index = 0;
		type = 0;
		difficulty = 0;
		fatigue = 0;
		count = 0;
		reward = "";
	}

	@Override
	protected boolean assemble() {
		// 活力值不允许低于1
		if (fatigue < 1) {
			return false;
		}

		// 计算InstanceChapter
		InstanceUtil.addInstance(this);

		return true;
	}

	@Override
	protected boolean checkValid() {
		if (chapter != GsConst.UNUSABLE) {
			chapterCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceChapterCfg.class, chapter);
			if (null == chapterCfg) {
				HawkLog.errPrintln(String.format("config invalid InstanceChapterCfg : %s", chapter));
				return false;
			}
		}

		// 检测奖励是否存在，并建立引用
		if (reward != null && reward.equals("") == false) {
			rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, reward);
			if (null == rewardCfg) {
				HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", reward));
				return false;
			}
		}

		return true;
	}

	public String getInstanceId() {
		return id;
	}

	public String getName() {
		return name;
	}

	public int getChapter() {
		return chapter;
	}

	public int getIndex() {
		return index;
	}

	public InstanceChapterCfg getChapterCfg() {
		return chapterCfg;
	}

	public int getType() {
		return type;
	}

	public int getDifficult() {
		return difficulty;
	}

	public int getFatigue() {
		return fatigue;
	}

	public int getCount() {
		return count;
	}

	public RewardCfg getReward() {
		return rewardCfg;
	}

}
