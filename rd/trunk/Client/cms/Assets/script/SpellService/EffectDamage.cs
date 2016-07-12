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
}

public class EffectDamage : Effect
{
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
        base.Init(pt, owner);
    }
    //---------------------------------------------------------------------------------------------
    public override void Apply(float applyTime, string wpID)
    {
        base.Apply(applyTime, wpID);
        CalculateDamage(wpID);
    }
    //---------------------------------------------------------------------------------------------
    public override int CalculateHit()
    {
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

        return base.CalculateHit();
    }
    //---------------------------------------------------------------------------------------------
    public void CalculateDamage(string wpID)
    {
        if (CalculateHit() == SpellConst.hitSuccess)
        {
            GameUnit caster = spellService.GetUnit(casterID);
            GameUnit target = spellService.GetUnit(targetID);
            GameDataMgr gdMgr = GameDataMgr.Instance;

            //暴击计算 min(max(N+L(lv1-lv2))+总附加命中率,60%,100%)
            float damageRatio = 1.0f;
            float randKey = UnityEngine.Random.Range(0.0f, 1.0f);
            //暴击率 = 暴击常数 + 施法者暴击率 - 目标抗暴
            bool critical = randKey <= (SpellConst.criticalRatio + caster.criticalRatio - target.antiCriticalRatio);
            if (critical)
            {
                //暴击加成 =   暴击加成常数 + 附加暴击加成
                damageRatio = SpellConst.criticalDamgeRatio + caster.criticalDamageRatio;
            }

            //受伤比计算 max(1/(1+(守方总防御力-攻方防御穿透)/I(min(lv1,lv2))),25%)
            float injuryRatio = 1.0f / (1.0f + (target.defense * (1.0f + target.spellDefenseRatio) - caster.defensePierce) / SpellFunctions.GetInjuryAdjustNum(caster.pbUnit.level, target.pbUnit.level));
            injuryRatio = injuryRatio < 0.25f ? 0.25f : injuryRatio;

            EffectDamageProtoType damageProto = protoEffect as EffectDamageProtoType;
            int damageAmount = 0;
            float spellLevelRatio = ownedSpell.level * ownedSpell.spellData.levelAdjust;
            //弱点
			WeakPointRuntimeData wpRuntime = null;
			if(!string.IsNullOrEmpty(wpID) )
			{
				target.battleUnit.wpGroup.allWpDic.TryGetValue(wpID,out wpRuntime);
			}

            if (damageProto.isHeal == true)
            {
                //治疗
                damageAmount = (int)(
                                damageRatio * SpellConst.intelligenceToAttack * caster.intelligence *  //暴击伤害系数 * 攻击
                                (1.0f + gdMgr.PlayerDataAttr.equipIntelligenceRatio + caster.additionHealRatio) * //主角和怪物装备加成
                                (damageProto.attackFactor + spellLevelRatio) * //技能加成
                                (1.0f + caster.spellIntelligenceRatio)
                                );//buff加成(队长技 etc)

            }
            else
            {

                float wpRatio = wpRuntime != null ? wpRuntime.damageRate : 1.0f;

                //物理伤害
                if (damageProto.damageType == SpellConst.damagePhy)
                {
                    damageAmount = (int)(
                                    damageRatio * injuryRatio * SpellConst.strengthToAttack * caster.strength *  //暴击伤害系数 * 受伤比 * 攻击
                                    (1.0f + gdMgr.PlayerDataAttr.equipStrengthRatio + caster.additionDamageRatio - target.minusDamageRatio) * //主角和怪物装备加成
                                    (damageProto.attackFactor + spellLevelRatio) * //技能加成
                                    (1.0f + caster.spellStrengthRatio) *
                                    wpRatio *
                                    (1.0f + target.spellDefenseDamageRatio)
                                    ); //buff加成(队长技 etc)

                    //物理伤害反应
                    List<Buff> buffList = target.buffList;
                    int count = buffList.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        if (buffList[i].buffProto.category == (int)(BuffType.Buff_Type_Dazhao))
                        {
                            buffList[i].CheckDazhaoInterrupt(applyTime);
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

                    damageAmount = (int)(
                                    damageRatio * injuryRatio * SpellConst.intelligenceToAttack * caster.intelligence *  //暴击伤害系数 * 受伤比 * 攻击
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
                //    damageAmount = -1;
                //else
                //    damageAmount = -1;

                //弱点伤害计算
                if (wpRuntime != null)
                {
                    target.OnDamageWeakPoint(wpRuntime.id, damageAmount, applyTime);
                }
            }

            //没有弱点或者弱点属性关联伤害，则扣除/增加怪物血量
            if (wpRuntime == null || wpRuntime.staticData.isDamagePoint == 1)
            {
                target.curLife += damageAmount;
                if (target.curLife <= 0)
                {
                    target.curLife = 0;
                    SpellUnitDeadArgs args = new SpellUnitDeadArgs();
                    args.triggerTime = applyTime;
                    args.casterID = casterID;
                    args.deathID = targetID;
                    spellService.AddDeadData(args);
                }
                else if (target.curLife > target.maxLife)
                {
                    target.curLife = target.maxLife;
                }
            }

            //trigger damage event
            {
                SpellVitalChangeArgs args = new SpellVitalChangeArgs();
                args.vitalType = (int)VitalType.Vital_Type_Default;
                args.triggerTime = applyTime;
                args.casterID = casterID;
                args.targetID = targetID;
                args.isCritical = critical;
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
                    args.wpID = string.Empty;
                    args.wpNode = string.Empty;
                }
                spellService.TriggerEvent(GameEventList.SpellLifeChange, args);
            }
        }

		//统计攻击次数
		GameEventMgr.Instance.FireEvent<SpellAttackStatisticsParam> (GameEventList.SpellAttackStatistics, new SpellAttackStatisticsParam (ownedSpell, casterID, targetID));

    }
    //---------------------------------------------------------------------------------------------
}
