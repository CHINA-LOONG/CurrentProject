using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class EffectPersistentProtoType : EffectPrototype
{
    public EffectPersistentProtoType()
    {
        effectList = new List<KeyValuePair<float, string>>();
    }

    public string effectStartID;
    public float startDelayTime;

    public List<KeyValuePair<float, string>> effectList;
}


public class EffectPersistent : Effect
{
    //---------------------------------------------------------------------------------------------
    public override void Init(EffectPrototype pt, SpellService owner)
    {
        EffectPersistentProtoType persistPtOut = pt as EffectPersistentProtoType;
        protoEffect = new EffectPersistentProtoType();
        EffectPersistentProtoType persistPt = protoEffect as EffectPersistentProtoType;
        persistPt.effectStartID = persistPtOut.effectStartID;
        persistPt.startDelayTime = persistPtOut.startDelayTime;
        persistPt.effectList = persistPtOut.effectList;
        base.Init(pt,owner);
    }
    //---------------------------------------------------------------------------------------------
    public override void Apply(float applyTime, float aniDelayTime)
    {
        base.Apply(applyTime);

        EffectPersistentProtoType persistProto = protoEffect as EffectPersistentProtoType;
        if (persistProto == null)
            return;

        Logger.Log("apply EffectPersistent");
        float delayTime = persistProto.startDelayTime;
        Effect startEffect = spellService.GetEffect(persistProto.effectStartID);
        if (startEffect != null)
        {
            startEffect.SetOwnedBuff(ownedBuff);
            startEffect.SetOwnedSpell(ownedSpell);
            startEffect.targetID = targetID;
            startEffect.Apply(applyTime + persistProto.startDelayTime, persistProto.startDelayTime);
        }

        foreach (KeyValuePair<float, string> effect in persistProto.effectList)
        {
            Effect curEffect = spellService.GetEffect(effect.Value);
            if (curEffect != null)
            {
                delayTime += effect.Key;
                curEffect.SetOwnedBuff(ownedBuff);
                curEffect.SetOwnedSpell(ownedSpell);
                curEffect.targetID = targetID;
                curEffect.Apply(applyTime + delayTime, delayTime);
            }
        }

        //cache spell length
        ownedSpell.AddSpellLength(delayTime);
    }
    //---------------------------------------------------------------------------------------------
}
