using UnityEngine;
using System.Collections;

public class SpellFunctions  
{
    public static float GetInjuryAdjustNum(int casterLvl, int targetLvl)
    {
        return 2100.0f;
    }

    public static float GetHitRatio(int casterLvl, int targetLvl)
    {
        return 0.0f;
    }

    public static float GetPropertyDamageRatio(int casterProp, int targetProp)
    {
        //propertyGold->guang propertyEarth->an
        float damageRatio = 1.0f;
        if (casterProp == SpellConst.propertyGold)
        {
            if (targetProp == SpellConst.propertyEarth)
            {
                return SpellConst.propertyEnhance;
            }
        }
        else if (casterProp == SpellConst.propertyWood)
        {
            if (targetProp == SpellConst.propertyWater)
            {
                return SpellConst.propertyEnhance;
            }
            else if (targetProp == SpellConst.propertyFire)
            {
                return SpellConst.propertyWeaken;
            }
        }
        else if (casterProp == SpellConst.propertyWater)
        {
            if (targetProp == SpellConst.propertyFire)
            {
                return SpellConst.propertyEnhance;
            }
            else if (targetProp == SpellConst.propertyWood)
            {
                return SpellConst.propertyWeaken;
            }
        }
        else if (casterProp == SpellConst.propertyFire)
        {
            if (targetProp == SpellConst.propertyWood)
            {
                return SpellConst.propertyEnhance;
            }
            else if (targetProp == SpellConst.propertyWater)
            {
                return SpellConst.propertyWeaken;
            }
        }
        else if (casterProp == SpellConst.propertyEarth)
        {
            if (targetProp == SpellConst.propertyGold)
            {
                return SpellConst.propertyEnhance;
            }
        }
        return damageRatio;
    }

    public static int IsEnemy(int camp)
    {
        return (camp == (int)UnitCamp.Enemy) ? 1 : 0;
    }

    public static int Default(int args)
    {
        return 1;
    }

