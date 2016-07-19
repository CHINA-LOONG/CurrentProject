using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class BuffPrototype
{
    public string id;
    public string periodEffectID;
    public int category;
    //public int isClear;//切进程是否删除
    public int duration;

    //状态改变
    public int stun;//眩晕
    public int invincible;//无敌
    public int dazhao;//大招状态

    //属性改变
    public float strengthRatio;
    public float intelligenceRatio;
    public float defenseRatio;//防御力
    public float speedRatio;
    public float defenseDamageRatio;//防御系数
     
    public float phyReduceInjury;//物理减伤护盾
    public float mgReduceInjury;//物理减伤护盾

    public int shieldNum;//吸收护盾固定值
    public float shieldRatio;//吸收护盾比例
    public byte shieldBaseAttr;//吸收护盾依赖属性（力量 智力）

    public float healthRatio;//体力
    public float hitRatio;//命中率
    public float criticalRatio;//暴击率

    //显示相关
    public string icon;

    //
    public int noDead;

    //buff response
    public int responseCount;
    public string damageResponse;
    public string deadResponse;
}

public class BuffPrototypes : ScriptableObject
{
    public List<BuffPrototype> data = new List<BuffPrototype>();
}

public class Buff
{
    public SpellService spellService;
    public BuffPrototype buffProto;
    public Spell ownedSpell;
    public int casterID;
    public int targetID;
    public float applyTime;
    private int periodCount;
    private bool isFinish;
    public bool IsFinish
    {
        get { return isFinish; }
    }
    private BuffFinisType finishType;
    private int responseCount;
    private int phyShield;
    private int magicShield;

