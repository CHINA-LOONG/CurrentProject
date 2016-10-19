

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosshuoshan47Ifrit4 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Ifrit4Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Ifrit4SpellDic = GetUnitSpellList (Ifrit4Unit);

		Spell useSpell = null;

		attackResult.attackTarget = GetAttackRandomTarget(Ifrit4Unit);
		
		int a = NormalScript.GetWpLifeLeft(Ifrit4Unit.battleUnit, "bosshuoshan47Ifrit4wp02");
		int b = NormalScript.GetWpLifeLeft(Ifrit4Unit.battleUnit, "bosshuoshan47Ifrit4wp03");
		int c = NormalScript.GetWpLifeLeft(Ifrit4Unit.battleUnit, "bosshuoshan47Ifrit4wp04");
		
		
		
		if (a != 0 && b != 0 && c !=0 )
		{
			Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit41", out useSpell);
			if (GetAttackCount(Ifrit4Unit) % 5 == 0 && GetAttackCount(Ifrit4Unit) != 0) 
			{
				Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit44", out useSpell);
			}
		}
		if (a == 0 && b != 0 && c !=0 )
		{
			Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit41", out useSpell);
			if (GetAttackCount(Ifrit4Unit) % 5 == 0 && GetAttackCount(Ifrit4Unit) != 0) 
			{
				Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit45", out useSpell);
			}
		}
		if (a != 0 && b == 0 && c !=0 )
		{
			Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit42", out useSpell);
			if (GetAttackCount(Ifrit4Unit) % 5 == 0 && GetAttackCount(Ifrit4Unit) != 0) 
			{
				Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit44", out useSpell);
			}
		}
		if (a != 0 && b != 0 && c ==0 )
		{
			Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit42", out useSpell);
			if (GetAttackCount(Ifrit4Unit) % 5 == 0 && GetAttackCount(Ifrit4Unit) != 0) 
			{
				Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit44", out useSpell);
			}
		}
		if (a != 0 && b == 0 && c ==0 )
		{
			Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit43", out useSpell);
			if (GetAttackCount(Ifrit4Unit) % 5 == 0 && GetAttackCount(Ifrit4Unit) != 0) 
			{
				Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit44", out useSpell);
			}
		}
		if (a == 0 && b == 0 && c !=0 )
		{
			Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit42", out useSpell);
			if (GetAttackCount(Ifrit4Unit) % 5 == 0 && GetAttackCount(Ifrit4Unit) != 0) 
			{
				Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit45", out useSpell);
			}
		}
		if (a == 0 && b != 0 && c ==0 )
		{
			Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit42", out useSpell);
			if (GetAttackCount(Ifrit4Unit) % 5 == 0 && GetAttackCount(Ifrit4Unit) != 0) 
			{
				Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit45", out useSpell);
			}
		}
		if (a == 0 && b == 0 && c ==0 )
		{
			Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit43", out useSpell);
			if (GetAttackCount(Ifrit4Unit) % 5 == 0 && GetAttackCount(Ifrit4Unit) != 0) 
			{
				Ifrit4SpellDic.TryGetValue ("bosshuoshan47Ifrit45", out useSpell);
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
		if (args.wpID == "bosshuoshan47Ifrit4wp02")
		{
			target.TriggerEvent("ifrit4_wp02dead", Time.time, null);
		}
		if (args.wpID == "bosshuoshan47Ifrit4wp03")
		{
			target.TriggerEvent("ifrit4_wp02dead", Time.time, null);
		}
		if (args.wpID == "bosshuoshan47Ifrit4wp04")
		{
			target.TriggerEvent("ifrit4_wp02dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
