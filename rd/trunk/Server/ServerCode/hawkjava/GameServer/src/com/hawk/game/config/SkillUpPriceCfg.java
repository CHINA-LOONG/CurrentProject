package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "xml/skillUpPrice.csv", struct = "map")
public class SkillUpPriceCfg extends HawkConfigBase {

	@Id
	protected final int level;
	protected final int coin;

	public SkillUpPriceCfg() {
		level = 0;
		coin = 0;
	}

	public int getLevel() {
		return level;
	}

	public int getCoin() {
		return coin;
	}

}
