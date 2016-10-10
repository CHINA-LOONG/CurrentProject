

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossxiaoxiang25Kairuisi5 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Kairuisi5Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Kairuisi5SpellDic = GetUnitSpellList (Kairuisi5Unit);

		Spell useSpell = null;

		attackResult.attackTarget = GetAttackRandomTarget(Kairuisi5Unit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Kairuisi5Unit);

		if (NormalScript.GetWpLifeLeft(Kairuisi5Unit.battleUnit, "bossxiaoxiang25Kairuisi5wp03")==0)
		{
			
			Kairuisi5SpellDic.TryGetValue ("bossxiaoxiang25Kairuisi52", out useSpell);

			if (GetAttackCount(Kairuisi5Unit) % 5 == 0 && GetAttackCount(Kairuisi5Unit) != 0) 
			{
				Kairuisi5SpellDic.TryGetValue ("bossxiaoxiang25Kairuisi55", out useSpell);
			}
			else if (GetAttackCount(Kairuisi5Unit) % 4 == 0 && GetAttackCount(Kairuisi5Unit) != 0) 
			{
				Kairuisi5SpellDic.TryGetValue ("bossxiaoxiang25Kairuisi53", out useSpell);
			}
		} 

		else 
		{
			Kairuisi5SpellDic.TryGetValue ("bossxiaoxiang25Kairuisi51", out useSpell);

			if (GetAttackCount(Kairuisi5Unit) % 5 == 0 && GetAttackCount(Kairuisi5Unit) != 0) 
			{
				Kairuisi5SpellDic.TryGetValue ("bossxiaoxiang25Kairuisi54", out useSpell);
			}
			else if (GetAttackCount(Kairuisi5Unit) % 4 == 0 && GetAttackCount(Kairuisi5Unit) != 0) 
			{
				Kairuisi5SpellDic.TryGetValue ("bossxiaoxiang25Kairuisi53", out useSpell);
			}
			else if (GetAttackCount(Kairuisi5Unit) % 3 == 0 && GetAttackCount(Kairuisi5Unit) != 0) 
			{
				Kairuisi5SpellDic.TryGetValue ("bossxiaoxiang25Kairuisi52", out useSpell);
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
		if (args.wpID == "bossxiaoxiang25Kairuisi5wp03")
		{
			target.TriggerEvent("kairuisi5_wp03dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
