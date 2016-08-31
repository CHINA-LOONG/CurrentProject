package com.hawk.game.BILog;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.LanguageStaticCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.player.Player;

public class BIEquipAdvanceFlow extends BIData{
	/**
	 * 
	 * @param player
	 * @param equipId
	 * @param equipName
	 * @param petId
	 * @param petName
	 * @param stage
	 * @param equipLevel
	 * @param afterLevel
	 */
	public void log(Player player, ItemCfg equipCfg, long equipId, MonsterCfg monsterCfg, int monsterId, int stage, int  equipLevel, int afterLevel) {
		logBegin(player, "Equip_Advance_Flow");
		jsonPropertyData.put("equipname", LanguageStaticCfg.getEnglishName(equipCfg.getName()));
		jsonPropertyData.put("equipid", equipCfg.getId());
		jsonPropertyData.put("equip_identify", equipId);
		jsonPropertyData.put("pet_name", LanguageStaticCfg.getEnglishName(monsterCfg.getNickname()));
		jsonPropertyData.put("pet_id", monsterId);
		jsonPropertyData.put("equiplevel", equipLevel);
		jsonPropertyData.put("afterequiplevel", afterLevel);
		jsonPropertyData.put("item_stage", stage);
		logEnd();
	}
}
