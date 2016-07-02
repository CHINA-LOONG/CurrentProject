using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JiuWeiHuUnitAi : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	
	public	 BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit jiuWeihuUnit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();

		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;

		//spell todo
		Dictionary<string,Spell> jiuWeihuSpellDic = GetUnitSpellList (jiuWeihuUnit);

		Spell magicSpell = null;
		if (jiuWeihuSpellDic.TryGetValue ("", out magicSpell))
		{

		} 
		else 
		{

		}

		foreach (KeyValuePair<string,Spell> subSpellDic in jiuWeihuSpellDic)
		{
			attackResult.useSpell = subSpellDic.Value;
			break;
		}
		

		//target todo
		List<GameUnit> listUnit = GetCanAttackList (jiuWeihuUnit);

		int randomIndex = Random.Range (0, listUnit.Count);

		attackResult.attackTarget = listUnit [randomIndex];


		return attackResult;
	}

	private List<GameUnit> GetCanAttackList(GameUnit jiuWeihuUnit)
	{
		return BattleUnitAi.Instance.GetOppositeSideFiledList(jiuWeihuUnit);
	}

	private int GetAttackCount(GameUnit jiuWeihuUnit)
	{
		return jiuWeihuUnit.attackCount;
	}

	private Dictionary<string,Spell> GetUnitSpellList(GameUnit battleUnit)
	{
		return battleUnit.spellList;
	}

	private List<Buff> GetUnitBuffList(GameUnit battleUnit)
	{
		return battleUnit.buffList;
	}

	private List<string> GetAliveWeakPointList(GameUnit battleUnit)
	{
		List<string> wpList = new List<string> ();

		foreach (KeyValuePair<string,WeakPointRuntimeData> subWp in battleUnit.wpHpList)
		{
			WeakPointRuntimeData wpData = subWp.Value;
			if(wpData.hp > 0)
			{
				wpList.Add(subWp.Key);
			}
		}
		return wpList;
	}

	private int GetUnitMaxHp(GameUnit battleUnit)
	{
		return battleUnit.maxLife;
	}

	private int GetUnitMinHp(GameUnit battleUnit)
	{
		return battleUnit.curLife;
	}
}
