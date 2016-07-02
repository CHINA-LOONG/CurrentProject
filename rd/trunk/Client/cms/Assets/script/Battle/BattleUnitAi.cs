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
		Gain,
		UnKown
	}

	public	class AiAttackResult
	{
		public	AiAttackStyle attackStyle = AiAttackStyle.UnKown;
		public	GameUnit attackTarget = null;
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

	// Use this for initialization
	void Start () 
	{
	
	}
	
	public	void	Init()
	{
		instance = this;
	}



	public	AiAttackResult GetAiAttackResult(GameUnit battleUnit)
	{
		AiAttackResult attackResult = new AiAttackResult ();

		string aIIndex = battleUnit.assetID;
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
		if (!IsNeedAttackTarget (attackResult.attackStyle,battleUnit ))
			return attackResult;

		GameUnit attackTarget = GetAttackTargetUnit (battleUnit);
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

	bool	IsNeedAttackTarget( AiAttackStyle attackStyle, GameUnit battleUnit )
	{
		if (attackStyle == AiAttackStyle.Lazy)
			return false;

		if (attackStyle == AiAttackStyle.Dazhao)
			return false;

		if (attackStyle == AiAttackStyle.Defence)
			return false;

		if (attackStyle == AiAttackStyle.Gain)
			return false;

		if (attackStyle == AiAttackStyle.MagicAttack) 
		{
			//如果法术技能是“治疗” 则不需要攻击目标
			if(IsCureMagic(battleUnit))
			{
				return false;
			}
		}
		return true;
	}

	GameUnit GetAttackTargetUnit(GameUnit battleUnit)
	{
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

		List<GameUnit> listTarget = GetOppositeSideFiledList (battleUnit);
		int rondomIndex = Util.RondomWithWeight (listWeight);

		//rodom style
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
			break;
		case 1:
			return AiAttackStyle.MagicAttack;
			break;
		case 2:
			return AiAttackStyle.Gain;
			break;
		case 3:
			return AiAttackStyle.Defence;
			break;
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

	List<GameUnit> GetOurSideFiledList(GameUnit battleUnit)
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

	List<GameUnit> GetOppositeSideFiledList(GameUnit battleUnit)
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
}
