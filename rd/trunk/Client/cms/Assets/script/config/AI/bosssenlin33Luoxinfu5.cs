

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosssenlin33Luoxinfu5 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Luoxinfu5Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Luoxinfu5SpellDic = GetUnitSpellList (Luoxinfu5Unit);

		Spell useSpell = null;
		Luoxinfu5SpellDic.TryGetValue ("bosssenlin33Luoxinfu51", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Luoxinfu5Unit);
		
		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Luoxinfu5Unit);
		int count = wpList.Count -1;		

		if (GetAttackCount(Luoxinfu5Unit) % 4 == 0 && GetAttackCount(Luoxinfu5Unit) != 0 && count == 4) 
		{
			Luoxinfu5SpellDic.TryGetValue ("bosssenlin33Luoxinfu52", out useSpell);
		}
		else if (GetAttackCount(Luoxinfu5Unit) % 4 == 0 && GetAttackCount(Luoxinfu5Unit) != 0 && count == 3)
		{
			Luoxinfu5SpellDic.TryGetValue ("bosssenlin33Luoxinfu53", out useSpell);

		}
		else if (GetAttackCount(Luoxinfu5Unit) % 4 == 0 && GetAttackCount(Luoxinfu5Unit) != 0 && count == 2)
		{
			Luoxinfu5SpellDic.TryGetValue ("bosssenlin33Luoxinfu54", out useSpell);

		}
		else if (GetAttackCount(Luoxinfu5Unit) % 4 == 0 && GetAttackCount(Luoxinfu5Unit) != 0 && count == 1)
		{
			Luoxinfu5SpellDic.TryGetValue ("bosssenlin33Luoxinfu55", out useSpell);

		}
		else if (GetAttackCount(Luoxinfu5Unit) % 4 == 0 && GetAttackCount(Luoxinfu5Unit) != 0 && count == 0)
		{
			Luoxinfu5SpellDic.TryGetValue ("bosssenlin33Luoxinfu56", out useSpell);

		}
		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		if (args.wpID == "bosssenlin33Luoxinfu5wp02")
		{
			target.TriggerEvent("luoxinfu5_wp02dead", Time.time, null);
		}
		if (args.wpID == "bosssenlin33Luoxinfu5wp03")
		{
			target.TriggerEvent("luoxinfu5_wp03dead", Time.time, null);
		}
		if (args.wpID == "bosssenlin33Luoxinfu5wp04")
		{
			target.TriggerEvent("luoxinfu5_wp04dead", Time.time, null);
		}
		if (args.wpID == "bosssenlin33Luoxinfu5wp03")
		{
			target.TriggerEvent("luoxinfu5_wp05dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
