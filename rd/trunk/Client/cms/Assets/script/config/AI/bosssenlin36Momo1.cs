

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

		if (GetAttackCount(Momo1Unit) % 7 == 0 && GetAttackCount(Momo1Unit) != 0) 
			{
				Momo1SpellDic.TryGetValue ("bosssenlin36Momo13", out useSpell);
			}
		else if (GetAttackCount(Momo1Unit) % 3 == 0 && GetAttackCount(Momo1Unit) != 0) 
			{
				Momo1SpellDic.TryGetValue ("bosssenlin36Momo12", out useSpell);

			}
		
		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		if (args.wpID == "bosssenlin36Momo1wp02")
		{
			target.TriggerEvent("momo1_wp02dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
