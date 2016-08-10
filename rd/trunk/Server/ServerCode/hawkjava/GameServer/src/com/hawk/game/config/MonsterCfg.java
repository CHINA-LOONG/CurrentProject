package com.hawk.game.config;

import net.sf.json.JSONArray;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;
import org.hawk.log.HawkLog;

import com.hawk.game.util.GsConst;

@HawkConfigManager.CsvResource(file = "staticData/unitData.csv", struct = "map")
public class MonsterCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final String assetID;
	protected final String uiAsset;
	protected final String nickName;
	protected final int type;
	protected final int rarity;
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
	protected final float recoveryRate;
	protected final int equip;
	protected final String spellIDList;
	protected final String weakpointList;
	protected final int friendship;
	protected final int disposition;
	protected final String fragmentId;
	protected final int fragmentCount;

	//client only
	protected final String closeUp = null;
	protected final String say = null;
	protected final String monsterfounds = null;

	// assemble
	protected boolean canEvolve;
	protected String[] spellIdListAssemble;
	protected String[] weakpointListAssemble;
	protected ItemCfg fragment;

	public MonsterCfg() {
		id = "";
		assetID = "";
		uiAsset = "";
		nickName = "";
		type = 0;
		rarity = 0;
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
		recoveryRate = 1.0f;
		equip = 0;
		spellIDList = "";
		weakpointList = "";
		friendship = 0;
		disposition = 0;
		fragmentId = "";
		fragmentCount = 0;
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

		return true;
	}

	public String getId() {
		return id;
	}

	public String getNickname() {
		return nickName;
	}

	public int getRarity() {
		return rarity;
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

	public float getRecoveryRate() {
		return recoveryRate;
	}

	public int getEquip() {
		return equip;
	}

	public String[] getSpellIdList() {
		return spellIdListAssemble;
	}

	public String[] getWeakpointList() {
		return weakpointListAssemble;
	}

	public int getFriendship() {
		return friendship;
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
