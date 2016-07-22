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
}
