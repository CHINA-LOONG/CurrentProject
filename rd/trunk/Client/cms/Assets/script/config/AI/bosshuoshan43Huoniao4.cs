

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosshuoshan43Huoniao4 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Huoniao4Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Huoniao4SpellDic = GetUnitSpellList (Huoniao4Unit);

		Spell useSpell = null;
		Huoniao4SpellDic.TryGetValue ("bosshuoshan43Huoniao41", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Huoniao4Unit);
		if (NormalScript.GetWpLifeLeft(Huoniao4Unit.battleUnit, "bosshuoshan43Huoniao4wp02") == 0 && NormalScript.GetWpLifeLeft(Huoniao4Unit.battleUnit, "bosshuoshan43Huoniao4wp03") == 0 && NormalScript.GetWpLifeLeft(Huoniao4Unit.battleUnit, "bosshuoshan43Huoniao4wp04") == 0)
		{
			if (GetAttackCount(Huoniao4Unit) % 7 == 0 && GetAttackCount(Huoniao4Unit) != 0) 
			{
				Huoniao4SpellDic.TryGetValue ("bosshuoshan43Huoniao43", out useSpell);
			}
			else if (GetAttackCount(Huoniao4Unit) % 3 == 0 && GetAttackCount(Huoniao4Unit) != 0) 
			{
				Huoniao4SpellDic.TryGetValue ("bosshuoshan43Huoniao42", out useSpell);
			}
		}
		else
		{
			if (GetUnitHp(Huoniao4Unit) <= (GetUnitMaxHp(Huoniao4Unit) * 0.5 ) && GetAttackCount(Huoniao4Unit) != 0)
			{
				Huoniao4SpellDic.TryGetValue ("bosshuoshan43Huoniao44", out useSpell);
			}			
			else if (GetAttackCount(Huoniao4Unit) % 7 == 0 && GetAttackCount(Huoniao4Unit) != 0) 
			{
				Huoniao4SpellDic.TryGetValue ("bosshuoshan43Huoniao43", out useSpell);
			}
			else if (GetAttackCount(Huoniao4Unit) % 3 == 0 && GetAttackCount(Huoniao4Unit) != 0) 
			{
				Huoniao4SpellDic.TryGetValue ("bosshuoshan43Huoniao42", out useSpell);
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
	//	if (args.wpID == "bossMinghe14Huoniao4wp03" && jishu==0)
	//  {
	//target.TriggerEvent("Huoniao4_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//  }
	//}
	//---------------------------------------------------------------------------------------------
}
