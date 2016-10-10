

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosshuoshan41Hongniu4 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Hongniu4Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Hongniu4SpellDic = GetUnitSpellList (Hongniu4Unit);

		Spell useSpell = null;
		attackResult.attackTarget = GetAttackRandomTarget(Hongniu4Unit);
		if (NormalScript.GetWpLifeLeft(Hongniu4Unit.battleUnit, "bosshuoshan41Hongniu4wp02") != 0 && NormalScript.GetWpLifeLeft(Hongniu4Unit.battleUnit, "bosshuoshan41Hongniu4wp03") != 0)
		{
			Hongniu4SpellDic.TryGetValue ("bosshuoshan41Hongniu41", out useSpell);
			if (GetAttackCount(Hongniu4Unit) % 7 == 0 && GetAttackCount(Hongniu4Unit) != 0) 
			{
				Hongniu4SpellDic.TryGetValue ("bosshuoshan41Hongniu44", out useSpell);
			}
		}
		else if (NormalScript.GetWpLifeLeft(Hongniu4Unit.battleUnit, "bosshuoshan41Hongniu4wp02") == 0 && NormalScript.GetWpLifeLeft(Hongniu4Unit.battleUnit, "bosshuoshan41Hongniu4wp03") == 0)
		{
			Hongniu4SpellDic.TryGetValue ("bosshuoshan41Hongniu43", out useSpell);
			if (GetAttackCount(Hongniu4Unit) % 7 == 0 && GetAttackCount(Hongniu4Unit) != 0) 
			{
				Hongniu4SpellDic.TryGetValue ("bosshuoshan41Hongniu44", out useSpell);
			}
		}
		else		
		{
			Hongniu4SpellDic.TryGetValue ("bosshuoshan41Hongniu42", out useSpell);
			if (GetAttackCount(Hongniu4Unit) % 7 == 0 && GetAttackCount(Hongniu4Unit) != 0) 
			{
				Hongniu4SpellDic.TryGetValue ("bosshuoshan41Hongniu44", out useSpell);
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
		if (args.wpID == "bosshuoshan41Hongniu4wp02")
		{
			target.TriggerEvent("hongniu4_wp02dead", Time.time, null);
		}
		if (args.wpID == "bosshuoshan41Hongniu4wp03")
		{
			target.TriggerEvent("hongniu4_wp03dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
