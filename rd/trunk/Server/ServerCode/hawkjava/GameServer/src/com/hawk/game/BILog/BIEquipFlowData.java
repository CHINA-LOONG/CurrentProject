package com.hawk.game.BILog;

import com.hawk.game.player.Player;

public class BIEquipFlowData extends BIData{
	/**
	 * 
	 * @param player
	 * @param equipName
	 * @param petName
	 * @param stage
	 * @param level
	 * @param equip
	 */
	public void assemble(Player player, String equipName, String petName, int stage, int level, boolean equip) {
		logBegin(player, "Equip_Flow");
		jsonPropertyData.put("equipname", equipName);
		jsonPropertyData.put("pet_name", petName);
		jsonPropertyData.put("item_stage", stage);
		jsonPropertyData.put("pet_level", level);
		jsonPropertyData.put("action", equip ? 1 : 0);
		logEnd();
	}
}
