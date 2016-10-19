

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bosssenlin32Longnv3 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;
	int i = 1 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Longnv3Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Longnv3SpellDic = GetUnitSpellList (Longnv3Unit);

		Spell useSpell = null;
		Longnv3SpellDic.TryGetValue ("bosssenlin32Longnv31", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Longnv3Unit);
		if (NormalScript.GetWpLifeLeft(Longnv3Unit.battleUnit, "bosssenlin32Longnv3wp03") == 0 && NormalScript.GetWpLifeLeft(Longnv3Unit.battleUnit, "bosssenlin32Longnv3wp04") == 0 && i == 1)
		{
			Longnv3SpellDic.TryGetValue ("bosssenlin32Longnv35", out useSpell);
			i--;
		}
		else if ((GetAttackCount(Longnv3Unit) - 4) % 6 == 0  ) 
		{
			Longnv3SpellDic.TryGetValue ("bosssenlin32Longnv34", out useSpell);
			Longnv3Unit.battleUnit.TriggerEvent("spell_longnv34", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(1.5f);
			i = 1;
		}
		else if (GetAttackCount(Longnv3Unit) % 6 == 0 && GetAttackCount(Longnv3Unit) != 0) 
		{
			Longnv3SpellDic.TryGetValue ("bosssenlin32Longnv33", out useSpell);

		}
		else if (GetAttackCount(Longnv3Unit) % 3 == 0 && GetAttackCount(Longnv3Unit) != 0) 
		{
			Longnv3SpellDic.TryGetValue ("bosssenlin32Longnv32", out useSpell);

		}

		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		if (args.wpID == "bosssenlin31Longnv3wp03")
		{
			target.TriggerEvent("longnv3_wp03dead", Time.time, null);
		}
		if (args.wpID == "bosssenlin31Longnv3wp04")
		{
			target.TriggerEvent("longnv3_wp04dead", Time.time, null);
		}
	}
	//---------------------------------------------------------------------------------------------
}
