using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class EffectDispelProtoType : EffectPrototype
{
    public int dispelCategory;
}
//---------------------------------------------------------------------------------------------
public class EffectDispel : Effect
{
    //---------------------------------------------------------------------------------------------
    public override void Init(EffectPrototype pt, SpellService owner)
    {
        EffectDispelProtoType dispelPtOut = pt as EffectDispelProtoType;
        protoEffect = new EffectDispelProtoType();
        EffectDispelProtoType dispelPt = protoEffect as EffectDispelProtoType;
        dispelPt.dispelCategory = dispelPtOut.dispelCategory;
        base.Init(pt, owner);
    }
    //---------------------------------------------------------------------------------------------
    public override void Apply(float applyTime, string wpID)
    {
        base.Apply(applyTime, wpID);

        EffectDispelProtoType dispelPt = protoEffect as EffectDispelProtoType;
        if (dispelPt == null)
            return;

        GameUnit target = spellService.GetUnit(targetID);
        if (target == null)
            return;

        List<Buff> targetBuffList = target.buffList;
        int buffCount = targetBuffList.Count;
        Buff curBuff = null;
        for (int i = 0; i < buffCount; ++i)
        {
            curBuff = targetBuffList[i];
            if (curBuff != null && curBuff.buffProto.category == dispelPt.dispelCategory)
                curBuff.Finish(applyTime);
        }
    }
    //---------------------------------------------------------------------------------------------
}
