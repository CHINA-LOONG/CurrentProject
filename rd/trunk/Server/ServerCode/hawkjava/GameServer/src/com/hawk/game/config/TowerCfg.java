package com.hawk.game.config;

import java.util.List;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase.Id;

@HawkConfigManager.CsvResource(file = "staticData/tower.csv", struct = "map")
public class TowerCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final String time;
	protected final int level;
	protected final String floor;

	// assemble
	protected List<Integer> refreshTimeList;
	protected List<String> instanceList;

	public TowerCfg() {
		id = "";
		time = "";
		level = 0;
		floor = "";
	}

	@Override
	protected boolean assemble() {
		return true;
	}

	@Override
	protected boolean checkValid() {
		return true;
	}

	public String getId() {
		return id;
	}

}