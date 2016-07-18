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
    //dispel
    public int dispelCategory;
    //
    public float fixLifeRatio;
    public string linkEffect;
    public float chance;
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
    public string linkEffect;
    public float chance;
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
    //public float aniDelayTime;
    public Buff ownedBuff;
    public Spell ownedSpell;
    public bool noDamageResponse = false;
    public bool absoluteHit;
    
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
        protoEffect.linkEffect = pt.linkEffect;
        protoEffect.chance = pt.chance;

        absoluteHit = false;
        spellService = owner;
    }
    //---------------------------------------------------------------------------------------------
    public virtual bool Apply(float applyTime, string wpID)
    {
        if (IsTriggeredSuccess() == false)
            return false;

        GenerateTarget(casterID, targetID);
        this.applyTime = applyTime;

        SpellEffectArgs args = new SpellEffectArgs();
        GameUnit target = spellService.GetUnit(targetID);
        if (target != null)
        {
            WeakPointData wp = null;
            if (target.attackWpName != null)
            {
                wp = StaticDataMgr.Instance.GetWeakPointData(target.attackWpName);
            }
            args.wpNode = wp != null ? wp.node : string.Empty;
        }
        args.triggerTime = applyTime;
        args.casterID = casterID;
        args.targetID = targetID;
        args.effectID = protoEffect.id;
        spellService.TriggerEvent(GameEventList.SpellEffect, args);

        if (protoEffect.energy != 0)
        {
            SpellVitalChangeArgs energyArgs = new SpellVitalChangeArgs();
            energyArgs.vitalType = (int)VitalType.Vital_Type_Default;
            energyArgs.triggerTime = applyTime;
            energyArgs.casterID = casterID;
            energyArgs.vitalChange = protoEffect.energy;
            energyArgs.vitalCurrent = 0;
            energyArgs.vitalMax = 0;
            spellService.TriggerEvent(GameEventList.SpellEnergyChange, energyArgs);

        }

        return true;
    }
    //---------------------------------------------------------------------------------------------
    public bool IsTriggeredSuccess()
    {
        if (protoEffect.chance < 1.0f)
        {
            return UnityEngine.Random.Range(0.0f, 1.0f) <= protoEffect.chance; 
        }

        return true;
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
        if (absoluteHit == true)
            return SpellConst.hitSuccess;

        GameUnit caster = spellService.GetUnit(casterID);
        GameUnit target = spellService.GetUnit(targetID);
        //max(N+L(lv1-lv2))+总附加命中率,60%
        float hitRatio = SpellConst.hitRatio + caster.hitRatio + SpellFunctions.GetHitRatio(caster.pbUnit.level, target.pbUnit.level);
        hitRatio = hitRatio < SpellConst.minHitRatio ? SpellConst.minHitRatio : hitRatio;
        float randKey = UnityEngine.Random.Range(0.0f, 1.0f);
        if (randKey < hitRatio)
        {
            return SpellConst.hitSuccess;
        }
        else 
        {
            //miss event
            SpellVitalChangeArgs args = new SpellVitalChangeArgs();
            args.vitalType = (int)VitalType.Vital_Type_Miss;
            args.triggerTime = applyTime;
            args.casterID = casterID;
            args.targetID = targetID;
            spellService.TriggerEvent(GameEventList.SpellLifeChange, args);
            return SpellConst.hitMiss;
        }
    }
    //---------------------------------------------------------------------------------------------
}
