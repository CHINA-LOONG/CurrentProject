

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossdajie11Moguguai4 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Moguguai4Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Moguguai4SpellDic = GetUnitSpellList (Moguguai4Unit);

		Spell useSpell = null;
		Moguguai4SpellDic.TryGetValue ("bossdajie11Moguguai41", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Moguguai4Unit);

		if (GetAttackCount(Moguguai4Unit) % 7 == 0 && GetAttackCount(Moguguai4Unit) != 0) 
		{
			Moguguai4SpellDic.TryGetValue ("bossdajie11Moguguai43", out useSpell);
		}
		else if (GetAttackCount(Moguguai4Unit) % 3 == 0 && GetAttackCount(Moguguai4Unit) != 0) 
		{
			Moguguai4SpellDic.TryGetValue ("bossdajie11Moguguai42", out useSpell);

		}
		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	// public override void OnWpDead(WeakPointDeadArgs args)
	//{
	//	BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
	//	if (args.wpID == "bossMinghe14Moguguai4wp03" && jishu==0)
	//  {
	//target.TriggerEvent("Moguguai4_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//  }
	//}
	//---------------------------------------------------------------------------------------------
}
