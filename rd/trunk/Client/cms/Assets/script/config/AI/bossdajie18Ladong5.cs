

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossdajie18Ladong5 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Ladong5Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Ladong5SpellDic = GetUnitSpellList (Ladong5Unit);

		Spell useSpell = null;
		Ladong5SpellDic.TryGetValue ("bossdajie18Ladong51", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Ladong5Unit);
		if (NormalScript.GetWpLifeLeft(Ladong5Unit.battleUnit, "bossdajie18Ladong5wp02") == 0 )
		{
			Ladong5SpellDic.TryGetValue ("bossdajie18Ladong53", out useSpell);
		}
		else if (GetAttackCount(Ladong5Unit) % 7 == 0 && GetAttackCount(Ladong5Unit) != 0) 
		{
			Ladong5SpellDic.TryGetValue ("bossdajie18Ladong54", out useSpell);
		}
		else if (GetAttackCount(Ladong5Unit) % 3 == 0 && GetAttackCount(Ladong5Unit) != 0) 
		{
			Ladong5SpellDic.TryGetValue ("bossdajie18Ladong52", out useSpell);

		}
		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	// public override void OnWpDead(WeakPointDeadArgs args)
	//{
	//	BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
	//	if (args.wpID == "bossMinghe14Ladong5wp03" && jishu==0)
	//  {
	//target.TriggerEvent("Ladong5_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//  }
	//}
	//---------------------------------------------------------------------------------------------
}
