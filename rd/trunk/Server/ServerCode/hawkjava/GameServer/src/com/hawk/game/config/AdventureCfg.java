package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

import com.hawk.game.util.AdventureUtil;

@HawkConfigManager.CsvResource(file = "staticData/adventure.csv", struct = "map")
public class AdventureCfg extends HawkConfigBase {

	@Id
	protected final int id;
	protected final int type;
	protected final int time;
	protected final String level;
	protected final String basicReward;
	protected final String extraReward;

	// client only
	protected final String comments = null;
	protected final String image = null;

	// assemble
	protected int minLevel;
	protected int maxLevel;
	protected RewardCfg basicRewardCfg;
	protected RewardCfg extraRewardCfg;

	public AdventureCfg() {
		id = 0;
		type = 0;
		time = 0;
		level = "";
		basicReward = "";
		extraReward = "";
	}

	@Override
	protected boolean assemble() {
		String[] levelRange = level.split(",");
		if (levelRange.length != 2) {
			return false;
		}
		minLevel = Integer.parseInt(levelRange[0]);
		maxLevel = Integer.parseInt(levelRange[1]);

		AdventureUtil.addAdventureCfg(this);

		return true;
	}

	@Override
	protected boolean checkValid() {
		// 检测奖励是否存在，并建立引用
		basicRewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, basicReward);
		if (null == basicRewardCfg) {
			HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", basicReward));
			return false;
		}
		extraRewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, extraReward);
		if (null == extraRewardCfg) {
			HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", extraReward));
			return false;
		}

		return true;
	}

	public int getId() {
		return id;
	}

	public int getType() {
		return type;
	}

	public int getGear() {
		return time;
	}

	public RewardCfg getBasicReward() {
		return basicRewardCfg;
	}

	public RewardCfg getExtraReward() {
		return extraRewardCfg;
	}

	public boolean isInLevelRange(int level) {
		return (level >= minLevel && level <= maxLevel) ? true : false;
	}
}
