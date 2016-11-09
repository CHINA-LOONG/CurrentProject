package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.KVResource(file = "sysData/sysBasic.cfg")
public class SysBasicCfg extends HawkConfigBase {
	/**
	 * 玩家对象缓存时间
	 */
	protected final int playerCacheTime;
	/**
	 * 创建公会级别限制
	 */
	protected final int allianceMinLevel;
	/**
	 * 创建公会消耗金币数量
	 */
	protected final int allianceCreateCoin;
	/**
	 * 刷新商店所需钻石
	 */
	protected final int shopRefreshGold;
	/**
	 * 最大缓存数量
	 */
	protected final int maxPlayerSnapShotQty;
	/**
	 * 工会祈福次数
	 */
	protected final int alliancePrayCount;
	/**
	 * 工会冻结时间
	 */
	protected final int allianceFrizenTime;
	/**
	 * 工会疲劳值接受最大值
	 */
	protected final int allianceMaxFatigueReceived;
	/**
	 * 工会冻结时间
	 */
	protected final int allianceMaxApply;
	/**
	 * 最大大任务数量
	 */
	protected final int allianceMaxBigTask;
	/**
	 * 最大小任务数量
	 */
	protected final int allianceMaxSmallTask;
	/**
	 * 解散公会返还比例
	 */
	protected final float allianceDissoveGoldRate;
	/**
	 * 解散公会返还比例
	 */
	protected final float allianceSecondPositionContribution;
	/**
	 * 解散公会返还比例
	 */
	protected final float allianceThirdPositionContribution;
	/**
	 * 工会贡献值奖励1阶贡献值
	 */
	protected final int allianceContributionReward1;
	/**
	 * 工会贡献值奖励2阶贡献值
	 */
	protected final int allianceContributionReward2;
	/**
	 * 工会贡献值奖励3阶贡献值
	 */
	protected final int allianceContributionReward3;
	/**
	 * 工会贡献值奖励1
	 */
	protected final String allianceContributionRewardID1;
	/**
	 * 工会贡献值奖励2
	 */
	protected final String allianceContributionRewardID2;
	/**
	 * 工会贡献值奖励3
	 */
	protected final String allianceContributionRewardID3;
	/**
	 * pvp匹配池初始化大小
	 */
	protected final int pvpStageInitSize;
	/**
	 * pvp匹配池初始化最大值
	 */
	protected final int pvpStageMaxSize;
	/**
	 * pvp排行榜缓存时间
	 */
	protected final int pvpRankBuilderRefreshTime;
	
	/**
	 * 全局静态对象
	 */
	private static SysBasicCfg instance = null;

	/**
	 * 获取全局静态对象
	 * 
	 * @return
	 */
	public static SysBasicCfg getInstance() {
		return instance;
	}

	public SysBasicCfg() {
		instance = this;
		playerCacheTime = 86400000;
		allianceCreateCoin = 0;
		allianceMinLevel = 0;
		shopRefreshGold = 0;
		maxPlayerSnapShotQty = 0;
		alliancePrayCount = 0;
		allianceFrizenTime = 0;
		allianceMaxFatigueReceived = 0;
		allianceMaxApply = 0;
		allianceMaxBigTask = 0;
		allianceMaxSmallTask = 0;
		allianceDissoveGoldRate = .0f;
		allianceSecondPositionContribution = 0;
		allianceThirdPositionContribution = 0;
		pvpStageInitSize = 0;
		pvpStageMaxSize = 0;
		pvpRankBuilderRefreshTime = 0;
		allianceContributionReward1 = 0;
		allianceContributionReward2 = 0;
		allianceContributionReward3 = 0;
		allianceContributionRewardID1 = null;
		allianceContributionRewardID2 = null;
		allianceContributionRewardID3 = null;
	}

	public int getPlayerCacheTime() {
		return playerCacheTime;
	}

	public int getAllianceMinLevel() {
		return allianceMinLevel;
	}

	public int getAllianceCreateCoin() {
		return allianceCreateCoin;
	}

	public int getShopRefreshGold() {
		return shopRefreshGold;
	}

	public int getMaxPlayerSnapShotQty() {
		return maxPlayerSnapShotQty;
	}

	public int getAlliancePrayCount() {
		return alliancePrayCount;
	}

	public int getAllianceFrizenTime() {

		return allianceFrizenTime;
	}

	public int getAllianceMaxFatigueReceived() {

		return allianceMaxFatigueReceived;
	}

	public int getAllianceMaxApply() {

		return allianceMaxApply;
	}

	public int getAllianceMaxBigTask() {
		return allianceMaxBigTask;
	}

	public int getAllianceMaxSmallTask() {
		return allianceMaxSmallTask;
	}

	public float getAllianceDissoveGoldRate() {
		return allianceDissoveGoldRate;
	}

	public float getAllianceSecondPositionContribution() {
		return allianceSecondPositionContribution;
	}

	public float getAllianceThirdPositionContribution() {
		return allianceThirdPositionContribution;
	}

	public int getPvpStageInitSize() {
		return pvpStageInitSize;
	}

	public int getPvpStageMaxSize() {
		return pvpStageMaxSize;
	}

	public int getPvpRankBuilderRefreshTime() {
		return pvpRankBuilderRefreshTime;
	}

	public int getAllianceContributionReward1() {
		return allianceContributionReward1;
	}

	public int getAllianceContributionReward2() {
		return allianceContributionReward2;
	}

	public int getAllianceContributionReward3() {
		return allianceContributionReward3;
	}

	public String getAllianceContributionRewardID1() {
		return allianceContributionRewardID1;
	}

	public String getAllianceContributionRewardID2() {
		return allianceContributionRewardID2;
	}

	public String getAllianceContributionRewardID3() {
		return allianceContributionRewardID3;
	}	
}
