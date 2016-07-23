package com.hawk.game.util;

import java.util.Map;

import org.hawk.config.HawkConfigManager;

import com.hawk.game.config.MonsterBaseCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Const;

public class ItemUtil {
	/**
	 * 是否能堆叠
	 * 
	 * @param itemType
	 * @return
	 */
	public static boolean itemCanOverlap(int itemType) {
		return itemType == Const.itemType.PLAYER_ATTR_VALUE || itemType == Const.itemType.ITEM_VALUE;
	}
	
	/**
	 * 查看最大使用的经验药水数量
	 * 
	 * @param player
	 * @param expItemCount 提供的经验药水数量
	 * @param expPerItem   每瓶经验药水提高的经验数量
	 * @return 实际使用的瓶数
	 */
	public static int checkExpItemCount(MonsterEntity monster, int expItemCount, int expPerItem){	
		Map<Object, MonsterBaseCfg> monsterBaseCfg = HawkConfigManager.getInstance().getConfigMap(MonsterBaseCfg.class);	
		int itemUseCount = 0;
		float levelUpExpRate = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monster.getCfgId()).getNextExpRate();
		int level = monster.getLevel();
		int exp = monster.getExp();
		while (expItemCount > 0 && level != monsterBaseCfg.size()) {
			itemUseCount++;
			expItemCount--;
			exp += expPerItem;
			if (exp >= monsterBaseCfg.get(level).getNextExp() * levelUpExpRate) {
				exp -= monsterBaseCfg.get(level).getNextExp() * levelUpExpRate;
				level++;
			}
		}
		
		return itemUseCount;
	}
}
