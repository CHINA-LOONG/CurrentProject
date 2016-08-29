package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.player.Player;

public class BIPetFlowData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param petId
	 * @param petName
	 * @param petStage
	 * @param petLevel
	 * @param petStarLevel
	 */
	public void log(Player player, Action action, int petId, String petName, int petStage, int petLevel, int petStarLevel){
		logBegin(player, "pet_flow");
		jsonPropertyData.put("pet_id", petId);
		jsonPropertyData.put("pet_name", petName);
		jsonPropertyData.put("pet_level", petLevel);
		jsonPropertyData.put("petstage", petStage);
		jsonPropertyData.put("petstarleve", petStarLevel);
		jsonPropertyData.put("action", action.getBICode());
		logEnd();
	}
}
