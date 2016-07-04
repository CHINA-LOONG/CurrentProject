package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "xml/instanceDrop.csv", struct = "map")
public class InstanceDropCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final int rewardId;
	
	// assemble
	protected RewardCfg rewardCfg;
	
	/**
	 * 全局静态对象
	 */
	private static InstanceDropCfg instance = null;

	/**
	 * 获取全局静态对象
	 */
	public static InstanceDropCfg getInstance() {
		return instance;
	}

	public InstanceDropCfg() {
		instance = this;
		id = "";
		rewardId = 0;
	}
	
	@Override
	protected boolean assemble() {
		return true;
	}
	
	@Override
	protected boolean checkValid() {
		// 检测奖励是否存在，并建立引用
		rewardCfg = HawkConfigManager.getInstance().getConfigByKey(RewardCfg.class, rewardId);
		if (null == rewardCfg) {
			return false;
		}
		return true;
	}
	
	public String getInstanceMonsterId() {
		return id;
	}
	
	public RewardCfg getReward() {
		return rewardCfg;
	}
	
}
