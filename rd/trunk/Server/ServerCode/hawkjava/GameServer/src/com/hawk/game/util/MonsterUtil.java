package com.hawk.game.util;

import java.util.List;

import org.hawk.config.HawkConfigManager;

import com.hawk.game.config.BaseAttrCfg;
import com.hawk.game.config.EquipAttr;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.MonsterBaseCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.MonsterStageCfg;
import com.hawk.game.config.SpellCfg;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Equip.EquipInfo;
import com.hawk.game.protocol.Equip.GemPunch;
import com.hawk.game.protocol.Monster.HSMonster;
import com.hawk.game.protocol.Skill.HSSkill;

public class MonsterUtil {

	public static float calculateBP(List<HSMonster.Builder> monsters){
		float result = 0;
		for (HSMonster.Builder monsterBuilder : monsters) {
			result += calculateBP(monsterBuilder);
		}
		return result;
	}
	
	/**
	 * 计算宠物战斗力
	 * @param monsterBuilder
	 * @return
	 */
 	public static float calculateBP(HSMonster.Builder monsterBuilder){
		
		float strengthToAttack = 1.0f;
	    float intelligenceToAttack = 1.0f;
	    float healthToLife = 1.0f;
	
	    float criticalDamgeRatio = 1.5f;
	    float criticalRatio = 0.1f;
	    
		MonsterStageCfg stageCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterStageCfg.class, monsterBuilder.getStage());
		
