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
    //public int cdTime;
}

public class Spell
{
    public int casterID;
    public int targetID;
    public SpellProtoType spellData;
    public SpellService spellService;
    public int level;

    private float spellLength;

    public Spell(SpellProtoType spellPt, int level)
    {
        spellData = spellPt;
        this.level = level;
    }

    public void Init(SpellService owner)
    {
        spellLength = 0.0f;
        spellService = owner;
    }

    public void Apply(float triggerTime, string wpID)
    {
        //generate spell event
        SpellFireArgs args = new SpellFireArgs();
        args.triggerTime = triggerTime;
        args.spellID = spellData.id;
        args.casterID = casterID;
        args.targetID = targetID;
        GameUnit caster = spellService.GetUnit(casterID);
        //args.castResult = SpellConst.spellCastOK;

        //generate energy
        if (spellData.energyGenerate > 0)
        {
            caster.energy += spellData.energyGenerate;
            if (caster.energy > BattleConst.enegyMax)
				caster.energy = BattleConst.enegyMax;

            SpellVitalChangeArgs energyArgs = new SpellVitalChangeArgs();
            energyArgs.vitalType = (int)VitalType.Vital_Type_Default;
            energyArgs.triggerTime = triggerTime;
            energyArgs.casterID = casterID;
            energyArgs.vitalChange = spellData.energyGenerate;
            energyArgs.vitalCurrent = caster.energy;
            energyArgs.vitalMax = 0;
            spellService.TriggerEvent(GameEventList.SpellEnergyChange, energyArgs);
        }

        //take energy if needed
        //if (caster.energy < spellData.energyCost)
        //{
        //    Logger.LogWarning("Energy not enough!");
        //    return;
        //}
        //if (spellData.energyCost > 0)
        //{
        //    SpellVitalChangeArgs energyArgs = new SpellVitalChangeArgs();
        //    energyArgs.triggerTime = triggerTime;
        //    energyArgs.casterID = casterID;
        //    energyArgs.vitalChange = spellData.energyCost * -1;//minus
        //    caster.energy = caster.energy - spellData.energyCost;
        //    if (caster.energy <= 0)
        //    {
        //        caster.energy = 0;
        //    }
        //    energyArgs.vitalCurrent = caster.energy;
        //    energyArgs.vitalMax = 0;
        //    spellService.TriggerEvent(GameEventList.SpellEnergyChange, energyArgs);
        //}


        Effect rootEffect = spellService.GetEffect(spellData.rootEffectID);
        if (rootEffect != null)
        {
            rootEffect.SetOwnedSpell(this);
            rootEffect.Apply(triggerTime, wpID);
        }

        args.aniTime = spellLength;
        spellService.TriggerEvent(GameEventList.SpellFire, args);
    }

    public void AddSpellLength(float delayTime)
    {
        spellLength += delayTime;
    }
}
