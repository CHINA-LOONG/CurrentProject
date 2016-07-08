using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HundunUnitAi : MonoBehaviour {

	void Start () 
	{
		
	}
	
	public	 BattleUnitAi.AiAttackResult GetAiAttackResult(GameUnit HundunUnit)
	{
		BattleUnitAi.AiAttackResult attackResult = new BattleUnitAi.AiAttackResult ();
		
		//attackStyle todo
		attackResult.attackStyle = BattleUnitAi.AiAttackStyle.PhysicsAttack;
		
		//spell todo
		Dictionary<string,Spell> HundunSpellDic = GetUnitSpellList (HundunUnit);
		
		Spell useSpell = null;
		HundunSpellDic.TryGetValue ("attackStabStrong", out useSpell);
		
		attackResult.attackTarget = GetAttackRandomTarget(HundunUnit);
		
		/***********************************
		List<string> wpList = null;
		wpList = GetAliveWeakPointList (HundunUnit);
		int count = 0;
		for(int n = wpList.Count -1 ;n > 0;n--)
		{
			if (wpList[n] == "jiuweihu_wp2")
				count++;
			if (wpList[n] == "jiuweihu_wp4")
				count++;
			if (wpList[n] == "jiuweihu_wp5")
				count++;
			if (wpList[n] == "jiuweihu_wp9")
				count++;
		}
		*********************************/

		if (GetAttackCount(HundunUnit) % 7 == 0 && GetAttackCount(HundunUnit)!=0 ) 
		{
			HundunSpellDic.TryGetValue ("bossHundun", out useSpell);
		}
		else if(GetAttackCount(HundunUnit) % 4 == 0 ) 
		{
			HundunSpellDic.TryGetValue ("buffAttack", out useSpell);
			attackResult.attackTarget = HundunUnit;
		}
		else if(GetAttackCount(HundunUnit) % 3 == 0 ) 
		{
			HundunSpellDic.TryGetValue ("magicEarthSlight", out useSpell);
		}
		attackResult.useSpell = useSpell;
		
		return attackResult;
	}
	
	private List<GameUnit> GetCanAttackList(GameUnit HundunUnit)
	{
		return BattleUnitAi.Instance.GetOppositeSideFiledList(HundunUnit);
	}
	
	private GameUnit GetAttackRandomTarget(GameUnit HundunUnit)
	{
		List<GameUnit> listTarge = GetCanAttackList (HundunUnit);
		
		int index = Random.Range (0, listTarge.Count);
		
		return listTarge[index];
	}
	
	private int GetAttackCount(GameUnit HundunUnit)
	{
		return HundunUnit.attackCount;
	}
	
	private Dictionary<string,Spell> GetUnitSpellList(GameUnit battleUnit)
	{
		return battleUnit.spellList;
	}
	
	private List<Buff> GetUnitBuffList(GameUnit battleUnit)
	{
		return battleUnit.buffList;
	}
	
	private List<string> GetAliveWeakPointList(GameUnit gUnit)
	{
		List<string> wpList = new List<string> ();
		
		foreach (KeyValuePair<string,WeakPointRuntimeData> subWp in gUnit.battleUnit.wpGroup.allWpDic)
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
