

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossdajie12Mantuoluo2 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Mantuoluo2Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Mantuoluo2SpellDic = GetUnitSpellList (Mantuoluo2Unit);

		Spell useSpell = null;
		Mantuoluo2SpellDic.TryGetValue ("bossdajie12Mantuoluo21", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Mantuoluo2Unit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (Mantuoluo2Unit);

		if (NormalScript.GetWpLifeLeftRatio(Mantuoluo2Unit.battleUnit, "bossdajie12Mantuoluo2wp02") != 0)
		{
			if (GetUnitHp(Mantuoluo2Unit) <= (GetUnitMaxHp(Mantuoluo2Unit) * 0.5 ) && GetAttackCount(Mantuoluo2Unit) != 0)
			{
				Mantuoluo2SpellDic.TryGetValue ("bossdajie12Mantuoluo24", out useSpell);
			}			
			else if (GetAttackCount(Mantuoluo2Unit) % 4 == 0 && GetAttackCount(Mantuoluo2Unit) != 0) 
			{
				Mantuoluo2SpellDic.TryGetValue ("bossdajie12Mantuoluo23", out useSpell);
			}
			else if (GetAttackCount(Mantuoluo2Unit) % 3 == 0 && GetAttackCount(Mantuoluo2Unit) != 0) 
			{
				Mantuoluo2SpellDic.TryGetValue ("bossdajie12Mantuoluo22", out useSpell);
			}
			

		} 

		else 
		{
			if (GetAttackCount(Mantuoluo2Unit) % 4 == 0 && GetAttackCount(Mantuoluo2Unit) != 0) 
			{
				Mantuoluo2SpellDic.TryGetValue ("bossdajie12Mantuoluo23", out useSpell);
			}
			else if (GetAttackCount(Mantuoluo2Unit) % 3 == 0 && GetAttackCount(Mantuoluo2Unit) != 0) 
			{
				Mantuoluo2SpellDic.TryGetValue ("bossdajie12Mantuoluo22", out useSpell);
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
		if (args.wpID == "bossdajie12Mantuoluo2wp02" && jishu==0)
        {
			target.TriggerEvent("Mantuoluo2_state1to2", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
			jishu ++;
        }
	}
	//---------------------------------------------------------------------------------------------
}
