

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosssenlin38Meidusha2 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;
	int i = 1 ;
	int n = 1 ;
	int p = 1 ;
	int q = 1 ;
	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Meidusha2Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Meidusha2SpellDic = GetUnitSpellList (Meidusha2Unit);

		Spell useSpell = null;
		Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha25", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Meidusha2Unit);
		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Meidusha2Unit);

		int count = 0;
		for(int n = wpList.Count -1 ;n > 0;n--)
		{
			if (wpList[n] == "bosssenlin38Meidusha2wp02")
				count++;
			if (wpList[n] == "bosssenlin38Meidusha2wp03")
				count++;
			if (wpList[n] == "bosssenlin38Meidusha2wp04")
				count++;
			if (wpList[n] == "bosssenlin38Meidusha2wp05")
				count++;
		}
		if(count == 4)
		{
			if (GetAttackCount(Meidusha2Unit) % 7 == 0 && GetAttackCount(Meidusha2Unit) != 0) 
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha27", out useSpell);
			}
			else if (GetAttackCount(Meidusha2Unit) % 3 == 0 && GetAttackCount(Meidusha2Unit) != 0) 
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha26", out useSpell);

			}
		}
		else if(count == 3)
		{
			if(i == 1)
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha21", out useSpell);
				i--;
			}
			else if (GetAttackCount(Meidusha2Unit) % 7 == 0 && GetAttackCount(Meidusha2Unit) != 0) 
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha27", out useSpell);
			}
			else if (GetAttackCount(Meidusha2Unit) % 3 == 0 && GetAttackCount(Meidusha2Unit) != 0) 
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha26", out useSpell);

			}
		}	
		else if(count == 2)
		{
			if(n == 1)
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha22", out useSpell);
				n--;
			}
			else if (GetAttackCount(Meidusha2Unit) % 7 == 0 && GetAttackCount(Meidusha2Unit) != 0) 
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha27", out useSpell);
			}
			else if (GetAttackCount(Meidusha2Unit) % 3 == 0 && GetAttackCount(Meidusha2Unit) != 0) 
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha26", out useSpell);

			}
		}	
		else if(count == 1)
		{
			if(p == 1)
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha23", out useSpell);
				p--;
			}
			else if (GetAttackCount(Meidusha2Unit) % 7 == 0 && GetAttackCount(Meidusha2Unit) != 0) 
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha27", out useSpell);
			}
			else if (GetAttackCount(Meidusha2Unit) % 3 == 0 && GetAttackCount(Meidusha2Unit) != 0) 
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha26", out useSpell);

			}
		}	
		else if(count == 0)
		{
			if(q == 1)
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha24", out useSpell);
				q--;
			}
			else if (GetAttackCount(Meidusha2Unit) % 7 == 0 && GetAttackCount(Meidusha2Unit) != 0) 
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha27", out useSpell);
			}
			else if (GetAttackCount(Meidusha2Unit) % 3 == 0 && GetAttackCount(Meidusha2Unit) != 0) 
			{
				Meidusha2SpellDic.TryGetValue ("bosssenlin38Meidusha26", out useSpell);

			}
		}
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	// public override void OnWpDead(WeakPointDeadArgs args)
	//{
	//	BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
	//	if (args.wpID == "bossMinghe14Meidusha2wp03" && jishu==0)
	//  {
	//target.TriggerEvent("Meidusha2_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//  }
	//}
	//---------------------------------------------------------------------------------------------
}
