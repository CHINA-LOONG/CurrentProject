package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.player.Player;

public class BIPetEnhanceData extends BIData{

	public void log(Player player, Action action, int petId, String petName, int petLevel, int petStage, int afterPetStage) {
		logBegin(player, "pet_enhance");
		jsonPropertyData.put("pet_id", petId);
		jsonPropertyData.put("pet_name", petName);
		jsonPropertyData.put("pet_level", petLevel);
		jsonPropertyData.put("petstage", petStage);
		jsonPropertyData.put("after_petstage", afterPetStage);
		jsonPropertyData.put("action", action.getBICode());
		logEnd();
	}
}
