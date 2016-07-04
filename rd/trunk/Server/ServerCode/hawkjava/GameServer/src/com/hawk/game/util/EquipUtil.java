package com.hawk.game.util;

import java.util.LinkedList;
import java.util.List;

import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBManager;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkRand;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.SysBasicCfg;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.RoleEntity;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Const;

/**
 * 装备帮助类
 * 
 * @author hawk
 */
public class EquipUtil {
	
	/**
	 * 生成装备
	 * 
	 * @param equipId
	 * @param isFullAttr 
	 * @param punchSize 
	 * @return
	 */
	public static EquipEntity generateEquip(Player player, int equipId) {
		return generateEquip(player,equipId,0,0);
	}

	/**
	 * 生成装备
	 * 
	 * @param equipId
	 * @param isFullAttr 
	 * @param punchSize 
	 * @return
	 */
	public static EquipEntity generateEquip(Player player, int equipId, int stage, int level) {
		try {
			ItemCfg equipCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, equipId);
			if (equipCfg == null) {
				return null;
			}
			
			EquipEntity equipEntity = new EquipEntity();
			equipEntity.setPlayerId(player.getId());
			equipEntity.setItemId(equipCfg.getId());
			equipEntity.setStage(stage);
			equipEntity.setLevel(level);
			
			
			return equipEntity;
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return null;
	}

	
}

