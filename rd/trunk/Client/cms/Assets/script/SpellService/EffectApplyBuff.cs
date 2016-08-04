using UnityEngine;
using System;
using System.Collections;


[Serializable]
public class EffectApplyBuffPrototype : EffectPrototype
{
    public string buffID;
    public int validatorNum;
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
        buffPt.validatorNum = buffPtOut.validatorNum;
        base.Init(pt, owner);
    }
    //---------------------------------------------------------------------------------------------
    public override bool Apply(float applyTime, string wpName)
    {
        if (base.Apply(applyTime, wpName) == false)
            return false;

        int hitResult = CalculateHit(wpName);

        Logger.Log("[SpellService]trigger apply buff effect");
        if (hitResult == SpellConst.hitSuccess)
        {
            EffectApplyBuffPrototype applyBuffProt = protoEffect as EffectApplyBuffPrototype;
            if (applyBuffProt == null)
            {
                return false;
            }
            Buff curBuff = spellService.GetBuff(applyBuffProt.buffID);
            if (curBuff != null)
            {
                GameUnit caster = spellService.GetUnit(casterID);
                int buffCount = caster.buffList.Count;
                for (int i = 0; i < buffCount; ++i)
                {
                    caster.buffList[i].DamageResponse(applyTime, this, wpName);
                }
                curBuff.casterID = casterID;
                curBuff.targetID = targetID;
                curBuff.SetOwnedSpell(ownedSpell);
                curBuff.Apply(applyTime);
            }

            //link effect
            Effect curEffect = spellService.GetEffect(protoEffect.linkEffect);
            if (curEffect != null)
            {
                curEffect.SetOwnedBuff(ownedBuff);
                curEffect.SetOwnedSpell(ownedSpell);
                curEffect.targetID = targetID;
                curEffect.absoluteHit = true;
                curEffect.Apply(applyTime, wpName);
            }
        }

        return true;
    }
    //---------------------------------------------------------------------------------------------
    public override int CalculateHit(string wpName)
    {
        GameUnit target = spellService.GetUnit(targetID);
        //check prop first
        EffectApplyBuffPrototype buffPt = protoEffect as EffectApplyBuffPrototype;
        if (buffPt != null && buffPt.validatorNum > 0)
        {
            int targetProp = 1 << (target.property - 1);
            if ((targetProp & buffPt.validatorNum) == 0)
            {
                return SpellConst.hitIgnore;
            }
        }

        if (absoluteHit == true || casterID == BattleConst.battleSceneGuid)
            return SpellConst.hitSuccess;
        //TODO:
        //1 check buff replace

        //2 check ally team
        GameUnit caster = spellService.GetUnit(casterID);
        if (caster.pbUnit.camp == target.pbUnit.camp)
        {
            return SpellConst.hitSuccess;
        }

        //check is weak point damagepoint
        WeakPointRuntimeData wpRuntime = null;
        if (string.IsNullOrEmpty(wpName) == false)
        {
            target.battleUnit.wpGroup.allWpDic.TryGetValue(wpName, out wpRuntime);
            if (wpRuntime != null && wpRuntime.staticData.isDamagePoint != 1)
            {
                EffectApplyBuffPrototype applyBuffProt = protoEffect as EffectApplyBuffPrototype;
                if (applyBuffProt != null)
                {
                    Buff curBuff = spellService.GetBuff(applyBuffProt.buffID);
                    if (curBuff != null && curBuff.buffProto.category == (int)BuffType.Buff_Type_Dot)
                    {
                        //miss event
                        SpellVitalChangeArgs args = new SpellVitalChangeArgs();
                        args.vitalType = (int)VitalType.Vital_Type_Immune;
                        args.triggerTime = applyTime;
                        args.casterID = casterID;
                        args.targetID = targetID;
                        spellService.TriggerEvent(GameEventList.SpellLifeChange, args);
                        return SpellConst.hitImmune;
                    }
                }
            }
        }

       return base.CalculateHit(wpName);
    }
    //--------------------------------------------------------------------------------------------- 
}
