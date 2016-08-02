using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class EffectDamageProtoType : EffectPrototype
{
    public int damageType;
    public float attackFactor;
    public bool isHeal;
    public int damageProperty;//五行伤害

    public float fixLifeRatio;//血量置数
}

public class EffectDamage : Effect
{
    //for spellfunctions use
    public int damageAmount = 0;
    public bool isCritical = false;

    //---------------------------------------------------------------------------------------------
    public override void Init(EffectPrototype pt, SpellService owner)
    {
        EffectDamageProtoType damagePtOut = pt as EffectDamageProtoType;
        protoEffect = new EffectDamageProtoType();
        EffectDamageProtoType damagePt = protoEffect as EffectDamageProtoType;
        damagePt.damageType = damagePtOut.damageType;
        damagePt.attackFactor = damagePtOut.attackFactor;
        damagePt.isHeal = damagePtOut.isHeal;
        damagePt.damageProperty = damagePtOut.damageProperty;
        damagePt.fixLifeRatio = damagePtOut.fixLifeRatio;
        base.Init(pt, owner);
        autoGenerateEvet = false;
    }
    //---------------------------------------------------------------------------------------------
    public override bool Apply(float applyTime, string wpID)
    {
        if (base.Apply(applyTime, wpID) == false)
            return false;

        CalculateDamage(wpID);
        return true;
    }
    //---------------------------------------------------------------------------------------------
    public override int CalculateHit(string wpName)
    {
        if (absoluteHit == true)
            return SpellConst.hitSuccess;

        EffectDamageProtoType damageProto = protoEffect as EffectDamageProtoType;
        if (damageProto.isHeal)
        {
            return SpellConst.hitSuccess;
        }
        else 
        {
            GameUnit target = spellService.GetUnit(targetID);
            if (target.invincible > 0)
            {
                //immune event
                SpellEffectArgs args = new SpellEffectArgs();
                args.triggerTime = applyTime;
                args.casterID = casterID;
                args.targetID = targetID;
                spellService.TriggerEvent(GameEventList.SpellImmune, args);
                return SpellConst.hitImmune;
            }
        }

        return base.CalculateHit(wpName);
    }
    //---------------------------------------------------------------------------------------------
    public void CalculateDamage(string wpID)
    {
        GameUnit caster = spellService.GetUnit(casterID);
        GameUnit target = spellService.GetUnit(targetID);
        EffectDamageProtoType damageProto = protoEffect as EffectDamageProtoType;

        if (damageProto.fixLifeRatio >0)
        {
            float blood = target.maxLife * damageProto.fixLifeRatio;
            target.curLife = (int)blood;

            SpellVitalChangeArgs args = new SpellVitalChangeArgs();
            args.vitalType = (int)VitalType.Vital_Type_FixLife;
            args.triggerTime = applyTime;
            args.casterID = casterID;
            args.targetID = targetID;
            args.isCritical = false;
            args.vitalChange = 0;
            args.vitalCurrent = target.curLife;//TODO: need weak point life?
            args.vitalMax = target.maxLife;
            spellService.TriggerEvent(GameEventList.SpellLifeChange, args);

            SpellEffectArgs effectArgs = new SpellEffectArgs();
            if (target != null)
            {
                WeakPointData wp = null;
                if (target.attackWpName != null)
                {
                    wp = StaticDataMgr.Instance.GetWeakPointData(target.attackWpName);
                }
                effectArgs.wpNode = wp != null ? wp.node : "e_shouji";
            }
            effectArgs.triggerTime = applyTime;
            effectArgs.casterID = casterID;
            effectArgs.targetID = targetID;
            effectArgs.effectID = protoEffect.id;
            spellService.TriggerEvent(GameEventList.SpellEffect, effectArgs);
        }
        else
        {
            if (CalculateHit(wpID) == SpellConst.hitSuccess)
            {
                GameDataMgr gdMgr = GameDataMgr.Instance;

                //暴击计算 min(max(N+L(lv1-lv2))+总附加命中率,60%,100%)
                float damageRatio = 1.0f;
                float randKey = UnityEngine.Random.Range(0.0f, 1.0f);
                //暴击率 = 暴击常数 + 施法者暴击率 - 目标抗暴
                isCritical = randKey <= (SpellConst.criticalRatio + caster.criticalRatio - target.antiCriticalRatio);
                if (isCritical)
                {
                    //暴击加成 =   暴击加成常数 + 附加暴击加成
                    damageRatio = SpellConst.criticalDamgeRatio + caster.criticalDamageRatio;
                }

                //受伤比计算 max(1/(1+(守方总防御力-攻方防御穿透)/I(min(lv1,lv2))),25%)
                float injuryRatio = 1.0f / (1.0f + (target.defense * (1.0f + target.spellDefenseRatio) - caster.defensePierce) / SpellFunctions.GetInjuryAdjustNum(caster.pbUnit.level, target.pbUnit.level));
                injuryRatio = injuryRatio < 0.25f ? 0.25f : injuryRatio;

                damageAmount = 0;
                float spellLevelRatio = ownedSpell.level * ownedSpell.spellData.levelAdjust;
                //弱点
                WeakPointRuntimeData wpRuntime = null;
                if (!string.IsNullOrEmpty(wpID))
                {
                    target.battleUnit.wpGroup.allWpDic.TryGetValue(wpID, out wpRuntime);
                }

                //无法治疗检测
                bool noHeal = false;
                bool immune = false;
                List<Buff> buffList = target.buffList;
                int count = buffList.Count;
                if (damageProto.isHeal == true)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        if (buffList[i].buffProto.category == (int)(BuffType.Buff_Type_NoHeal) && buffList[i].IsFinish == false)
                        {
                            noHeal = true;
                            buffList[i].Finish(applyTime);
                        }
                    }
                }
                //免疫检测
                if (damageProto.isHeal == false)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        if (buffList[i].IsFinish)
                            continue;

                        if (
                            (damageProto.damageType == SpellConst.damagePhy && buffList[i].buffProto.category == (int)BuffType.Buff_Type_PhyImmune)
                            || (damageProto.damageType == SpellConst.damageMagic && buffList[i].buffProto.category == (int)BuffType.Buff_Type_MagicImmune)
                            )
                        {
                            immune = true;
                            break;
                        }
                    }
                }

                if (damageProto.isHeal == true)
                {
                    if (noHeal == false && 
                        (spellService.IsInDeathList(targetID) == false || (ownedBuff != null && ownedBuff.buffProto.noDead > 0))
                        )
                    {
                        //治疗
                        damageAmount = (int)(
                                        damageRatio * SpellConst.intelligenceToAttack * caster.intelligence *  //暴击伤害系数 * 攻击
                                        (1.0f + target.additionHealRatio) * //主角和怪物装备加成
                                        (damageProto.attackFactor + spellLevelRatio) * //技能加成
                                        (1.0f + caster.spellIntelligenceRatio)
                                        );//buff加成(队长技 etc)
                    }
                    else
                    {
                        damageAmount = 0;
                        if (noHeal == true)
                        {
                            SpellVitalChangeArgs interruptArgs = new SpellVitalChangeArgs();
                            interruptArgs.vitalType = (int)VitalType.Vital_Type_NoHeal;
                            interruptArgs.triggerTime = applyTime;
                            interruptArgs.casterID = 0;
                            interruptArgs.targetID = targetID;
                            spellService.TriggerEvent(GameEventList.SpellLifeChange, interruptArgs);
                        }
                    }
                }
                else if (immune == false)
                {
                    float wpRatio = wpRuntime != null ? wpRuntime.damageRate : 1.0f;
                    //物理伤害
                    if (damageProto.damageType == SpellConst.damagePhy)
                    {
                        //减伤护盾
                        float curInjuryRatio = injuryRatio - target.spellphyReduceInjury;
                        curInjuryRatio = curInjuryRatio < 0.0f ? 0.0f : curInjuryRatio;
                        damageAmount = (int)(
                                        damageRatio * curInjuryRatio * SpellConst.strengthToAttack * caster.strength *  //暴击伤害系数 * 受伤比 * 攻击
                                        (1.0f + caster.additionDamageRatio - target.minusDamageRatio) * //主角和怪物装备加成
                                        (damageProto.attackFactor + spellLevelRatio) * //技能加成
                                        (1.0f + caster.spellStrengthRatio) *
                                        wpRatio *
                                        (1.0f + target.spellDefenseDamageRatio)
                                        ); //buff加成(队长技 etc)

                        //物理伤害打断检测
                        int targetBuffcount = target.buffList.Count;
                        for (int i = 0; i < targetBuffcount; ++i)
                        {
                            if (target.buffList[i].buffProto.category == (int)(BuffType.Buff_Type_Dazhao))
                            {
                                target.buffList[i].CheckDazhaoInterrupt(applyTime);
                                break;
                            }
                        }
                    }
                    //法术伤害
                    else
                    {
                        //主角五行伤害加成
                        float propertyDamageRatio = 1.0f;
                        switch (damageProto.damageProperty)
                        {
                            case SpellConst.propertyGold:
                                propertyDamageRatio += gdMgr.PlayerDataAttr.goldDamageRatio;
                                break;
                            case SpellConst.propertyWood:
                                propertyDamageRatio += gdMgr.PlayerDataAttr.woodDamageRatio;
                                break;
                            case SpellConst.propertyWater:
                                propertyDamageRatio += gdMgr.PlayerDataAttr.waterDamageRatio;
                                break;
                            case SpellConst.propertyFire:
                                propertyDamageRatio += gdMgr.PlayerDataAttr.fireDamageRatio;
                                break;
                            case SpellConst.propertyEarth:
                                propertyDamageRatio += gdMgr.PlayerDataAttr.earthDamageRatio;
                                break;
                        }
                        //五行相生相克系数
                        int targetProp = wpRuntime != null ? wpRuntime.property : target.property;
                        EffectDamageProtoType damagePt = protoEffect as EffectDamageProtoType;
                        propertyDamageRatio *= SpellFunctions.GetPropertyDamageRatio((int)(damagePt.damageProperty), targetProp);
                        //减伤护盾
                        float curInjuryRatio = injuryRatio - target.spellmgReduceInjury;
                        curInjuryRatio = curInjuryRatio < 0.0f ? 0.0f : curInjuryRatio;
                        damageAmount = (int)(
                                        damageRatio * curInjuryRatio * SpellConst.intelligenceToAttack * caster.intelligence *  //暴击伤害系数 * 受伤比 * 攻击
                                        (1.0f + gdMgr.PlayerDataAttr.equipIntelligenceRatio + caster.additionDamageRatio - target.minusDamageRatio) * //主角和怪物装备加成
                                        (damageProto.attackFactor + spellLevelRatio) * //技能加成
                                        (1.0f + caster.spellIntelligenceRatio) *//buff加成(队长技 etc)
                                        propertyDamageRatio *
                                        wpRatio *
                                        (1.0f + target.spellDefenseDamageRatio)
                                        ); //五行相关
                    }
                    //伤害*-1 修正为负数
                    damageAmount *= -1;
                    //if (caster.pbUnit.camp == UnitCamp.Enemy)
                    //    damageAmount = -3000;
                    //else
                    //    damageAmount = -5;
                }

                if (immune == false)
                {
                    if (damageProto.isHeal == false)
                    {
                        //伤害为0 扣1血 在护盾检测前检查
                        if (damageAmount == 0)
                        {
                            damageAmount = -1;
                        }
                        //检测护盾
                        int buffCount1 = target.buffList.Count;
                        for (int i = 0; i < buffCount1; ++i)
                        {
                            target.buffList[i].OnShield(applyTime, this, ref damageAmount);
                        }
                        //弱点伤害计算
                        if (wpRuntime != null)
                        {
                            target.OnDamageWeakPoint(wpRuntime.id, damageAmount, applyTime);
                        }
                    }

                    //没有弱点或者弱点属性关联伤害，则扣除/增加怪物血量
                    if (wpRuntime == null || wpRuntime.staticData.isDamagePoint == 1)
                    {
                        if (damageAmount < 0 || damageProto.isHeal == true)
                        {
                            target.curLife += damageAmount;
                            if (target.curLife <= 0)
                            {
                                target.curLife = 0;
                                SpellUnitDeadArgs args = new SpellUnitDeadArgs();
                                args.triggerTime = applyTime;
                                args.casterID = casterID;
                                args.deathID = targetID;
                                spellService.AddDeadData(args, target, this);

                                //death response
                                for (int i = 0; i < target.buffList.Count; ++i)
                                {
                                    target.buffList[i].DeadResponse(applyTime, this, wpID);
                                }
                            }
                            else if (target.curLife > target.maxLife)
                            {
                                target.curLife = target.maxLife;
                            }
                        }
                    }

                    //大招受击事件
                    if (damageAmount < 0 && damageProto.isHeal == false)
                    {
                        SpellEffectArgs effectArgs = new SpellEffectArgs();
                        if (ownedSpell.spellData.category == (int)SpellType.Spell_Type_PhyDaZhao ||
                            ownedSpell.spellData.category == (int)SpellType.Spell_Type_MagicDazhao || isCritical)
                        {
                            effectArgs.targetID = targetID;
                            effectArgs.triggerTime = applyTime;
                            spellService.TriggerEvent(GameEventList.BashHit, effectArgs);
                        }
                        else
                        {
                            effectArgs.targetID = targetID;
                            effectArgs.triggerTime = applyTime;
                            spellService.TriggerEvent(GameEventList.NormalHit, effectArgs);
                        }
                    }

                    //trigger damage event
                    if (damageAmount < 0 || (noHeal == false && damageProto.isHeal == true))
                    {
                        SpellEffectArgs effectArgs = new SpellEffectArgs();
                        if (target != null)
                        {
                            WeakPointData wp = null;
                            if (target.attackWpName != null)
                            {
                                wp = StaticDataMgr.Instance.GetWeakPointData(target.attackWpName);
                            }
                            effectArgs.wpNode = wp != null ? wp.node : "e_shouji";
                        }
                        effectArgs.triggerTime = applyTime;
                        effectArgs.casterID = casterID;
                        effectArgs.targetID = targetID;
                        effectArgs.effectID = protoEffect.id;
                        spellService.TriggerEvent(GameEventList.SpellEffect, effectArgs);

                        SpellVitalChangeArgs args = new SpellVitalChangeArgs();
                        args.vitalType = (int)VitalType.Vital_Type_Default;
                        args.triggerTime = applyTime;
                        args.casterID = casterID;
                        args.targetID = targetID;
                        args.isCritical = isCritical;
                        args.vitalChange = damageAmount;
                        args.vitalCurrent = target.curLife;//TODO: need weak point life?
                        args.vitalMax = target.maxLife;
                        if (wpRuntime != null)
                        {
                            args.wpID = wpRuntime.id;
                            args.wpNode = wpRuntime.staticData.node;
                        }
                        else
                        {
                            args.wpID = "e_shouji";
                            args.wpNode = "e_shouji";
                        }
                        spellService.TriggerEvent(GameEventList.SpellLifeChange, args);
                        //伤害反应
                        if (noDamageResponse == false)
                        {
                            //caster
                            int buffCount = caster.buffList.Count;
                            for (int i = 0; i < buffCount; ++i)
                            {
                                caster.buffList[i].DamageResponse(applyTime, this, wpID);
                            }
                            //target
                            buffCount = target.buffList.Count;
                            for (int i = 0; i < buffCount; ++i)
                            {
                                target.buffList[i].DamageResponse(applyTime, this, wpID);
                            }
                        }
                    }
                }
                else
                {
                    SpellVitalChangeArgs interruptArgs = new SpellVitalChangeArgs();
                    if (damageProto.damageType == SpellConst.damagePhy)
                    {
                        interruptArgs.vitalType = (int)VitalType.Vital_Type_PhyImmune;
                    }
                    else
                    {
                        interruptArgs.vitalType = (int)VitalType.Vital_Type_MagicImmune;
                    }
                    interruptArgs.triggerTime = applyTime;
                    interruptArgs.casterID = 0;
                    interruptArgs.targetID = targetID;
                    spellService.TriggerEvent(GameEventList.SpellLifeChange, interruptArgs);
                }

                //link effect
                Effect curEffect = spellService.GetEffect(protoEffect.linkEffect);
                if (curEffect != null)
                {
                    curEffect.SetOwnedBuff(ownedBuff);
                    curEffect.SetOwnedSpell(ownedSpell);
                    curEffect.targetID = targetID;
                    curEffect.absoluteHit = true;
                    curEffect.Apply(applyTime, wpID);
                }
            }
        }       

		//统计攻击次数
		GameEventMgr.Instance.FireEvent<SpellAttackStatisticsParam> (GameEventList.SpellAttackStatistics, new SpellAttackStatisticsParam (ownedSpell, casterID, targetID));

    }
    //---------------------------------------------------------------------------------------------
}
