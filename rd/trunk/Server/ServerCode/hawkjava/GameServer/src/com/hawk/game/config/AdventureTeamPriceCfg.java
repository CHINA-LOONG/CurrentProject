package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/adventureTeamPrice.csv", struct = "map")
public class AdventureTeamPriceCfg extends HawkConfigBase {

	@Id
	protected final int id;
	protected final int gold;

	public AdventureTeamPriceCfg() {
		id = 0;
		gold = 0;
	}

	public int getId() {
		return id;
	}

	public int getGold() {
		return gold;
	}

}
