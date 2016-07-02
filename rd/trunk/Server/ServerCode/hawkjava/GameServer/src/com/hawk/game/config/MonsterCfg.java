package com.hawk.game.config;

import net.sf.json.JSONArray;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;

@HawkConfigManager.CsvResource(file = "xml/unitData.csv", struct = "map")
public class MonsterCfg extends HawkConfigBase {
	
	@Id
	protected final String index ;
	protected final String assetID;
	protected final String nickName;
	protected final int grade;
	protected final int isEvolutionable;
	protected final String evolutionID;
	protected final int property;
	protected final float levelUpExpRate;	
	protected final float healthModifyRate;	
	protected final float strengthModifyRate;	
	protected final float intelligenceModifyRate;	
	protected final float speedModifyRate; 	
	protected final float defenseModifyRate;	
	protected final float enduranceModifyRate;	
	protected final float goldNoteMinValueModifyRate;	
	protected final float goldNoteMaxValueModifyRate;	
	protected final float expMinValueModifyRate;	
	protected final float expMaxValueModifyRate;	
	protected final float recoveryRate;
	protected final int equip;	
	protected final String spellIDList;	
	protected final int lazy;	
	protected final int AI;
	protected final String weakpointList;
	
	// assemble
	boolean canEvolve;
	private String[] spellIdListAssemble;
	private String[] weakpointListAssemble;
	
	/**
	 * 全局静态对象
	 */
	private static MonsterCfg instance = null;

	/**
	 * 获取全局静态对象
	 * 
	 * @return
	 */
	public static MonsterCfg getInstance() {
		return instance;
	}

	public MonsterCfg() {
		instance = this;
		index = "";
		assetID = "";
		nickName = "";
		grade = 0;
		isEvolutionable = 0;
		evolutionID = "";
		property = 0;
		levelUpExpRate = 1.0f;
		healthModifyRate = 1.0f;
		strengthModifyRate = 1.0f;
		intelligenceModifyRate = 1.0f;
		speedModifyRate = 1.0f;
		defenseModifyRate = 1.0f;
		enduranceModifyRate = 1.0f;
		goldNoteMinValueModifyRate = 1.0f;
		goldNoteMaxValueModifyRate = 1.0f;
		expMinValueModifyRate = 1.0f;
		expMaxValueModifyRate = 1.0f;
		recoveryRate = 1.0f;
		equip = 0;
		spellIDList = "";
		lazy = 0;
		AI = 0;
		weakpointList = "";
	}
	
	@Override
	protected boolean assemble() {
		canEvolve = (isEvolutionable != 0);
		
		JSONArray jsonArray = JSONArray.fromObject(spellIDList);
		if (jsonArray == null || jsonArray.isArray() == false) {
			return false;
		}
		spellIdListAssemble = new String[jsonArray.size()];
		for (int i = 0; i < jsonArray.size(); ++i) {
			spellIdListAssemble[i] = jsonArray.getString(i);
		}
		
		jsonArray = JSONArray.fromObject(weakpointList);
		if (jsonArray == null || jsonArray.isArray() == false) {
			return false;
		}
		weakpointListAssemble = new String[jsonArray.size()];
		for (int i = 0; i < jsonArray.size(); ++i) {
			weakpointListAssemble[i] = jsonArray.getString(i);
		}
		
		return true;
	}
	
	public String getId() {
		return index;
	}
	
	public String getNickname() {
		return nickName;
	}
	
	public int getGrade() {
		return grade;
	}
	
	public boolean getCanEvolve() {
		return canEvolve;
	}
	
	public String getEvolveId() {
		return evolutionID;
	}
	
	public int getProperty() {
		return property;
	}
	
	public float getNextExpRate() {
		return levelUpExpRate;
	}
	
	public float getHealthModifyRate() {
		return healthModifyRate;
	}
	
	public float getStrengthModifyRate() {
		return strengthModifyRate;
	}
	
	public float getIntelligenceModifyRate() {
		return intelligenceModifyRate;
	}
	
	public float getSpeedModifyRate() {
		return speedModifyRate;
	}
	
	public float getDefenseModifyRate() {
		return defenseModifyRate;
	}
	
	public float getEnduranceModifyRate() {
		return enduranceModifyRate;
	}
	
	public float getGoldNoteMinModifyRate() {
		return goldNoteMinValueModifyRate;
	}
	
	public float getGoldNoteMaxModifyRate() {
		return goldNoteMaxValueModifyRate;
	}
	
	public float getExpMinModifyRate() {
		return expMinValueModifyRate;
	}
	
	public float getExpMaxModifyRate() {
		return expMaxValueModifyRate;
	}
	
	public float getRecoveryRate() {
		return recoveryRate;
	}
	
	public int getEquip() {
		return equip;
	}
	
	public String[] getSpellIdList() {
		return spellIdListAssemble;
	}
	
	public int getLazy() {
		return lazy;
	}
	
	public int getAi() {
		return AI;
	}
	
	public String[] getWeakpointList() {
		return weakpointListAssemble;
	}
	
}
