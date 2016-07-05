using UnityEngine;
using System.Collections;

public class DazhaoExitCheck : MonoBehaviour {

	int  attackedByPhysics = 0;
	public	BattleObject checkBattle = null;
	// Use this for initialization
	void Start () 
	{
		attackedByPhysics = 0;
		BindListener ();
	}

	void OnDestroy()
	{
		UnBindListener ();
	}
	
	void BindListener()
	{
		GameEventMgr.Instance.AddListener< SpellAttackStatisticsParam >(GameEventList.SpellAttackStatistics , OnSpellAttackStatistics);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener< SpellAttackStatisticsParam> (GameEventList.SpellAttackStatistics, OnSpellAttackStatistics);
	}
	
	void OnSpellAttackStatistics(SpellAttackStatisticsParam param)
	{
		if (param.useSpell == null || attackedByPhysics == null)
			return;

		if (param.useSpell.spellData.category != (int)SpellType.Spell_Type_MgicAttack)
			return;

		if (param.targetID != checkBattle.guid)
			return;

		attackedByPhysics ++;

		if (IsExitByPhyAttacked (attackedByPhysics)) 
		{
			GameEventMgr.Instance.FireEvent<int>(GameEventList.ExitDazhaoByPhyAttacked,param.casterID);
		}
	}

	public static bool IsExitByPhyAttacked(int attackNumber)
	{
		int exitPercent = 1;
		int randomValue = 100;
		if (attackNumber < 2)
		{
			exitPercent = 5;

		} 
		else if (attackNumber < 3) 
		{
			exitPercent = 100;//test
		}
		else if (attackNumber < 4) 
		{
			exitPercent = 12;
		}
		else if (attackNumber < 5) 
		{
			exitPercent = 15;
		} 
		else if (attackNumber < 6) 
		{
			exitPercent = 30;
		} 
		else 
		{
			exitPercent = 50;
		}
		randomValue = Random.Range(0,100);
		if (randomValue < exitPercent)
		{
			return true;
		}

		return false;
	}
}
