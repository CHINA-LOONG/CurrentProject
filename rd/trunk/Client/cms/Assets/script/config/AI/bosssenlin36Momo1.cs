

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosssenlin36Momo1 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;
	int i = 1 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Momo1Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Momo1SpellDic = GetUnitSpellList (Momo1Unit);

		Spell useSpell = null;
		Momo1SpellDic.TryGetValue ("bosssenlin36Momo11", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Momo1Unit);
		if (NormalScript.GetWpLifeLeft(Momo1Unit.battleUnit, "bosssenlin36Momo1wp02") != 0 )
		{
			if (GetAttackCount(Momo1Unit) % 7 == 0 && GetAttackCount(Momo1Unit) != 0) 
			{
				Momo1SpellDic.TryGetValue ("bosssenlin36Momo13", out useSpell);
			}
			else if (GetAttackCount(Momo1Unit) % 3 == 0 && GetAttackCount(Momo1Unit) != 0) 
			{
				Momo1SpellDic.TryGetValue ("bosssenlin36Momo12", out useSpell);

			}
		}
		else
		{
			if (i == 1)
			{
				Momo1SpellDic.TryGetValue ("dispelPassive", out useSpell);
				i--;
			}
			else if (GetAttackCount(Momo1Unit) % 7 == 0 && GetAttackCount(Momo1Unit) != 0) 
			{
				Momo1SpellDic.TryGetValue ("bosssenlin36Momo13", out useSpell);
			}
			else if (GetAttackCount(Momo1Unit) % 3 == 0 && GetAttackCount(Momo1Unit) != 0) 
			{
				Momo1SpellDic.TryGetValue ("bosssenlin36Momo12", out useSpell);

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
	//	if (args.wpID == "bossMinghe14Momo1wp03" && jishu==0)
	//  {
	//target.TriggerEvent("Momo1_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//  }
	//}
	//---------------------------------------------------------------------------------------------
}
