

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossxiaoxiang28Shujing : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit ShujingUnit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> ShujingSpellDic = GetUnitSpellList (ShujingUnit);

		Spell useSpell = null;

		attackResult.attackTarget = GetAttackRandomTarget(ShujingUnit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (ShujingUnit);
		
		ShujingSpellDic.TryGetValue ("bossxiaoxiang28Shujing1", out useSpell);
		if (GetAttackCount(ShujingUnit) % 10 == 0) 
		{
			ShujingSpellDic.TryGetValue ("bossxiaoxiang28Shujing4", out useSpell);
		}
		else if (GetAttackCount(ShujingUnit) % 5 == 0) 
		{
			ShujingSpellDic.TryGetValue ("bossxiaoxiang28Shujing3", out useSpell);
		}
		else if (GetAttackCount(ShujingUnit) % 3 == 0) 
		{
			ShujingSpellDic.TryGetValue ("bossxiaoxiang28Shujing2", out useSpell);
		}			
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
    public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		if (args.wpID == "bossMinghe14Shujingwp03" && jishu==0)
        {
			target.TriggerEvent("Shujing_state1to2", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
			jishu ++;
        }
	}
	//---------------------------------------------------------------------------------------------
}
