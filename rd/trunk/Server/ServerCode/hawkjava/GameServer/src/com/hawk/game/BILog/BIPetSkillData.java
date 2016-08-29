package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.player.Player;

public class BIPetSkillData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param petId
	 * @param petName
	 * @param skillId
	 * @param skillName
	 * @param petSkillLevel
	 * @param afterPetSkillLevel
	 */
	public void log(Player player, Action action, int petId, String petName, String skillId, String skillName, int petSkillLevel, int afterPetSkillLevel){
		logBegin(player, "pet_skill");
		jsonPropertyData.put("pet_id", petId);
		jsonPropertyData.put("pet_name", petName);
		jsonPropertyData.put("skill_id", skillId);
		jsonPropertyData.put("skill_name", skillName);
		jsonPropertyData.put("petskilllevel", petSkillLevel);
		jsonPropertyData.put("after_petsskilllevel", afterPetSkillLevel);
		jsonPropertyData.put("action", action.getBICode());
		logEnd();
	}
}
