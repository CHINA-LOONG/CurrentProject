

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossxiaoxiang28Muling2 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;
	int jishu1 = 0 ;

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
		
		ShujingSpellDic.TryGetValue ("bossxiaoxiang28Muling21", out useSpell);
		if (GetAttackCount(ShujingUnit) % 10 == 0) 
		{
			ShujingSpellDic.TryGetValue ("bossxiaoxiang28Muling24", out useSpell);
		}
		else if (GetAttackCount(ShujingUnit) % 5 == 0) 
		{
			ShujingSpellDic.TryGetValue ("bossxiaoxiang28Muling23", out useSpell);
		}
		else if (GetAttackCount(ShujingUnit) % 3 == 0) 
		{
			ShujingSpellDic.TryGetValue ("bossxiaoxiang28Muling22", out useSpell);
		}			
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
	//---------------------------------------------------------------------------------------------
	public override void OnWpDead(WeakPointDeadArgs args)
	{
		BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
		
		if (args.wpID == "bossxiaoxiang28Muling2wp03" && jishu <= 2) 
		{
			if (UnityEngine.Random.Range (0.0f, 1.0f) <= 0.33 && UnityEngine.Random.Range (0.0f, 1.0f) > 0)
			{
				target.TriggerEvent ("xiaoren3_state3to1", Time.time, null);
				BattleController.Instance.GetUIBattle ().wpUI.ChangeBatch (3.0f);
				jishu ++;
			}
		}
		
		else if (args.wpID == "bossxiaoxiang28Muling2wp05" && jishu <= 2)
		{
			if  (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.66 && UnityEngine.Random.Range(0.0f, 1.0f) > 0.33)
			{
				target.TriggerEvent ("xiaoren2_state3to1", Time.time, null);
				BattleController.Instance.GetUIBattle ().wpUI.ChangeBatch (3.0f);
				jishu ++;
			}
			
		}
		
		else
			if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.66 && UnityEngine.Random.Range(0.0f, 1.0f) > 0.33)
		{
			if (args.wpID == "bossxiaoxiang28Muling2wp07" && jishu<=2)
			{
				target.TriggerEvent("xiaoren1_state3to1", Time.time, null);
				BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
				jishu ++;
			}
		}
		
	}
	//---------------------------------------------------------------------------------------------

	//---------------------------------------------------------------------------------------------
	
	public override void OnVitalChange(SpellVitalChangeArgs args)
	{
		if (args.vitalCurrent <= args.vitalMax*0.33 && args.vitalType == (int)VitalType.Vital_Type_Default && jishu1==1)
		{
			BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
			target.TriggerEvent("xiaoren_chongzhi",Time.time,null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
			jishu1 ++;
		}

		else if (args.vitalCurrent <= args.vitalMax*0.66 && args.vitalType == (int)VitalType.Vital_Type_Default && jishu1==0)
		{
			BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
			target.TriggerEvent("xiaoren_chongzhi",Time.time,null);
			BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(3.0f);
			jishu1 ++;
		}
		
	}
}
