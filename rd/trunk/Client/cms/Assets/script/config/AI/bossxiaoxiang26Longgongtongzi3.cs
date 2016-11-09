

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossxiaoxiang26Longgongtongzi3 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;
	int huihe = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Longgongtongzi3Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Longgongtongzi3SpellDic = GetUnitSpellList (Longgongtongzi3Unit);

		Spell useSpell = null;
		Longgongtongzi3SpellDic.TryGetValue ("bossxiaoxiang26Longgongtongzi31", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Longgongtongzi3Unit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Longgongtongzi3Unit);

		if (NormalScript.GetWpLifeLeft(Longgongtongzi3Unit.battleUnit, "bossxiaoxiang26Longgongtongzi3wp03")==0)
		{
			if (huihe % 10 == 0 && huihe != 0)
			{
				Longgongtongzi3SpellDic.TryGetValue ("bossxiaoxiang26Longgongtongzi34", out useSpell);
			}				
			else if (huihe % 3 == 0 && huihe != 0)
			{
				Longgongtongzi3SpellDic.TryGetValue ("bossxiaoxiang26Longgongtongzi32", out useSpell);
			}
			huihe++ ;	
		} 

		else 
		{
			if (GetAttackCount(Longgongtongzi3Unit) % 7 == 0 && GetAttackCount(Longgongtongzi3Unit) != 0)
			{
				Longgongtongzi3SpellDic.TryGetValue ("bossxiaoxiang26Longgongtongzi34", out useSpell);
			}				
			else if (GetAttackCount(Longgongtongzi3Unit) % 3 == 0 && GetAttackCount(Longgongtongzi3Unit) != 0)
			{
				Longgongtongzi3SpellDic.TryGetValue ("bossxiaoxiang26Longgongtongzi32", out useSpell);
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
		if (args.wpID == "bossxiaoxiang26Longgongtongzi3wp03")
		{
			target.TriggerEvent("longgongtongzi3_wp03dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
