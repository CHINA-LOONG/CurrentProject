using UnityEngine;
using System;
using System.Collections;


[Serializable]
public class EffectApplyBuffPrototype : EffectPrototype
{
    public string buffID;
}

public class EffectApplyBuff : Effect
{
    //---------------------------------------------------------------------------------------------
    public override void Init(EffectPrototype pt, SpellService owner)
    {
        EffectApplyBuffPrototype buffPtOut = pt as EffectApplyBuffPrototype;
        protoEffect = new EffectApplyBuffPrototype();
        EffectApplyBuffPrototype  buffPt = protoEffect as EffectApplyBuffPrototype;
        buffPt.buffID = buffPtOut.buffID;
        base.Init(pt, owner);
    }
    //---------------------------------------------------------------------------------------------
    public override void Apply(float applyTime, string wpName)
    {
        base.Apply(applyTime, wpName);
        int hitResult = CalculateHit();

        Logger.Log("[SpellService]trigger apply buff effect");
        if (hitResult == SpellConst.hitSuccess)
        {
            EffectApplyBuffPrototype applyBuffProt = protoEffect as EffectApplyBuffPrototype;
            if (applyBuffProt == null)
            {
                return;
            }
            Buff curBuff = spellService.GetBuff(applyBuffProt.buffID);
            if (curBuff != null)
            {
                GameUnit caster = spellService.GetUnit(casterID);
                int buffCount = caster.buffList.Count;
                for (int i = 0; i < buffCount; ++i)
                {
                    caster.buffList[i].DamageResponse(applyTime, this);
                }
                curBuff.casterID = casterID;
                curBuff.targetID = targetID;
                curBuff.SetOwnedSpell(ownedSpell);
                curBuff.Apply(applyTime);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public override int CalculateHit()
    {
        //TODO:
        //1 check buff replace

        //2 check ally team
        GameUnit caster = spellService.GetUnit(casterID);
        GameUnit target = spellService.GetUnit(targetID);
        if (caster.pbUnit.camp == target.pbUnit.camp)
        {
            return SpellConst.hitSuccess;
        }

       return base.CalculateHit();
    }
    //--------------------------------------------------------------------------------------------- 
}
