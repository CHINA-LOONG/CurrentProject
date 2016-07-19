package com.hawk.game.config;

import java.util.HashMap;
import java.util.Map;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.util.HawkJsonUtil;

import com.google.gson.reflect.TypeToken;

@HawkConfigManager.CsvResource(file = "sysData/testAccount.csv", struct = "list")
public class TestAccountCfg extends HawkConfigBase {

	protected final String puid;
	protected final String nickname;
	protected final String monsterId;
	protected final int stage;
	protected final int level;
	protected final int exp;
	protected final int lazy;
	protected final int lazyExp;
	protected final int disposition;
	protected final String skillList;

	// assemble
	protected Map<String, Integer> skillLevelMap;

	public TestAccountCfg() {
		puid = "";
		nickname = "";
		monsterId = "";
		stage = 0;
		level = 0;
		exp = 0;
		lazy = 0;
		lazyExp = 0;
		disposition = 0;
		skillList = "";
	}

	@Override
	protected boolean assemble() {
		skillLevelMap = HawkJsonUtil.getJsonInstance().fromJson(skillList, new TypeToken<HashMap<String, Integer>>() {}.getType());
		return true;
	}

	@Override
	protected boolean checkValid() {
		return true;
	}

	public String getPuid() {
		return puid;
	}

	public String getNickname() {
		return nickname;
	}

	public String getMonsterId() {
		return monsterId;
	}

	public byte getStage() {
		return (byte)stage;
	}

	public short getLevel() {
		return (short)level;
	}

	public int getExp() {
		return exp;
	}

	public byte getLazy() {
		return (byte)lazy;
	}

	public int getLazyExp() {
		return lazyExp;
	}

	public byte getDisposition() {
		return (byte)disposition;
	}

	public  Map<String, Integer> getSkillLevelMap() {
		return skillLevelMap;
	}
}
