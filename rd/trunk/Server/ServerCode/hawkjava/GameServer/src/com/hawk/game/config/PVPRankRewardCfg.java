package com.hawk.game.config;

import java.util.LinkedHashMap;
import java.util.Map;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/pvpRankReward.csv", struct = "list")
public class PVPRankRewardCfg extends HawkConfigBase{
	/**
	 * id
	 */
	@Id
	final protected int id;

	/**
	 * 最低级别
	 */
	final protected int level;

	/**
	 * 最低排名
	 */
	final protected int rank;

	/**
	 * 奖励
	 */
	final protected String reward1;

	/**
	 * 界面显示奖励
	 */
	final protected String reward2;

	static Map<Integer, LinkedHashMap<Integer, PVPRankRewardCfg>> pvpRankCfgList = new LinkedHashMap<Integer, LinkedHashMap<Integer,PVPRankRewardCfg>>();

	public PVPRankRewardCfg() {
		super();
		id = 0;
		level = 0;
		rank = 0;
		reward1 = null;
		reward2 = null;
	}

	public int getId() {
		return id;
	}

	public int getLevel() {
		return level;
	}

	public int getRank() {
		return rank;
	}

	public String getReward1() {
		return reward1;
	}

	public String getReward2() {
		return reward2;
	}

	public static PVPRankRewardCfg getPVPRankCfg(int level, int rank){
		LinkedHashMap<Integer, PVPRankRewardCfg> levelList = null;
		for (Map.Entry<Integer, LinkedHashMap<Integer, PVPRankRewardCfg>> entry: pvpRankCfgList.entrySet()) {
			if (rank < entry.getKey()) {
				break;
			}

			levelList = entry.getValue();
		}

		PVPRankRewardCfg pvpRankCfg = null;
		if (levelList != null) {
			for (Map.Entry<Integer, PVPRankRewardCfg> entry : levelList.entrySet()) {
				if (level < entry.getKey()) {
					break;
				}

				pvpRankCfg = entry.getValue();
			}
		}

		return pvpRankCfg;
	}

	@Override
	protected boolean assemble() {
		LinkedHashMap<Integer, PVPRankRewardCfg> levelList = pvpRankCfgList.get(rank);
		if (levelList == null) {
			levelList = new LinkedHashMap<>();
			pvpRankCfgList.put(rank, levelList);
		}

		levelList.put(level, this);

		return true;
	}

	@Override
	protected boolean checkValid() {
		if (HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, reward1) == null) {
			return false;
		}

		if (HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, reward2) == null) {
			return false;
		}

		return true;
	}
}
