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
    public override void Apply(int applyTime, float aniDelayTime)
    {
        base.Apply(applyTime);

        EffectSetPrototype setProto = protoEffect as EffectSetPrototype;
        if (setProto == null)
            return;

        Logger.Log("apply EffectSet");
        foreach (string id in setProto.effectList)
        {
            Effect curEffect = spellService.GetEffect(id);
            if (curEffect != null)
            {
                curEffect.SetOwnedBuff(ownedBuff);
                curEffect.SetOwnedSpell(ownedSpell);
                curEffect.targetID = targetID;
                curEffect.Apply(applyTime);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
}
