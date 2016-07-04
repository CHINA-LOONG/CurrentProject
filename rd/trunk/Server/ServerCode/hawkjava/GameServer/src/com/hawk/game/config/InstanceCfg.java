package com.hawk.game.config;

import java.util.HashMap;
import java.util.Map;
import java.util.Map.Entry;

import net.sf.json.JSONArray;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;

@HawkConfigManager.CsvResource(file = "xml/instance.csv", struct = "map")
public class InstanceCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final String name;
	protected final int level;
	protected final float lifeCoef;
	protected final float attackCoef;
	protected final float expCoef;
	protected final float goldCoef;
	protected final String sceneBattle;
	protected final int sceneAmount;
	protected final int monsterAmount;
	protected final String monster1;
	protected final int monster1Amount;
	protected final String monster2;
	protected final int monster2Amount;
	protected final String monster3;
	protected final int monster3Amount;
	protected final String monster4;
	protected final int monster4Amount;
	protected final String monster5;
	protected final int monster5Amount;
	
	// client only
	protected final String normalValiVic = "";
	protected final String bossValiVic = "";
	protected final String bossID = "";
	protected final String bossStoryStartAnimation = null;
	protected final String bossStoryEndAnimation = null;	
	protected final String pre1Animation = null;
	protected final String process1Animation = null;
	protected final int is1ClearBuff = 0;
	protected final String bossValiP1 = null;
	protected final String pre2Animation = null;
	protected final String process2Animation = null;
	protected final int is2ClearBuff = 0;
	protected final String bossValiP2 = null;
	protected final String pre3Animation = null;
	protected final String process3Animation = null;
	protected final int is3ClearBuff = 0;
	protected final String bossValiP3 = null;
	protected final String pre4Animation = null;
	protected final String process4Animation = null;
	protected final int is4ClearBuff = 0;
	protected final String bossValiP4 = null;
	protected final String pre5Animation = null;
	protected final String process5Animation = null;
	protected final int is5ClearBuff = 0;
	protected final String bossValiP5 = null;
	protected final String rareValiVic = null;
	protected final String rareID = null;
	protected final float rareProbability = 0;
	protected final String rareStoryStartAnimation = null;
	protected final String rareStoryEndAnimation = null;
	protected final String preRare1Animation = null;
	protected final String processRare1Animation = null;
	protected final int isRare1ClearBuff = 0;
	protected final String rareValiP1 = null;
	protected final String preRare2Animation = null;
	protected final String processRare2Animation = null;
	protected final int isRare2ClearBuff = 0;
	protected final String rareValiP2 = null;
	protected final String preRare3Animation = null;
	protected final String processRare3Animation = null;
	protected final int isRare3ClearBuff = 0;
	protected final String rareValiP3 = null;
	protected final String preRare4Animation = null;
	protected final String processRare4Animation = null;
	protected final int isRare4ClearBuff = 0;
	protected final String rareValiP4 = null;
	protected final String preRare5Animation = null;
	protected final String processRare5Animation = null;
	protected final int isRare5ClearBuff = 0;
	protected final String rareValiP5 = null;
		
	// assemble
	private Map<String, Integer> monsterAmountList;
	
	/**
	 * 全局静态对象
	 */
	private static InstanceCfg instance = null;

	/**
	 * 获取全局静态对象
	 */
	public static InstanceCfg getInstance() {
		return instance;
	}

	public InstanceCfg() {
		instance = this;
		id = "";
		name = "";
		level = 0;
		lifeCoef = 1.0f;
		attackCoef = 1.0f;
		expCoef = 1.0f;
		goldCoef = 1.0f;
		sceneBattle = "";
		sceneAmount = 0;
		monsterAmount = 0;
		monster1 = "";
		monster1Amount = 0;;
		monster2 = "";
		monster2Amount = 0;
		monster3 = "";
		monster3Amount = 0;
		monster4 = "";
		monster4Amount = 0;
		monster5 = "";
		monster5Amount = 0;
	}
	
	@Override
	protected boolean assemble() {
		monsterAmountList =  new HashMap<String, Integer>();
		if (monster1 != "" && monster1Amount > 0) {
			monsterAmountList.put(monster1, monster1Amount);
		}
		if (monster2 != "" && monster2Amount > 0) {
			monsterAmountList.put(monster2, monster2Amount);
		}
		if (monster3 != "" && monster3Amount > 0) {
			monsterAmountList.put(monster3, monster3Amount);
		}
		if (monster4 != "" && monster4Amount > 0) {
			monsterAmountList.put(monster4, monster4Amount);
		}
		if (monster5 != "" && monster5Amount > 0) {
			monsterAmountList.put(monster5, monster5Amount);
		}

		return true;
	}
	
	@Override
	protected boolean checkValid() {
		// TODO:检查掉落组
		
		int sum = 0;
		for (Entry<String, Integer> entry : monsterAmountList.entrySet()) {
			MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, entry.getKey());
			if(monsterCfg == null) {
				return false;
			}
			sum += entry.getValue();
		}
		
		if (sceneAmount * monsterAmount != sum) {
			return false;
		}
		
		return true;
	}

	public String getId() {
		return id;
	}
	
	public String getName() {
		return name;
	}
	
	public int getLevel() {
		return level;
	}
	
	public float getLifeCoef() {
		return lifeCoef;
	}
	
	public float getAttackCoef() {
		return attackCoef;
	}
	
	public float getExpCoef() {
		return expCoef;
	}
	
	public float getGoldCoef() {
		return goldCoef;
	}
	
	public String getBattleScene() {
		return sceneBattle;
	}
	
	public int getBattleAmount() {
		return sceneAmount;
	}
	
	public int getBattleMonsterAmount() {
		return monsterAmount;
	}
	
	public Map<String,Integer> getMonsterAmountList() {
		return monsterAmountList;
	}
	
	public int getMonsterAmount(String monsterId) {
		return monsterAmountList.get(monsterId);
	}
}
