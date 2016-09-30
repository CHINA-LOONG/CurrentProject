

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
		
		if (NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05") != 0)
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

		else if (NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05") != 0)
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
		else if (NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05") != 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang41", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang43", out useSpell);

			}
		}
		else if (NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05") == 0)
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
		else if (NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05") == 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang42", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang43", out useSpell);

			}
		}
		else if (NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05") != 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang41", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang45", out useSpell);

			}
		}
		else if (NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05") == 0)
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
		else if (NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05") == 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang42", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang45", out useSpell);

			}
		}
		else if (NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02") == 0 || NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05") != 0)
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
		else if (NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02") == 0 || NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05") != 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang41", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang44", out useSpell);

			}
		}
		else if (NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02") == 0 || NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04") != 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05") == 0)
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
		else if (NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp02") == 0 || NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp03") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp04") == 0 && NormalScript.GetWpLifeLeft(Tianshizhang4Unit.battleUnit, "bosshuoshan46Tianshizhang4wp05") == 0)
		{
			Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang42", out useSpell);
			if (GetAttackCount(Tianshizhang4Unit) % 3 == 0 && GetAttackCount(Tianshizhang4Unit) != 0) 
			{
				Tianshizhang4SpellDic.TryGetValue ("bosshuoshan46Tianshizhang44", out useSpell);

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
	//	if (args.wpID == "bossMinghe14Tianshizhang4wp03" && jishu==0)
	//  {
	//target.TriggerEvent("Tianshizhang4_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//  }
	//}
	//---------------------------------------------------------------------------------------------
}
