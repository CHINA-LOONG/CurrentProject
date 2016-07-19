using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossMinghe14Amute : BossAi
{

	// Use this for initialization
	void Start () 
	{
	
	}

    int jishu = 0 ;

    public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit amuteUnit, BattleUnitAi.AiAttackResult xgAiResult)
	{

		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();
        		
        List<string> wpList = null;
        wpList = GetAliveWeakPointList (amuteUnit);
        int wp_count = wpList.Count -1;

        attackResult = xgAiResult;
		return attackResult;
	}
    //---------------------------------------------------------------------------------------------
    public override void OnVitalChange(SpellVitalChangeArgs args)
    {

    }
    //---------------------------------------------------------------------------------------------
    public override void OnWpDead(WeakPointDeadArgs args)
    {
        if (args.wpID == "bossMinghe14Amutewp03" &&jishu==0)
        {
            BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
            target.TriggerEvent("amute_stage1to2", Time.time, null);
            jishu++;
        }
    }

}
