

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossxinshouBingxuenvwang3 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Bingxuenvwang3Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Bingxuenvwang3SpellDic = GetUnitSpellList (Bingxuenvwang3Unit);

		Spell useSpell = null;
		Bingxuenvwang3SpellDic.TryGetValue ("bossxinshouBingxuenvwang31", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Bingxuenvwang3Unit);

		if (GetAttackCount(Bingxuenvwang3Unit) % 4 == 0 && GetAttackCount(Bingxuenvwang3Unit) != 0) 
		{
			Bingxuenvwang3SpellDic.TryGetValue ("bossxinshouBingxuenvwang32", out useSpell);
		}
		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	// public override void OnWpDead(WeakPointDeadArgs args)
	//{
	//	BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
	//	if (args.wpID == "bossMinghe14Bingxuenvwang3wp03" && jishu==0)
	//  {
	//target.TriggerEvent("Bingxuenvwang3_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//  }
	//}
	//---------------------------------------------------------------------------------------------
}
