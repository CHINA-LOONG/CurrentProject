

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

		int count = wpList.Count -1;

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
				i--;
				double blood = Meidusha2Unit.maxLife * 0.75;
				Meidusha2Unit.curLife = (int)blood;
				
				SpellVitalChangeArgs args = new SpellVitalChangeArgs();
				args.vitalType = (int)VitalType.Vital_Type_FixLife;
				args.triggerTime = Time.time;
				args.casterID = Meidusha2Unit.battleUnit.guid;
				args.targetID = args.casterID;
				args.isCritical = false;
				args.vitalChange = 0;
				args.vitalCurrent = Meidusha2Unit.curLife;
				args.vitalMax = Meidusha2Unit.maxLife;
				GameEventMgr.Instance.FireEvent<System.EventArgs>(GameEventList.SpellLifeChange, args);
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
				n--;
				double blood = Meidusha2Unit.maxLife * 0.50;
				Meidusha2Unit.curLife = (int)blood;
				
				SpellVitalChangeArgs args = new SpellVitalChangeArgs();
				args.vitalType = (int)VitalType.Vital_Type_FixLife;
				args.triggerTime = Time.time;
				args.casterID = Meidusha2Unit.battleUnit.guid;
				args.targetID = args.casterID;
				args.isCritical = false;
				args.vitalChange = 0;
				args.vitalCurrent = Meidusha2Unit.curLife;
				args.vitalMax = Meidusha2Unit.maxLife;
				GameEventMgr.Instance.FireEvent<System.EventArgs>(GameEventList.SpellLifeChange, args);
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
				p--;
				double blood = Meidusha2Unit.maxLife * 0.25;
				Meidusha2Unit.curLife = (int)blood;
				
				SpellVitalChangeArgs args = new SpellVitalChangeArgs();
				args.vitalType = (int)VitalType.Vital_Type_FixLife;
				args.triggerTime = Time.time;
				args.casterID = Meidusha2Unit.battleUnit.guid;
				args.targetID = args.casterID;
				args.isCritical = false;
				args.vitalChange = 0;
				args.vitalCurrent = Meidusha2Unit.curLife;
				args.vitalMax = Meidusha2Unit.maxLife;
				GameEventMgr.Instance.FireEvent<System.EventArgs>(GameEventList.SpellLifeChange, args);

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
				q--;
				double blood = Meidusha2Unit.maxLife * 0.0001;
				Meidusha2Unit.curLife = (int)blood;
				
				SpellVitalChangeArgs args = new SpellVitalChangeArgs();
				args.vitalType = (int)VitalType.Vital_Type_FixLife;
				args.triggerTime = Time.time;
				args.casterID = Meidusha2Unit.battleUnit.guid;
				args.targetID = args.casterID;
				args.isCritical = false;
				args.vitalChange = 0;
				args.vitalCurrent = Meidusha2Unit.curLife;
				args.vitalMax = Meidusha2Unit.maxLife;
				GameEventMgr.Instance.FireEvent<System.EventArgs>(GameEventList.SpellLifeChange, args);

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
	public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		if (args.wpID == "bosssenlin38Meidusha2wp02")
			{
				target.TriggerEvent("meidusha2_wp02dead", Time.time, null);
			}
		if (args.wpID == "bosssenlin38Meidusha2wp03")
			{
				target.TriggerEvent("meidusha2_wp03dead", Time.time, null);
			}
		if (args.wpID == "bosssenlin38Meidusha2wp04")
			{
				target.TriggerEvent("meidusha2_wp04dead", Time.time, null);
			}
		if (args.wpID == "bosssenlin38Meidusha2wp05")
			{
				target.TriggerEvent("meidusha2_wp05dead", Time.time, null);
			}	
	}
	//---------------------------------------------------------------------------------------------
}
