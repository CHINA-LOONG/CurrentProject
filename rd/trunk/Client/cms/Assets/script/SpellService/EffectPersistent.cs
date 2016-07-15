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
    public override bool Apply(float applyTime, string wpID)
    {
        if (base.Apply(applyTime, wpID) == false)
            return false;

        EffectPersistentProtoType persistProto = protoEffect as EffectPersistentProtoType;
        if (persistProto == null)
            return false;

        Logger.Log("apply EffectPersistent");
        float delayTime = persistProto.startDelayTime;
        Effect startEffect = spellService.GetEffect(persistProto.effectStartID);
        if (startEffect != null)
        {
            startEffect.SetOwnedBuff(ownedBuff);
            startEffect.SetOwnedSpell(ownedSpell);
            startEffect.targetID = targetID;
            //NOTE: if put this to config file, remove this
            startEffect.noDamageResponse = noDamageResponse;
            startEffect.Apply(applyTime + persistProto.startDelayTime, wpID);
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
                //NOTE: if put this to config file, remove this
                curEffect.noDamageResponse = noDamageResponse;
                curEffect.Apply(applyTime + delayTime, wpID);
            }
        }

        //cache spell length
        spellService.SetSpellEndTime(applyTime + delayTime);
        return true;
    }
    //---------------------------------------------------------------------------------------------
}
