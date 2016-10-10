

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosssenlin35Xiaochou2 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Xiaochou2Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Xiaochou2SpellDic = GetUnitSpellList (Xiaochou2Unit);

		Spell useSpell = null;

		attackResult.attackTarget = GetAttackRandomTarget(Xiaochou2Unit);

		if (NormalScript.GetWpLifeLeft(Xiaochou2Unit.battleUnit, "bosssenlin35Xiaochou2wp02") != 0 )
		{
			Xiaochou2SpellDic.TryGetValue ("bosssenlin35Xiaochou21", out useSpell);
		
			if (GetAttackCount(Xiaochou2Unit) % 7 == 0 && GetAttackCount(Xiaochou2Unit) != 0) 
			{
				Xiaochou2SpellDic.TryGetValue ("bosssenlin35Xiaochou24", out useSpell);
			}
			else if (GetAttackCount(Xiaochou2Unit) % 3 == 0 && GetAttackCount(Xiaochou2Unit) != 0) 
			{
				Xiaochou2SpellDic.TryGetValue ("bosssenlin35Xiaochou23", out useSpell);
			}
		}
		else
		{
			Xiaochou2SpellDic.TryGetValue ("bosssenlin35Xiaochou22", out useSpell);
			if (GetAttackCount(Xiaochou2Unit) % 7 == 0 && GetAttackCount(Xiaochou2Unit) != 0) 
			{
				Xiaochou2SpellDic.TryGetValue ("bosssenlin35Xiaochou25", out useSpell);
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
		if (args.wpID == "bosssenlin35Xiaochou2wp02")
		{
			target.TriggerEvent("xiaochou2_wp02dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
