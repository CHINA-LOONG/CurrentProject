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
    public override bool Apply(float applyTime, string wpName)
    {
        if (base.Apply(applyTime, wpName) == false)
            return false;

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
            camp = (camp == (int)UnitCamp.Player) ? (int)(UnitCamp.Enemy) : (int)(UnitCamp.Player);
        }

        List<BattleObject> boList = spellService.GetUnitList(camp, casterID == BattleConst.battleSceneGuid);
        foreach (BattleObject bo in boList)
        {
            if (bo == null || bo.unit.isVisible == false || bo.unit.State == UnitState.Dead)
                continue;

			List<string> wpList = null;
			if(bo.wpGroup != null)
			{
				wpList = bo.wpGroup.GetAiCanAttackList();
			}

            if (null == wpList || wpList.Count == 0)
            {
                Effect curEffect = spellService.GetEffect(searchProt.effectID);
                if (curEffect != null)
                {
                    curEffect.SetOwnedBuff(ownedBuff);
                    curEffect.SetOwnedSpell(ownedSpell);
                    curEffect.targetID = bo.guid;
                    //NOTE: if put this to config file, remove this
                    curEffect.noDamageResponse = noDamageResponse;
                    curEffect.Apply(applyTime, string.Empty);
                }
            }
            else
            {
                for (int i = 0; i < wpList.Count; ++i)
                {
                    Effect curEffect = spellService.GetEffect(searchProt.effectID);
                    if (curEffect != null)
                    {
                        curEffect.SetOwnedBuff(ownedBuff);
                        curEffect.SetOwnedSpell(ownedSpell);
                        curEffect.targetID = bo.guid;
                        //NOTE: if put this to config file, remove this
                        curEffect.noDamageResponse = noDamageResponse;
                        curEffect.Apply(applyTime, wpList[i]);
                    }
                }
            }
        }

        return true;
    }
    //---------------------------------------------------------------------------------------------
}
