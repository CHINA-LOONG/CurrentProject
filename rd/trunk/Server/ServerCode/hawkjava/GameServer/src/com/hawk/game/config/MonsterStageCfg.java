package com.hawk.game.config;

import java.util.Collections;
import java.util.Comparator;
import java.util.List;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.hawk.game.item.ItemInfo;
import com.hawk.game.util.GsConst.ItemParseType;

@HawkConfigManager.CsvResource(file = "staticData/unitStage.csv", struct = "map")
public class MonsterStageCfg extends HawkConfigBase {

	@Id
	protected final int stage;
	protected final int health;
	protected final int strength;
	protected final int intelligence;
	protected final int speed;
	protected final int defense;
	protected final int endurance;
	protected final int recovery;
	protected final float modifyRate;
	protected final int demandLevel;
	protected final int demandCoin;
	/**
	 * 格式: reward标准配置，只支持ITEM类型
	 */
	protected final String demandItem;
	/**
	 * 格式: reward标准配置，只支持MONSTER类型
	 */
	protected final String demandMonster;

	// assemble
	List<ItemInfo> demandItemList;
	List<ItemInfo> demandMonsterList;

	public MonsterStageCfg() {
		stage = 0;
		health = 0;
		strength = 0;
		intelligence = 0;
		speed = 0;
		defense = 0;
		endurance = 0;
		recovery = 0;
		modifyRate = 1.0f;
		demandLevel = 0;
		demandCoin = 0;
		demandItem = "";
		demandMonster = "";
	}

	@Override
	protected boolean assemble() {
		// TODO: 合并重复的

		try {
			demandItemList = ItemInfo.GetItemInfo(demandItem, ItemParseType.PARSE_MONSTER_STAGE);
			demandMonsterList = ItemInfo.GetItemInfo(demandMonster, ItemParseType.PARSE_MONSTER_STAGE);
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		return true;
	}

	@Override
	protected boolean checkValid() {
		for (ItemInfo item: demandItemList) {
			ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, item.getItemId());
			if (null == itemCfg) {
				HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", item.getItemId()));
				return false;
			}
		}

		for (ItemInfo monster : demandMonsterList) {
			MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monster.getItemId());
			if (null == monsterCfg) {
				HawkLog.errPrintln(String.format("config invalid MonsterCfg : %s", monster.getItemId()));
				return false;
			}
		}

		// 按stage降序排序
		Collections.sort(demandMonsterList, new Comparator<ItemInfo>() {
            public int compare(ItemInfo arg0, ItemInfo arg1) {
                return Integer.compare(arg1.getStage(), arg0.getStage());
            }
        });

		return true;
	}

	public int getStage() {
		return stage;
	}

	public int getHealth() {
		return health;
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

	public int getEndurance() {
		return endurance;
	}

	public int getRecovery() {
		return recovery;
	}

	public float getModifyRate() {
		return modifyRate;
	}

	public int getDemandLevel() {
		return demandLevel;
	}

	public int getDemandCoin() {
		return demandCoin;
	}

	public List<ItemInfo> getDemandItemList() {
		return demandItemList;
	}

	public List<ItemInfo> getDemandMonsterList() {
		return demandMonsterList;
	}
}
