package com.hawk.game.config;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.util.HawkJsonUtil;

import com.google.gson.reflect.TypeToken;
import com.hawk.game.util.GsConst;

@HawkConfigManager.CsvResource(file = "staticData/unitData.csv", struct = "map")
public class MonsterCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final String nickName;
	protected final int type;
	protected final int rarity;
	protected final int property;
	protected final float levelUpExpRate;
	protected final float healthModifyRate;
	protected final float strengthModifyRate;
	protected final float intelligenceModifyRate;
	protected final float speedModifyRate;
	protected final float defenseModifyRate;
	protected final float enduranceModifyRate;
	protected final float recoveryRate;
	protected final int equip;
	protected final String spellIDList;
	protected final int disposition;
	protected final String fragmentId;
	protected final int fragmentCount;

	//client only
	protected final String assetID = null;
	protected final String uiAsset = null;
	protected final int isEvolutionable = 0;
	protected final String evolutionID = null;
	protected final int friendship = 0;
	protected final String weakpointList = null;
	protected final String closeUp = null;
	protected final String say = null;
	protected final String monsterfounds = null;

	// assemble
	protected int[] skillIdList;
	protected ItemCfg fragment;

	public MonsterCfg() {
		id = "";
		nickName = "";
		type = 0;
		rarity = 0;
		property = 0;
		levelUpExpRate = 1.0f;
		healthModifyRate = 1.0f;
		strengthModifyRate = 1.0f;
		intelligenceModifyRate = 1.0f;
		speedModifyRate = 1.0f;
		defenseModifyRate = 1.0f;
		enduranceModifyRate = 1.0f;
		recoveryRate = 1.0f;
		equip = 0;
		spellIDList = "";
		disposition = 0;
		fragmentId = "";
		fragmentCount = 0;
	}

	@Override
	protected boolean assemble() {
		return true;
	}

	@Override
	protected boolean checkValid() {
		// 检测碎片是否存在，并建立引用
		fragment = null;
		if (fragmentId != null && fragmentId.equals("") == false) {
			fragment = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, fragmentId);
			if (null == fragment) {
				HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", fragmentId));
				return false;
			}
		} else if (fragmentCount != GsConst.UNUSABLE) {
			return false;
		}

		// 检测技能，textId转为numId
		String[] textSkillIdList = HawkJsonUtil.getJsonInstance().fromJson(spellIDList, new TypeToken<String[]>() {}.getType());
		skillIdList = new int[textSkillIdList.length];
		for (int i = 0; i < textSkillIdList.length; ++i) {
			SpellCfg skillCfg = SpellCfg.getCfg(textSkillIdList[i]);
			if (null == skillCfg) {
				HawkLog.errPrintln(String.format("config invalid SpellCfg : %s", textSkillIdList[i]));
				return false;
			}
			skillIdList[i] = skillCfg.getId();
		}

		return true;
	}

	public String getId() {
		return id;
	}

	public String getNickname() {
		return nickName;
	}

	public int getType() {
		return type;
	}

	public int getRarity() {
		return rarity;
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

	public float getRecoveryRate() {
		return recoveryRate;
	}

	public int getEquip() {
		return equip;
	}

	public int[] getSkillIdList() {
		return skillIdList;
	}

	public int getDisposition() {
		return disposition;
	}

	public String getFragmentId() {
		return fragmentId;
	}

	public int getFragmentCount() {
		return fragmentCount;
	}
}
