package com.hawk.game.config;

import com.hawk.game.util.GsConst.EvolutionType;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;

@HawkConfigManager.XmlResource(file = "xml/monsterAttr.xml", struct = "map")
public class MonsterAttr extends HawkConfigBase {
	
	@Id
	protected final String id ;
	
	protected final String nickname;
	
	protected final String grade;
	
	protected final int evolutionType;
	
	protected final String evolutionValue;
	
	protected final String equipID; 

	protected final String captainSkill; 
	
	protected final String[] skillList; 
	
	protected final float maxLifeModifyRate; 
	
	protected final float initLifeRate; 
	
	protected final float experienceModifyRate; 
	
	protected final float strengthModifyRate; 
	
	protected final float intelligenceModifyRate; 
	
	protected final float speedModifyRate; 
	
	protected final float resistanceModifyRate; 
	
	protected final float enduranceModifyRate; 
	
	protected final float hitRateModifyVal; 
	
	protected final float criticalRateModifyVal; 
	
	protected final float criticalFactorModifyVal; 
	
	protected final float goldNoteValueModifyRate; 
	
	protected final float expValueModifyRate; 
	
	protected final float recoveryRate; 
	
	/**
	 * 全局静态对象
	 */
	private static MonsterAttr instance = null;

	/**
	 * 获取全局静态对象
	 * 
	 * @return
	 */
	public static MonsterAttr getInstance() {
		return instance;
	}

	public MonsterAttr() {
		instance = this;
		id = "";
		nickname = "";
		grade = "";
		evolutionType = EvolutionType.NULL_EVOLUTION;
		evolutionValue = "";
		equipID = "";
		captainSkill = "";
		skillList = null;
		maxLifeModifyRate = 1.0f; 
		initLifeRate = 1.0f;
		experienceModifyRate = 1.0f;
		strengthModifyRate = 1.0f;
		intelligenceModifyRate = 1.0f;
		speedModifyRate = 1.0f; 
		resistanceModifyRate = 1.0f;
		enduranceModifyRate = 1.0f; 
		hitRateModifyVal = 1.0f;
		criticalRateModifyVal = 1.0f;
		criticalFactorModifyVal = 1.0f;
		goldNoteValueModifyRate = 1.0f;
		expValueModifyRate = 1.0f;
		recoveryRate = 1.0f;
	}	
}
