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
}
