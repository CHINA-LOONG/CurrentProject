package com.hawk.game.BILog;

import com.hawk.game.config.LanguageStaticCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.SpellCfg;
import com.hawk.game.player.Player;

public class BIPetSkillData extends BIData{
	/**
	 * 
	 * @param player
	 * @param petId
	 * @param monsterCfg
	 * @param spellCfg
	 * @param petSkillLevel
	 * @param afterPetSkillLevel
	 */
	public void log(Player player, int petId, MonsterCfg monsterCfg, SpellCfg spellCfg, int petSkillLevel, int afterPetSkillLevel){
		logBegin(player, "pet_skill");
		jsonPropertyData.put("pet_id", monsterCfg.getId());
		jsonPropertyData.put("pet_identify", petId);
		jsonPropertyData.put("pet_name", LanguageStaticCfg.getEnglishName(monsterCfg.getNickname()));
		jsonPropertyData.put("skill_id", spellCfg.getTextId());
		jsonPropertyData.put("skill_name", LanguageStaticCfg.getEnglishName(spellCfg.getName()));
		jsonPropertyData.put("petskilllevel", petSkillLevel);
		jsonPropertyData.put("after_petsskilllevel", afterPetSkillLevel);
		logEnd();
	}
}
