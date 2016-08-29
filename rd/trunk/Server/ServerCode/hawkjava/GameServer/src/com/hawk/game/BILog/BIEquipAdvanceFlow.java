package com.hawk.game.BILog;

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
	public void log(Player player, int equipId, String equipName, String petId, String petName, int stage, int  equipLevel, int afterLevel) {
		logBegin(player, "Equip_Advance_Flow");
		jsonPropertyData.put("equipid", equipId);
		jsonPropertyData.put("equipname", equipName);
		jsonPropertyData.put("item_stage", stage);
		jsonPropertyData.put("pet_id", petId);
		jsonPropertyData.put("pet_name", petName);
		jsonPropertyData.put("equiplevel", equipLevel);
		jsonPropertyData.put("afterequiplevel", afterLevel);
		logEnd();
	}
}
