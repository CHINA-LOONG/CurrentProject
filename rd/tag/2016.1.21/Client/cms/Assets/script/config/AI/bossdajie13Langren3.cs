

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossdajie13Langren3 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Langren3Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Langren3SpellDic = GetUnitSpellList (Langren3Unit);

		Spell useSpell = null;
		Langren3SpellDic.TryGetValue ("bossdajie13Langren31", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Langren3Unit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Langren3Unit);
		int count = 0;
		for(int n = wpList.Count -1 ;n > 0;n--)
		{
			if (wpList[n] == "bossdajie13Langren3wp02")
				count++;
			if (wpList[n] == "bossdajie13Langren3wp03")
				count++;
		}

		if (count == 2)
		{
			if (GetAttackCount(Langren3Unit) % 7 == 0 && GetAttackCount(Langren3Unit) != 0) 
			{
				Langren3SpellDic.TryGetValue ("bossdajie13Langren34", out useSpell);
			}
			else if (GetAttackCount(Langren3Unit) % 3 == 0 && GetAttackCount(Langren3Unit) != 0) 
			{
				Langren3SpellDic.TryGetValue ("bossdajie13Langren32", out useSpell);
			}

		} 

		else if (count == 1)
		{
			if (GetAttackCount(Langren3Unit) % 7 == 0 && GetAttackCount(Langren3Unit) != 0) 
			{
				Langren3SpellDic.TryGetValue ("bossdajie13Langren34", out useSpell);
			}
			else if (GetAttackCount(Langren3Unit) % 3 == 0 && GetAttackCount(Langren3Unit) != 0) 
			{
				Langren3SpellDic.TryGetValue ("bossdajie13Langren33", out useSpell);
			}
		}
		else if (count == 0)
		{
			if (GetAttackCount(Langren3Unit) % 7 == 0 && GetAttackCount(Langren3Unit) != 0) 
			{
				Langren3SpellDic.TryGetValue ("bossdajie13Langren34", out useSpell);
			}
		}		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	//  public override void OnWpDead(WeakPointDeadArgs args)
	//{
	//	BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
	//	if (args.wpID == "bossdajie13Langren3wp02" && jishu==0)
	//	{
	//		target.TriggerEvent("Langren3_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
	//		jishu ++;
	//	}

	//	if (args.wpID == "bossdajie13Langren3wp03" && jishu==0)
	//	{
	//		target.TriggerEvent("Langren3_state1to3", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
	//		jishu ++;
	//	}
		
	//	if (args.wpID == "bossdajie13Langren3wp03" && args.wpID == "bossdajie13Langren3wp02" && jishu==0)
	//	{
	//		target.TriggerEvent("Langren3_stateNto4", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
	//		jishu ++;
	//	}
	//}
	//---------------------------------------------------------------------------------------------
}
