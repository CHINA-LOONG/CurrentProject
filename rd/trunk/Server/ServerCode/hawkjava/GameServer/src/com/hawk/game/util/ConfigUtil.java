package com.hawk.game.util;

import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.RewardGroupCfg;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.protocol.Const;

/**
 * 配置检查
 * 
 * 
 */
public class ConfigUtil {

	/**
	 * 检测itemInfo是否合法
	 */
	public static boolean checkItemInfoValid(ItemInfo itemInfo) {
		return checkItemValid(itemInfo.getType(), itemInfo.getItemId());
	}

	/**
	 * 检测item是否合法
	 */
	public static boolean checkItemValid(int itemType, String itemId){
		if (itemType == Const.itemType.PLAYER_ATTR_VALUE 
				|| itemType == Const.itemType.SKILL_VALUE 
				|| itemType == Const.itemType.NONE_ITEM_VALUE) {
			return true;
		} 
		else if (itemType == Const.itemType.ITEM_VALUE || itemType == Const.itemType.EQUIP_VALUE) {
			if (null == HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId)) {
				HawkLog.errPrintln("Item config not found : " + itemId);
				return false;
			}
		}
		else if (itemType == Const.itemType.GROUP_VALUE) {
			if (null == HawkConfigManager.getInstance().getConfigByKey(RewardGroupCfg.class, itemId)) {
				HawkLog.errPrintln("RewardGroup config not found : " + itemId);
				return false;
			}
		}
		else if (itemType == Const.itemType.MONSTER_VALUE) {
			if (null == HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, itemId)) {
				HawkLog.errPrintln("Monster config not found : " + itemId);
				return false;
			}
		}
		else if (itemType == Const.itemType.MONSTER_ATTR_VALUE) {
			if (false == itemId.equals(String.valueOf(Const.changeType.CHANGE_MONSTER_EXP_VALUE))
					&& false == itemId.equals(String.valueOf(Const.changeType.CHANGE_MONSTER_LEVEL_VALUE))) {
				HawkLog.errPrintln("Item monster attr config invalid : " + itemId);
				return false;
			}
		} else {
			return false;
		}

		return true;
	}
}
