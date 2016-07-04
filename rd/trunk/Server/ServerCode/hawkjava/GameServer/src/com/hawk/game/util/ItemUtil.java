package com.hawk.game.util;

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
}
