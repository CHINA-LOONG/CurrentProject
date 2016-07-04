package com.hawk.game.config;
import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "xml/baseAttr.csv", struct = "map")
public class BaseAttrCfg extends HawkConfigBase{
	/**
	 * 配置id
	 */
	@Id
	protected final int id ;
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
	
	public BaseAttrCfg(){
		this.id = 0;
		this.strength = 0;
		this.intelligence = 0;
		this.health = 0;
		this.defense = 0;
		this.speed = 0;
	}

	public int getId() {
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
