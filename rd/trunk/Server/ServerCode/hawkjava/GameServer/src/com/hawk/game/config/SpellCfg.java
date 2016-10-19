package com.hawk.game.config;

import java.util.HashMap;
import java.util.Map;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

@HawkConfigManager.CsvResource(file = "staticData/spell.csv", struct = "map")
public class SpellCfg extends HawkConfigBase{
	/**
	 * itemId
	 */
	@Id
	protected final int id;
	protected final String textId;

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

	// global
	protected static Map<String, SpellCfg> textSpellCfgMap = new HashMap<>();

	public static SpellCfg getCfg(String textSkillId) {
		return textSpellCfgMap.get(textSkillId);
	}

	public SpellCfg() {
		super();
		this.id = 0;
		this.textId = "";
		this.category = 0;
		this.name = "";
	}

	@Override
	protected boolean assemble() {
		textSpellCfgMap.put(textId, this);

		return true;
	}

	public int getId() {
		return id;
	}

	public String getTextId() {
		return textId;
	}

	public int getCategory() {
		return category;
	}

	public String getName() {
		return name;
	}
}
