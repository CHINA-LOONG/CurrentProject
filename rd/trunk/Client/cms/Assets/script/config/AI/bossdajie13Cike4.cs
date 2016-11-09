

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossdajie13Cike4 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Cike4Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Cike4SpellDic = GetUnitSpellList (Cike4Unit);

		Spell useSpell = null;
		Cike4SpellDic.TryGetValue ("bossdajie13Cike41", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Cike4Unit);
		if (GetAttackCount(Cike4Unit) % 7 == 0 && GetAttackCount(Cike4Unit) != 0) 
		{
			Cike4SpellDic.TryGetValue ("bossdajie13Cike43", out useSpell);
		}
		else if (GetAttackCount(Cike4Unit) % 3 == 0 && GetAttackCount(Cike4Unit) != 0) 
		{
			Cike4SpellDic.TryGetValue ("bossdajie13Cike42", out useSpell);

		}
		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	// public override void OnWpDead(WeakPointDeadArgs args)
	//{
	//	BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
	//	if (args.wpID == "bossMinghe14Cike4wp03" && jishu==0)
	//  {
	//target.TriggerEvent("Cike4_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//  }
	//}
	//---------------------------------------------------------------------------------------------
}
