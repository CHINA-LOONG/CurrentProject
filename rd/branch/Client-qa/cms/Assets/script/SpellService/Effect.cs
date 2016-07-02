using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//for csv loader
public class EffectWholeData
{
    //TODO:Remove(moved to spell)
    public int energy;

    public int effectType;
    public string id;
    public int targetType;
    public int casterType;
    //apply buff
    public string buffID;
    //damamge effect
    public int damageType;
    public float attackFactor;
    public int isHeal;
    public int damageProperty;//五行伤害
    //persistent
    public string effectStartID;
    public float startDelayTime;
    public string periodEffectList;
    //search
    public int count;
    public int camp;
    public string searchEffect;
    //set
    public string effectList;
}

[Serializable]
public class EffectPrototype
{
    public string id;
    public int targetType;
    public int casterType;
    //TODO:Remove(moved to spell)
    public int energy;

    public EffectType effectType;
}

public class EffectPrototypes : ScriptableObject
{
    public List<EffectPrototype> data = new List<EffectPrototype>();
}

public class Effect
{
    //---------------------------------------------------------------------------------------------
    //public params
    //---------------------------------------------------------------------------------------------
    public EffectPrototype protoEffect;
    public SpellService spellService;
    public int casterID;
    public int targetID;
    public float applyTime;
    public float aniDelayTime;
    public Buff ownedBuff;
    public Spell ownedSpell;
    
    //---------------------------------------------------------------------------------------------
    //methods
    //---------------------------------------------------------------------------------------------
    public virtual void Init(EffectPrototype pt, SpellService owner)
    {
        protoEffect.id = pt.id;
        protoEffect.casterType = pt.casterType;
        protoEffect.targetType = pt.targetType;
        protoEffect.energy = pt.energy;
        protoEffect.effectType = pt.effectType;

        spellService = owner;
    }
    //---------------------------------------------------------------------------------------------
    public virtual void Apply(float applyTime, float aniDelayTime = 0.0f)
    {
        GenerateTarget(casterID, targetID);
        this.applyTime = applyTime;
        this.aniDelayTime = aniDelayTime;

        SpellEffectArgs args = new SpellEffectArgs();
        args.triggerTime = applyTime;
        args.casterID = casterID;
        args.targetID = targetID;
        args.effectID = protoEffect.id;
        spellService.TriggerEvent(GameEventList.SpellEffect, args);

        if (protoEffect.energy != 0)
        {
            SpellVitalChangeArgs energyArgs = new SpellVitalChangeArgs();
            energyArgs.triggerTime = applyTime;
            energyArgs.casterID = casterID;
            energyArgs.vitalChange = protoEffect.energy;
            energyArgs.vitalCurrent = 0;
            spellService.TriggerEvent(GameEventList.SpellEnergyChange, energyArgs);

        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetOwnedBuff(Buff buffOwner)
    {
        ownedBuff = buffOwner;
        if (buffOwner != null)
        {
            casterID = buffOwner.casterID;
            targetID = buffOwner.targetID;
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetOwnedSpell(Spell spellOwner)
    {
        ownedSpell = spellOwner;
        if (spellOwner != null)
        {
            casterID = spellOwner.casterID;
            targetID = spellOwner.targetID;
        }
    }
    //---------------------------------------------------------------------------------------------
    public void GenerateTarget(int casterID, int targetID)
    {
        if (protoEffect.targetType == SpellConst.targetTypeCaster)
        {
            this.targetID = casterID;
        }
        else 
        {
            this.targetID = targetID;
        }

        if (protoEffect.casterType == SpellConst.targetTypeCaster)
        {
            this.casterID = casterID;
        }
        else 
        {
            this.casterID = targetID;
        }
    }
    //---------------------------------------------------------------------------------------------
    public virtual int CalculateHit()
    {
        GameUnit caster = spellService.GetUnit(casterID);
        GameUnit target = spellService.GetUnit(targetID);
        float hitRatio = SpellConst.hitRatio + caster.hitRatio + SpellFunctions.GetHitRatio(caster.pbUnit.level, target.pbUnit.level);
        hitRatio = hitRatio < SpellConst.minHitRatio ? SpellConst.minHitRatio : hitRatio;
        System.Random ran = new System.Random();
        int randKey = ran.Next(0, 1);
        Logger.LogFormat("总命中率：{0}   randkey：{1}    总附加命中率：{2}  攻击方等级：{3}   防御方等级：{4}   攻防命中：{5}", hitRatio, randKey, caster.hitRatio, caster.pbUnit.level, target.pbUnit.level, SpellFunctions.GetHitRatio(caster.pbUnit.level, target.pbUnit.level));
        if (randKey < hitRatio)
        {
            return SpellConst.hitSuccess;
        }
        else 
        {
            //miss event
            SpellEffectArgs args = new SpellEffectArgs();
            args.triggerTime = applyTime;
            args.casterID = casterID;
            args.targetID = targetID;
            spellService.TriggerEvent(GameEventList.SpellMiss, args);
            return SpellConst.hitMiss;
        }
    }
    //---------------------------------------------------------------------------------------------
}
