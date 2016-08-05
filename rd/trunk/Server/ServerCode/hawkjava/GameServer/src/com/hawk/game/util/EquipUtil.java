package com.hawk.game.util;

import java.util.Map;

import org.hawk.config.HawkConfigManager;
import org.hawk.os.HawkException;

import com.hawk.game.attr.Attribute;
import com.hawk.game.config.EquipAttr;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Attribute.Attr;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Reward.RewardItem;
import com.hawk.game.util.GsConst.EquipStagePunch;

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
	public static EquipEntity generateEquip(Player player, String equipId) {
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
	public static EquipEntity generateEquip(Player player, String equipId, int stage, int level) {
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
			
			Attribute attr = EquipAttr.getAdditionAttr(equipId, stage);
			if (attr != null) {
				equipEntity.setAttr(attr);
			}
			
			return equipEntity;
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		return null;
	}

	/**
	 * 填充附加属性
	 * 
	 * @param equipId
	 * @param isFullAttr 
	 * @param punchSize 
	 * @return
	 */
	public static void generateAttr(EquipEntity equipEntity, RewardItem.Builder rewardItem) {
		if (equipEntity.getAttr() != null) {
			try {			
				for (Map.Entry<Const.attr, Float> entry : equipEntity.getAttr().getAttrMap().entrySet()){
					Attr.Builder attr = Attr.newBuilder();
					attr.setAttrId(entry.getKey().getNumber());
					attr.setAttrValue(entry.getValue());
					rewardItem.addAttrDatas(attr);
				}
				
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}	
	}

	/**
	 * 获取装备打孔数量
	 */
	public static int getPunchCount(EquipEntity equipEntity)
	{
		if (equipEntity.getStage() == EquipStagePunch.WHITE_STAGE.ordinal()) {
			return EquipStagePunch.WHITE_STAGE.GetCount();
		}
		else if (equipEntity.getStage() == EquipStagePunch.GREEN_STAGE.ordinal()) {
			return EquipStagePunch.GREEN_STAGE.GetCount();
		}
		else if (equipEntity.getStage() == EquipStagePunch.BLUE_STAGE.ordinal()) {
			return EquipStagePunch.BLUE_STAGE.GetCount();
		}
		else if (equipEntity.getStage() == EquipStagePunch.PURPLE_STAGE.ordinal()) {
			return EquipStagePunch.PURPLE_STAGE.GetCount();
		}
		else if (equipEntity.getStage() == EquipStagePunch.ORANGE_STAGE.ordinal()) {
			return EquipStagePunch.ORANGE_STAGE.GetCount();
		}
		else if (equipEntity.getStage() == EquipStagePunch.RED_STAGE.ordinal()) {
			return EquipStagePunch.RED_STAGE.GetCount();
		}
		
		return EquipStagePunch.NONE_STAGE.GetCount();
	}
	
}

