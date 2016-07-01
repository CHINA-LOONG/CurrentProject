package com.hawk.game.config;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;

@HawkConfigManager.XmlResource(file = "xml/monsterGrade.xml", struct = "map")
public class MonsterGrade extends HawkConfigBase {

	@Id
	protected final String id ;
	
	protected final int exp;
	
	protected final int life;
	
	protected final int strength;

	protected final int intelligence;
	
	protected final int speed;
	
	protected final int resistance;
	
	protected final int endurance;
	
	protected final int disposition;
	
	protected final int recovery;
	
	protected final float hitRate;
	
	protected final float criticalRate;
	
	protected final float criticalFactor;
	
	protected final int goldNoteValue;
	
	protected final int[] goldNoteRandom;
	
	protected final int expValue;
	
	protected final int[] expRandom;
	
	/**
	 * 全局静态对象
	 */
	private static MonsterGrade instance = null;

	/**
	 * 获取全局静态对象
	 * 
	 * @return
	 */
	public static MonsterGrade getInstance() {
		return instance;
	}

	public MonsterGrade() {
		instance = this;
		id = "";
		exp = 0;
		life = 0;
		strength = 0;
		intelligence = 0;
		speed = 0;
		resistance = 0;
		endurance = 0;
		disposition = 0;
		recovery = 0;
		hitRate = 0.0f;
		criticalRate = 0.0f;
		criticalFactor = 0.0f;
		goldNoteValue = 0;
		goldNoteRandom = null;
		expValue = 0;
		expRandom = null;
	}
	
	//合成配置文件的key
	public String GenerateKey(String grade, int level)
	{
		return grade + "_" + level;
	}
	
}