    // buff response validators
    public static int BuffValidatorSample(
        Buff triggerBuff,
        Effect triggerEffect,
        SpellService spellService
        )
    {
        EffectDamage damageEffect = triggerEffect as EffectDamage;
        EffectApplyBuff buffEffect = triggerEffect as EffectApplyBuff;
        //血量低于xx触发示例
        if (damageEffect != null)
        {
            GameUnit target = spellService.GetUnit(triggerEffect.targetID);
            float lifeRatio = target.curLife / (float)target.maxLife;
            if (lifeRatio < 0.05f)
                return 1;
        }
        //受到某个类型伤害触发示例
        if (damageEffect != null && damageEffect.targetID == triggerBuff.targetID)
        {
            EffectDamageProtoType damageProto = damageEffect.protoEffect as EffectDamageProtoType;
            if (damageProto.isHeal == false && damageProto.damageType == SpellConst.damagePhy)//SpellConst.damageMagic)
            {
                return 1;
            }
        }
        //造成某个类型伤害触发示例
        if (damageEffect != null && damageEffect.casterID == triggerBuff.targetID)
        {
            EffectDamageProtoType damageProto = damageEffect.protoEffect as EffectDamageProtoType;
            //造成某类伤害示例
            if (damageProto.isHeal == false && damageProto.damageType == SpellConst.damagePhy)//SpellConst.damageMagic)
            {
                return 1;
            }
        }
        //使用某类技能造成伤害触发示例
        if (triggerEffect != null && triggerEffect.casterID == triggerBuff.targetID)
        {
            Spell ownedSpell = triggerEffect.ownedSpell;
            if (ownedSpell != null && ownedSpell.spellData.category == (int)SpellType.Spell_Type_MgicAttack)
            {
                return 1;
            }
        }

        //触发几率示例
        float randNum = Random.Range(0.0f, 1.0f);
        if (randNum > 0.5f)
        {
            return 1;
        }

        return 0;
    }
	//无条件
	public static int uncondition(
        Buff triggerBuff,
        Effect triggerEffect,
        SpellService spellService
        )
    {
        EffectDamage damageEffect = triggerEffect as EffectDamage;
        EffectApplyBuff buffEffect = triggerEffect as EffectApplyBuff;
		
		return 1;
	}
    //使用物理攻击(未包含大招)
    public static int UsingPhysicalRate_40(
        Buff triggerBuff,
        Effect triggerEffect,
        SpellService spellService
        )
    {
        EffectDamage damageEffect = triggerEffect as EffectDamage;
        EffectApplyBuff buffEffect = triggerEffect as EffectApplyBuff;

        //使用某类技能造成伤害触发示例
		float randNum = Random.Range(0.0f, 1.0f);
        if (triggerEffect != null && triggerEffect.casterID == triggerBuff.targetID)
        {
            Spell ownedSpell = triggerEffect.ownedSpell;
            if (ownedSpell != null && ownedSpell.spellData.category == (int)SpellType.Spell_Type_PhyAttack && randNum <= 0.4f)
            {
                return 1;
            }
        }

        return 0;
    }
    public static int UsingPhysicalRate_35(
        Buff triggerBuff,
        Effect triggerEffect,
        SpellService spellService
        ) 
    {
        EffectDamage damageEffect = triggerEffect as EffectDamage;
        EffectApplyBuff buffEffect = triggerEffect as EffectApplyBuff;

        //使用某类技能造成伤害触发示例
		float randNum = Random.Range(0.0f, 1.0f);
		if (damageEffect != null && damageEffect.casterID == triggerBuff.targetID)
		{
			EffectDamageProtoType damageProto = damageEffect.protoEffect as EffectDamageProtoType;
			if (damageProto.isHeal == false)
			{
				Spell ownedSpell = triggerEffect.ownedSpell;
				if (ownedSpell != null && ownedSpell.spellData.category == (int)SpellType.Spell_Type_PhyAttack && randNum <= 0.35f)
				{
					return 1;
				}
			}				
		}	
		return 0;
    } 
    //使用物理攻击(未包含大招)
    public static int UsingPhysicalRate_50(
        Buff triggerBuff,
        Effect triggerEffect,
        SpellService spellService
        ) 
    {
        EffectDamage damageEffect = triggerEffect as EffectDamage;
        EffectApplyBuff buffEffect = triggerEffect as EffectApplyBuff;

        //使用某类技能造成伤害触发示例
		float randNum = Random.Range(0.0f, 1.0f);
		if (damageEffect != null && damageEffect.casterID == triggerBuff.targetID)
		{
			EffectDamageProtoType damageProto = damageEffect.protoEffect as EffectDamageProtoType;
			if (damageProto.isHeal == false)
			{
				Spell ownedSpell = triggerEffect.ownedSpell;
				if (ownedSpell != null && ownedSpell.spellData.category == (int)SpellType.Spell_Type_PhyAttack && randNum <= 0.5f)
				{
					return 1;
				}
			}				
		}	
		return 0;
    }      
    //使用魔法攻击(未包含大招)
    public static int UsingMagicRate_20(
        Buff triggerBuff,
        Effect triggerEffect,
        SpellService spellService
        )
    {
        EffectDamage damageEffect = triggerEffect as EffectDamage;
        EffectApplyBuff buffEffect = triggerEffect as EffectApplyBuff;

        //使用某类技能造成伤害触发示例
		float randNum = Random.Range(0.0f, 1.0f);
		if (damageEffect != null && damageEffect.casterID == triggerBuff.targetID)
		{
			EffectDamageProtoType damageProto = damageEffect.protoEffect as EffectDamageProtoType;
			if (damageProto.isHeal == false)
			{
				Spell ownedSpell = triggerEffect.ownedSpell;
				if (ownedSpell != null && ownedSpell.spellData.category == (int)SpellType.Spell_Type_MgicAttack && randNum <= 0.2f)
				{
					return 1;
				}
			}				
		}	
		return 0;
    }   

