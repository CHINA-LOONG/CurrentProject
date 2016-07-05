using UnityEngine;
using System.Collections;

public class PhyDazhaoController : MonoBehaviour 
{
	BattleObject  casterBattleGo;
	Spell dazhaoSpell;
	int	dazhaoUseCount = 0;
	int dazhaoFinishCount =0;
	float dazhaoStartTime = 0;

	DazhaoExitCheck  dazhaoExitCheck = null;

	DazhaoState dazhaoState = DazhaoState.Finished;


	enum DazhaoState
	{
		Wait = 0,
		Prepare,
		Start,
		Finished
	}


	static PhyDazhaoController instance = null;
	public static PhyDazhaoController Instance
	{
		get
		{
			return instance;
		}
	}
	// Use this for initialization
	void Start () 
	{
		BindListener ();
		instance = this;
	}
	void OnDestroy()
	{
		UnBindListener ();
	}
	
	void BindListener()
	{
		GameEventMgr.Instance.AddListener< int >(GameEventList.ExitDazhaoByPhyAttacked , OnExitByPhyAttacked);
		GameEventMgr.Instance.AddListener (GameEventList.MonsterShowoffOver, OnMonsterShowoffOver);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener< int > (GameEventList.ExitDazhaoByPhyAttacked, OnExitByPhyAttacked);
		GameEventMgr.Instance.RemoveListener (GameEventList.MonsterShowoffOver, OnMonsterShowoffOver);
	}


	public void PrepareDazhao(BattleObject casterGo)
	{
		casterBattleGo = casterGo;
		dazhaoState = DazhaoState.Wait;

		//蓄气效果
	//	casterGo.ShowDazhaoPrepareEffect ();
		if(casterBattleGo.shifaNodeEffect != null)
		{
			casterBattleGo.shifaNodeEffect.ShowEffectWithKey(EffectList.dazhaoPreprare);
		}

		//中断检测
		dazhaoExitCheck = casterGo.gameObject.GetComponent<DazhaoExitCheck> ();
		if (null != dazhaoExitCheck)
		{
			Destroy(dazhaoExitCheck);
			dazhaoExitCheck = null;
		}
		dazhaoExitCheck = casterGo.gameObject.GetComponent<DazhaoExitCheck>();
	}

	public void RunActionWithDazhao(BattleObject casterGo)
	{
		casterBattleGo = casterGo;
		dazhaoUseCount = 0;
		dazhaoFinishCount = 0;
		dazhaoStartTime = Time.time;

		if(casterBattleGo.shifaNodeEffect !=null)
		{
			casterBattleGo.shifaNodeEffect.HideEffectWithKey(EffectList.dazhaoPreprare);
		}

		dazhaoSpell = casterBattleGo.unit.GetDazhao ();
		if (dazhaoSpell == null)
		{
			Logger.LogErrorFormat("[SERIOUS]Unit {0}'s dazhao error! No dazhao is configured! Exit dazhao mode!!!", casterBattleGo.guid);
			DazhaoFinished();
			return;
		}
		dazhaoState = DazhaoState.Prepare;
		//BattleCamera.Instance.animator.SetBool (BattleCamera.AniControlParam.phyDazhao, true);

		GameEventMgr.Instance.FireEvent<UIBattle.UiState> (GameEventList.ChangeUIBattleState, UIBattle.UiState.Dazhao);
		//爆点
		if (casterGo.shifaNodeEffect != null) 
		{
			casterGo.shifaNodeEffect.ShowEffectWithKey(EffectList.dazhaoReady);
		}
		
		StartCoroutine (showOffCo ());
	}

	IEnumerator showOffCo()
	{
		yield return new WaitForSeconds (1.0f);
		
		if (casterBattleGo.shifaNodeEffect != null) 
		{
			casterBattleGo.shifaNodeEffect.HideEffectWithKey(EffectList.dazhaoReady);
		}
		DazhaofocusController.Instance.ShowoffDazhao (casterBattleGo);
	}


	void OnMonsterShowoffOver()
	{
		if (dazhaoState != DazhaoState.Prepare)
			return;
		Transform testCameraTrans =	BattleCamera.Instance.transform;
		Vector3 pos = testCameraTrans.position;
		pos.z -= 2;
		testCameraTrans.position = pos;
		StartCoroutine (PrepareDazhaoCo ());
	}
	IEnumerator PrepareDazhaoCo()
	{
		yield return new WaitForSeconds (1.0f);
		dazhaoState = DazhaoState.Start;
		GameEventMgr.Instance.FireEvent(GameEventList.ShowDazhaoTip);
		GameEventMgr.Instance.FireEvent<UIBattle.UiState> (GameEventList.ChangeUIBattleState, UIBattle.UiState.Dazhao);
	}

