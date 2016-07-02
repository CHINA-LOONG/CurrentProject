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
	BattleUnitAiData  battleUnitAiData = null;
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



	public	AiAttackResult GetAiAttackResult(GameUnit battleUnit)
	{
		if (battleUnit.isBoss)
		{
			return jiuWeiHuAi.GetAiAttackResult(battleUnit);
		}

		AiAttackResult attackResult = new AiAttackResult ();

		string aIIndex = battleUnit.Ai;
		battleUnitAiData = StaticDataMgr.Instance.GetBattleUnitAiData (aIIndex);
		if (null == battleUnitAiData)
		{
			Logger.LogErrorFormat("Can't find  AiData index = " + aIIndex);
			return attackResult;
		}


		if (battleUnit.lazyList.Count < 1)
		{
			InitLazyList(battleUnit);
		}
		if (battleUnit.dazhaoList.Count < 1)
		{
			InitDazhaoList(battleUnit);
		}

		attackResult.attackStyle = GetAttackStyle (battleUnit);
		//test
		//if (attackResult.attackStyle != AiAttackStyle.PhysicsAttack )
		//{
		//	attackResult.attackStyle = AiAttackStyle.PhysicsAttack;
		//	Debug.LogError("todo:liws test attack Style,need spellList!!");
		//}
		///end test
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
		case (int) SpellType.Spell_Type_DaZhao:
		{

				List<GameUnit> listTarget = GetOppositeSideFiledList (battleUnit);
				//对方是否有可攻击的弱点
				GameUnit unit = null;
				string wpName = null;
				if(CheckAttackWpAi(listTarget,out unit, out wpName))
				{
					attackResult.attackTarget = unit;
					unit.attackWpName = wpName;
					return attackResult;
				}

			attackTarget = GetAttackTargetUnitNormalStyle (battleUnit,true);
		}
			break;

		case (int) SpellType.Spell_Type_Cure:
			attackTarget = GetAttackTargetUnitNormalStyle (battleUnit,false);
			break;
		
		case (int) SpellType.Spell_Type_Beneficial:
			attackTarget = GetAttackTargetUnitBuffStyle (battleUnit,attackResult.useSpell);
			break;
		case (int) SpellType.Spell_Type_Negative:
			attackTarget = GetAttackTargetUnitBuffStyle (battleUnit,attackResult.useSpell);
			break;
		default:
			Debug.LogError("battleAi Can't did the spelltype " + spellType);
			break;
		}
		attackTarget.attackWpName = null;
		attackResult.attackTarget = attackTarget;

		return attackResult;
	}

	void	InitLazyList(GameUnit battleUnit)
	{
		battleUnit.lazyList.Clear ();
		int maxGroup = 0;
		int lazyGroup = battleUnitAiData.lazyGroup;
		maxGroup = attackMaxTimes / lazyGroup + 1;

		int timesPerGroup = battleUnitAiData.lazyTimes;
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
		int dazhaoGroup = battleUnitAiData.dazhaoGroup;
		int dazhaoAdjust = battleUnitAiData.dazhaoAdjust;

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
		foreach (KeyValuePair<string,Spell> subSpellDic in spellDic)
		{
			Spell subSpel = subSpellDic.Value;
			AiAttackStyle spellStyle = GetAttackStyleWithSpellType(subSpel.spellData.category);
			if(attackStyle == spellStyle)
			{
				return subSpel;
			}
			else
			{
				Debug.LogError("spellStyle = " + spellStyle  + "  " + subSpel.spellData.category + " attackStyle = " + attackStyle);
			}
		}

		Debug.LogError ("Error for getSpell.. spell Count = " + spellDic.Count);
		return null;
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

	GameUnit GetAttackTargetUnitNormalStyle(GameUnit battleUnit,bool isAttack)
	{
		List<GameUnit> listTarget;
		if (isAttack)
		{
			listTarget = GetOppositeSideFiledList (battleUnit);
		}
		else
		{
			listTarget = GetOurSideFiledList  (battleUnit);
		}


		int [] weightSz = new int[8];
		weightSz [0] = battleUnitAiData.rondomWeight;

		weightSz [1] = battleUnitAiData.goldWeight;
		weightSz [2] = battleUnitAiData.woodWeight;
		weightSz [3] = battleUnitAiData.waterWeight;
		weightSz [4] = battleUnitAiData.fireWeight;
		weightSz [5] = battleUnitAiData.soilWeight;

		weightSz [6] = battleUnitAiData.maxBloodWeight;
		weightSz [7] = battleUnitAiData.minBloodWeight;
		List<int> listWeight = new List<int> (weightSz);

		int rondomIndex = Util.RondomWithWeight (listWeight);

		//rondom style
		if (0 == rondomIndex)
		{
			int index =  Random.Range(0,listTarget.Count);
			return listTarget[index];
		}

		// property stle
		if (rondomIndex > 0 && rondomIndex < 6) 
		{
			int property = rondomIndex - 1;//test

			GameUnit subUnit;
			for(int i =0;i<listTarget.Count;++i)
			{
				subUnit = listTarget[i];
				if(subUnit.property == property)
				{
					return subUnit;
				}
			}
			int index =  Random.Range(0,listTarget.Count);
			return  listTarget[index];
		}
		//blood style
		//max blood
		bool isAttackMaxBlood = false;
		if (6 == rondomIndex)
		{
			isAttackMaxBlood = true;
		}
		listTarget.Sort(delegate(GameUnit x, GameUnit y){
			if(isAttackMaxBlood)
			{
				return y.curLife.CompareTo(x.curLife);
			}
			else
			{
				return x.curLife.CompareTo(y.curLife);
			}
		});

		return listTarget [0];

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
		if (battleUnit.dazhaoList.Contains (battleUnit.attackCount))
		{
			return AiAttackStyle.Dazhao;
		}

		int [] weightSz = new int[4];
		weightSz[0] = battleUnitAiData.physicsWeight;
		weightSz[1] = GetMagicWeight(battleUnit);
		weightSz[2] = battleUnitAiData.gainWeight;
		weightSz[3] = battleUnitAiData.defenseWeight;
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
		int magicWeight = battleUnitAiData.magicWeight;
		if (IsCureMagic (battleUnit)) 
		{
			List<GameUnit> listUnit = GetOurSideFiledList(battleUnit);
			bool isCanCureMagic = false;
			foreach(GameUnit subUnit in listUnit)
			{
				if(subUnit.curLife/(float)subUnit.maxLife >= GameConfig.Instance.MaxCureMagicLifeRate )
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
		//todo by zz
		//Debug.LogError ("todo by ZZ,comming later!");
		return false;
	}

	public List<GameUnit> GetOurSideFiledList(GameUnit battleUnit)
	{
		BattleObject battleObject = battleUnit.gameObject.GetComponent<BattleObject>();
		UnitCamp camp = battleObject.camp;

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
		BattleObject battleObject = battleUnit.gameObject.GetComponent<BattleObject>();
		UnitCamp camp = battleObject.camp;
		
		if (camp == UnitCamp.Enemy) 
		{
			return GetValidGameUnitList(BattleController.Instance.BattleGroup.PlayerFieldList);
		} 
		else
		{
			return GetValidGameUnitList(BattleController.Instance.BattleGroup.EnemyFieldList);
		}
	}

	List<GameUnit> GetValidGameUnitList(List<GameUnit> battleList)
	{
		List<GameUnit> listField = new List<GameUnit> ();

		GameUnit subUnit = null;
		for (int i =0; i<battleList.Count; ++i) 
		{
			subUnit = battleList[i];
			if(null!=subUnit)
			{
				listField.Add(subUnit);
			}
		}
		return  listField;
	}

	private bool CheckAttackWpAi(List<GameUnit> listTarget,out GameUnit unit,out string wpName)
	{
		int rondomAi = Random.Range (0, 2);
		if (rondomAi == 0) 
		{
			unit = null;
			wpName = null;
			return false;
		}
		foreach (GameUnit subUnit in listTarget)
		{
			List<string> wpList = WeakPointController.Instance.GetCanAttackWeakpointList(subUnit);
			if(wpList == null || wpList.Count < 1)
			{
				continue;
			}
			int rindex = Random.Range(0,wpList.Count);
			unit = subUnit;
			//Debug.LogError("rindex = " + rindex + " count = " + wpList.Count);
			wpName = wpList[rindex];
			return true;
		}
		unit = null;
		wpName = null;
		return false;
	}
}
