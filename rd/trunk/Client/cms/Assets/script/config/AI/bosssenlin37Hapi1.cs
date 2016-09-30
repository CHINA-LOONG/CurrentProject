

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosssenlin37Hapi1 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Hapi1Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Hapi1SpellDic = GetUnitSpellList (Hapi1Unit);

		Spell useSpell = null;
		Hapi1SpellDic.TryGetValue ("bosssenlin37Hapi12", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Hapi1Unit);
		if (NormalScript.GetWpLifeLeft(Hapi1Unit.battleUnit, "bosssenlin37Hapi1wp02") != 0 && NormalScript.GetWpLifeLeft(Hapi1Unit.battleUnit, "bosssenlin37Hapi1wp03") != 0)
		{
			if (GetAttackCount(Hapi1Unit) % 7 == 0 && GetAttackCount(Hapi1Unit) != 0) 
			{
				Hapi1SpellDic.TryGetValue ("bosssenlin37Hapi15", out useSpell);
			}
			else if (GetAttackCount(Hapi1Unit) % 3 == 0 && GetAttackCount(Hapi1Unit) != 0) 
			{
				Hapi1SpellDic.TryGetValue ("bosssenlin37Hapi13", out useSpell);

			}
		}
		else if	(NormalScript.GetWpLifeLeft(Hapi1Unit.battleUnit, "bosssenlin37Hapi1wp02") == 0 && NormalScript.GetWpLifeLeft(Hapi1Unit.battleUnit, "bosssenlin37Hapi1wp03") == 0)
		{
			if (GetAttackCount(Hapi1Unit) % 7 == 0 && GetAttackCount(Hapi1Unit) != 0) 
			{
				Hapi1SpellDic.TryGetValue ("bosssenlin37Hapi15", out useSpell);
			}
		}
		else
		{
			if (GetAttackCount(Hapi1Unit) % 7 == 0 && GetAttackCount(Hapi1Unit) != 0) 
			{
				Hapi1SpellDic.TryGetValue ("bosssenlin37Hapi15", out useSpell);
			}
			else if (GetAttackCount(Hapi1Unit) % 3 == 0 && GetAttackCount(Hapi1Unit) != 0) 
			{
				Hapi1SpellDic.TryGetValue ("bosssenlin37Hapi14", out useSpell);

			}
		}
		
		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	// public override void OnWpDead(WeakPointDeadArgs args)
	//{
	//	BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
	//	if (args.wpID == "bossMinghe14Hapi1wp03" && jishu==0)
	//  {
	//target.TriggerEvent("Hapi1_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//  }
	//}
	//---------------------------------------------------------------------------------------------
}
