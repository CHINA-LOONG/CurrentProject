package com.hawk.game.config;

import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;
import org.hawk.log.HawkLog;
import org.hawk.util.HawkJsonUtil;

import com.google.gson.reflect.TypeToken;

@HawkConfigManager.CsvResource(file = "staticData/instance.csv", struct = "map")
public class InstanceCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final int level;
	protected final String battleLevelList;
	protected final String battleBoss;

	// client only
	protected final float lifeCoef = 0;
	protected final float attackCoef = 0;
	protected final String sceneID = null;
	protected final int dazhaoGroup = 0;
	protected final int dazhaoAdjust = 0;
	protected final String backgroundmusic = null;

	// assemble
	protected List<String> normalBattleIdList;
	protected Map<String, Integer> normalBattleMonsterMap;
	protected Map<String, Integer> bossBattleMonsterMap;

	public InstanceCfg() {
		id = "";
		level = 0;
		battleLevelList = "";
		battleBoss = "";
	}
	
	@Override
	protected boolean assemble() {
		normalBattleIdList = HawkJsonUtil.getJsonInstance().fromJson(battleLevelList, new TypeToken<List<String>>() {}.getType());
		return true;
	}
	
	@Override
	protected boolean checkValid() {
		normalBattleMonsterMap = new HashMap<String, Integer>();
		for (int i = 0; i < normalBattleIdList.size(); ++i) {
			BattleLevelCfg battleCfg = HawkConfigManager.getInstance().getConfigByKey(BattleLevelCfg.class, normalBattleIdList.get(i));
			if (null == battleCfg) {
				HawkLog.errPrintln(String.format("config invalid BattleLevelCfg : %s", normalBattleIdList.get(i)));
				return false;
			}
			
			for (Entry<String, Integer> entry : battleCfg.getMonsterMap().entrySet()) {
				Integer count = normalBattleMonsterMap.get(entry.getKey());
				if (null == count) {
					count = 0;
				}
				normalBattleMonsterMap.put(entry.getKey(), entry.getValue() + count);
			}
		}
		
		BattleLevelCfg bossBattleCfg = HawkConfigManager.getInstance().getConfigByKey(BattleLevelCfg.class, battleBoss);
		if (null == bossBattleCfg) {
			HawkLog.errPrintln(String.format("config invalid BattleLevelCfg : %s", battleBoss));
			return false;
		}
		bossBattleMonsterMap = bossBattleCfg.getMonsterMap();

		return true;
	}

	public String getId() {
		return id;
	}
	
	public int getMonsterLevel() {
		return level;
	}
	
	public List<String> getNormalBattleIdList() {
		return Collections.unmodifiableList(normalBattleIdList);
	}
	
	public String getBossBattleId() {
		return battleBoss;
	}

	public int getNormalBattleCount() {
		return normalBattleIdList.size();
	}
	
	public Map<String, Integer> getNormalBattleMonsterMap() {
		return Collections.unmodifiableMap(normalBattleMonsterMap);
	}
	
	public Map<String, Integer> getBossBattleMonsterMap() {
		return Collections.unmodifiableMap(bossBattleMonsterMap);
	}
}