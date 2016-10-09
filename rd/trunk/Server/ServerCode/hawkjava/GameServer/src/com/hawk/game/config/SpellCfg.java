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
	protected final String name;

	// client only
	protected final String rootEffectID = null;
	protected final int actionCount = 0;
	protected final int channelTime = 0;
	protected final int energyCost = 0;
	protected final int energyGenerate = 0;
	protected final float levelAdjust = 0;
	protected final String icon = null;

	protected final String tips = null;
	protected final float baseTipValue = 0;
	protected final int isAoe = 0;
	protected final String firstSpell = null;
	protected final String tipsDescription = null;
	protected final String tipsCurlvl = null;
	protected final String tipsNextlvl = null;

	public SpellCfg() {
		super();
		this.id = "";
		this.category = 0;
		this.name = "";
	}

	public String getId() {
		return id;
	}

	public int getCategory() {
		return category;
	}

	public String getName() {
		return name;
	}
}
