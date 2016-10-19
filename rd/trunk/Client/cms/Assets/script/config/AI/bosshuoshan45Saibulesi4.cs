

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosshuoshan45Saibulesi4 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Saibulesi4Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Saibulesi4SpellDic = GetUnitSpellList (Saibulesi4Unit);

		Spell useSpell = null;
		Saibulesi4SpellDic.TryGetValue ("bosshuoshan45Saibulesi41", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Saibulesi4Unit);
		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Saibulesi4Unit);
		int count = wpList.Count -1;

		if(count == 3)
		{
			if (GetAttackCount(Saibulesi4Unit) % 7 == 0 && GetAttackCount(Saibulesi4Unit) != 0 ) 
			{
				Saibulesi4SpellDic.TryGetValue ("bosshuoshan45Saibulesi43", out useSpell);
			}
			else if (GetAttackCount(Saibulesi4Unit) % 3 == 0 && GetAttackCount(Saibulesi4Unit) != 0) 
			{
				Saibulesi4SpellDic.TryGetValue ("bosshuoshan45Saibulesi42", out useSpell);

			}
		}
		else if(count == 2)
		{
			if (GetAttackCount(Saibulesi4Unit) % 7 == 0 && GetAttackCount(Saibulesi4Unit) != 0 ) 
			{
				Saibulesi4SpellDic.TryGetValue ("bosshuoshan45Saibulesi44", out useSpell);
			}
			else if (GetAttackCount(Saibulesi4Unit) % 3 == 0 && GetAttackCount(Saibulesi4Unit) != 0) 
			{
				Saibulesi4SpellDic.TryGetValue ("bosshuoshan45Saibulesi42", out useSpell);

			}
		}
		else if(count == 1)
		{
			if (GetAttackCount(Saibulesi4Unit) % 7 == 0 && GetAttackCount(Saibulesi4Unit) != 0 ) 
			{
				Saibulesi4SpellDic.TryGetValue ("bosshuoshan45Saibulesi45", out useSpell);
			}
			else if (GetAttackCount(Saibulesi4Unit) % 3 == 0 && GetAttackCount(Saibulesi4Unit) != 0) 
			{
				Saibulesi4SpellDic.TryGetValue ("bosshuoshan45Saibulesi42", out useSpell);

			}
		}
		else if(count == 0)
		{
			if (GetAttackCount(Saibulesi4Unit) % 3 == 0 && GetAttackCount(Saibulesi4Unit) != 0) 
			{
				Saibulesi4SpellDic.TryGetValue ("bosshuoshan45Saibulesi42", out useSpell);

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
		if (args.wpID == "bosshuoshan45Saibulesi4wp02")
		{
			target.TriggerEvent("saibulesi4_wp02dead", Time.time, null);
		}
		if (args.wpID == "bosshuoshan45Saibulesi4wp03")
		{
			target.TriggerEvent("saibulesi4_wp03dead", Time.time, null);
		}
		if (args.wpID == "bosshuoshan45Saibulesi4wp04")
		{
			target.TriggerEvent("saibulesi4_wp04dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
