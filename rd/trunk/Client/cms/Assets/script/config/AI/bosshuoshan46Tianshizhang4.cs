

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosshuoshan46Tianshizhang4 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Tianshizhang4Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Tianshizhang4SpellDic = GetUnitSpellList (Tianshizhang4Unit);

		Spell useSpell = null;

		attackResult.attackTarget = GetAttackRandomTarget(Tianshizhang4Unit);
		
		int a = NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02");
		int b = NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03");
		int c = NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04");
		int d = NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05");
			
		if(a != 0 && b != 0 && c != 0 && d != 0)	
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang41", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 7 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang46", out useSpell);
			}
			else if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang43", out useSpell);

			}
		}

		else if (a == 0 && b != 0 && c != 0 && d != 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang41", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 7 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang46", out useSpell);
			}
			else if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang44", out useSpell);

			}
		}
		else if (a != 0 && b == 0 && c != 0 && d != 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang41", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 7 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang46", out useSpell);
			}
			else if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang44", out useSpell);

			}
		}
		else if (a == 0 && b == 0 && c != 0 && d != 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang41", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 7 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang46", out useSpell);
			}
			else if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang45", out useSpell);

			}
		}
		else if (a != 0 && b != 0 && c == 0 && d != 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang41", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang43", out useSpell);

			}
		}
		else if (a != 0 && b != 0 && c != 0 && d == 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang42", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 7 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang46", out useSpell);
			}
			else if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang43", out useSpell);

			}
		}
		else if (a == 0 && b != 0 && c == 0 && d != 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang41", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang44", out useSpell);

			}
		}
		else if (a != 0 && b == 0 && c == 0 && d != 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang41", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang44", out useSpell);

			}
		}
		else if (a == 0 && b != 0 && c != 0 && d == 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang42", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 7 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang46", out useSpell);
			}
			else if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang44", out useSpell);

			}
		}
		else if (a != 0 && b == 0 && c != 0 && d == 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang42", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 7 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang46", out useSpell);
			}
			else if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang44", out useSpell);

			}
		}
		else if (a != 0 && b != 0 && c == 0 && d == 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang42", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang43", out useSpell);

			}
		}
		else if (a == 0 && b == 0 && c == 0 && d != 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang41", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang45", out useSpell);

			}
		}
		else if (a == 0 && b == 0 && c != 0 && d == 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang42", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 7 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang46", out useSpell);
			}
			else if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang45", out useSpell);

			}
		}
		else if (a == 0 && b != 0 && c == 0 && d == 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang42", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang44", out useSpell);

			}
		}
		else if (a != 0 && b == 0 && c == 0 && d == 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang42", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang44", out useSpell);

			}
		}
		else if (a == 0 && b == 0 && c == 0 && d == 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang42", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang45", out useSpell);

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
		if (args.wpID == "bosshuoshan46Tianshizhang4wp02")
		{
			target.TriggerEvent("tianshizhang4_wp02dead", Time.time, null);
		}
		if (args.wpID == "bosshuoshan46Tianshizhang4wp03")
		{
			target.TriggerEvent("tianshizhang4_wp03dead", Time.time, null);
		}
		if (args.wpID == "bosshuoshan46Tianshizhang4wp04")
		{
			target.TriggerEvent("tianshizhang4_wp04dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
