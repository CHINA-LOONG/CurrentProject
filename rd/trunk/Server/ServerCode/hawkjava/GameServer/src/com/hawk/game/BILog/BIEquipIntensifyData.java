package com.hawk.game.BILog;

import com.hawk.game.player.Player;

public class BIEquipIntensifyData extends BIData{
	/**
	 * 
	 * @param player
	 * @param equipid
	 * @param equipname
	 * @param stage
	 * @param petName
	 * @param petId
	 * @param equipLevel
	 * @param afterLevel
	 */
	public void assemble(Player player, int equipid, String equipname, int stage, String petName, int petId, int  equipLevel, int afterLevel) {
		logBegin(player, "Equip_Intensify_Flow");
		jsonPropertyData.put("equipname", equipname);
		jsonPropertyData.put("equipid", equipid);
		jsonPropertyData.put("item_stage", stage);
		jsonPropertyData.put("pet_name", petName);
		jsonPropertyData.put("pet_id", petId);
		jsonPropertyData.put("equiplevel", equipLevel);
		jsonPropertyData.put("afterequiplevel", afterLevel);
		logEnd();
	}
}
