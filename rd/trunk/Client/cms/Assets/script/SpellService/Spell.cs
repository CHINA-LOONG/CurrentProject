using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class SpellProtoType
{
    public string id;
    public string rootEffectID;
    public int energyCost;
    public int category;
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
    }

    public void Init(SpellService owner)
    {
        spellService = owner;
    }

    public void Apply(int triggerTime)
    {
        //generate spell event
        SpellFireArgs args = new SpellFireArgs();
        args.triggerTime = triggerTime;
        args.spellID = spellData.id;
        args.casterID = casterID;
        args.targetID = targetID;
        //args.castResult = SpellConst.spellCastOK;
        spellService.TriggerEvent(GameEventList.SpellFire, args);

        Effect rootEffect = spellService.GetEffect(spellData.rootEffectID);
        if (rootEffect != null)
        {
            rootEffect.SetOwnedSpell(this);
            rootEffect.Apply(triggerTime);
        }

        //take energy if needed
        if (spellData.energyCost > 0)
        {
            SpellVitalChangeArgs energyArgs = new SpellVitalChangeArgs();
            energyArgs.triggerTime = triggerTime;
            energyArgs.casterID = casterID;
            energyArgs.vitalChange = spellData.energyCost;
            energyArgs.vitalCurrent = 0;
            spellService.TriggerEvent(GameEventList.SpellEnergyChange, energyArgs);
        }
    }

    public void AddSpellLength(float delayTime)
    {
        spellLength += delayTime;
        Logger.Log("spell length added");
    }
}
