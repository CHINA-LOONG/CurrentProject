

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossxiaoxiang24Shuigui3 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Shuigui3Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Shuigui3SpellDic = GetUnitSpellList (Shuigui3Unit);

		Spell useSpell = null;
		Shuigui3SpellDic.TryGetValue ("bossxiaoxiang24Shuigui31", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Shuigui3Unit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Shuigui3Unit);

		if (NormalScript.GetWpLifeLeftRatio(Shuigui3Unit.battleUnit, "bossxiaoxiang24Shuigui3wp02")==0)
		{
			
			if (GetAttackCount(Shuigui3Unit) % 7 == 0 && GetAttackCount(Shuigui3Unit) != 0) 
			{
				Shuigui3SpellDic.TryGetValue ("bossxiaoxiang24Shuigui34", out useSpell);
			}
			else if (GetAttackCount(Shuigui3Unit) % 5 == 0 && GetAttackCount(Shuigui3Unit) != 0) 
			{
				Shuigui3SpellDic.TryGetValue ("bossxiaoxiang24Shuigui32", out useSpell);
			}
		} 

		else 
		{
			if (GetAttackCount(Shuigui3Unit) % 7 == 0 && GetAttackCount(Shuigui3Unit) != 0) 
			{
				Shuigui3SpellDic.TryGetValue ("bossxiaoxiang24Shuigui33", out useSpell);
			}
			else if (GetAttackCount(Shuigui3Unit) % 5 == 0 && GetAttackCount(Shuigui3Unit) != 0) 
			{
				Shuigui3SpellDic.TryGetValue ("bossxiaoxiang24Shuigui32", out useSpell);
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
		if (args.wpID == "bossxiaoxiang24Shuigui3wp02" && jishu==0)
        {
			target.TriggerEvent("Shuigui3_state1to2", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
			jishu ++;
        }
	}
	//---------------------------------------------------------------------------------------------
}
