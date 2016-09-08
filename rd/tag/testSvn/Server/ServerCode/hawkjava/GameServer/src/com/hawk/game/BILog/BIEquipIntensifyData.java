package com.hawk.game.BILog;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.LanguageStaticCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.player.Player;

public class BIEquipIntensifyData extends BIData{
	/**
	 * 
	 * @param player
	 * @param equipCfg
	 * @param equipId
	 * @param stage
	 * @param monsterCfg
	 * @param monsterId
	 * @param equipLevel
	 * @param afterLevel
	 */
	public void log(Player player, ItemCfg equipCfg, long equipId, int stage, MonsterCfg monsterCfg, int monsterId, int  equipLevel, int afterLevel) {
		logBegin(player, "Equip_Intensify_Flow");
		jsonPropertyData.put("equipname", LanguageStaticCfg.getEnglishName(equipCfg.getName()));
		jsonPropertyData.put("equipid", equipCfg.getId());
		jsonPropertyData.put("equip_identify", equipId);
		jsonPropertyData.put("pet_name", monsterCfg != null ? LanguageStaticCfg.getEnglishName(monsterCfg.getNickname()) : 0);
		jsonPropertyData.put("pet_id", monsterId);
		jsonPropertyData.put("equiplevel", equipLevel);
		jsonPropertyData.put("afterequiplevel", afterLevel);
		jsonPropertyData.put("item_stage", stage);
		logEnd();
	}
}
