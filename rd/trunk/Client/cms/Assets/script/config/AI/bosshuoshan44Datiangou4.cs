

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosshuoshan44Datiangou4 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Datiangou4Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Datiangou4SpellDic = GetUnitSpellList (Datiangou4Unit);

		Spell useSpell = null;

		attackResult.attackTarget = GetAttackRandomTarget(Datiangou4Unit);
		if (NormalScript.GetWpLifeLeft(Datiangou4Unit.battleUnit, "bosshuoshan44Datiangou4wp02") == 0 && NormalScript.GetWpLifeLeft(Datiangou4Unit.battleUnit, "bosshuoshan44Datiangou4wp03") == 0 && NormalScript.GetWpLifeLeft(Datiangou4Unit.battleUnit, "bosshuoshan44Datiangou4wp04") == 0)
		{
			Datiangou4SpellDic.TryGetValue ("bosshuoshan44Datiangou42", out useSpell);
			if (GetAttackCount(Datiangou4Unit) % 9 == 0 && GetAttackCount(Datiangou4Unit) != 0) 
			{
				Datiangou4SpellDic.TryGetValue ("bosshuoshan44Datiangou44", out useSpell);
			}
			else if (GetAttackCount(Datiangou4Unit) % 5 == 0 && GetAttackCount(Datiangou4Unit) != 0) 
			{
				Datiangou4SpellDic.TryGetValue ("bosshuoshan44Datiangou43", out useSpell);
	
			}
		}
		else if	(NormalScript.GetWpLifeLeft(Datiangou4Unit.battleUnit, "bosshuoshan44Datiangou4wp02") == 0 && NormalScript.GetWpLifeLeft(Datiangou4Unit.battleUnit, "bosshuoshan44Datiangou4wp03") == 0 && NormalScript.GetWpLifeLeft(Datiangou4Unit.battleUnit, "bosshuoshan44Datiangou4wp04") != 0)
		{
			Datiangou4SpellDic.TryGetValue ("bosshuoshan44Datiangou42", out useSpell);
			if (GetAttackCount(Datiangou4Unit) % 9 == 0 && GetAttackCount(Datiangou4Unit) != 0) 
			{
				Datiangou4SpellDic.TryGetValue ("bosshuoshan44Datiangou44", out useSpell);
			}
		}
		else if	(NormalScript.GetWpLifeLeft(Datiangou4Unit.battleUnit, "bosshuoshan44Datiangou4wp02") != 0 && NormalScript.GetWpLifeLeft(Datiangou4Unit.battleUnit, "bosshuoshan44Datiangou4wp03") != 0 && NormalScript.GetWpLifeLeft(Datiangou4Unit.battleUnit, "bosshuoshan44Datiangou4wp04") == 0)
		{
			Datiangou4SpellDic.TryGetValue ("bosshuoshan44Datiangou41", out useSpell);
			if (GetAttackCount(Datiangou4Unit) % 5 == 0 && GetAttackCount(Datiangou4Unit) != 0) 
			{
				Datiangou4SpellDic.TryGetValue ("bosshuoshan44Datiangou43", out useSpell);
			}
		}
		else
		{
			Datiangou4SpellDic.TryGetValue ("bosshuoshan44Datiangou41", out useSpell);
		}
		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		if (args.wpID == "bosshuoshan44Datiangou4wp02")
		{
			target.TriggerEvent("datiangou4_wp02dead", Time.time, null);
		}
		if (args.wpID == "bosshuoshan44Datiangou4wp03")
		{
			target.TriggerEvent("datiangou4_wp03dead", Time.time, null);
		}
		if (args.wpID == "bosshuoshan44Datiangou4wp04")
		{
			target.TriggerEvent("datiangou4_wp04dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
