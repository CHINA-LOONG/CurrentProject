using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BattleUnitAi : MonoBehaviour {

	public enum AiAttackStyle
	{
		Lazy = 0,
		Dazhao,
		PhysicsAttack,
		MagicAttack,
		Defence,
		Beneficial,
		UnKown
	}

	int enemyLazyIndex = 3;
	int enmeyCharacterIndex = 3;//性格

	public	class AiAttackResult
	{
		public	AiAttackStyle attackStyle = AiAttackStyle.UnKown;
		public	GameUnit attackTarget = null;
		public  Spell useSpell = null;
	}

	static BattleUnitAi instance = null;
	public	static BattleUnitAi Instance
	{
		get
		{
			return instance;
		}
	}

	// 

	int	attackMaxTimes = 100;

	JiuWeiHuUnitAi jiuWeiHuAi = null;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	public	void	Init()
	{
		instance = this;
		jiuWeiHuAi = gameObject.AddComponent<JiuWeiHuUnitAi> ();
	}

    void OnDestroy()
    {
        Destroy(jiuWeiHuAi);
    }


	public	AiAttackResult GetAiAttackResult(GameUnit battleUnit)
	{
		if (battleUnit.isBoss)
		{
			return jiuWeiHuAi.GetAiAttackResult(battleUnit);
		}

		AiAttackResult attackResult = new AiAttackResult ();


		if ( battleUnit.lazyList.Count < 1)
		{
			InitLazyList(battleUnit);
		}
		if (UnitCamp.Enemy == battleUnit.pbUnit.camp && battleUnit.dazhaoList.Count < 1)
		{
			InitDazhaoList(battleUnit);
		}

		attackResult.attackStyle = GetAttackStyle (battleUnit);

		attackResult.useSpell = GetSpell (attackResult.attackStyle, battleUnit);

		GameUnit attackTarget = null;

		int spellType = attackResult.useSpell.spellData.category;
		switch ( spellType )
		{
		case (int) SpellType.Spell_Type_Defense:
		case (int) SpellType.Spell_Type_Lazy:
			attackTarget = battleUnit;
			break;

		case (int) SpellType.Spell_Type_PhyAttack:
		case (int) SpellType.Spell_Type_MgicAttack:
		{
			attackTarget = GetAttackTargetNormalStyle (battleUnit);
		}
			break;

		case (int) SpellType.Spell_Type_Cure:
			attackTarget = GetCurveAiTarget(battleUnit);
			break;
		
		case (int) SpellType.Spell_Type_Beneficial:
		case (int) SpellType.Spell_Type_Negative:
			attackTarget = GetAttackTargetUnitBuffStyle (battleUnit,attackResult.useSpell);
			break;
		case (int) SpellType.Spell_Type_DaZhao:
			attackTarget = GetDazhaoAttackTarget(battleUnit);
			break;
		default:
			Debug.LogError("battleAi Can't did the spelltype " + spellType);
			break;
		}
	//	attackTarget.attackWpName = null;
		attackResult.attackTarget = attackTarget;

		return attackResult;
	}

	void	InitLazyList(GameUnit battleUnit)
	{
		battleUnit.lazyList.Clear ();
		int maxGroup = 0;

		int lazyIndex = battleUnit.lazy;
		if (battleUnit.pbUnit.camp == UnitCamp.Enemy) 
		{
			lazyIndex = enemyLazyIndex;
		}
		LazyData lazyData = StaticDataMgr.Instance.GetLazyData (lazyIndex);
		if (null == lazyData) 
		{
			Debug.LogError("static lazy data Can't contain index = " + lazyIndex);
			return;
		}

		int lazyGroup = lazyData.lazyGroup;
		maxGroup = attackMaxTimes / lazyGroup + 1;

		int timesPerGroup = lazyData.lazyTimes;
		for (int i = 0; i< maxGroup; ++ i) 
		{
			List<int> rondomList = Util.RondomNoneReatNumbers(0,lazyGroup,timesPerGroup);
			foreach(int subRoodom in rondomList)
			{
				battleUnit.lazyList.Add(subRoodom + i * lazyGroup); 
			}
		}
	}
	
	void InitDazhaoList(GameUnit battleUnit)
	{
		battleUnit.dazhaoList.Clear ();

		int maxGroup = 0;

		InstanceData instanceData = BattleController.Instance.InstanceData;
		int dazhaoGroup = instanceData.dazhaoGroup;
		int dazhaoAdjust = instanceData.dazhaoAdjust;

		maxGroup = attackMaxTimes / dazhaoGroup + 1;

		int rondomIndex = 0;
		int rondomAdjust = 0;
		int dazhaoIndex = 0;
		for (int i = 0; i<maxGroup; ++i)
		{
			rondomIndex = Random.Range(0,dazhaoGroup);
			rondomAdjust = Random.Range(0,dazhaoAdjust);
			dazhaoIndex = (i+1) * dazhaoGroup + rondomIndex - rondomAdjust;

			int minDazhaoIndex = i*dazhaoGroup;

			for(int j = dazhaoIndex;j>=minDazhaoIndex;--j)
			{
				if(!battleUnit.lazyList.Contains(j))
				{
					battleUnit.dazhaoList.Add(j);
					break;
				}
			}
		}
	}

	Spell	GetSpell( AiAttackStyle attackStyle, GameUnit battleUnit )
	{
		Dictionary<string, Spell> spellDic = battleUnit.spellList;
        Spell subSpel = null;
		foreach (KeyValuePair<string,Spell> subSpellDic in spellDic)
		{
			subSpel = subSpellDic.Value;
			AiAttackStyle spellStyle = GetAttackStyleWithSpellType(subSpel.spellData.category);
			if(attackStyle == spellStyle)
			{
				return subSpel;
			}
			else
			{
				//Debug.LogError("spellStyle = " + spellStyle  + "  " + subSpel.spellData.category + " attackStyle = " + attackStyle);
			}
		}

		Logger.LogWarning("Error for getSpell.. spell Count = " + spellDic.Count);
		return subSpel;
	}

	AiAttackStyle GetAttackStyleWithSpellType(int spelltype )
	{
		switch (spelltype)
		{
		case (int) SpellType.Spell_Type_PhyAttack:
			return AiAttackStyle.PhysicsAttack;
			break;
		case (int) SpellType.Spell_Type_MgicAttack:
		case (int) SpellType.Spell_Type_Cure:
			return AiAttackStyle.MagicAttack;
			break;
		case (int) SpellType.Spell_Type_Defense:
			return AiAttackStyle.Defence;
			break;
		case (int) SpellType.Spell_Type_Beneficial:
		case (int) SpellType.Spell_Type_Negative:
			return AiAttackStyle.Beneficial;
		
		case (int) SpellType.Spell_Type_Lazy:
			return AiAttackStyle.Lazy;
		case (int) SpellType.Spell_Type_DaZhao:
			return AiAttackStyle.Dazhao;
			break;
		case (int) SpellType.Spell_Type_Passive:
			break;
		default:
			return AiAttackStyle.UnKown;
			break;
		}
		return AiAttackStyle.UnKown;
	}

	GameUnit GetAttackTargetNormalStyle(GameUnit battleUnit)
	{
		List<GameUnit> listTarget;
		listTarget = GetOppositeSideFiledList (battleUnit);

		int iIndex = Random.Range (0, listTarget.Count);

		GameUnit attackResult = listTarget [iIndex];

		if (attackResult.pbUnit.camp == UnitCamp.Enemy) 
		{
			string wpName = RandowmAttackWp(attackResult);
			if(!string.IsNullOrEmpty(wpName))
			{
				attackResult.attackWpName = wpName;
			}
		}

		return attackResult;
	}

	private string RandowmAttackWp(GameUnit attackUnit)
	{
		
		List<string> wpList = WeakPointController.Instance.GetAiCanAttackWeakpointList(attackUnit);
		if(wpList == null || wpList.Count < 1)
		{
			return null;
		}
		int rindex = Random.Range(0,wpList.Count);
		return wpList [rindex];
	}

	GameUnit GetAttackTargetUnitBuffStyle(GameUnit battleUnit,Spell casterSpell)
	{
		int spellType = casterSpell.spellData.category;

		List<GameUnit> listTarget;

		if (spellType == (int)SpellType.Spell_Type_Beneficial)
		{
			listTarget = GetOurSideFiledList (battleUnit);
		}
		else if ( spellType == (int)SpellType.Spell_Type_Negative )
		{
			listTarget = GetOppositeSideFiledList (battleUnit);
		}
		else
		{
			Logger.LogError("Attack Spell should buffer,  but curSpelltype = " + spellType);
			return null;
		}
		if (listTarget.Count < 1) 
		{
			return null;
		}

		List<GameUnit> listValidTarget = new List<GameUnit> ();

		GameUnit subUnit;
		for (int i =0; i< listTarget.Count; ++ i)
		{
			subUnit = listTarget[i];
			if(!IsGameUnitHaveBuff(subUnit,casterSpell.spellData.id))
			{
				listValidTarget.Add(subUnit);
			}
		}
		if (listValidTarget.Count < 1)
		{
			int rondomIndex = Random.Range (0, listTarget.Count);
			return listTarget [rondomIndex];
		} 
		else if (listValidTarget.Count == 1) 
		{
			return listValidTarget [0];
		} 
		else 
		{
			int rondomIndex = Random.Range (0, listValidTarget.Count);
			return listValidTarget [rondomIndex];
		}
	}

	GameUnit GetDazhaoAttackTarget(GameUnit battleUnit)
	{
		List<GameUnit> listTarget;
		listTarget = GetOppositeSideFiledList (battleUnit);
		
		int iIndex = Random.Range (0, listTarget.Count);
		
		GameUnit attackResult = listTarget [iIndex];
		return attackResult;
	}

	GameUnit GetCurveAiTarget(GameUnit battleUnit)
	{
	 	 List<GameUnit>	allTarget = GetOurSideFiledList (battleUnit);
		List<GameUnit> listTarget = new List<GameUnit> ();
		foreach(GameUnit subUnit in allTarget)
		{
			if(subUnit.curLife/(float)subUnit.maxLife <= GameConfig.Instance.MaxCureMagicLifeRate )
			{
				listTarget.Add(subUnit);
				break;
			}
			
		}
		int iIndex = 0;
		if (listTarget.Count > 0) 
		{
			iIndex = Random.Range (0, listTarget.Count);
			return listTarget [iIndex];
		}
		else
		{
			iIndex = Random.Range(0,allTarget.Count);
			return allTarget[iIndex];
		}
	}

	bool IsGameUnitHaveBuff(GameUnit unit,string spellID)
	{
		List<Buff> listBuffer = unit.buffList;
		if (null == listBuffer || listBuffer.Count < 1)
		{
			return false;
		}
		foreach (Buff subBuff in listBuffer)
		{
			if(subBuff.ownedSpell.spellData.id == spellID)
			{
				return true;
			}
		}
		return false;
	}

	AiAttackStyle GetAttackStyle(GameUnit battleUnit)
	{
		//lazy 
		if (battleUnit.lazyList.Contains (battleUnit.attackCount))
		{
			return AiAttackStyle.Lazy;
		}
		
		//大招
		if ( UnitCamp.Enemy == battleUnit.pbUnit.camp &&
		    battleUnit.dazhaoList.Contains (battleUnit.attackCount))
		{
			return AiAttackStyle.Dazhao;
		}

		int unitCharacter = battleUnit.character;
		if (battleUnit.pbUnit.camp == UnitCamp.Enemy) 
		{
			unitCharacter = enmeyCharacterIndex;
		}

		CharacterData characterData = StaticDataMgr.Instance.GetCharacterData (unitCharacter);
		if (null == characterData)
		{
			Debug.LogError("Can't Find characterData index = " + battleUnit.character);
			return AiAttackStyle.UnKown;
		}

		int [] weightSz = new int[4];
		weightSz[0] = characterData.physicsWeight;
		weightSz[1] = GetMagicWeight(battleUnit);
		weightSz[2] = characterData.gainWeight;
		weightSz[3] = characterData.defenseWeight;
		List<int> listWeight = new List<int> (weightSz);

		int rondomIndex = Util.RondomWithWeight (listWeight);
		switch (rondomIndex)
		{
		case 0:
			return AiAttackStyle.PhysicsAttack;
		case 1:
			return AiAttackStyle.MagicAttack;
		case 2:
			return AiAttackStyle.Beneficial;
		case 3:
			return AiAttackStyle.Defence;
		}

		return AiAttackStyle.UnKown;
	}

	int	GetMagicWeight(GameUnit battleUnit)
	{
		CharacterData characterData = StaticDataMgr.Instance.GetCharacterData (battleUnit.character);
		if (null == characterData)
		{
			Debug.LogError("Can't Find  1characterData index = " + battleUnit.character);
			return 0;
		}

		int magicWeight = characterData.magicWeight;
		if (IsCureMagic (battleUnit)) 
		{
			List<GameUnit> listUnit = GetOurSideFiledList(battleUnit);
			bool isCanCureMagic = false;
			foreach(GameUnit subUnit in listUnit)
			{
				if(subUnit.curLife/(float)subUnit.maxLife <= GameConfig.Instance.MaxCureMagicLifeRate )
				{
					isCanCureMagic = true;
					break;
				}

			}
			if(!isCanCureMagic)
			{
				magicWeight = 0;
			}
		}
		return magicWeight;
	}

	bool IsCureMagic(GameUnit battleUnit)
	{
		Dictionary<string,Spell> spellList = battleUnit.spellList;
		foreach (var subSpell in spellList.Values)
		{
			if(subSpell.spellData.category == (int)SpellType.Spell_Type_Cure)
			{
				return true;
			}
		}

		return false;
	}

	public List<GameUnit> GetOurSideFiledList(GameUnit battleUnit)
	{
		//BattleObject battleObject = battleUnit.gameObject.GetComponent<BattleObject>();
        UnitCamp camp = battleUnit.pbUnit.camp;

		if (camp == UnitCamp.Player) 
		{
			return GetValidGameUnitList(BattleController.Instance.BattleGroup.PlayerFieldList);
		} 
		else
		{
			return GetValidGameUnitList(BattleController.Instance.BattleGroup.EnemyFieldList);
		}
	}

	public List<GameUnit> GetOppositeSideFiledList(GameUnit battleUnit)
	{
		//BattleObject battleObject = battleUnit.gameObject.GetComponent<BattleObject>();
		UnitCamp camp = battleUnit.pbUnit.camp;
		
		if (camp == UnitCamp.Enemy) 
		{
			return GetValidGameUnitList(BattleController.Instance.BattleGroup.PlayerFieldList);
		} 
		else
		{
			return GetValidGameUnitList(BattleController.Instance.BattleGroup.EnemyFieldList);
		}
	}

	List<GameUnit> GetValidGameUnitList(List<BattleObject> battleList)
	{
		List<GameUnit> listField = new List<GameUnit> ();

		BattleObject subUnit = null;
		for (int i =0; i<battleList.Count; ++i) 
		{
			subUnit = battleList[i];
			if(null!=subUnit && subUnit.unit.curLife > 0)
			{
				listField.Add(subUnit.unit);
			}
		}
		return  listField;
	}
	
}
