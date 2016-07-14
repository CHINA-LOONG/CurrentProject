using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class EffectSetPrototype : EffectPrototype
{
    public EffectSetPrototype()
    {
        effectList = new List<string>();
    }
    public List<string> effectList;
}

public class EffectSet : Effect
{
    //---------------------------------------------------------------------------------------------
    public override void Init(EffectPrototype pt, SpellService owner)
    {
        EffectSetPrototype setPtOut = pt as EffectSetPrototype;
        protoEffect = new EffectSetPrototype();
        EffectSetPrototype setPt = protoEffect as EffectSetPrototype;
        setPt.effectList = setPtOut.effectList;
        base.Init(pt, owner);
    }
    //---------------------------------------------------------------------------------------------
    public override bool Apply(float applyTime, string wpID)
    {
        if (base.Apply(applyTime, wpID) == false)
            return false;

        EffectSetPrototype setProto = protoEffect as EffectSetPrototype;
        if (setProto == null)
            return false;

        foreach (string id in setProto.effectList)
        {
            Effect curEffect = spellService.GetEffect(id);
            if (curEffect != null)
            {
                curEffect.SetOwnedBuff(ownedBuff);
                curEffect.SetOwnedSpell(ownedSpell);
                curEffect.targetID = targetID;
                //NOTE: if put this to config file, remove this
                curEffect.noDamageResponse = noDamageResponse;
                curEffect.Apply(applyTime, wpID);
            }
        }

        return true;
    }
    //---------------------------------------------------------------------------------------------
}
