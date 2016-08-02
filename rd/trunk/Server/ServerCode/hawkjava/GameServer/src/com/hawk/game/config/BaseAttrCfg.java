package com.hawk.game.config;
import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/baseAttr.csv", struct = "map")
public class BaseAttrCfg extends HawkConfigBase{
	/**
	 * 配置id
	 */
	@Id
	protected final String id;
	/**
	 * 力量
	 */
	protected final int strength;
	/**
	 * 智力
	 */
	protected final int intelligence;
	/**
	 * 速度
	 */
	protected final int speed;
	/**
	 * 防御
	 */
	protected final int defense;
	/**
	 * 体力
	 */
	protected final int health;
	/**
	 * 初始能量
	 */
	protected final int energy;
	/**
	 * 被治疗加成
	 */
	protected final float healRatio;
	/**
	 * 暴击率
	 */
	protected final float criticalRatio;
	/**
	 * 暴击加成
	 */
	protected final float criticalDmg;
	
	public BaseAttrCfg(){
		this.id = null;
		this.strength = 0;
		this.intelligence = 0;
		this.health = 0;
		this.defense = 0;
		this.speed = 0;
		this.energy = 0;
		this.healRatio = .0f;
		this.criticalDmg = .0f;
		this.criticalRatio = .0f;
	}

	public int getEnergy() {
		return energy;
	}

	public float getHealRatio() {
		return healRatio;
	}

	public float getCriticalRatio() {
		return criticalRatio;
	}

	public float getCriticalDmg() {
		return criticalDmg;
	}

	public String getId() {
		return id;
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

	public int getHealth() {
		return health;
	}
	
	@Override
	protected boolean assemble() {
		return true;
	}

	@Override
	protected boolean checkValid() {
		return true;
	}
}
