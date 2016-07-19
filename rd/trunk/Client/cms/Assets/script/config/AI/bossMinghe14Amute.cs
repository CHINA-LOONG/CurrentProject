using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossMinghe14Amute : BossAi
{

	// Use this for initialization
	void Start () 
	{
	
	}

    int yazhi_count = 0 ;
    int count = 0; //state chage count
    int attck_count = 0;

    public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit amuteUnit, BattleUnitAi.AiAttackResult xgAiResult)
	{

		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();
        		
        List<string> wpList = null;
        wpList = GetAliveWeakPointList (amuteUnit);
        int wp_count = wpList.Count -1;

        if (NormalScript.GetWpLifeLeft(amuteUnit.battleUnit, "bossMinghe14Amutewp03") <= 0)
        {
            amuteUnit.battleUnit.TriggerEvent("amute_stage1to2", Time.time, null);
        }
       
        attackResult = xgAiResult;
		return attackResult;
	}

}
