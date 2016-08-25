

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossxiaoxiang23Panshen2 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Panshen2Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Panshen2SpellDic = GetUnitSpellList (Panshen2Unit);

		Spell useSpell = null;
		Panshen2SpellDic.TryGetValue ("bossxiaoxiang23Panshen22", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Panshen2Unit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Panshen2Unit);


		if (GetAttackCount(Panshen2Unit) % 6 == 0 && GetAttackCount(Panshen2Unit) != 0) 
		{
			Panshen2SpellDic.TryGetValue ("bossxiaoxiang23Panshen24", out useSpell);
		}

		else if (GetAttackCount(Panshen2Unit) % 3 == 0 && GetAttackCount(Panshen2Unit) != 0) 
		{
			Panshen2SpellDic.TryGetValue ("bossxiaoxiang23Panshen23", out useSpell);

		}			
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
    public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		if (args.wpID == "bossMinghe14Panshen2wp03" && jishu==0)
        {
			target.TriggerEvent("Panshen2_state1to2", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
			jishu ++;
        }
	}
	//---------------------------------------------------------------------------------------------
}
