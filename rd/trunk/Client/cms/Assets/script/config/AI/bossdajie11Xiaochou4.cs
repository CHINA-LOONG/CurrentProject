

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossdajie11Xiaochou4 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Xiaochou4Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Xiaochou4SpellDic = GetUnitSpellList (Xiaochou4Unit);

		Spell useSpell = null;
		Xiaochou4SpellDic.TryGetValue ("bossdajie11Xiaochou41", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(Xiaochou4Unit);

		if (GetAttackCount(Xiaochou4Unit) % 7 == 0 && GetAttackCount(Xiaochou4Unit) != 0) 
		{
			Xiaochou4SpellDic.TryGetValue ("bossdajie11Xiaochou43", out useSpell);
		}
		if (GetAttackCount(Xiaochou4Unit) % 3 == 0 && GetAttackCount(Xiaochou4Unit) != 0) 
		{
			Xiaochou4SpellDic.TryGetValue ("bossdajie11Xiaochou42", out useSpell);

		}
		
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
    public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		if (args.wpID == "bossMinghe14Xiaochou4wp03" && jishu==0)
        {
			target.TriggerEvent("Xiaochou4_state1to2", Time.time, null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
			jishu ++;
        }
	}
	//---------------------------------------------------------------------------------------------
}
