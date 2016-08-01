using UnityEngine;
using System.Collections;

public class DazhaoExitCheck : MonoBehaviour {

	int  attackedByPhysics = 0;
	public	BattleObject checkBattle = null;
	// Use this for initialization
	void Start () 
	{
		checkBattle = gameObject.GetComponent<BattleObject> ();
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
		if (param.useSpell == null  || checkBattle == null || checkBattle.guid != param.targetID)
			return;

		if (param.useSpell.spellData.category != (int)SpellType.Spell_Type_PhyAttack)
			return;


		attackedByPhysics ++;

		if (IsExitByPhyAttacked (attackedByPhysics)) 
		{
			GameEventMgr.Instance.FireEvent<int>(GameEventList.ExitDazhaoByPhyAttacked,checkBattle.guid);
		}
	}

	public static bool IsExitByPhyAttacked(int attackNumber)
	{
		int exitPercent = 1;
		int randomValue = 100;
		if (attackNumber < 2)
		{
			exitPercent = 0;

		} 
		else if (attackNumber < 3) 
		{
			exitPercent = 10;
		}
		else if (attackNumber < 4) 
		{
			exitPercent = 20;
		}
		else if (attackNumber < 5) 
		{
			exitPercent = 30;
		} 
		else if (attackNumber < 6) 
		{
			exitPercent = 40;
		} 
		else 
		{
			exitPercent = 40;
		}
		randomValue = Random.Range(0,100);
		if (randomValue < exitPercent)
		{
			return true;
		}

		return false;
	}
}
