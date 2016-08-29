package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.player.Player;

public class BIPetLevelUpData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param petId
	 * @param petName
	 * @param petLevel
	 * @param afterPetLevel
	 * @param expNum
	 * @param petExp
	 * @param afterPetExp
	 */
	public void log(Player player, Action action, int petId, String petName, int petLevel, int afterPetLevel, int expNum, int petExp, int afterPetExp){
		logBegin(player, "pet_levelup");
		jsonPropertyData.put("pet_id", petId);
		jsonPropertyData.put("pet_name", petName);
		jsonPropertyData.put("petlevel", petLevel);
		jsonPropertyData.put("after_petlevel", afterPetLevel);
		jsonPropertyData.put("exp_num", expNum);
		jsonPropertyData.put("petexp", petExp);
		jsonPropertyData.put("after_petexp", afterPetExp);
		jsonPropertyData.put("action", action.getBICode());
		logEnd();
	}
}
