package com.hawk.game.config;

import java.util.LinkedList;
import java.util.List;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

import com.hawk.game.util.GsConst;

@HawkConfigManager.CsvResource(file = "staticData/sociatyTask.csv", struct = "map")
public class SociatyTaskCfg extends HawkConfigBase{
	/**
	 * 配置id
	 */
	@Id
	protected final int id;

	/**
	 * 最低等级
	 */
	protected final int minLevel;

	/**
	 * 描述
	 */
	protected final String taskDesc;

	/**
	 * 任务时长
	 */
	protected final int time;

	/**
	 * 任务名称
	 */
	protected final String taskName;

	/**
	 *开任务消耗
	 */
	protected final int taskStart;

	/**
	 * 队长奖励
	 */
	protected final String leaderReward;

	/**
	 * 成员奖励
	 */
	protected final String reward;

	/**
	 * 道具组
	 */
	protected final String itemTask;

	/**
	 * 金币组
	 */
	protected final String coinTask;

	/**
	 * 副本组
	 */
	protected final String instanceTask;

	// assemble
	protected RewardCfg rewardCfg = null;
	protected RewardCfg leaderRewardCfg = null;
	/**
	 * 道具组列表
	 */
	private  List<Integer> itemTaskSet = new LinkedList<Integer>();
	/**
	 * 金币组列表
	 */
	private  List<Integer> coinTaskSet = new LinkedList<Integer>();
	/**
	 * 副本组列表
	 */
	private  List<Integer> instanceTaskSet = new LinkedList<Integer>();

	public SociatyTaskCfg(){
		id = 0;
		taskDesc = null;
		minLevel = 0;
		time = 0;
		taskName = null;
		taskStart = 0;
		leaderReward = null;
		reward = null;
		itemTask = null;
		coinTask = null;
		instanceTask = null;
	}

	public int getId() {
		return id;
	}

	public String getTaskDesc() {
		return taskDesc;
	}

	public int getTime() {
		return time;
	}

	public int getMinLevel() {
		return minLevel;
	}

	public String getTaskName() {
		return taskName;
	}

	public int getTaskStart() {
		return taskStart;
	}

	public RewardCfg getLeaderReward() {
		return leaderRewardCfg;
	}

	public RewardCfg getReward() {
		return rewardCfg;
	}

	public String getItemTask() {
		return itemTask;
	}

	public String getCoinTask() {
		return coinTask;
	}

	public String getInstanceTask() {
		return instanceTask;
	}

	public List<Integer> getItemTaskSet() {
		return itemTaskSet;
	}

	public List<Integer> getCoinTaskSet() {
		return coinTaskSet;
	}

	public List<Integer> getInstanceTaskSet() {
		return instanceTaskSet;
	}

	@Override
	protected boolean assemble() {
		// 道具任务
		if (this.itemTask != null && this.itemTask.length() > 0 && !"0".equals(this.itemTask)) {
			String[] items = itemTask.split("_");
			for (String item : items) {
				itemTaskSet.add(Integer.valueOf(item));
			}
		}

		// 金币任务
		if (this.coinTask != null && this.coinTask.length() > 0 && !"0".equals(this.coinTask)) {
			String[] items = coinTask.split("_");
			for (String item : items) {
				coinTaskSet.add(Integer.valueOf(item));
			}
		}

		// 副本任务
		if (this.instanceTask != null && this.instanceTask.length() > 0 && !"0".equals(this.instanceTask)) {
			String[] items = instanceTask.split("_");
			for (String item : items) {
				instanceTaskSet.add(Integer.valueOf(item));
			}
		}

		if (itemTaskSet.size() < 3) {
			return false;
		}
		if (coinTaskSet.size() < 2) {
			return false;
		}
		if (instanceTaskSet.size() < 1) {
			return false;
		}
		return true;
	}

	@Override
	protected boolean checkValid(){
		rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, reward);
		if (null == rewardCfg) {
			HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", reward));
			return false;
		}

		leaderRewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, leaderReward);
		if (null == leaderRewardCfg) {
			HawkLog.errPrintln(String.format("config invalid RewardCfg : %s", leaderReward));
			return false;
		}

		for (int element : itemTaskSet) {
			if (HawkConfigManager.getInstance().getConfigByKey(SociatyQuestCfg.class, element) == null || 
				HawkConfigManager.getInstance().getConfigByKey(SociatyQuestCfg.class, element).getGoalType() != GsConst.AllianceQuestType.ITEM_QUEST) {
				return false;
			}
		}

		for (int element : coinTaskSet) {
			if (HawkConfigManager.getInstance().getConfigByKey(SociatyQuestCfg.class, element) == null || 
					HawkConfigManager.getInstance().getConfigByKey(SociatyQuestCfg.class, element).getGoalType() != GsConst.AllianceQuestType.COIN_QUEST) {
					return false;
				}
		}

		for (int element : instanceTaskSet) {
			if (HawkConfigManager.getInstance().getConfigByKey(SociatyQuestCfg.class, element) == null || 
					HawkConfigManager.getInstance().getConfigByKey(SociatyQuestCfg.class, element).getGoalType() != GsConst.AllianceQuestType.INSTANCE_QUEST) {
					return false;
				}
		}

		return true;
	}
}
