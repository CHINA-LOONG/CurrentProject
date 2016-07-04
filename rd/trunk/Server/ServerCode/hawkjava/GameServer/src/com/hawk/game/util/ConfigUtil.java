package com.hawk.game.util;

import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.protocol.Const;

/**
 * 配置检查
 * 
 * @author xulinqs
 * 
 */
public class ConfigUtil {

	/**
	 * 检测itemType的itemId 
	 * 
	 * @param itemType
	 * @param itemId
	 * @return
	 */
	public static boolean check(int itemType, int itemId) {
		if (itemType == Const.itemType.PLAYER_ATTR_VALUE) {
			return true;
		} 
		else if (itemType == Const.itemType.EQUIP_VALUE) {
			if (HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId) == null) {
				HawkLog.errPrintln("equip config not found, itemId: " + itemId);
				return false;
			}
			return true;
		} else if (itemType == Const.itemType.ITEM_VALUE) {
			if (HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId) == null) {
				HawkLog.errPrintln("item config not found, itemId: " + itemId);
				return false;
			}
			return true;
		}
		return false;
	}

	/**
	 * 怪物检测
	 * @param monsterId
	 * @return
	 */
	public static boolean checkMonster(int monsterId) {
		MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterId);
		if (monsterCfg == null) {
			HawkLog.errPrintln("monster config not found, dropId：" + monsterId);
			return false;
		}
		return true;
	}
}
