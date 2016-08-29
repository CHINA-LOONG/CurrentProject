package com.hawk.game.BILog;

import com.hawk.game.player.Player;

public class BIGemFlowData extends BIData{
	/**
	 * 
	 * @param player
	 * @param equipName
	 * @param petName
	 * @param stage
	 * @param level
	 * @param equip
	 */
	public void assemble(Player player, String equipName, String petName, int stage, boolean equip) {
		logBegin(player, "Gem_Flow");
		jsonPropertyData.put("equipname", equipName);
		jsonPropertyData.put("item_stage", stage);
		jsonPropertyData.put("pet_name", petName);
		jsonPropertyData.put("action", equip ? 1 : 0);
		logEnd();
	}
}
