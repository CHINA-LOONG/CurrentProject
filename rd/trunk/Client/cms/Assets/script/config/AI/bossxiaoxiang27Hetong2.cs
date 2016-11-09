

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossxiaoxiang27Hetong2 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;
	int huihe = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Hetong2Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Hetong2SpellDic = GetUnitSpellList (Hetong2Unit);

		Spell useSpell = null;
		Hetong2SpellDic.TryGetValue ("bossxiaoxiang27Hetong21", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Hetong2Unit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Hetong2Unit);

		if (NormalScript.GetWpLifeLeft(Hetong2Unit.battleUnit, "bossxiaoxiang27Hetong2wp03")==0)
		{
			if (huihe % 5 == 0 && huihe != 0) 
			{
				Hetong2SpellDic.TryGetValue ("bossxiaoxiang27Hetong24", out useSpell);
			}
			else if (huihe % 2 == 0 && huihe != 0) 
			{
				Hetong2SpellDic.TryGetValue ("bossxiaoxiang27Hetong22", out useSpell);
			}
			huihe ++;
		} 

		else 
		{
			if (GetAttackCount(Hetong2Unit) % 6 == 0 && GetAttackCount(Hetong2Unit) != 0) 
			{
				Hetong2SpellDic.TryGetValue ("bossxiaoxiang27Hetong24", out useSpell);
			}
			else if (GetAttackCount(Hetong2Unit) % 2 == 0 && GetAttackCount(Hetong2Unit) != 0) 
			{
				Hetong2SpellDic.TryGetValue ("bossxiaoxiang27Hetong22", out useSpell);
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
		if (args.wpID == "bossxiaoxiang27Hetong2wp03")
		{
			target.TriggerEvent("hetong2_wp03dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
