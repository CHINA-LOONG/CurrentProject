using UnityEngine;
using System.Collections;

public class MagicDazhaoController : MonoBehaviour 
{
	enum DazhaoState
	{
		Shifa =0,
		Wait = 1,
		Prepare,
		Start,
		Finished
	}

	BattleProcess.Action magicAction;//法术攻击action
	BattleObject  casterBattleGo;
	Spell dazhaoSpell;
	float dazhaoStartTime = 0;
	
	DazhaoExitCheck  dazhaoExitCheck = null;
	
	DazhaoState dazhaoState = DazhaoState.Finished;

	static MagicDazhaoController instance = null;
	public static MagicDazhaoController Instance
	{
		get
		{
			return instance;
		}
	}

	// Use this for initialization
	void Start () 
	{
		instance = this;
		BindListener ();
	}

	void OnDestroy()
	{
		UnBindListener ();
	}
	
	void BindListener()
	{
		GameEventMgr.Instance.AddListener< int >(GameEventList.ExitDazhaoByPhyAttacked , OnExitByPhyAttacked);
		GameEventMgr.Instance.AddListener<int > (GameEventList.OverMagicShifaWithResult, OnOverMagicShifaWithResult);
		GameEventMgr.Instance.AddListener (GameEventList.MonsterShowoffOver, OnMonsterShowoffOver);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener< int > (GameEventList.ExitDazhaoByPhyAttacked, OnExitByPhyAttacked);
		GameEventMgr.Instance.RemoveListener<int > (GameEventList.OverMagicShifaWithResult, OnOverMagicShifaWithResult);
		GameEventMgr.Instance.RemoveListener (GameEventList.MonsterShowoffOver, OnMonsterShowoffOver);
	}

	public BattleObject GetCasterBattleObj()
	{
		if (dazhaoState == DazhaoState.Finished) 
		{
			return null;
		}

		return casterBattleGo;
	}


	public void PrepareShifa(BattleProcess.Action magicAction)
	{
		this.magicAction = magicAction;
		dazhaoState = DazhaoState.Shifa;
		casterBattleGo = magicAction.caster;

		//蓄气
		//casterBattleGo.ShowDazhaoPrepareEffect ();
		if(casterBattleGo.shifaNodeEffect !=null)
		{
			casterBattleGo.shifaNodeEffect.ShowEffectWithKey(EffectList.dazhaoPreprare);
		}

		//显示法阵
		OpenFazhenUI ();

		//中断检测
		dazhaoExitCheck = casterBattleGo.gameObject.GetComponent<DazhaoExitCheck> ();
		if (null != dazhaoExitCheck)
		{
			Destroy(dazhaoExitCheck);
			dazhaoExitCheck = null;
		}
		dazhaoExitCheck = casterBattleGo.gameObject.GetComponent<DazhaoExitCheck>();
	}

	public void RunActionWithDazhao(BattleObject casterGo)
	{
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

		StartCoroutine (PrepareDazhaoCo ());
	}

	IEnumerator PrepareDazhaoCo()
	{
		yield return new WaitForSeconds (1.0f);
		dazhaoState = DazhaoState.Start;
		//大招攻击
		BattleController.Instance.Process.RunMagicDazhao (magicAction);
		//SpellService.Instance.SpellRequest(dazhaoSpell.spellData.id, casterBattleGo.unit, battleGo.unit, Time.time);
	}

	public void DazhaoAttackFinished(int casterID)
	{
		if (casterBattleGo!=null && casterID == casterBattleGo.guid)
		{
			DazhaoFinished();
		}
	}


	void OnOverMagicShifaWithResult(int iresult)
	{
		CloseFazhenUI ();
		if (1 == iresult) 
		{
			//画阵（施法）成功，插入事件
			//插入法术事件
			BattleController.Instance.Process.InsertAction(magicAction);
		}
		else 
		{
			//over
			DazhaoFinished();
		}
	}


	public void FinishDazhaoWithAllEnemyDead()
	{
		CloseFazhenUI ();
		DazhaoFinished ();
	}
	public void FinishDazhaoWithSelfDead()
	{
		CloseFazhenUI ();
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
		yield return new WaitForSeconds (1.0f);
		//Debug.LogError ("大招结束.... attack times: " + dazhaoFinishCount);
		//BattleCamera.Instance.animator.SetBool (BattleCamera.AniControlParam.phyDazhao, false);
		
		GameEventMgr.Instance.FireEvent<BattleObject>(GameEventList.DazhaoActionOver, casterBattleGo);
		GameEventMgr.Instance.FireEvent(GameEventList.HideDazhaoTip);
		GameEventMgr.Instance.FireEvent<UIBattle.UiState> (GameEventList.ChangeUIBattleState, UIBattle.UiState.Normal);
		
		if (null != dazhaoExitCheck)
		{
			Destroy(dazhaoExitCheck);
			dazhaoExitCheck = null;
		}
	}

	private void OnExitByPhyAttacked( int casterID)
	{
		if (dazhaoState == DazhaoState.Finished)
			return;
		if (null == casterBattleGo)
			return;
		CloseFazhenUI ();
		if (casterBattleGo.guid != casterID)
		{
			Logger.LogErrorFormat("ExitDazhao by PhyAttack Error: dazhao castID = {0}, getCasterID = {1}",casterBattleGo.guid,casterID);
			return;
		}
		if (dazhaoState == DazhaoState.Prepare)
		{
			//大招被打断
			//GameEventMgr.Instance.FireEvent(GameEventList.RemoveDazhaoAction);
			
			//casterBattleGo.HideDazhaoPrepareEffect();
			//todo:大招被打断ui提示
			DazhaoFinished();
			
		}
		if (null != dazhaoExitCheck)
		{
			Destroy(dazhaoExitCheck);
			dazhaoExitCheck = null;
		}
	}

	private void OpenFazhenUI()
	{
		UIMgr.Instance.OpenUI (UIFazhen.AssertName, UIFazhen.ViewName);
	}

	private void CloseFazhenUI()
	{
		UIMgr.Instance.CloseUI (UIFazhen.ViewName);
	}
}
