package com.hawk.game.BILog;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.LanguageStaticCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.player.Player;

public class BIPetFlowData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param petId
	 * @param monsterCfg
	 * @param petStage
	 * @param petLevel
	 * @param petStarLevel
	 * @param petIn
	 * @param petOut
	 */
	public void log(Player player, Action action, int petId, MonsterCfg monsterCfg, int petStage, int petLevel, int petStarLevel, int petIn, int petOut){
		logBegin(player, "pet_flow");
		jsonPropertyData.put("pet_id", monsterCfg.getId());
		jsonPropertyData.put("pet_identify", petId);
		jsonPropertyData.put("pet_name", LanguageStaticCfg.getEnglishName(monsterCfg.getNickname()));
		jsonPropertyData.put("pet_level", petLevel);
		jsonPropertyData.put("petstage", petStage);
		jsonPropertyData.put("petstarleve", petStarLevel);
		jsonPropertyData.put("pet_in", petIn);
		jsonPropertyData.put("pet_out", petOut);
		jsonPropertyData.put("action", action.getBICode());
		logEnd();
	}
}
