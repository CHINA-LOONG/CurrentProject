

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosshuoshan48Hehuaisituosi4 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Hehuaisituosi4Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Hehuaisituosi4SpellDic = GetUnitSpellList (Hehuaisituosi4Unit);

		Spell useSpell = null;
		Hehuaisituosi4SpellDic.TryGetValue ("bosshuoshan48Hehuaisituosi41", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Hehuaisituosi4Unit);

		if (NormalScript.GetTotalRound() <= 15 ) 
		{
		
			if (GetAttackCount(Hehuaisituosi4Unit) % 3 == 0 && GetAttackCount(Hehuaisituosi4Unit) != 0) 
			{
				Hehuaisituosi4SpellDic.TryGetValue ("bosshuoshan48Hehuaisituosi42", out useSpell);

			}
		}
		else 
		{
			if (NormalScript.GetWpLifeLeft(Hehuaisituosi4Unit.battleUnit, "bosshuoshan48Hehuaisituosi4wp02") == 0 && NormalScript.GetWpLifeLeft(Hehuaisituosi4Unit.battleUnit, "bosshuoshan48Hehuaisituosi4wp03") == 0 && NormalScript.GetWpLifeLeft(Hehuaisituosi4Unit.battleUnit, "bosshuoshan48Hehuaisituosi4wp04") == 0 && NormalScript.GetWpLifeLeft(Hehuaisituosi4Unit.battleUnit, "bosshuoshan48Hehuaisituosi4wp05") == 0)
			{		
				if (GetAttackCount(Hehuaisituosi4Unit) % 3 == 0 && GetAttackCount(Hehuaisituosi4Unit) != 0) 
				{
					Hehuaisituosi4SpellDic.TryGetValue ("bosshuoshan48Hehuaisituosi42", out useSpell);

				}
			}
			else if (NormalScript.GetWpLifeLeft(Hehuaisituosi4Unit.battleUnit, "bosshuoshan48Hehuaisituosi4wp02") == 0 && NormalScript.GetWpLifeLeft(Hehuaisituosi4Unit.battleUnit, "bosshuoshan48Hehuaisituosi4wp03") != 0 && NormalScript.GetWpLifeLeft(Hehuaisituosi4Unit.battleUnit, "bosshuoshan48Hehuaisituosi4wp04") == 0 && NormalScript.GetWpLifeLeft(Hehuaisituosi4Unit.battleUnit, "bosshuoshan48Hehuaisituosi4wp05") != 0)
			{		
				if (GetAttackCount(Hehuaisituosi4Unit) % 7 == 0 && GetAttackCount(Hehuaisituosi4Unit) != 0) 
				{
					Hehuaisituosi4SpellDic.TryGetValue ("bosshuoshan48Hehuaisituosi44", out useSpell);

				}
				else if(GetAttackCount(Hehuaisituosi4Unit) % 3 == 0 && GetAttackCount(Hehuaisituosi4Unit) != 0) 
				{
					Hehuaisituosi4SpellDic.TryGetValue ("bosshuoshan48Hehuaisituosi42", out useSpell);

				}
			}
			else if (NormalScript.GetWpLifeLeft(Hehuaisituosi4Unit.battleUnit, "bosshuoshan48Hehuaisituosi4wp02") != 0 && NormalScript.GetWpLifeLeft(Hehuaisituosi4Unit.battleUnit, "bosshuoshan48Hehuaisituosi4wp03") == 0 && NormalScript.GetWpLifeLeft(Hehuaisituosi4Unit.battleUnit, "bosshuoshan48Hehuaisituosi4wp04") != 0 && NormalScript.GetWpLifeLeft(Hehuaisituosi4Unit.battleUnit, "bosshuoshan48Hehuaisituosi4wp05") == 0)
			{		
				if (GetAttackCount(Hehuaisituosi4Unit) % 7 == 0 && GetAttackCount(Hehuaisituosi4Unit) != 0) 
				{
					Hehuaisituosi4SpellDic.TryGetValue ("bosshuoshan48Hehuaisituosi43", out useSpell);

				}
				else if(GetAttackCount(Hehuaisituosi4Unit) % 3 == 0 && GetAttackCount(Hehuaisituosi4Unit) != 0) 
				{
					Hehuaisituosi4SpellDic.TryGetValue ("bosshuoshan48Hehuaisituosi42", out useSpell);

				}
			}
			else
			{		
				if (GetAttackCount(Hehuaisituosi4Unit) % 7 == 0 && GetAttackCount(Hehuaisituosi4Unit) != 0) 
				{
					Hehuaisituosi4SpellDic.TryGetValue ("bosshuoshan48Hehuaisituosi45", out useSpell);

				}
				else if(GetAttackCount(Hehuaisituosi4Unit) % 3 == 0 && GetAttackCount(Hehuaisituosi4Unit) != 0) 
				{
					Hehuaisituosi4SpellDic.TryGetValue ("bosshuoshan48Hehuaisituosi42", out useSpell);

				}
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
		if (args.wpID == "bosshuoshan48Hehuaisituosi4wp02")
		{
			target.TriggerEvent("hehuaisituosi4_wp02dead", Time.time, null);
		}
		if (args.wpID == "bosshuoshan48Hehuaisituosi4wp03")
		{
			target.TriggerEvent("hehuaisituosi4_wp03dead", Time.time, null);
		}
		if (args.wpID == "bosshuoshan48Hehuaisituosi4wp04")
		{
			target.TriggerEvent("hehuaisituosi4_wp04dead", Time.time, null);
		}
		if (args.wpID == "bosshuoshan48Hehuaisituosi4wp05")
		{
			target.TriggerEvent("hehuaisituosi4_wp05dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
