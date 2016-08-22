

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossdajie14Piying5 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Piying5Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Piying5SpellDic = GetUnitSpellList (Piying5Unit);

		Spell useSpell = null;
		Piying5SpellDic.TryGetValue ("bossdajie14Piying51", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Piying5Unit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Piying5Unit);

		if (NormalScript.GetWpLifeLeftRatio(Piying5Unit.battleUnit, "bossdajie14Piying5wp02") != 0 && NormalScript.GetWpLifeLeftRatio(Piying5Unit.battleUnit, "bossMinghe14Piying5wp03") != 0 )
		{
			if (GetAttackCount(Piying5Unit) % 6 == 0) 
			{
				Piying5SpellDic.TryGetValue ("bossdajie14Piying54", out useSpell);
			}
			else if (GetAttackCount(Piying5Unit) % 3 == 0) 
			{
				Piying5SpellDic.TryGetValue ("bossdajie14Piying52", out useSpell);
			}
			else if (GetAttackCount(Piying5Unit) % 2 == 0) 
			{
				Piying5SpellDic.TryGetValue ("bossdajie14Piying53", out useSpell);
			}
					
		} 

		else if (NormalScript.GetWpLifeLeftRatio(Piying5Unit.battleUnit, "bossdajie14Piying5wp02") == 0 && NormalScript.GetWpLifeLeftRatio(Piying5Unit.battleUnit, "bossMinghe14Piying5wp03") != 0 )
		{
			if (GetAttackCount(Piying5Unit) % 6 == 0) 
			{
				Piying5SpellDic.TryGetValue ("bossdajie14Piying54", out useSpell);
			}
			else if (GetAttackCount(Piying5Unit) % 2 == 0) 
			{
				Piying5SpellDic.TryGetValue ("bossdajie14Piying53", out useSpell);
			}

		}
		else if (NormalScript.GetWpLifeLeftRatio(Piying5Unit.battleUnit, "bossdajie14Piying5wp02") != 0 && NormalScript.GetWpLifeLeftRatio(Piying5Unit.battleUnit, "bossMinghe14Piying5wp03") == 0 )
		{
			if (GetAttackCount(Piying5Unit) % 6 == 0) 
			{
				Piying5SpellDic.TryGetValue ("bossdajie14Piying54", out useSpell);
			}
			else if (GetAttackCount(Piying5Unit) % 2 == 0) 
			{
				Piying5SpellDic.TryGetValue ("bossdajie14Piying52", out useSpell);
			}

		}
		else
		{
			if (GetAttackCount(Piying5Unit) % 6 == 0) 
			{
				Piying5SpellDic.TryGetValue ("bossdajie14Piying54", out useSpell);
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
		if (args.wpID == "bossdajie14Piying5wp02" && jishu==0)
		{
			target.TriggerEvent("Piying5_state1to2", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
			jishu ++;
		}

		if (args.wpID == "bossdajie14Piying5wp03" && jishu==0)
		{
			target.TriggerEvent("Piying5_state1to3", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
			jishu ++;
		}
		
		if (args.wpID == "bossdajie14Piying5wp02" && args.wpID == "bossdajie14Piying5wp03" && jishu==0)
		{
			target.TriggerEvent("Piying5_stateNto4", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
			jishu ++;
		}
	}
	//---------------------------------------------------------------------------------------------
}
