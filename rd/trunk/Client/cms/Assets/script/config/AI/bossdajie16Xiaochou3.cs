

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossdajie16Xiaochou3 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Xiaochou3Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Xiaochou3SpellDic = GetUnitSpellList (Xiaochou3Unit);

		Spell useSpell = null;


		attackResult.attackTarget = GetAttackRandomTarget(Xiaochou3Unit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Xiaochou3Unit);

		if (NormalScript.GetWpLifeLeftRatio(Xiaochou3Unit.battleUnit, "bossdajie16Xiaochou3wp02")== 0)
		{
			
			Xiaochou3SpellDic.TryGetValue ("bossdajie16Xiaochou32", out useSpell);

			if (GetAttackCount(Xiaochou3Unit) % 7 == 0 && GetAttackCount(Xiaochou3Unit) != 0) 
			{
				Xiaochou3SpellDic.TryGetValue ("bossdajie16Xiaochou35", out useSpell);
			}
		} 

		else 
		{
			Xiaochou3SpellDic.TryGetValue ("bossdajie16Xiaochou31", out useSpell);

			if (GetAttackCount(Xiaochou3Unit) % 7 == 0 && GetAttackCount(Xiaochou3Unit) != 0) 
			{
				Xiaochou3SpellDic.TryGetValue ("bossdajie16Xiaochou34", out useSpell);
			}
			else if (GetAttackCount(Xiaochou3Unit) % 3 == 0 && GetAttackCount(Xiaochou3Unit) != 0) 
			{
				Xiaochou3SpellDic.TryGetValue ("bossdajie16Xiaochou33", out useSpell);
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
		if (args.wpID == "bossdajie16Xiaochou3wp02" && jishu==0)
        {
			target.TriggerEvent("Xiaochou3_state1to2", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
			jishu ++;
        }
	}
	//---------------------------------------------------------------------------------------------
}
