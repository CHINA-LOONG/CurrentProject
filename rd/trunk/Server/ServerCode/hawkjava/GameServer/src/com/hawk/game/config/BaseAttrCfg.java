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
	protected final float strength;
	/**
	 * 智力
	 */
	protected final float intelligence;
	/**
	 * 速度
	 */
	protected final float speed;
	/**
	 * 防御
	 */
	protected final float defense;
	/**
	 * 体力
	 */
	protected final float health;
	/**
	 * 初始能量
	 */
	protected final float energy;
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
		this.strength = .0f;
		this.intelligence = .0f;
		this.health = .0f;
		this.defense = .0f;
		this.speed = .0f;
		this.energy = .0f;
		this.healRatio = .0f;
		this.criticalDmg = .0f;
		this.criticalRatio = .0f;
	}

	public float getEnergy() {
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

	public float getStrength() {
		return strength;
	}

	public float getIntelligence() {
		return intelligence;
	}

	public float getSpeed() {
		return speed;
	}

	public float getDefense() {
		return defense;
	}

	public float getHealth() {
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
