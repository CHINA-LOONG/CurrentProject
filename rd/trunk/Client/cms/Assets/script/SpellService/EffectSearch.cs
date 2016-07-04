using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


[Serializable]
public class EffectSearchPrototype : EffectPrototype
{
    public int count;
    public int camp;
    public string effectID;
}

public class EffectSearch : Effect
{
    //---------------------------------------------------------------------------------------------
    public override void Init(EffectPrototype pt, SpellService owner)
    {
        EffectSearchPrototype searchPtOut = pt as EffectSearchPrototype;
        protoEffect = new EffectSearchPrototype();
        EffectSearchPrototype searchPt = protoEffect as EffectSearchPrototype;
        searchPt.count = searchPtOut.count;
        searchPt.camp = searchPtOut.camp;
        searchPt.effectID = searchPtOut.effectID;
        base.Init(pt, owner);
    }
    //---------------------------------------------------------------------------------------------
    public override void Apply(float applyTime, float aniDelayTime)
    {
        base.Apply(applyTime);

        Logger.Log("[SpellService]trigger search effect");
        EffectSearchPrototype searchProt = protoEffect as EffectSearchPrototype;
        UnitCamp casterCamp = spellService.GetUnit(casterID).pbUnit.camp;
        int camp = searchProt.camp;
        if (casterCamp == UnitCamp.Player)
        {
            camp = (camp == (int)UnitCamp.Player) ? camp : (int)(UnitCamp.Enemy);
        }
        else
        {
            camp = (camp == (int)UnitCamp.Player) ? (int)(UnitCamp.Enemy) : camp;
        }

        List<GameUnit> unitList = spellService.GetUnitList(camp);
        foreach (GameUnit unit in unitList)
        {
            if (unit == null)
                continue;

            Effect curEffect = spellService.GetEffect(searchProt.effectID);
            if (curEffect != null)
            {
                curEffect.SetOwnedBuff(ownedBuff);
                curEffect.SetOwnedSpell(ownedSpell);
                curEffect.targetID = unit.pbUnit.guid;
                curEffect.Apply(applyTime);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
}
