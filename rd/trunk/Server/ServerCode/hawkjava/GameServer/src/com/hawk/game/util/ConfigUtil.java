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
	 * 检测itemType的itemId 
	 * 
	 * @param itemType
	 * @param itemId
	 * @return
	 */
	public static boolean check(int itemType, String itemId) {
		if (itemType == Const.itemType.PLAYER_ATTR_VALUE) {
			return true;
		}
		else if (itemType == Const.itemType.ITEM_VALUE || itemType == Const.itemType.EQUIP_VALUE) {
			if (HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId) == null) {
				HawkLog.errPrintln("item config not found, itemId: " + itemId);
				return false;
			}
			return true;
		}
		return false;
	}
	
	/**
	 * 
	 * @param itemInfo
	 * @return
	 */
	public static boolean checkItemValid(ItemInfo itemInfo){
		if (itemInfo.getType() == Const.itemType.NONE_ITEM_VALUE) {
			return true;
		}
		else if (itemInfo.getType() == Const.itemType.ITEM_VALUE || itemInfo.getType() == Const.itemType.EQUIP_VALUE) {
			return HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemInfo.getItemId()) != null;
		}
		else if (itemInfo.getType() == Const.itemType.GROUP_VALUE) {
			return HawkConfigManager.getInstance().getConfigByKey(RewardGroupCfg.class, itemInfo.getItemId()) != null;
		}
		else if (itemInfo.getType() == Const.itemType.MONSTER_VALUE) {
			return HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, itemInfo.getItemId()) != null;
		}
		else if (itemInfo.getType() == Const.itemType.MONSTER_ATTR_VALUE) {
			if (itemInfo.getItemId().equals(String.valueOf(Const.changeType.CHANGE_MONSTER_EXP_VALUE)) ||
				itemInfo.getItemId().equals(String.valueOf(Const.changeType.CHANGE_MONSTER_LEVEL_VALUE))) {
				return true;
			}
		}
		else if (itemInfo.getType() == Const.itemType.PLAYER_ATTR_VALUE) {
			return true;
		}
		else if (itemInfo.getType() == Const.itemType.SKILL_VALUE) {
			return true;
		}
		
		return false;
	}
}
