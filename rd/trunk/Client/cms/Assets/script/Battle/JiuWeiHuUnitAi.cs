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

		Spell useSpell = null;
		jiuWeihuSpellDic.TryGetValue ("attackTriStrong", out useSpell);

		attackResult.attackTarget = GetAttackRandomTarget(jiuWeihuUnit);

		List<string> wpList = null;
		wpList = GetAliveWeakPointList (jiuWeihuUnit);
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

		int i = 1;
		if (GetAttackCount(jiuWeihuUnit) % 4 == 3 && GetUnitMinHp(jiuWeihuUnit) <= (GetUnitMaxHp(jiuWeihuUnit) * 0.75))
		{
			jiuWeihuSpellDic.TryGetValue ("buffMagic", out useSpell);
			attackResult.attackTarget = jiuWeihuUnit;
		} 
		else if(GetAttackCount(jiuWeihuUnit) % 4 == 0 && GetUnitMinHp(jiuWeihuUnit) <= (GetUnitMaxHp(jiuWeihuUnit) * 0.75)) 
		{
			jiuWeihuSpellDic.TryGetValue ("magicCureSlight", out useSpell);
			attackResult.attackTarget = jiuWeihuUnit;
		}
		else if(GetAttackCount(jiuWeihuUnit) % 10 == 0 && count == 4) 
		{
			jiuWeihuSpellDic.TryGetValue ("bossJiuweihu4", out useSpell);
			attackResult.attackTarget = jiuWeihuUnit;
		}
		else if(GetAttackCount(jiuWeihuUnit) % 10 == 0 && count == 3) 
		{
			jiuWeihuSpellDic.TryGetValue ("bossJiuweihu3", out useSpell);
			attackResult.attackTarget = jiuWeihuUnit;
		}
		else if(GetAttackCount(jiuWeihuUnit) % 10 == 0 && count == 2) 
		{
			jiuWeihuSpellDic.TryGetValue ("bossJiuweihu2", out useSpell);
			attackResult.attackTarget = jiuWeihuUnit;
		}
		else if(GetAttackCount(jiuWeihuUnit) % 10 == 0 && count == 1) 
		{
			jiuWeihuSpellDic.TryGetValue ("bossJiuweihu1", out useSpell);
			attackResult.attackTarget = jiuWeihuUnit;
		}
		else if(GetUnitMinHp(jiuWeihuUnit) <= (GetUnitMaxHp(jiuWeihuUnit) * 0.3) && i == 1) 
		{
			jiuWeihuSpellDic.TryGetValue ("defend", out useSpell);
			attackResult.attackTarget = jiuWeihuUnit;
			i--;
		}

		attackResult.useSpell = useSpell;

		return attackResult;
	}

	private List<GameUnit> GetCanAttackList(GameUnit jiuWeihuUnit)
	{
		return BattleUnitAi.Instance.GetOppositeSideFiledList(jiuWeihuUnit);
	}

	private GameUnit GetAttackRandomTarget(GameUnit jiuWeiHuUnit)
	{
		List<GameUnit> listTarge = GetCanAttackList (jiuWeiHuUnit);
		
		int index = Random.Range (0, listTarge.Count);
		
		return listTarge[index];
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
