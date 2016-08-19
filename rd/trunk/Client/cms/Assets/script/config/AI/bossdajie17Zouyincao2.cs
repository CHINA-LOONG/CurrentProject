

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossdajie17Zouyincao2 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Zouyincao2Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Zouyincao2SpellDic = GetUnitSpellList (Zouyincao2Unit);

		Spell useSpell = null;
		Zouyincao2SpellDic.TryGetValue ("bossdajie17Zouyincao21", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Zouyincao2Unit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Zouyincao2Unit);

		if (NormalScript.GetWpLifeLeftRatio(Zouyincao2Unit.battleUnit, "bossdajie17Zouyincao2wp02")!=0)
		{
			
			if (GetAttackCount(Zouyincao2Unit) % 7 == 0) 
			{
				Zouyincao2SpellDic.TryGetValue ("bossdajie17Zouyincao23", out useSpell);
			}
			else if (GetAttackCount(Zouyincao2Unit) % 3 == 0) 
			{
				Zouyincao2SpellDic.TryGetValue ("bossdajie17Zouyincao22", out useSpell);
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
		if (args.wpID == "bossMinghe14Zouyincao2wp03" && jishu==0)
        {
			target.TriggerEvent("Zouyincao2_state1to2", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
			jishu ++;
        }
	}
	//---------------------------------------------------------------------------------------------
}
