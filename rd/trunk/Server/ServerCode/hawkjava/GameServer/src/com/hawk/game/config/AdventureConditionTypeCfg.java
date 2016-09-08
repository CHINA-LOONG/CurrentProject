package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/adventureConditionType.csv", struct = "map")
public class AdventureConditionTypeCfg extends HawkConfigBase {

	@Id
	protected final int id;
	protected final String level;
	protected final int monsterType;
	protected final int monsterProperty;
	protected final int weight;

	// client only
	protected final String comments = null;
	protected final String desc = null;

	// assemble
	protected int minLevel;
	protected int maxLevel;

	public AdventureConditionTypeCfg() {
		id = 0;
		level = "";
		monsterType = 0;
		monsterProperty = 0;
		weight = 0;
	}

	@Override
	protected boolean assemble() {
		String[] levelRange = level.split(",");
		if (levelRange.length != 2) {
			return false;
		}
		minLevel = Integer.parseInt(levelRange[0]);
		maxLevel = Integer.parseInt(levelRange[1]);

		return true;
	}

	@Override
	protected boolean checkValid() {
		return true;
	}

	public int getId() {
		return id;
	}

	public int getMonsterType() {
		return monsterType;
	}

	public int getMonsterProperty() {
		return monsterProperty;
	}

	public int getWeight() {
		return weight;
	}

	public boolean isInLevelRange(int level) {
		return (level >= minLevel && level <= maxLevel) ? true : false;
	}
}
