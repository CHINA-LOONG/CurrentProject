

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossxiaoxiang22Xiyiren2 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Xiyiren2Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Xiyiren2SpellDic = GetUnitSpellList (Xiyiren2Unit);

		Spell useSpell = null;
		Xiyiren2SpellDic.TryGetValue ("bossxiaoxiang22Xiyiren21", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Xiyiren2Unit);

		if (GetAttackCount(Xiyiren2Unit) % 7 == 0 && GetAttackCount(Xiyiren2Unit) != 0) 
		{
			Xiyiren2SpellDic.TryGetValue ("bossxiaoxiang22Xiyiren24", out useSpell);
		}
		if (GetAttackCount(Xiyiren2Unit) % 5 == 0 && GetAttackCount(Xiyiren2Unit) != 0) 
		{
			Xiyiren2SpellDic.TryGetValue ("bossxiaoxiang22Xiyiren23", out useSpell);

		}	
		if (GetAttackCount(Xiyiren2Unit) % 2 == 0 && GetAttackCount(Xiyiren2Unit) != 0) 
		{
			Xiyiren2SpellDic.TryGetValue ("bossxiaoxiang22Xiyiren22", out useSpell);

		}			
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
    public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		if (args.wpID == "bossxiaoxiang22Xiyiren2wp03" && jishu==0)
        {
			target.TriggerEvent("Xiyiren2_state1to2", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(1.5f);
			jishu ++;
        }
	}
	//---------------------------------------------------------------------------------------------
}
