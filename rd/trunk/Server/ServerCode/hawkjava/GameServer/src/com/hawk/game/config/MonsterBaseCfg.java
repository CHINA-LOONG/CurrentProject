package com.hawk.game.config;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;

@HawkConfigManager.CsvResource(file = "staticData/unitBaseData.csv", struct = "map")
public class MonsterBaseCfg extends HawkConfigBase {

	@Id
	protected final int level;
	protected final int experience;
	protected final int health;
	protected final int strength;
	protected final int intelligence;
	protected final int speed;
	protected final int defense;
	protected final int endurance;
	protected final int recovery;

	public MonsterBaseCfg() {
		level = 0;
		experience = 0;
		health = 0;
		strength = 0;
		intelligence = 0;
		speed = 0;
		defense = 0;
		endurance = 0;
		recovery = 0;
	}

	public int getLevel() {
		return level;
	}

	public int getNextExp() {
		return experience;
	}

	public int getHealth() {
		return health;
	}

	public int getStrength() {
		return strength;
	}

	public int getIntelligence() {
		return intelligence;
	}

	public int getSpeed() {
		return speed;
	}

	public int getDefense() {
		return defense;
	}

	public int getEndurance() {
		return endurance;
	}

	public int getRecovery() {
		return recovery;
	}

}
