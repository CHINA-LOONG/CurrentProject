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
		Buff,
        DazhaoPrepare,
		UnKown
	}

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

	public	void	Init()
	{
		instance = this;
	}

    void OnDestroy()
    {
    }

	public GameUnit GetMagicDazhaoAttackUnit(GameUnit battleUnit)
	{
		return GetMagicAttackTarget (battleUnit);
	}

	//Ai 目标
    public GameUnit GetTargetThroughSpell(Spell spell, GameUnit caster)
    {
        if (spell == null)
            return null;
		
        GameUnit attackTarget = null;
        int spellType = spell.spellData.category;
		switch (spellType)
		{
		case (int)SpellType.Spell_Type_Passive:
		case (int)SpellType.Spell_Type_Defense:
		case (int)SpellType.Spell_Type_Lazy:
			attackTarget = caster;
			break;
			
		case (int)SpellType.Spell_Type_PhyAttack:
		case (int)SpellType.Spell_Type_PhyDaZhao:
			attackTarget = GetMagicAttackTarget(caster);
			break;
		case (int)SpellType.Spell_Type_MgicAttack:
		case (int)SpellType.Spell_Type_MagicDazhao:
			attackTarget = GetMagicAttackTarget(caster);
			break;
		case (int)SpellType.Spell_Type_Cure:
			attackTarget = GetCurveAiTarget(caster);
			break;

		case (int)SpellType.Spell_Type_Hot:
		case (int)SpellType.Spell_Type_Beneficial:
			attackTarget = GetValidGainBuffTarget(caster, spellType);
			break;
		case (int)SpellType.Spell_Type_Negative:
		case (int)SpellType.Spell_Type_Dot:
			attackTarget = GetValidNegativeBuffTarget(caster,spellType);
			break;
		case (int)SpellType.Spell_Type_PrepareDazhao:
			Spell dazhaoSpell = GetSpell(AiAttackStyle.Dazhao,caster);
			if(null != dazhaoSpell)
			{
				if(dazhaoSpell.spellData.category == (int)SpellType.Spell_Type_PhyAttack)
				{
					attackTarget = GetPhyAttackTarget(caster);
				}
				else
				{
					attackTarget = GetMagicAttackTarget(caster);
				}
			}

			break;
		default:
			Logger.LogError("battleAi Can't did the spelltype " + spellType);
			break;
		}
        return attackTarget;
    }

	public	AiAttackResult GetAiAttackResult(GameUnit battleUnit)
	{
		var  bossAi = battleUnit.battleUnit.GetComponent<BossAi> ();
		if (null != bossAi) 
		{
            if (bossAi.isUseXgAi)
            {
                AiAttackResult xgAiReulst = GetXgAi(battleUnit);
                return bossAi.GetAiAttackResult(battleUnit,xgAiReulst);
            }
            else
            {
                return bossAi.GetAiAttackResult(battleUnit);
            }
		}
        return GetXgAi(battleUnit);
	}

    AiAttackResult  GetXgAi(GameUnit battleUnit)
    {
        AiAttackResult attackResult = new AiAttackResult ();

		//if ( battleUnit.lazyList.Count < 1)
		//{
			//InitLazyList(battleUnit);
	//	}
		if (UnitCamp.Enemy == battleUnit.pbUnit.camp && battleUnit.dazhaoList.Count < 1)
		{
			InitDazhaoList(battleUnit);
		}

		attackResult.attackStyle = GetAttackStyle (battleUnit);

		attackResult.useSpell = GetSpell (attackResult.attackStyle, battleUnit);

        attackResult.attackTarget = GetTargetThroughSpell(attackResult.useSpell, battleUnit);
		CheckBossWeakPoint (attackResult.attackTarget);

		return attackResult;
    }

	void	InitLazyList(GameUnit battleUnit)
	{
		battleUnit.lazyList.Clear ();
		int maxGroup = 0;

		int lazyIndex = battleUnit.lazy;
		LazyData lazyData = StaticDataMgr.Instance.GetLazyData (lazyIndex);
		if (null == lazyData) 
		{
			Logger.LogError("static lazy data Can't contain index = " + lazyIndex);
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
        int dazhaoGroup = instanceData.instanceProtoData.dazhaoGroup;
        int dazhaoAdjust = instanceData.instanceProtoData.dazhaoAdjust;

		maxGroup = attackMaxTimes / dazhaoGroup + 1;

		//int rondomIndex = 0;
		int rondomAdjust = 0;
		int dazhaoIndex = 0;
		for (int i = 0; i<maxGroup; ++i)
		{
			//rondomIndex = Random.Range(0,dazhaoGroup);
			rondomAdjust = Random.Range(0,dazhaoAdjust);
			dazhaoIndex = (i+1) * dazhaoGroup - 1 - rondomAdjust;

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
	#region  -----------------------  AttackStyle---------------

	AiAttackStyle GetAttackStyle(GameUnit battleUnit)
	{
		if (battleUnit.dazhao > 0)
		{
            if (battleUnit.dazhaoPrepareCount == 0)
            {
                return AiAttackStyle.Dazhao;
            }
		}
		
		//lazy 
		if (battleUnit.lazyList.Contains (battleUnit.attackCount))
		{
			return AiAttackStyle.Lazy;
		}
		
		//大招
		if ( UnitCamp.Enemy == battleUnit.pbUnit.camp &&
		    battleUnit.dazhaoList.Contains (battleUnit.attackCount))
		{
			if(battleUnit.energy >= BattleConst.enegyMax)
			{
				return AiAttackStyle.DazhaoPrepare;
			}
		}
		
		int unitCharacter = battleUnit.character;
		
		CharacterData characterData = StaticDataMgr.Instance.GetCharacterData (unitCharacter);
		if (null == characterData)
		{
			Logger.LogError("Can't Find characterData index = " + battleUnit.character);
			return AiAttackStyle.UnKown;
		}
		
		int [] weightSz = new int[4];
		weightSz[0] = characterData.physicsWeight;
		
		weightSz[1] = GetMagicWeight(battleUnit);
		weightSz [2] = GetBuffWeight (battleUnit); 
		weightSz[3] = GetDefenseWeight(battleUnit);
		
		
		List<int> listWeight = new List<int> (weightSz);
		
		int rondomIndex = Util.RondomWithWeight (listWeight);
		switch (rondomIndex)
		{
		case 0:
			return AiAttackStyle.PhysicsAttack;
		case 1:
			return AiAttackStyle.MagicAttack;
		case 2:
			return AiAttackStyle.Buff;
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
			Logger.LogError("Can't Find  1characterData index = " + battleUnit.character);
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
					magicWeight = characterData.cureMagicWeight;
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
	
	int GetBuffWeight(GameUnit battleUnit)
	{
		CharacterData characterData = StaticDataMgr.Instance.GetCharacterData (battleUnit.character);
		Spell buffSpell = GetSpell(AiAttackStyle.Buff, battleUnit);
		if (null == buffSpell)
		{
			return 0;
		}
		if (buffSpell.spellData.category == (int)SpellType.Spell_Type_Beneficial ||
		    buffSpell.spellData.category == (int)SpellType.Spell_Type_Hot)
		{
			if(null == GetValidGainBuffTarget(battleUnit,buffSpell.spellData.category))
			{
				return 0;
			}
			return characterData.gainWeight;
		}
		else
		{
			if(null == GetValidNegativeBuffTarget(battleUnit,buffSpell.spellData.category))
			{
				return 0;
			}
			return characterData.negativeWeight;
		}
	}
	
	int GetDefenseWeight(GameUnit battleUnit)
	{
		CharacterData characterData = StaticDataMgr.Instance.GetCharacterData (battleUnit.character);
		
		Spell buffSpell = GetSpell(AiAttackStyle.Defence, battleUnit);
		if (null == buffSpell)
		{
			return 0;
		}
		List<GameUnit> targetList =  GetOppositeSideFiledList (battleUnit);
		foreach (GameUnit subUnit in targetList) 
		{
			if(subUnit.tauntTargetID == battleUnit.pbUnit.guid)
			{
				return characterData.tauntDefenseWeight;
			}
		}
		
		return characterData.defenseWeight;
	}

	#endregion

	#region  -----------------------  AttackTarget--------------
	AiAttackStyle GetAttackStyleWithSpellType(int spelltype )
	{
		switch (spelltype)
		{
		case (int) SpellType.Spell_Type_PhyAttack:
			return AiAttackStyle.PhysicsAttack;
			
		case (int) SpellType.Spell_Type_MgicAttack:
		case (int) SpellType.Spell_Type_Cure:
			return AiAttackStyle.MagicAttack;
			
		case (int) SpellType.Spell_Type_Defense:
			return AiAttackStyle.Defence;
			
		case (int) SpellType.Spell_Type_Beneficial:
		case (int) SpellType.Spell_Type_Hot:
		case (int) SpellType.Spell_Type_Negative:
		case (int) SpellType.Spell_Type_Dot:
			return AiAttackStyle.Buff;
			
		case (int) SpellType.Spell_Type_Lazy:
			return AiAttackStyle.Lazy;
			
		case (int) SpellType.Spell_Type_PhyDaZhao:
		case (int) SpellType.Spell_Type_MagicDazhao:
			return AiAttackStyle.Dazhao;
			
		case (int)SpellType.Spell_Type_PrepareDazhao:
			return AiAttackStyle.DazhaoPrepare;

		default:
			return AiAttackStyle.UnKown;
		}
	}


	GameUnit GetTauntTarget(GameUnit casterUnit)
	{
		int tauntId = casterUnit.tauntTargetID;
		if (BattleConst.battleSceneGuid != tauntId) 
		{
			BattleObject tauntTarget = ObjectDataMgr.Instance.GetBattleObject( tauntId );
			if (tauntTarget != null && 
			    tauntTarget.unit.pbUnit.slot >= BattleConst.slotIndexMin && 
			    tauntTarget.unit.pbUnit.slot <= BattleConst.slotIndexMax &&
			    tauntTarget.unit.State != UnitState.Dead
			    )
			{
				return tauntTarget.unit;
			}
		}
		return null;
	}
	
	GameUnit GetFireFocusTarget(GameUnit casterUnit)
	{
		if (casterUnit.pbUnit.camp == UnitCamp.Player) 
		{
			GameUnit fireTarget = BattleController.Instance.Process.fireFocusTarget;
			if (fireTarget != null) 
			{
				fireTarget.attackWpName = BattleController.Instance.Process.fireAttackWpName;
				return fireTarget;
			}
		}
		return null;
	}
	
	GameUnit GetMinLifeTarget(List<GameUnit> listTarget)
	{
		if (null == listTarget || listTarget.Count == 0)
		{
			return null;
		}
		int attackIndex = 0;
		int minLife = 99999999;
		GameUnit subUnit =null;
		for(int i =0;i<listTarget.Count;++i)
		{
			subUnit = listTarget[i];
			if(subUnit.curLife < minLife)
			{
				minLife = subUnit.curLife;
				attackIndex = i;
			}
		}
		return listTarget[attackIndex];
	}

	GameUnit GetPhyAttackTarget(GameUnit casterUnit)
	{
		GameUnit tauntTarget = GetTauntTarget (casterUnit);
		if (null != tauntTarget)
		{
			return tauntTarget;
		}
		GameUnit fireFocus = GetFireFocusTarget (casterUnit);
		if (null != fireFocus)
		{
			return fireFocus;
		}
		List<GameUnit> listTarget = GetOppositeSideFiledList (casterUnit);
		
		return GetMinLifeTarget (listTarget);
	}
	
	GameUnit GetMagicAttackTarget(GameUnit casterUnit)
	{
		GameUnit tauntTarget = GetTauntTarget (casterUnit);
		if (null != tauntTarget)
		{
			return tauntTarget;
		}
		GameUnit fireFocus = GetFireFocusTarget (casterUnit);
		if (null != fireFocus)
		{
			return fireFocus;
		}
		
		int bestAttackProperty = GetBestAttackProperty (casterUnit.property);
		List<GameUnit> listTarget = GetOppositeSideFiledList (casterUnit);
		
		if (listTarget.Count == 0)
		{
			Logger.LogError("Error:Opposide no unit");
			return null;
		}
		if (listTarget.Count == 1)
		{
			return listTarget[0];
		}
		
		List<GameUnit> bestAttackList = new List<GameUnit> ();
		foreach (GameUnit subUnit in listTarget)
		{
			if(subUnit.property == bestAttackProperty)
			{
				bestAttackList.Add(subUnit);
			}
		}
		if (bestAttackList.Count > 0)
		{
			return GetMinLifeTarget(bestAttackList);
		}
        return GetMinLifeTarget(listTarget);
        //int randomIndex = Random.Range (0, listTarget.Count);
		//return listTarget [randomIndex];
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
			}
		}
		
		int iIndex = 0;
		if (listTarget.Count > 0) 
		{
			float cureValue =  -1;
			
			for(int i = 0; i < listTarget.Count; ++i)
			{
				GameUnit subTargetUnit = listTarget[i];
				float tempValue = 0;
				tempValue = (subTargetUnit.maxLife - subTargetUnit.curLife)/GetInjuryRatio(battleUnit,subTargetUnit);
				if(tempValue > cureValue)
				{
					iIndex = i;
					cureValue = tempValue;
				}
			}
			return listTarget [iIndex];
		}
		else
		{
			iIndex = Random.Range(0,allTarget.Count);
			return allTarget[iIndex];
		}
	}

	GameUnit GetValidGainBuffTarget(GameUnit battleUnit, int spellType)
	{
		Spell selfBuffSpell = GetSpell(AiAttackStyle.Buff, battleUnit);
		
		if (null == selfBuffSpell)
			return null;
		
		if (selfBuffSpell.spellData.category != spellType)
			return null;
		
		bool isBuffSelf = false;
		if (selfBuffSpell.spellData.id.Contains ("Self")) 
		{
			isBuffSelf = true;
		}
		
		List<GameUnit> listValidTarget = new List<GameUnit> ();
		
		List<GameUnit> listTarget = GetOurSideFiledList (battleUnit);
		GameUnit subUnit;
		for (int i =0; i < listTarget.Count; ++i) 
		{
			subUnit = listTarget[i];
			if(isBuffSelf)
			{
				if(subUnit != battleUnit)
					continue;
			}
			if(IsGameUnitHaveBuff(subUnit,selfBuffSpell.spellData.id))
				continue;
			
			if(spellType == (int)SpellType.Spell_Type_Beneficial)
				listValidTarget.Add(subUnit);
			
			if(spellType == (int)SpellType.Spell_Type_Hot &&
			   (float)subUnit.curLife/(float)subUnit.maxLife < 0.9f)
				listValidTarget.Add(subUnit);
		}
		
		if (0 == listValidTarget.Count)
		{
			return null;
		}
		else if (1 == listValidTarget.Count) 
		{
			return listValidTarget [0];
		}
		else
		{
			if(spellType == (int)SpellType.Spell_Type_Beneficial)
			{
				int tempIndex = Random.Range(0,listValidTarget.Count);
				return listValidTarget[tempIndex];
			}
			
			int selIndex = 0;
			float maxInjuryValue = -1;
			for(int i = 0; i < listValidTarget.Count; ++i)
			{
				float injuryValue = GetInjuryRatio(battleUnit,listValidTarget[i]);
				float tempValue = 0;

				tempValue = (listValidTarget[i].maxLife - listValidTarget[i].curLife)/injuryValue;
				if(tempValue > maxInjuryValue)
				{
					maxInjuryValue = tempValue;
					selIndex = i;
				}
			}
			return listValidTarget[selIndex];
		}
	}
	
	GameUnit GetValidNegativeBuffTarget(GameUnit battleUnit, int spellType)
	{
		Spell buffSpell = GetSpell(AiAttackStyle.Buff, battleUnit);
		
		if (null == buffSpell)
			return null;
		
		if (buffSpell.spellData.category != spellType)
			return null;
		
		GameUnit fireTarget = GetFireFocusTarget (battleUnit);
		if (null != fireTarget)
		{
			if(!IsGameUnitHaveBuff(fireTarget,buffSpell.spellData.id))
			{
				return fireTarget;
			}
		}
		
		List<GameUnit> listTarget = GetOppositeSideFiledList (battleUnit);
		List<GameUnit> listValidTarget = new List<GameUnit> ();
		
		GameUnit subUnit = null;
		for (int i = 0; i< listTarget.Count; ++i)
		{
			subUnit = listTarget[i];
			if(!IsGameUnitHaveBuff(subUnit,buffSpell.spellData.id))
			{
				listValidTarget.Add(subUnit);
			}
		}
		
		if (0 == listValidTarget.Count)
			return null;
		
		int rondomIndex = Random.Range (0, listValidTarget.Count);
		return listValidTarget [rondomIndex];
	}

	public void	CheckBossWeakPoint(GameUnit targetUnit)
	{
		if (null == targetUnit)
			return;
		if (!targetUnit.isBoss)
			return;
		targetUnit.attackWpName = null;
		if (targetUnit == BattleController.Instance.Process.fireFocusTarget)
		{
			targetUnit.attackWpName = BattleController.Instance.Process.fireAttackWpName;
		}
		
		if (!string.IsNullOrEmpty (targetUnit.attackWpName))
			return;
		List<string> wpList = targetUnit.battleUnit.wpGroup.GetAiCanAttackList ();
		if (wpList != null && wpList.Count > 0) 
		{
			int rondomIndex = Random.Range(0,wpList.Count);
			targetUnit.attackWpName = wpList[rondomIndex];
		}
	}

	#endregion



	public Spell GetSpell( AiAttackStyle attackStyle, GameUnit battleUnit )
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

		Logger.LogWarning("Error for getSpell.. spell Count = " + spellDic.Count + "  battle name = " + battleUnit.name);

		return null;
	}

	int GetBestAttackProperty(int property)
	{
		int attackProperty = 1;
		switch (property)
		{
		case  SpellConst.propertyGold :
			attackProperty = SpellConst.propertyEarth;
			break;
		case SpellConst.propertyWood :
			attackProperty = SpellConst.propertyWater;
			break;
		case SpellConst.propertyWater :
			attackProperty = SpellConst.propertyFire;
			break;
		case SpellConst.propertyFire:
			attackProperty = SpellConst.propertyWood;
			break;
		case  SpellConst.propertyEarth:
			attackProperty = SpellConst.propertyGold;
			break;
		}
		return attackProperty;
	}

	#region --------------help function-----------------------

	private string RandowmAttackWp(GameUnit attackUnit)
	{
		BattleObject bo = attackUnit.battleUnit;
		if (null == bo) 
		{
			Logger.LogError("Error:RandowmAttackWp unit can't connect battleobj");
			return null;
		}
		List<string> wpList = bo.wpGroup.GetAiCanAttackList ();
		if(wpList == null || wpList.Count < 1)
		{
			return null;
		}
		int rindex = Random.Range(0,wpList.Count);
		return wpList [rindex];
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
			if(subBuff.IsFinish == false && subBuff.ownedSpell.spellData.id == spellID)
			{
				return true;
			}
		}
		return false;
	}


	float GetInjuryRatio(GameUnit caster, GameUnit target)
	{
		//受伤比计算 max(1/(1+总防御力/I(min(lv1,lv2))),25%)
		float injuryRatio = 1.0f / (1.0f + (target.defense) / SpellFunctions.GetInjuryAdjustNum(caster.pbUnit.level, target.pbUnit.level));
		injuryRatio = injuryRatio < 0.25f ? 0.25f : injuryRatio;
		return injuryRatio;
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
			if(null!=subUnit && subUnit.unit.curLife > 0 && subUnit.unit.isVisible)
			{
				listField.Add(subUnit.unit);
			}
		}
		return  listField;
	}

	#endregion
	
}
