package com.hawk.game.config;

import java.util.ArrayList;
import java.util.List;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/adventureConditionNum.csv", struct = "map")
public class AdventureConditionNumCfg extends HawkConfigBase {

	@Id
	protected final int id;
	protected final String level;
	protected final String num;
	protected final int weight;

	// assemble
	protected int minLevel;
	protected int maxLevel;
	protected List<Integer> monsterCountList;

	public AdventureConditionNumCfg() {
		id = 0;
		level = "";
		num = "";
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
		
		String[] numList = num.split("_");
		if (numList.length != 3) {
			return false;
		}

		monsterCountList = new ArrayList<>();
		for (String numString : numList) {
			int num = Integer.parseInt(numString);
			if (num > 0) {
				monsterCountList.add(num);
			}
		}

		return true;
	}

	@Override
	protected boolean checkValid() {
		return true;
	}

	public int getId() {
		return id;
	}

	public int getWeight() {
		return weight;
	}

	public List<Integer> getMonsterCountList() {
		return monsterCountList;
	}

	public boolean isInLevelRange(int level) {
		return (level >= minLevel && level <= maxLevel) ? true : false;
	}

}
