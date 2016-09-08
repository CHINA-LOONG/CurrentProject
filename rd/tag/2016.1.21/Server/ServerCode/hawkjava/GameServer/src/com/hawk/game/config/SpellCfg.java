package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/spell.csv", struct = "map")
public class SpellCfg extends HawkConfigBase{
	/**
	 * itemId
	 */
	@Id
	protected final String id;
	
	/**
	 * 分类
	 */
	protected final int category;
	
	//客户端
	protected final String rootEffectID;
	protected final int actionCount;	
	protected final int channelTime;	
	protected final int energyCost;	
	protected final int energyGenerate;	
	protected final float levelAdjust;	
	protected final String icon;	
	protected final String name;	
	protected final String tips;	
	protected final float baseTipValue;	
	protected final int isAoe;	
	protected final String firstSpell;	
	protected final String tipsDescription;	
	protected final String tipsCurlvl;	
	protected final String tipsNextlvl;
	
	public SpellCfg() {
		super();
		this.id = null;
		this.category = 0;
		this.rootEffectID = null;
		this.actionCount = 0;
		this.channelTime = 0;
		this.energyCost = 0;
		this.energyGenerate = 0;
		this.levelAdjust = 0;
		this.icon = null;
		this.name = null;
		this.tips = null;
		this.baseTipValue = 0;
		this.isAoe = 0;
		this.firstSpell = null;
		this.tipsDescription = null;
		this.tipsCurlvl = null;
		this.tipsNextlvl = null;
	}

	public String getId() {
		return id;
	}

	public String getRootEffectID() {
		return rootEffectID;
	}

	public int getActionCount() {
		return actionCount;
	}

	public int getChannelTime() {
		return channelTime;
	}

	public int getEnergyCost() {
		return energyCost;
	}

	public int getEnergyGenerate() {
		return energyGenerate;
	}

	public float getLevelAdjust() {
		return levelAdjust;
	}

	public String getIcon() {
		return icon;
	}

	public String getName() {
		return name;
	}

	public String getTips() {
		return tips;
	}

	public float getBaseTipValue() {
		return baseTipValue;
	}

	public int getIsAoe() {
		return isAoe;
	}

	public String getFirstSpell() {
		return firstSpell;
	}

	public String getTipsDescription() {
		return tipsDescription;
	}

	public String getTipsCurlvl() {
		return tipsCurlvl;
	}

	public String getTipsNextlvl() {
		return tipsNextlvl;
	}

	public int getCategory() {
		return category;
	}

	
}
