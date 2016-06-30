package com.hawk.game.config;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;

@HawkConfigManager.XmlResource(file = "xml/item_reward_data.xml", struct = "map")
public class ItemRewardTest extends HawkConfigBase {

	@Id
	protected final String id ;

	protected final String type;

	protected final int level_min;

	protected final int level_max;

	/**
	 * 全局静态对象
	 */
	private static ItemRewardTest instance = null;

	/**
	 * 获取全局静态对象
	 * 
	 * @return
	 */
	public static ItemRewardTest getInstance() {
		return instance;
	}

	public ItemRewardTest() {
		instance = this;
		id = "";
		type = "";
		level_min = 0;
		level_max = 0;
	}
}
