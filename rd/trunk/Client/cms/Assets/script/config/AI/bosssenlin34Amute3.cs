

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosssenlin34Amute3 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;
	int i = 1 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Amute3Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Amute3SpellDic = GetUnitSpellList (Amute3Unit);

		Spell useSpell = null;
		Amute3SpellDic.TryGetValue ("bosssenlin34Amute31", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Amute3Unit);
		if (jishu == 0 )
		{

			if (GetAttackCount(Amute3Unit) % 6 == 0 && GetAttackCount(Amute3Unit) != 0) 
			{
				Amute3SpellDic.TryGetValue ("bosssenlin34Amute34", out useSpell);
			}
			else if (GetAttackCount(Amute3Unit) % 3 == 0 && GetAttackCount(Amute3Unit) != 0) 
			{
				Amute3SpellDic.TryGetValue ("bosssenlin34Amute33", out useSpell);

			}
		}
		else
		{
			if (i == 1) 
			{
				Amute3SpellDic.TryGetValue ("bosssenlin34Amute32", out useSpell);
				i--;
			}	
			
			else if (GetAttackCount(Amute3Unit) % 6 == 0 && GetAttackCount(Amute3Unit) != 0) 
			{
				Amute3SpellDic.TryGetValue ("bosssenlin34Amute34", out useSpell);
			}
			else if (GetAttackCount(Amute3Unit) % 3 == 0 && GetAttackCount(Amute3Unit) != 0) 
			{
				Amute3SpellDic.TryGetValue ("bosssenlin34Amute33", out useSpell);

			}
		}
		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		if (args.wpID == "bosssenlin34Amute3wp03" && jishu==0)
		{
			target.TriggerEvent("Amute_state1to2", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(1.5f);
			jishu ++;
		}
	}
	//---------------------------------------------------------------------------------------------
}
