package com.hawk.game.BILog;

import com.hawk.game.config.LanguageStaticCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.player.Player;

public class BIPetEnhanceData extends BIData{

	public void log(Player player, int petId, MonsterCfg monsterCfg, int petLevel, int petStage, int afterPetStage, int enhanceLevel, int afterEnhanceLevel) {
		logBegin(player, "pet_enhance");
		jsonPropertyData.put("pet_id", monsterCfg.getId());
		jsonPropertyData.put("pet_identify", petId);
		jsonPropertyData.put("pet_name", LanguageStaticCfg.getEnglishName(monsterCfg.getNickname()));
		jsonPropertyData.put("pet_level", petLevel);
		jsonPropertyData.put("petstage", petStage);
		jsonPropertyData.put("after_petstage", afterPetStage);
		jsonPropertyData.put("petenhancelevel", enhanceLevel);
		jsonPropertyData.put("after_petenhancelevel", afterEnhanceLevel);
		logEnd();
	}
}