	public void  HitBattleObjectWithDazhao(BattleObject battleGo, string weakpointName)
	{
		if (dazhaoState != DazhaoState.Start)
		{
			return;
		}
		if (dazhaoUseCount >= dazhaoSpell.spellData.actionCount) 
		{
			return;
		}
		battleGo.unit.attackWpName = weakpointName;
	
		SpellService.Instance.SpellRequest(dazhaoSpell.spellData.id, casterBattleGo.unit, battleGo.unit, Time.time);
		dazhaoUseCount++;
		if (dazhaoUseCount >= dazhaoSpell.spellData.actionCount)
		{
			DazhaoFinished();
		}
	}
	
	private void OnExitByPhyAttacked( int casterID)
	{
		if (dazhaoState == DazhaoState.Finished)
			return;
		if (null == casterBattleGo)
			return;

		if (casterBattleGo.guid != casterID)
		{
			Logger.LogErrorFormat("ExitDazhao by PhyAttack Error: dazhao castID = {0}, getCasterID = {1}",casterBattleGo.guid,casterID);
			return;
		}
		if (dazhaoState == DazhaoState.Prepare)
		{
			//大招被打断
			GameEventMgr.Instance.FireEvent(GameEventList.RemoveDazhaoAction);

			//casterBattleGo.HideDazhaoPrepareEffect();
			if(casterBattleGo.shifaNodeEffect !=null)
			{
				casterBattleGo.shifaNodeEffect.HideEffectWithKey(EffectList.dazhaoPreprare);
			}
			//todo:大招被打断ui提示

		}
		if (null != dazhaoExitCheck)
		{
			Destroy(dazhaoExitCheck);
			dazhaoExitCheck = null;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (dazhaoState != DazhaoState.Start)
		{
			return;
		}

		if (Time.time - dazhaoStartTime >  dazhaoSpell.spellData.channelTime )
		{
			DazhaoFinished();
		}
	}

	public void FinishDazhaoWithAllEnemyDead()
	{
		DazhaoFinished ();
	}

	//大招结束
	void DazhaoFinished()
	{
		dazhaoState = DazhaoState.Finished;

		StartCoroutine (ExitDazhaoCo());
	}

	IEnumerator ExitDazhaoCo()
	{
		while (true) 
		{
			if (dazhaoUseCount == dazhaoFinishCount)
			{
				break;
			}
			yield return new WaitForEndOfFrame ();
		}
		//Debug.LogError ("大招结束.... attack times: " + dazhaoFinishCount);
		//BattleCamera.Instance.animator.SetBool (BattleCamera.AniControlParam.phyDazhao, false);
		Transform testCameraTrans =	BattleCamera.Instance.transform;
		Vector3 pos = testCameraTrans.position;
		pos.z += 2;
		testCameraTrans.position = pos;

		GameEventMgr.Instance.FireEvent<BattleObject>(GameEventList.DazhaoActionOver, casterBattleGo);
		GameEventMgr.Instance.FireEvent(GameEventList.HideDazhaoTip);
		GameEventMgr.Instance.FireEvent<UIBattle.UiState> (GameEventList.ChangeUIBattleState, UIBattle.UiState.Normal);
		
		if (null != dazhaoExitCheck)
		{
			Destroy(dazhaoExitCheck);
			dazhaoExitCheck = null;
		}
	}

	public void DazhaoAttackFinished(int casterID)
	{
		if (casterBattleGo!=null && casterID == casterBattleGo.guid)
		{
			dazhaoFinishCount ++;
		}
	}

	public float DazhaoLeftTime
	{
		get 
		{
			if (dazhaoSpell != null)
			{
				float passTime = Time.time - dazhaoStartTime;
				return Mathf.Clamp(dazhaoSpell.spellData.channelTime - passTime, 0, dazhaoSpell.spellData.channelTime);
			}
			return 0;
		}
	}

	public int DazhaoAllCount 
	{
		get
		{
			return dazhaoSpell.spellData.actionCount;
		}
	}

	public int DazhaoUseCount 
	{
		get
		{
			return dazhaoUseCount;
		}
	}
}
