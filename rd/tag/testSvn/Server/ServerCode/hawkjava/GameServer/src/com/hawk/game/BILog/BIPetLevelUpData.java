package com.hawk.game.BILog;

import com.hawk.game.config.LanguageStaticCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.player.Player;

public class BIPetLevelUpData extends BIData{
	/**
	 * 
	 * @param player
	 * @param petId
	 * @param petName
	 * @param petLevel
	 * @param afterPetLevel
	 * @param expNum
	 * @param petExp
	 * @param afterPetExp
	 */
	public void log(Player player, int petId, MonsterCfg monsterCfg, int petLevel, int afterPetLevel, int expNum, int petExp, int afterPetExp){
		logBegin(player, "pet_levelup");
		jsonPropertyData.put("pet_id", monsterCfg.getId());
		jsonPropertyData.put("pet_identify", petId);
		jsonPropertyData.put("pet_name", LanguageStaticCfg.getEnglishName(monsterCfg.getNickname()));
		jsonPropertyData.put("petlevel", petLevel);
		jsonPropertyData.put("after_petlevel", afterPetLevel);
		jsonPropertyData.put("exp_num", expNum);
		jsonPropertyData.put("petexp", petExp);
		jsonPropertyData.put("after_petexp", afterPetExp);
		logEnd();
	}
}
