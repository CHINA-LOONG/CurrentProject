

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossmidasigongHuangjinzhiling1 : BossAi {

	// Use this for initialization
	void Start () 
	{
	
	}
	int jishu = 0 ;

	public override BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit Huangjinzhiling1Unit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> Huangjinzhiling1SpellDic = GetUnitSpellList (Huangjinzhiling1Unit);

		Spell useSpell = null;

		attackResult.attackTarget = GetAttackRandomTarget(Huangjinzhiling1Unit);
		if (Huangjinzhiling1Unit.curLife / (float)Huangjinzhiling1Unit.maxLife >= 0.5) 
		{
			Huangjinzhiling1SpellDic.TryGetValue ("bossmidasigongHuangjinzhiling11", out useSpell);
			if (GetAttackCount (Huangjinzhiling1Unit) % 3 == 0 && GetAttackCount (Huangjinzhiling1Unit) != 0) 
			{
				Huangjinzhiling1SpellDic.TryGetValue ("bossmidasigongHuangjinzhiling13", out useSpell);			
			}
		} 
		else 
		{
			Huangjinzhiling1SpellDic.TryGetValue ("bossmidasigongHuangjinzhiling12", out useSpell);
			if (GetAttackCount (Huangjinzhiling1Unit) % 3 == 0 && GetAttackCount (Huangjinzhiling1Unit) != 0) 
			{
				Huangjinzhiling1SpellDic.TryGetValue ("bossmidasigongHuangjinzhiling14", out useSpell);			
			}
		}
		attackResult.useSpell = useSpell;

		return attackResult;
    }
    //---------------------------------------------------------------------------------------------
    
    //---------------------------------------------------------------------------------------------
	public override void OnVitalChange(SpellVitalChangeArgs args)
	{

		if (args.vitalCurrent <= args.vitalMax*0.5 && args.vitalType == (int)VitalType.Vital_Type_Default && jishu==0)
		{
			BattleObject target = ObjectDataMgr.Instance.GetBattleObject(args.targetID);
			if(target.unit.State != UnitState.Dead)
			{
				target.TriggerEvent("huangjinzhiling1_ctrl",Time.time,null);
				BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(1.5f);
				jishu ++;
			}
		}

	}
	//---------------------------------------------------------------------------------------------
}
