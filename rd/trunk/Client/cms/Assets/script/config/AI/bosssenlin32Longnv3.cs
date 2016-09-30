

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosssenlin32Longnv3 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Longnv3Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Longnv3SpellDic = GetUnitSpellList (Longnv3Unit);

		Spell useSpell = null;
		Longnv3SpellDic.TryGetValue ("bosssenlin32Longnv31", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Longnv3Unit);

		if ((GetAttackCount(Longnv3Unit) - 4) % 6 == 0  ) 
		{
			Longnv3SpellDic.TryGetValue ("bosssenlin32Longnv34", out useSpell);
		}
		else if (GetAttackCount(Longnv3Unit) % 6 == 0 && GetAttackCount(Longnv3Unit) != 0) 
		{
			Longnv3SpellDic.TryGetValue ("bosssenlin32Longnv33", out useSpell);

		}
		else if (GetAttackCount(Longnv3Unit) % 3 == 0 && GetAttackCount(Longnv3Unit) != 0) 
		{
			Longnv3SpellDic.TryGetValue ("bosssenlin32Longnv32", out useSpell);

		}
		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	// public override void OnWpDead(WeakPointDeadArgs args)
	//{
	//	BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
	//	if (args.wpID == "bossMinghe14Longnv3wp03" && jishu==0)
	//  {
	//target.TriggerEvent("Longnv3_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//  }
	//}
	//---------------------------------------------------------------------------------------------
}