		// 体力
		float healthModifyRate = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterBuilder.getCfgId()).getHealthModifyRate();
		int baseHealth = HawkConfigManager.getInstance().getConfigByKey(MonsterBaseCfg.class, monsterBuilder.getLevel()).getHealth();
		int stageHealth = stageCfg.getHealth();
		float equipHealth = 0.0f;
		
		// 智力
		float intelligenceModifyRate = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterBuilder.getCfgId()).getIntelligenceModifyRate();
		int baseIntelligence = HawkConfigManager.getInstance().getConfigByKey(MonsterBaseCfg.class, monsterBuilder.getLevel()).getIntelligence();
		int stageIntelligence = stageCfg.getIntelligence();
		float equipIntelligence = 0.0f;
		
		// 力量
		float strengthModifyRate = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class,  monsterBuilder.getCfgId()).getStrengthModifyRate();
		int baseStrength = HawkConfigManager.getInstance().getConfigByKey(MonsterBaseCfg.class, monsterBuilder.getLevel()).getStrength();
		int stageStrength = stageCfg.getStrength();
		float equipStrength = 0.0f;
		
		// 防御力
		float defenceModifyRate = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterBuilder.getCfgId()).getDefenseModifyRate();
		int baseDefense = HawkConfigManager.getInstance().getConfigByKey(MonsterBaseCfg.class, monsterBuilder.getLevel()).getDefense();
		int stageDefense = stageCfg.getDefense();
		float equipDefense = 0.0f;
		
		// 速度
		float speedModifyRate = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class,  monsterBuilder.getCfgId()).getSpeedModifyRate();
		int baseSpeed = HawkConfigManager.getInstance().getConfigByKey(MonsterBaseCfg.class, monsterBuilder.getLevel()).getSpeed();
		int stageSpeed = stageCfg.getSpeed();
		float equipSpeed = 0.0f;
		
		// 暴击率
		float gemCriticalRatio = 0.0f;
		float gemCriticalDmg = 0.0f;
		float gemHealRatio = 0.0f;
		float gemEnergy = 0.0f;
		
		for (EquipInfo equipInfo : monsterBuilder.getEquipInfosList()) {
			
			String stageAttrId = EquipAttr.getStageAttrId(equipInfo.getEquipId(), equipInfo.getStage());
			String levelAttrId = EquipAttr.getLevelAttrId(equipInfo.getEquipId(), equipInfo.getStage());
					
			BaseAttrCfg baseAttrCfg = HawkConfigManager.getInstance().getConfigByKey(BaseAttrCfg.class, stageAttrId);
			equipHealth += baseAttrCfg.getHealth();
			equipIntelligence += baseAttrCfg.getIntelligence();
			equipStrength += baseAttrCfg.getStrength();
			equipDefense += baseAttrCfg.getDefense();
			equipSpeed += baseAttrCfg.getSpeed();
			
			baseAttrCfg = HawkConfigManager.getInstance().getConfigByKey(BaseAttrCfg.class, levelAttrId);
			equipHealth += baseAttrCfg.getHealth() * equipInfo.getLevel();
			equipIntelligence += baseAttrCfg.getIntelligence() * equipInfo.getLevel();
			equipStrength += baseAttrCfg.getStrength() * equipInfo.getLevel();
			equipDefense += baseAttrCfg.getDefense() * equipInfo.getLevel();
			equipSpeed += baseAttrCfg.getSpeed() *equipInfo.getLevel();
			
			for (GemPunch gem : equipInfo.getGemItemsList()) {
				ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, gem.getGemItemId());
				baseAttrCfg = HawkConfigManager.getInstance().getConfigByKey(BaseAttrCfg.class, itemCfg.getGemId());
				equipHealth += baseAttrCfg.getHealth();
				equipIntelligence += baseAttrCfg.getIntelligence();
				equipStrength += baseAttrCfg.getStrength();
				equipDefense += baseAttrCfg.getDefense();
				equipSpeed += baseAttrCfg.getSpeed();
				gemCriticalRatio += baseAttrCfg.getCriticalRatio();
				gemCriticalDmg += baseAttrCfg.getCriticalDmg();
				gemHealRatio += baseAttrCfg.getHealRatio();
				gemEnergy += baseAttrCfg.getEnergy();
			}
		}
		
		gemCriticalDmg += criticalDamgeRatio;
		gemCriticalRatio += criticalRatio;
		
		float health = healthModifyRate * (baseHealth * (1 + stageCfg.getModifyRate()) + stageHealth) + equipHealth;
		float defence = defenceModifyRate * (baseDefense * (1 + stageCfg.getModifyRate()) + stageDefense) + equipDefense;
		float strength = strengthModifyRate * (baseStrength * (1 + stageCfg.getModifyRate()) + stageStrength) + equipStrength;
		float intelligence= intelligenceModifyRate * (baseIntelligence * (1 + stageCfg.getModifyRate()) + stageIntelligence) + equipIntelligence;
		float speed = speedModifyRate * (baseSpeed * (1 + stageCfg.getModifyRate()) + stageSpeed) + equipSpeed;	
		
		float injuryRatio = (1 / (1 + defence / (monsterBuilder.getLevel() * 10 + 1000)));
		injuryRatio = Math.max(injuryRatio, 0.25f);
    
		float HP = (healthToLife * health) / injuryRatio * (1 + gemHealRatio * 0.33f);;
		float DPS = (intelligenceToAttack * (int)intelligence + strengthToAttack * (int)strength) / 2 * speed * (gemCriticalRatio * gemCriticalDmg + (1 - gemCriticalRatio) * 1) * (1 + gemEnergy * 0.0033f);
		float bpLvlAdjust = 0.000001f;
		
		float attrBattle = HP * DPS * bpLvlAdjust;
		
	    //战力相关
	    float bpSpellBasic = 100.0f;
	    float bpDazhaoLvl = 1.0f;
	    float bpPhyLvl = 1.0f;
	    float bpMagicLvl = 1.0f;
	    float bpDotLvl = 1.0f;
	    
        int dazhaoLvl = 0;
        int phyLvl = 0;
        int magicLvl = 0;
        int dotLvl = 0;
        
        for (HSSkill skill : monsterBuilder.getSkillList()) {
        	SpellCfg spellCfg = HawkConfigManager.getInstance().getConfigByKey(SpellCfg.class, skill.getSkillId());
        	if (spellCfg == null) {
				continue;
			}
        	int category = spellCfg.getCategory();
			if (category == Const.SpellType.Spell_Type_PhyDaZhao_VALUE || category == Const.SpellType.Spell_Type_MagicDazhao_VALUE) {
				dazhaoLvl = skill.getLevel();
			}
			else if (category == Const.SpellType.Spell_Type_PhyAttack_VALUE) {
				phyLvl = skill.getLevel();
			}
			else if (category == Const.SpellType.Spell_Type_MgicAttack_VALUE) {
				magicLvl = skill.getLevel();
			}
			else if (category == Const.SpellType.Spell_Type_Dot_VALUE) {
				dotLvl = skill.getLevel();
			}
		}
		
		float spellBattle = bpSpellBasic + bpDazhaoLvl * dazhaoLvl + bpPhyLvl * phyLvl + bpMagicLvl * magicLvl + bpDotLvl * dotLvl;
		return attrBattle + spellBattle;
	}
}
