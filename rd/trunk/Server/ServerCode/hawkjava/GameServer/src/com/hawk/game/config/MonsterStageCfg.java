package com.hawk.game.config;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigBase;
import org.hawk.os.HawkException;

import com.hawk.game.item.ItemInfo;
import com.hawk.game.util.GsConst.ItemParseType;

@HawkConfigManager.CsvResource(file = "xml/unitStage.csv", struct = "map")
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
//	public static class ItemConsume {
//		public int itemCfgId;
//		public int count;
//	}
//	
//	public static class MonsterConsume {
//		public String monsterCfgId;
//		public int stage;
//		public int count;
//	}
//
//	protected List<ItemConsume> demandItemList;
//	protected List<MonsterConsume> demandMonsterList;
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
//			demandItemList = new ArrayList<>();
//			String[] unitList = demandItem.split("\\|");
//			for (String unit : unitList) {
//				String[] fieldList = unit.split(";");
//				if (fieldList.length != 2) {
//					return false;
//				}
//				ItemConsume item = new ItemConsume();
//				item.itemCfgId = Integer.parseInt(fieldList[0]);
//				item.count = Integer.parseInt(fieldList[1]);
//				demandItemList.add(item);
//			}
//
//			demandMonsterList = new ArrayList<>();
//			unitList = demandMonster.split("\\|");
//			for (String unit : unitList) {
//				String[] fieldList = unit.split(";");
//				if (fieldList.length != 3) {
//					return false;
//				}
//				MonsterConsume monster = new MonsterConsume();
//				monster.monsterCfgId = fieldList[0];
//				monster.stage = Integer.parseInt(fieldList[1]);
//				monster.count = Integer.parseInt(fieldList[2]);
//				demandMonsterList.add(monster);
//			}
			
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
				return false;
			}
		}

		for (ItemInfo monster : demandMonsterList) {
			MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monster.getItemId());
			if (null == monsterCfg) {
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
