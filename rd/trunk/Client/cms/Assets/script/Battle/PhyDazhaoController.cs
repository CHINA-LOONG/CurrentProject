using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

public class PhyDazhaoController : MonoBehaviour 
{
	BattleObject  casterBattleGo;
	Spell dazhaoSpell;
	int	dazhaoUseCount = 0;
	int dazhaoFinishCount =0;
	int dazhaoLeftTime =0;
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
		dazhaoExitCheck = casterGo.gameObject.AddComponent<DazhaoExitCheck>();
	}

	public void RunActionWithDazhao(BattleObject casterGo)
	{
		casterBattleGo = casterGo;
		dazhaoUseCount = 0;
		dazhaoFinishCount = 0;

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

		//隐藏摄像机 
		GameEventMgr.Instance.FireEvent<bool> (GameEventList.SetMirrorModeState, false);
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

		BattleCameraAni.MotionToPhyDazhao ().OnComplete(PrepareDazhaoCo);

		//StartCoroutine (PrepareDazhaoCo ());
	}
	void PrepareDazhaoCo()
	{
		//yield return new WaitForSeconds (1.0f);
		dazhaoState = DazhaoState.Start;
		dazhaoStartTime = Time.time;
		GameEventMgr.Instance.FireEvent(GameEventList.ShowDazhaoTip);
		GameEventMgr.Instance.FireEvent<UIBattle.UiState> (GameEventList.ChangeUIBattleState, UIBattle.UiState.Dazhao);
		if (dazhaoSpell != null)
		{
			dazhaoLeftTime = dazhaoSpell.spellData.channelTime;
			StopCoroutine("LeftTimeCo");
			StartCoroutine("LeftTimeCo");
		}
	}

	IEnumerator LeftTimeCo()
	{
		while (dazhaoLeftTime >0)
		{
			yield return new WaitForSeconds( Time.timeScale);
			dazhaoLeftTime --;
		}
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
			//慢镜头(最后一次)
			GameSpeedService.Instance.SetTmpSpeed (BattleConst.dazhaoAttackTimeScale, BattleConst.dazhaoAttackTimeLength);
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

			if(casterBattleGo.shifaNodeEffect !=null)
			{
				casterBattleGo.shifaNodeEffect.HideEffectWithKey(EffectList.dazhaoPreprare);
			}
			//todo:大招被打断ui提示
			//casterBattleGo.TriggerEvent("",Time.time,null);

			SpellVitalChangeArgs args = new SpellVitalChangeArgs();
            args.vitalType = (int)VitalType.Vital_Type_Interrupt;
			args.triggerTime = Time.time;
			args.casterID = 0;
			args.targetID = casterBattleGo.guid;
			GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.SpellLifeChange, args);
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

		if (DazhaoLeftTime < 1)
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

		BattleCameraAni.MotionToDefault ().OnComplete (OnExitDazhao);
		GameEventMgr.Instance.FireEvent<UIBattle.UiState> (GameEventList.ChangeUIBattleState, UIBattle.UiState.Normal);

		if (null != dazhaoExitCheck)
		{
			Destroy(dazhaoExitCheck);
			dazhaoExitCheck = null;
		}
	}

	void OnExitDazhao()
	{
		GameEventMgr.Instance.FireEvent<BattleObject>(GameEventList.DazhaoActionOver, casterBattleGo);
		GameEventMgr.Instance.FireEvent(GameEventList.HideDazhaoTip);
		StopCoroutine ("LeftTimeCo");
	}


	public void ClearAll()
	{
		if (dazhaoState != DazhaoState.Finished)
		{
			dazhaoState = DazhaoState.Finished;

			if(casterBattleGo.shifaNodeEffect !=null)
			{
				casterBattleGo.shifaNodeEffect.HideEffectWithKey(EffectList.dazhaoPreprare);
			}
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
			return dazhaoLeftTime;
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
