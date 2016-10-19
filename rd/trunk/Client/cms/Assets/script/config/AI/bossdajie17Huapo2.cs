

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossdajie17Huapo2 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Huapo2Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Huapo2SpellDic = GetUnitSpellList (Huapo2Unit);

		Spell useSpell = null;
		Huapo2SpellDic.TryGetValue ("bossdajie17Huapo21", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Huapo2Unit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Huapo2Unit);

		if (NormalScript.GetWpLifeLeft(Huapo2Unit.battleUnit, "bossdajie17Huapo2wp02")==0)
		{
			
			if (GetAttackCount(Huapo2Unit) % 6 == 0 && GetAttackCount(Huapo2Unit) != 0) 
			{
				Huapo2SpellDic.TryGetValue ("bossdajie17Huapo24", out useSpell);
			}
			else if (GetAttackCount(Huapo2Unit) % 3 == 0 && GetAttackCount(Huapo2Unit) != 0) 
			{
				Huapo2SpellDic.TryGetValue ("bossdajie17Huapo22", out useSpell);
			}

		} 

		else 
		{
			if (GetAttackCount(Huapo2Unit) % 6 == 0 && GetAttackCount(Huapo2Unit) != 0) 
			{
				Huapo2SpellDic.TryGetValue ("bossdajie17Huapo23", out useSpell);
			}
			else if (GetAttackCount(Huapo2Unit) % 3 == 0 && GetAttackCount(Huapo2Unit) != 0) 
			{
				Huapo2SpellDic.TryGetValue ("bossdajie17Huapo22", out useSpell);
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
		if (args.wpID == "bossdajie17Huapo2wp02")
		{
			target.TriggerEvent("huapo2_wp02dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
