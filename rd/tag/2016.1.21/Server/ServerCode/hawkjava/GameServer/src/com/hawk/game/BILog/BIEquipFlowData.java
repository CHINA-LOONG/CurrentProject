package com.hawk.game.BILog;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.LanguageStaticCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.player.Player;

public class BIEquipFlowData extends BIData{
	
	/**
	 * 
	 * @param player
	 * @param itemCfg
	 * @param equipId
	 * @param monsterCfg
	 * @param monsterId
	 * @param stage
	 * @param level
	 * @param equip
	 */
	public void log(Player player, ItemCfg itemCfg, long equipId, MonsterCfg monsterCfg, int monsterId, int stage, int level, boolean equip) {		
		logBegin(player, "Equip_Flow");
		jsonPropertyData.put("equipname", LanguageStaticCfg.getEnglishName(itemCfg.getName()));
		jsonPropertyData.put("equip_identify", equipId);
		jsonPropertyData.put("pet_name", LanguageStaticCfg.getEnglishName(monsterCfg.getNickname()));
		jsonPropertyData.put("pet_identify", monsterId);
		jsonPropertyData.put("item_level", level);
		jsonPropertyData.put("item_stage", stage);
		jsonPropertyData.put("action", equip ? 1 : 0);
		logEnd();
	}
}
