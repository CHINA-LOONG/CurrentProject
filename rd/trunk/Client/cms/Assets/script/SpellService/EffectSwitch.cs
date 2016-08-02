using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class EffectSwitchPrototype : EffectPrototype
{
    public EffectSwitchPrototype()
    {
        effectList = new List<KeyValuePair<string, string>>();
    }
    public List<KeyValuePair<string, string>> effectList;
}

public class EffectSwitch : Effect
{
    //---------------------------------------------------------------------------------------------
    public override void Init(EffectPrototype pt, SpellService owner)
    {
        EffectSwitchPrototype setPtOut = pt as EffectSwitchPrototype;
        protoEffect = new EffectSwitchPrototype();
        EffectSwitchPrototype setPt = protoEffect as EffectSwitchPrototype;
        setPt.effectList = setPtOut.effectList;
        base.Init(pt, owner);
    }
    //---------------------------------------------------------------------------------------------
    public override bool Apply(float applyTime, string wpID)
    {
        if (base.Apply(applyTime, wpID) == false)
            return false;

        EffectSwitchPrototype setProto = protoEffect as EffectSwitchPrototype;
        if (setProto == null)
            return false;

        GameUnit caster = spellService.GetUnit(casterID);
        GameUnit target = spellService.GetUnit(targetID);
        var cls = typeof(SpellFunctions);

        foreach (KeyValuePair<string, string> effectkv in setProto.effectList)
        {
            string key = effectkv.Key;
            MethodInfo validator = cls.GetMethod(key);
            int result = (int)validator.Invoke(null, new object[]{(GameUnit)(caster), (GameUnit)(target)});
            if (result == 1)
            {
                Effect curEffect = spellService.GetEffect(effectkv.Value);
                if (curEffect != null)
                {
                    curEffect.SetOwnedBuff(ownedBuff);
                    curEffect.SetOwnedSpell(ownedSpell);
                    curEffect.targetID = targetID;
                    curEffect.Apply(applyTime, wpID);

                    return true;
                }
            }
        }

        return true;
    }
    //---------------------------------------------------------------------------------------------
}
