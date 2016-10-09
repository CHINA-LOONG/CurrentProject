package com.hawk.game.config;

import java.util.Collections;
import java.util.HashMap;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.util.HawkJsonUtil;

import com.google.gson.reflect.TypeToken;

@HawkConfigManager.CsvResource(file = "staticData/battleLevel.csv", struct = "map")
public class BattleLevelCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final String monsterList;

	// client only
	protected final String preStartEvent = null;
	protected final String startEvent = null;
	protected final String endStartEvent = null;
	protected final int appearType = 0;
	protected final String winFunc = null;
	protected final String loseFunc = null;
	protected final String endEvent = null;

	// assemble
	protected Map<String, Integer> monsterMap;

	public BattleLevelCfg() {
		id = "";
		monsterList = "";
	}

	@Override
	protected boolean assemble() {
		monsterMap = HawkJsonUtil.getJsonInstance().fromJson(monsterList, new TypeToken<HashMap<String, Integer>>() {}.getType());
		return true;
	}

	@Override
	protected boolean checkValid() {
		for (Entry<String, Integer> entry : monsterMap.entrySet()) {
			MonsterCfg monster = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, entry.getKey());
			if (null == monster) {
				HawkLog.errPrintln(String.format("config invalid MonsterCfg : %s", entry.getKey()));
				return false;
			}
		}
		return true;
	}

	public String getId() {
		return id;
	}

	public Map<String, Integer> getMonsterMap() {
		return Collections.unmodifiableMap(monsterMap);
	}
}