	//使用魔法攻击(未包含大招)
    public static int UsingMagicRate_50(
        Buff triggerBuff,
        Effect triggerEffect,
        SpellService spellService
        )
    {
        EffectDamage damageEffect = triggerEffect as EffectDamage;
        EffectApplyBuff buffEffect = triggerEffect as EffectApplyBuff;

        //使用某类技能造成伤害触发示例
		float randNum = Random.Range(0.0f, 1.0f);
        if (triggerEffect != null && triggerEffect.casterID == triggerBuff.targetID)
        {
            Spell ownedSpell = triggerEffect.ownedSpell;
            if (ownedSpell != null && ownedSpell.spellData.category == (int)SpellType.Spell_Type_MgicAttack && randNum <= 0.5f)
            {
                return 1;
            }
        }
        return 0;
    }
	//使用治疗攻击(未包含大招)
    public static int UsingCureRate_10(
        Buff triggerBuff,
        Effect triggerEffect,
        SpellService spellService
        )
    {
        EffectDamage damageEffect = triggerEffect as EffectDamage;
        EffectApplyBuff buffEffect = triggerEffect as EffectApplyBuff;

        //使用某类技能造成伤害触发示例
		float randNum = Random.Range(0.0f, 1.0f);
		if (triggerEffect != null && triggerEffect.casterID == triggerBuff.targetID)
        {
            Spell ownedSpell = triggerEffect.ownedSpell;
            if (ownedSpell != null && ownedSpell.spellData.category == (int)SpellType.Spell_Type_Cure && randNum <= 0.1f)
            {
                return 1;
            }
        }
        return 0;
    }


    //受到物理或魔法攻击（不包含大招等）
    public static int HurtbyPhysicalMagicRate_80(
        Buff triggerBuff,
        Effect triggerEffect,
        SpellService spellService
        )
    {
        EffectDamage damageEffect = triggerEffect as EffectDamage;
        EffectApplyBuff buffEffect = triggerEffect as EffectApplyBuff;
        //受到某类技能造成伤害触发示例
		float randNum = Random.Range(0.0f, 1.0f);
        if (damageEffect != null && damageEffect.targetID == triggerBuff.targetID)
        {
            EffectDamageProtoType damageProto = damageEffect.protoEffect as EffectDamageProtoType;
            if (damageProto.isHeal == false && (damageProto.damageType == SpellConst.damagePhy || damageProto.damageType == SpellConst.damageMagic) && randNum <= 0.8f)
            {
                return 1;
            }
			
        }
        return 0;
    }
	public static int HurtbyPhysicalRate_80(
        Buff triggerBuff,
        Effect triggerEffect,
        SpellService spellService
        )
    {
        EffectDamage damageEffect = triggerEffect as EffectDamage;
        EffectApplyBuff buffEffect = triggerEffect as EffectApplyBuff;
        //受到某类技能造成伤害触发示例
		float randNum = Random.Range(0.0f, 1.0f);
        if (damageEffect != null && damageEffect.targetID == triggerBuff.targetID)
        {
            EffectDamageProtoType damageProto = damageEffect.protoEffect as EffectDamageProtoType;
            if (damageProto.isHeal == false && damageProto.damageType == SpellConst.damagePhy && randNum <= 0.8f)
            {
                return 1;
            }
        }
        return 0;
    }


    //生命值低于0.25
    public static int LifeLower_25(
        Buff triggerBuff,
        Effect triggerEffect,
        SpellService spellService
        )
    {
        EffectDamage damageEffect = triggerEffect as EffectDamage;
        EffectApplyBuff buffEffect = triggerEffect as EffectApplyBuff;

        if (damageEffect != null &&
		    damageEffect.targetID == triggerBuff.targetID)
        {
            GameUnit target = spellService.GetUnit(triggerEffect.targetID);
            float lifeRatio = target.curLife / (float)target.maxLife;
            if (lifeRatio < 0.25f)
                return 1;
        }
        return 0;
    }

}
