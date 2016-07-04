using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class SpellProtoType
{
    public string id;
    public string rootEffectID;
    public int actionCount;
    public int channelTime;
    public int energyCost;
    public int energyGenerate;
    public int category;
    public float levelAdjust;
    public int level;
    //public int cdTime;
}

public class Spell
{
    public int casterID;
    public int targetID;
    public SpellProtoType spellData;
    public SpellService spellService;
    
    private float spellLength;

    public Spell(SpellProtoType spellPt)
    {
        spellData = spellPt;
        //TODO: form server pb data
        spellData.level = 1;
    }

    public void Init(SpellService owner)
    {
        spellService = owner;
    }

    public void Apply(float triggerTime)
    {
        //generate spell event
        SpellFireArgs args = new SpellFireArgs();
        args.triggerTime = triggerTime;
        args.spellID = spellData.id;
        args.casterID = casterID;
        args.targetID = targetID;
        GameUnit caster = spellService.GetUnit(casterID);
        //args.castResult = SpellConst.spellCastOK;
        spellService.TriggerEvent(GameEventList.SpellFire, args);

        //generate energy
        if (spellData.energyGenerate > 0)
        {
            caster.energy += spellData.energyGenerate;
            if (caster.energy > SpellConst.maxEnergy)
                caster.energy = SpellConst.maxEnergy;

            SpellVitalChangeArgs energyArgs = new SpellVitalChangeArgs();
            energyArgs.triggerTime = triggerTime;
            energyArgs.casterID = casterID;
            energyArgs.vitalChange = spellData.energyGenerate;
            energyArgs.vitalCurrent = caster.energy;
            energyArgs.vitalMax = 0;
            spellService.TriggerEvent(GameEventList.SpellEnergyChange, energyArgs);
        }

        //take energy if needed
        if (caster.energy < spellData.energyCost)
        {
            Logger.LogWarning("Energy not enough!");
            return;
        }
        {
            SpellVitalChangeArgs energyArgs = new SpellVitalChangeArgs();
            energyArgs.triggerTime = triggerTime;
            energyArgs.casterID = casterID;
            energyArgs.vitalChange = spellData.energyCost * -1;//minus
            energyArgs.vitalCurrent = caster.energy - spellData.energyCost;
            energyArgs.vitalMax = 0;
            spellService.TriggerEvent(GameEventList.SpellEnergyChange, energyArgs);
        }


        Effect rootEffect = spellService.GetEffect(spellData.rootEffectID);
        if (rootEffect != null)
        {
            rootEffect.SetOwnedSpell(this);
            rootEffect.Apply(triggerTime);
        }
    }

    public void AddSpellLength(float delayTime)
    {
        spellLength += delayTime;
        Logger.Log("spell length added");
    }
}
