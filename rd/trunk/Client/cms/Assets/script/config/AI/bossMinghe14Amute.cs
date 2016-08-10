

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossMinghe14Amute : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit AmuteUnit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> AmuteSpellDic = GetUnitSpellList (AmuteUnit);

		Spell useSpell = null;
		//jiuWeihuSpellDic.TryGetValue ("bossAmute_anyingzhua", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(AmuteUnit);
        if (attackResult.attackTarget == null)
            return null;

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (AmuteUnit);

		if (NormalScript.GetWpLifeLeftRatio(AmuteUnit.battleUnit, "bossMinghe14Amutewp03")==0)
		{
			
			AmuteSpellDic.TryGetValue ("bossAmute1", out useSpell);

			int i = 1;
			if (GetAttackCount(AmuteUnit) % 4 == 0) 
			{
				AmuteSpellDic.TryGetValue ("bossAmute2", out useSpell);
			}

		} 

		else 
		{
			AmuteSpellDic.TryGetValue ("bossAmute3", out useSpell);

		}			
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		if (args.wpID == "bossMinghe14Amutewp03" && jishu==0)
        {
			target.TriggerEvent("Amute_state1to2", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
			jishu ++;
        }
	}
	//---------------------------------------------------------------------------------------------
}
