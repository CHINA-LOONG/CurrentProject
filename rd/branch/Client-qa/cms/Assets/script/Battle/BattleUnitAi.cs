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
		public	int targetSlot = -1;
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
		if (!IsNeedAttackSlot (attackResult.attackStyle))
			return attackResult;

		return attackResult;
	}

	void	InitLazyList(GameUnit battleUnit)
	{
		
	}
	
	void InitDazhaoList(GameUnit battleUnit)
	{
		
	}

	bool	IsNeedAttackSlot( AiAttackStyle attackStyle )
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
		}
		return true;
	}

	int GetAttackSlot(GameUnit battleUnit)
	{
		return 0;
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
		return false;
	}

	List<GameUnit> GetOurSideFiledList(GameUnit battleUnit)
	{
		BattleObject battleObject = battleUnit.gameObject.GetComponent<BattleObject>();
		UnitCamp camp = battleObject.camp;

		if (camp == UnitCamp.Enemy) 
		{
			return BattleController.Instance.BattleGroup.PlayerFieldList;
		} 
		else
		{
			return BattleController.Instance.BattleGroup.EnemyFieldList;
		}
	}

	List<GameUnit> GetOppositeSideFiledList(GameUnit battleUnit)
	{
		BattleObject battleObject = battleUnit.gameObject.GetComponent<BattleObject>();
		UnitCamp camp = battleObject.camp;
		
		if (camp == UnitCamp.Player) 
		{
			return BattleController.Instance.BattleGroup.PlayerFieldList;
		} 
		else
		{
			return BattleController.Instance.BattleGroup.EnemyFieldList;
		}
	}
}