    public KeyValuePair<string, string> damageResponse;
    public KeyValuePair<string, string> deadResponse;
    //---------------------------------------------------------------------------------------------
    public void Init(BuffPrototype buffPt, SpellService owner)
    {
        spellService = owner;
        buffProto = new BuffPrototype();
        buffProto.id = buffPt.id;
        buffProto.icon = buffPt.icon;
        buffProto.category = buffPt.category;
        buffProto.periodEffectID = buffPt.periodEffectID;
        buffProto.duration = buffPt.duration;
        buffProto.stun = buffPt.stun;
        buffProto.invincible = buffPt.invincible;
        buffProto.dazhao = buffPt.dazhao;
        buffProto.strengthRatio = buffPt.strengthRatio;
        buffProto.intelligenceRatio = buffPt.intelligenceRatio;
        buffProto.defenseRatio = buffPt.defenseRatio;
        buffProto.speedRatio = buffPt.speedRatio;
        buffProto.defenseDamageRatio = buffPt.defenseDamageRatio;
        buffProto.noDead = buffPt.noDead;
        buffProto.phyReduceInjury = buffPt.phyReduceInjury;
        buffProto.mgReduceInjury = buffPt.mgReduceInjury;
        buffProto.shieldNum = buffPt.shieldNum;
        buffProto.shieldRatio = buffPt.shieldRatio;
        buffProto.shieldBaseAttr = buffPt.shieldBaseAttr;
        buffProto.deadResponse = buffPt.deadResponse;
        buffProto.damageResponse = buffPt.damageResponse;
        buffProto.responseCount = buffPt.responseCount;
        buffProto.criticalRatio = buffPt.criticalRatio;
        buffProto.hitRatio = buffPt.hitRatio;
        buffProto.healthRatio = buffPt.healthRatio;
        isFinish = false;

        phyShield = magicShield = 0;
        responseCount = buffPt.responseCount;

        if (string.IsNullOrEmpty(buffProto.damageResponse) == false)
        {
            string[] res = buffProto.damageResponse.Split(',');
            if (res.Length == 2)
            {
                damageResponse = new KeyValuePair<string, string>(res[0], res[1]);
            }
        }

        if (string.IsNullOrEmpty(buffProto.deadResponse) == false)
        {
            string[] res = buffProto.deadResponse.Split(',');
            if (res.Length == 2)
            {
                deadResponse = new KeyValuePair<string, string>(res[0], res[1]);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetOwnedSpell(Spell spell)
    {
        ownedSpell = spell;
    }
    //---------------------------------------------------------------------------------------------
    public void Apply(float curTime)
    {
        periodCount = 0;
        isFinish = false;
        applyTime = curTime;
        GameUnit target = spellService.GetUnit(targetID);
        AddBuff(target);
    }
    //---------------------------------------------------------------------------------------------
    public void Update(float curTime)
    {
        if (isFinish)
        {
            return;
        }

        //List<Buff> buffList = spellService.GetUnit(targetID).buffList;

        //hot and dot
        ++periodCount;
		if (!string.IsNullOrEmpty(buffProto.periodEffectID))
        {
            Logger.Log("[SpellService]take periodic effect");
            Effect eft = spellService.GetEffect(buffProto.periodEffectID);
            if (eft != null)
            {
                eft.SetOwnedSpell(ownedSpell);
                eft.SetOwnedBuff(this);
                eft.Apply(curTime, "");
            }
        }
        //response

        if (periodCount >= buffProto.duration)
        {
            Finish(curTime);
            //buffList.Remove(this);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void Reset()
    {
        periodCount = 0;
        isFinish = false;
    }
    //---------------------------------------------------------------------------------------------
    void AddBuff(GameUnit target)
    {
        if (target == null)
            return;

        List<Buff> buffList = target.buffList;
        if (buffProto.category == (int)(BuffType.Buff_Type_Dot) || buffProto.category == (int)(BuffType.Buff_Type_Debuff))
        {
            SpellEffectArgs effectArgs = new SpellEffectArgs();
            effectArgs.targetID = targetID;
            spellService.TriggerEvent(GameEventList.BashHit, effectArgs);
        }

        if (buffProto.category == (int)(BuffType.Buff_Type_Dot))
        {
            Buff firstDot = null;
            int dotCount = 0;
            foreach (Buff buff in buffList)
            {
                if (buff.buffProto.category == (int)(BuffType.Buff_Type_Dot))
                {
                    if (++dotCount == 1)
                    {
                        firstDot = buff;
                    }

                    //同源同id，刷新
                    if (buff.casterID == casterID && buff.buffProto.id == buffProto.id)
                    {
                        buff.Reset();
                        return;
                    }

                    if (dotCount >= 3)
                    {
                        firstDot.Finish(applyTime);
                        firstDot.ModifyUnit(true, applyTime);
                        //buffList.Remove(firstDot);
                        break;
                    }
                }
            }
        }
        else if (
                buffProto.category == (int)(BuffType.Buff_Type_PhyJanshang) ||
                buffProto.category == (int)(BuffType.Buff_Type_MgJanshang) ||
                buffProto.category == (int)(BuffType.Buff_Type_PhyShield) ||
                buffProto.category == (int)(BuffType.Buff_Type_MgShield) ||
                buffProto.category == (int)(BuffType.Buff_Type_Taunt)
                )
        {
            ReplaceSameCategoryBuff(buffProto.category, ref buffList);
            if (buffProto.category == (int)BuffType.Buff_Type_PhyShield)
            {
                int attr = (buffProto.shieldBaseAttr == 0) ? target.strength : target.intelligence;
                phyShield = (int)(buffProto.shieldNum + buffProto.shieldRatio * attr);
            }
            else if (buffProto.category == (int)BuffType.Buff_Type_MgShield)
            {
                int attr = (buffProto.shieldBaseAttr == 0) ? target.strength : target.intelligence;
                magicShield = (int)(buffProto.shieldNum + buffProto.shieldRatio * attr);
            }
        }
        else
        {
            foreach (Buff buff in buffList)
            {
                //同名刷新，不区分来源
                if (buff.buffProto.id == buffProto.id)
                {
                    buff.Reset();
                    return;
                }
            }
        }

        buffList.Add(this);
        ModifyUnit(false, applyTime);
        SpellBuffArgs args = new SpellBuffArgs();
        args.triggerTime = applyTime;
        args.casterID = casterID;
        args.targetID = targetID;
        args.isAdd = true;
        args.buffID = buffProto.id;
        spellService.TriggerEvent(GameEventList.SpellBuff, args);
    }
    //---------------------------------------------------------------------------------------------
    void ReplaceSameCategoryBuff(int category, ref List<Buff> buffList)
    {
        foreach (Buff buff in buffList)
        {
            if (buff.buffProto.category == category && buff.isFinish == false)
            {
                buff.Finish(applyTime, BuffFinisType.Buff_Finish_Replace);
                //buff.ModifyUnit(true, applyTime);
                break;
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void Finish(float finishTime, BuffFinisType finishType = BuffFinisType.Buff_Finish_Expire)
    {
        if (isFinish == true)
            return;

        isFinish = true;
        this.finishType = finishType;
        ModifyUnit(true, finishTime);

        SpellBuffArgs args = new SpellBuffArgs();
        args.triggerTime = finishTime;
        args.casterID = casterID;
        args.targetID = targetID;
        args.isAdd = false;
        args.buffID = buffProto.id;
        spellService.TriggerEvent(GameEventList.SpellBuff, args);
    }
    //---------------------------------------------------------------------------------------------
    void ModifyUnit(bool isRemove, float curTime)
    {
        GameUnit target = spellService.GetUnit(targetID);

        //状态改变
        if (buffProto.stun > 0)
        {
            if (isRemove)
            {
                --target.stun;
                if (target.stun == 0)
                {
                    SpellBuffArgs args = new SpellBuffArgs();
                    args.triggerTime = curTime;
                    args.casterID = 0;
                    args.targetID = targetID;
                    args.isAdd = false;
                    args.buffID = buffProto.id;
                    spellService.TriggerEvent(GameEventList.SpellStun, args);
                }
            }
            else
            {
                //trigger stun event
                SpellVitalChangeArgs stunArgs = new SpellVitalChangeArgs();
                stunArgs.vitalType = (int)VitalType.Vital_Type_Stun;
                stunArgs.triggerTime = Time.time;
                stunArgs.casterID = casterID;
                stunArgs.targetID = targetID;
                stunArgs.isCritical = false;
                stunArgs.vitalChange = 0;
                stunArgs.vitalCurrent = 0;
                stunArgs.vitalMax = 0;
                spellService.TriggerEvent(GameEventList.SpellLifeChange, stunArgs);

                if (target.stun == 0)
                {
                    SpellBuffArgs args = new SpellBuffArgs();
                    args.triggerTime = curTime;
                    args.casterID = 1;
                    args.targetID = targetID;
                    args.isAdd = false;
                    args.buffID = buffProto.id;
                    spellService.TriggerEvent(GameEventList.SpellStun, args);

                    //interrupt if in dazhao
                    if (target.pbUnit.camp == UnitCamp.Enemy)
                    {
                        List<Buff> buffList = target.buffList;
                        int count = buffList.Count;
                        for (int i = 0; i < count; ++i)
                        {
                            if (buffList[i].buffProto.category == (int)(BuffType.Buff_Type_Dazhao) && buffList[i].isFinish == false)
                            {
                                buffList[i].Finish(curTime);
                                SpellVitalChangeArgs interruptArgs = new SpellVitalChangeArgs();
                                interruptArgs.vitalType = (int)VitalType.Vital_Type_Interrupt;
                                interruptArgs.triggerTime = curTime;
                                interruptArgs.casterID = 0;
                                interruptArgs.targetID = targetID;
                                spellService.TriggerEvent(GameEventList.SpellLifeChange, interruptArgs);
                                break;
                            }
                        }
                    }
                    else
                    {
                        GameEventMgr.Instance.FireEvent<int>(GameEventList.ExitDazhaoByPhyAttacked, target.pbUnit.guid);
                    }
                }
                ++target.stun;
            }
        }

        if (buffProto.invincible > 0)
        {
            if (isRemove)
                --target.invincible;
            else
                ++target.invincible;
        }

        if (buffProto.dazhao > 0)
        {
            target.dazhaoDamageCount = 0;
            if (isRemove)
            {
                target.dazhao = 0;
                target.dazhaoPrepareCount = 0;
            }
            else
            {
                target.dazhao = buffProto.dazhao;
                target.dazhaoPrepareCount = target.dazhao;
            }
        }

        //嘲讽
        if (buffProto.category == (int)BuffType.Buff_Type_Taunt)
        {
            if (isRemove)
            {
                target.tauntTargetID = BattleConst.battleSceneGuid;
            }
            else 
            {
                target.tauntTargetID = casterID;
            }
        }

        //属性改变
        if (isRemove)
        {
            target.hitRatio -= buffProto.hitRatio;
            target.criticalRatio -= buffProto.criticalRatio;
            target.spellStrengthRatio -= buffProto.strengthRatio;
            target.spellIntelligenceRatio -= buffProto.intelligenceRatio;
            target.spellSpeedRatio -= buffProto.speedRatio;
            target.spellDefenseRatio -= buffProto.defenseRatio;
            if (buffProto.healthRatio != 0.0f)
            {
                target.spellHealthRatio -= buffProto.healthRatio;
                target.OnHealthChange(curTime);
            }

            if (buffProto.category == (int)BuffType.Buff_Type_Defend)
            {
                target.spellDefenseDamageRatio = 0.0f;
            }
            else if (buffProto.category == (int)BuffType.Buff_Type_PhyJanshang)
            {
                target.spellphyReduceInjury = 0.0f;
            }
            else if (buffProto.category == (int)BuffType.Buff_Type_MgJanshang)
            {
                target.spellmgReduceInjury = 0.0f;
            }
            else if (buffProto.category == (int)BuffType.Buff_Type_PhyShield)
            {
                target.spellPhyShield = 0;
                if (finishType != BuffFinisType.Buff_Finish_Replace)
                {
                    //trigger shield ui event
                    SpellVitalChangeArgs args = new SpellVitalChangeArgs();
                    args.vitalType = (int)VitalType.Vital_Type_Shield;
                    args.triggerTime = curTime;
                    args.vitalCurrent = 0;
                    args.vitalMax = (int)BuffType.Buff_Type_PhyShield;
                    spellService.TriggerEvent(GameEventList.SpellLifeChange, args);
                }
            }
            else if (buffProto.category == (int)BuffType.Buff_Type_MgShield)
            {
                target.spellMagicShield = 0;
                if (finishType != BuffFinisType.Buff_Finish_Replace)
                {
                    //trigger shield ui event
                    SpellVitalChangeArgs args = new SpellVitalChangeArgs();
                    args.vitalType = (int)VitalType.Vital_Type_Shield;
                    args.triggerTime = curTime;
                    args.vitalCurrent = 0;
                    args.vitalMax = (int)BuffType.Buff_Type_MgShield;
                    spellService.TriggerEvent(GameEventList.SpellLifeChange, args);
                }
            }
        }
        else
        {
            target.hitRatio += buffProto.hitRatio;
            target.criticalRatio += buffProto.criticalRatio;
            target.spellStrengthRatio += buffProto.strengthRatio;
            target.spellIntelligenceRatio += buffProto.intelligenceRatio;
            target.spellSpeedRatio += buffProto.speedRatio;
            target.spellDefenseRatio += buffProto.defenseRatio;
            if (buffProto.healthRatio != 0.0f)
            {
                target.spellHealthRatio += buffProto.healthRatio;
                target.OnHealthChange(curTime);
            }

            if (buffProto.category == (int)BuffType.Buff_Type_Defend)
            {
                target.spellDefenseDamageRatio = buffProto.defenseDamageRatio;
            }
            else if (buffProto.category == (int)BuffType.Buff_Type_PhyJanshang)
            {
                target.spellphyReduceInjury = buffProto.phyReduceInjury;
            }
            else if (buffProto.category == (int)BuffType.Buff_Type_MgJanshang)
            {
                target.spellmgReduceInjury = buffProto.mgReduceInjury;
            }
            else if (buffProto.category == (int)BuffType.Buff_Type_PhyShield)
            {
                target.spellPhyShield = phyShield;
                //trigger shield ui event
                SpellVitalChangeArgs args = new SpellVitalChangeArgs();
                args.vitalType = (int)VitalType.Vital_Type_Shield;
                args.triggerTime = curTime;
                args.vitalCurrent = phyShield;
                args.vitalMax = (int)BuffType.Buff_Type_PhyShield;
                spellService.TriggerEvent(GameEventList.SpellLifeChange, args);
            }
            else if (buffProto.category == (int)BuffType.Buff_Type_MgShield)
            {
                target.spellMagicShield = magicShield;
                //trigger shield ui event
                SpellVitalChangeArgs args = new SpellVitalChangeArgs();
                args.vitalType = (int)VitalType.Vital_Type_Shield;
                args.triggerTime = curTime;
                args.vitalCurrent = magicShield;
                args.vitalMax = (int)BuffType.Buff_Type_MgShield;
                spellService.TriggerEvent(GameEventList.SpellLifeChange, args);
            }
        }

        if (buffProto.speedRatio != 0.0f)
        {
            target.RecalcCurActionOrder();
        }
    }
    //---------------------------------------------------------------------------------------------
    //now only dazhao buff need response
    public void CheckDazhaoInterrupt(float curTime)
    {
        GameUnit target = spellService.GetUnit(targetID);
        if (target != null)
        {
            if (target.dazhao > 0)
            {
                ++target.dazhaoDamageCount;
                if (DazhaoExitCheck.IsExitByPhyAttacked(target.dazhaoDamageCount))
                {
                    Finish(curTime);
                    SpellVitalChangeArgs args = new SpellVitalChangeArgs();
                    args.vitalType = (int)VitalType.Vital_Type_Interrupt;
                    args.triggerTime = curTime;
                    args.casterID = 0;
                    args.targetID = targetID;
                    spellService.TriggerEvent(GameEventList.SpellLifeChange, args);
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void DamageResponse(float curTime, Effect triggerEffect)
    {
        if (damageResponse.Equals(default(KeyValuePair<string, string>)) == false)
        {
            RunBuffResponseInternal(ref damageResponse, curTime, triggerEffect);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void DeadResponse(float curTime, Effect triggerEffect)
    {
        if (deadResponse.Equals(default(KeyValuePair<string, string>)) == false)
        {
            RunBuffResponseInternal(ref deadResponse, curTime, triggerEffect);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void OnShield(float curTime, EffectDamage triggerEffect, ref int damageAmount)
    {
        if (isFinish == true)
            return;

        bool shield = false;
        EffectDamageProtoType damageProto = triggerEffect.protoEffect as EffectDamageProtoType;
        //check shield
        if (damageProto.isHeal == false)
        {
            GameUnit targetUnit = spellService.GetUnit(targetID);
            if (damageProto.damageType == SpellConst.damagePhy)
            {
                if (phyShield > 0)
                {
                    if (phyShield + damageAmount >= 0)
                    {
                        phyShield += damageAmount;
                        damageAmount = 0;
                        //trigger shield ui event
                        SpellVitalChangeArgs args = new SpellVitalChangeArgs();
                        args.vitalType = (int)VitalType.Vital_Type_Absorbed;
                        args.targetID = targetID;
                        args.triggerTime = curTime;
                        spellService.TriggerEvent(GameEventList.SpellLifeChange, args);

                        SpellEffectArgs effectArgs = new SpellEffectArgs();
                        effectArgs.targetID = targetID;
                        effectArgs.triggerTime = curTime;
                        spellService.TriggerEvent(GameEventList.SpellAbsrobed, effectArgs);
                    }
                    else
                    {
                        damageAmount = damageAmount + phyShield;
                        phyShield = 0;
                    }

                    targetUnit.spellPhyShield = phyShield;
                    SpellVitalChangeArgs phyShieldArgs = new SpellVitalChangeArgs();
                    phyShieldArgs.vitalType = (int)VitalType.Vital_Type_Shield;
                    phyShieldArgs.triggerTime = curTime;
                    phyShieldArgs.targetID = targetID;
                    phyShieldArgs.vitalCurrent = phyShield;
                    phyShieldArgs.vitalMax = (int)BuffType.Buff_Type_PhyShield;
                    spellService.TriggerEvent(GameEventList.SpellLifeChange, phyShieldArgs);
                }
            }
            else
            {
                if (magicShield > 0)
                {
                    if (magicShield + damageAmount >= 0)
                    {
                        magicShield += damageAmount;
                        damageAmount = 0;
                        //trigger shield ui event
                        SpellVitalChangeArgs args = new SpellVitalChangeArgs();
                        args.vitalType = (int)VitalType.Vital_Type_Absorbed;
                        args.targetID = targetID;
                        args.triggerTime = curTime;
                        spellService.TriggerEvent(GameEventList.SpellLifeChange, args);

                        SpellEffectArgs effectArgs = new SpellEffectArgs();
                        effectArgs.targetID = targetID;
                        effectArgs.triggerTime = curTime;
                        spellService.TriggerEvent(GameEventList.SpellAbsrobed, effectArgs);
                    }
                    else
                    {
                        damageAmount = damageAmount + magicShield;
                        magicShield = 0;
                    }

                    targetUnit.spellMagicShield = magicShield;
                    SpellVitalChangeArgs magicShieldArgs = new SpellVitalChangeArgs();
                    magicShieldArgs.vitalType = (int)VitalType.Vital_Type_Shield;
                    magicShieldArgs.triggerTime = curTime;
                    magicShieldArgs.vitalCurrent = magicShield;
                    magicShieldArgs.vitalMax = (int)BuffType.Buff_Type_PhyShield;
                    spellService.TriggerEvent(GameEventList.SpellLifeChange, magicShieldArgs);
                }
            }

            if (phyShield == 0 && magicShield == 0 && (buffProto.category == (int)BuffType.Buff_Type_MgShield || buffProto.category == (int)BuffType.Buff_Type_PhyShield))
            {
                Finish(curTime);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    void RunBuffResponseInternal(ref KeyValuePair<string, string> response, float curTime, Effect triggerEffect)
    {
        if (string.IsNullOrEmpty(response.Value)|| isFinish == true || responseCount <= 0)
            return;

        int result = 1;
        if (string.IsNullOrEmpty(response.Key) == false)
        {
            var cls = typeof(SpellFunctions);
            MethodInfo validator = cls.GetMethod(response.Key);
            if (validator != null)
            {
                result = (int)validator.Invoke(null, new object[] { this, triggerEffect, spellService });
            }
        }

        if (result == 1)
        {
            if (result == 1)
            {
                --responseCount;
                Effect curEffect = spellService.GetEffect(response.Value);
                if (curEffect != null)
                {
                    curEffect.SetOwnedBuff(this);
                    curEffect.SetOwnedSpell(ownedSpell);
                    curEffect.casterID = targetID;
                    curEffect.targetID = triggerEffect.casterID;
                    if (curEffect.targetID == curEffect.casterID)
                    {
                        curEffect.targetID = triggerEffect.targetID;
                    }
                    curEffect.noDamageResponse = true;
                    curEffect.Apply(curTime, null);
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
}
