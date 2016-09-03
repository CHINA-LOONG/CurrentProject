

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossxiaoxiang21Wushi4 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;
	int i = 1;
	int huihe = 0;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Wushi4Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Wushi4SpellDic = GetUnitSpellList (Wushi4Unit);

		Spell useSpell = null;
		Wushi4SpellDic.TryGetValue ("bossxiaoxiang21Wushi41", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Wushi4Unit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Wushi4Unit);

		if (NormalScript.GetWpLifeLeft(Wushi4Unit.battleUnit, "bossxiaoxiang21Wushi4wp03") !=0)
		{
			//if (GetUnitHp(Mantuoluo2Unit) <= (GetUnitMaxHp(Mantuoluo2Unit) * 0.2 && i == 1)
			//{
				//Wushi4SpellDic.TryGetValue ("bossxiaoxiang21Wushi43", out useSpell);
				//i--;
			//}
			if (GetAttackCount(Wushi4Unit) % 5 == 0 && GetAttackCount(Wushi4Unit) !=0 ) 
			{
				Wushi4SpellDic.TryGetValue ("bossxiaoxiang21Wushi44", out useSpell);
			}
			else if (GetAttackCount(Wushi4Unit) % 3 == 0 && GetAttackCount(Wushi4Unit) !=0) 
			{
				Wushi4SpellDic.TryGetValue ("bossxiaoxiang21Wushi42", out useSpell);
			}
		} 

		else 
		{
			//if (GetUnitHp(Mantuoluo2Unit) <= (GetUnitMaxHp(Mantuoluo2Unit) * 0.2 && i == 1)
			//{
				//Wushi4SpellDic.TryGetValue ("bossxiaoxiang21Wushi43", out useSpell);
				//i--;
			//}
			if (huihe % 7 == 0 && huihe != 0 ) 
			{
				Wushi4SpellDic.TryGetValue ("bossxiaoxiang21Wushi44", out useSpell);
			}
			huihe ++;
		}			
		attackResult.useSpell = useSpell;
		
		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	//   public override void OnWpDead(WeakPointDeadArgs args)
	//	{
	//	BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
	//	if (args.wpID == "bossxiaoxiang21Wushi4wp03" && jishu==0)
	//    {
	//		target.TriggerEvent("Wushi4_state1to2", Time.time, null);
	//		BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
	//		jishu ++;
	//    }
	//}
	//---------------------------------------------------------------------------------------------
}
