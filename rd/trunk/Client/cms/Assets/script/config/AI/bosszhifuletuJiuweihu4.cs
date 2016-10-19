

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosszhifuletuJiuweihu4 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Jiuweihu4Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Jiuweihu4SpellDic = GetUnitSpellList (Jiuweihu4Unit);

		Spell useSpell = null;
		Jiuweihu4SpellDic.TryGetValue ("bosszhifuletuJiuweihu41", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Jiuweihu4Unit);

		if (GetAttackCount(Jiuweihu4Unit) % 15 == 0 && GetAttackCount(Jiuweihu4Unit) != 0) 
		{
			Jiuweihu4SpellDic.TryGetValue ("bosszhifuletuJiuweihu43", out useSpell);
		}
		else if (GetAttackCount(Jiuweihu4Unit) % 3 == 0 && GetAttackCount(Jiuweihu4Unit) != 0) 
		{
			Jiuweihu4SpellDic.TryGetValue ("bosszhifuletuJiuweihu42", out useSpell);

		}
		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	// public override void OnWpDead(WeakPointDeadArgs args)
	//{
	//	BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
	//	if (args.wpID == "bossMinghe14Jiuweihu4wp03" && jishu==0)
	//  {
	//target.TriggerEvent("Jiuweihu4_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//  }
	//}
	//---------------------------------------------------------------------------------------------
}
