package com.hawk.game.BILog;

import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.LanguageStaticCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.player.Player;

public class BIGemFlowData extends BIData{
	/**
	 * 
	 * @param player
	 * @param itemCfg
	 * @param equipId
	 * @param monsterCfg
	 * @param stage
	 * @param equip
	 */
	public void log(Player player, ItemCfg itemCfg, long equipId, MonsterCfg monsterCfg, int stage, boolean equip) {
		logBegin(player, "Gem_Flow");
		jsonPropertyData.put("equipname", LanguageStaticCfg.getEnglishName(itemCfg.getName()));
		jsonPropertyData.put("equip_identify", equipId);
		jsonPropertyData.put("item_stage", stage);
		jsonPropertyData.put("pet_name", monsterCfg != null ? LanguageStaticCfg.getEnglishName(monsterCfg.getNickname()) : "");
		jsonPropertyData.put("action", equip ? 1 : 0);
		logEnd();
	}
}
