

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosssenlin31Wendige2 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Wendige2Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Wendige2SpellDic = GetUnitSpellList (Wendige2Unit);

		Spell useSpell = null;
		

		attackResult.attackTarget = GetAttackRandomTarget(Wendige2Unit);
		
		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Wendige2Unit);

		if (NormalScript.GetWpLifeLeft(Wendige2Unit.battleUnit, "bosssenlin31Wendige2wp02") == 0 && NormalScript.GetWpLifeLeft(Wendige2Unit.battleUnit, "bosssenlin31Wendige2wp03") == 0)
		{
			Wendige2SpellDic.TryGetValue ("bosssenlin31Wendige22", out useSpell);

			if (GetAttackCount(Wendige2Unit) % 7 == 0 && GetAttackCount(Wendige2Unit) != 0) 
			{
				Wendige2SpellDic.TryGetValue ("bosssenlin31Wendige23", out useSpell);
			}
		}
		else 
		{
			Wendige2SpellDic.TryGetValue ("bosssenlin31Wendige21", out useSpell);

			if (GetAttackCount(Wendige2Unit) % 7 == 0 && GetAttackCount(Wendige2Unit) != 0) 
			{
				Wendige2SpellDic.TryGetValue ("bosssenlin31Wendige23", out useSpell);
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
			if (args.wpID == "bosssenlin31Wendige2wp02")
				{
					target.TriggerEvent("wendige2_wp02dead", Time.time, null);
			  	}
			if (args.wpID == "bosssenlin31Wendige2wp03")
				{
					target.TriggerEvent("wendige2_wp03dead", Time.time, null);
				}
		}
	//---------------------------------------------------------------------------------------------
}
