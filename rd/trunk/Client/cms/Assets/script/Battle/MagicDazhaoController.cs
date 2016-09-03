using UnityEngine;
using System.Collections;
using System;

public class MagicDazhaoController : MonoBehaviour 
{
	enum DazhaoState:int
	{
		Shifa =0,
		Wait = 1,
		Prepare,
		Start,
		Finished
	}

	bool isActionDo = false;//大招事件进行了
	BattleProcess.Action magicAction;//法术攻击action
	BattleObject  casterBattleGo;
	Spell dazhaoSpell;
	
	//DazhaoExitCheck  dazhaoExitCheck = null;
	
	DazhaoState dazhaoState = DazhaoState.Finished;

	DazhaofocusController dazhaoFocusController = null;

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
		dazhaoFocusController = DazhaofocusController.Create (true);
	}

	void OnDestroy()
	{
		UnBindListener ();
	}
	
	void BindListener()
	{
		GameEventMgr.Instance.AddListener< int >(GameEventList.ExitDazhaoByPhyAttacked , OnExitByPhyAttacked);
		//GameEventMgr.Instance.AddListener<int > (GameEventList.OverMagicShifaWithResult, OnOverMagicShifaWithResult);
		GameEventMgr.Instance.AddListener (GameEventList.MonsterShowoffOver, OnMonsterShowoffOver);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener< int > (GameEventList.ExitDazhaoByPhyAttacked, OnExitByPhyAttacked);
		//GameEventMgr.Instance.RemoveListener<int > (GameEventList.OverMagicShifaWithResult, OnOverMagicShifaWithResult);
		GameEventMgr.Instance.RemoveListener (GameEventList.MonsterShowoffOver, OnMonsterShowoffOver);
	}

    public void DestroyController()
    {
        ResourceMgr.Instance.DestroyAsset(dazhaoFocusController.gameObject);
    }

	public BattleObject GetCasterBattleObj()
	{
		if (dazhaoState == DazhaoState.Finished) 
		{
			return null;
		}

		return casterBattleGo;
	}

    public void SetDazhaoPrepareState(bool isFinish)
    {
        if (casterBattleGo != null)
        {
            if (isFinish == false)
            {
                casterBattleGo.TriggerEvent("magicDazhaoPrepare", Time.time, null);
            }
            else
            {
                StartCoroutine(DelayFinishPrepare());
            }
        }
    }

    private IEnumerator DelayFinishPrepare()
    {
        yield return new WaitForSeconds(0.3f);
        casterBattleGo.TriggerEvent("magicDazhaoPrepare_Finish", Time.time, null);
    }
	public void PrepareShifa(BattleProcess.Action magicAction)
	{
		this.magicAction = magicAction;
		dazhaoState = DazhaoState.Shifa;
		casterBattleGo = magicAction.caster;

        //蓄气
        //casterBattleGo.ShowDazhaoPrepareEffect ();
        //if(casterBattleGo.shifaNodeEffect !=null)
        //{
        //casterBattleGo.shifaNodeEffect.ShowEffectWithKey(EffectList.dazhaoPreprare);
        //}
        //casterBattleGo.TriggerEvent ("magicDazhaoPrepare", Time.time, null);
        BattleController.Instance.Process.InsertAction(magicAction);

        //显示法阵
        //OpenFazhenUI ();

        ////中断检测
        //dazhaoExitCheck = casterBattleGo.gameObject.GetComponent<DazhaoExitCheck> ();
        //if (null != dazhaoExitCheck)
        //{
        //	Destroy(dazhaoExitCheck);
        //	dazhaoExitCheck = null;
        //}
        //dazhaoExitCheck = casterBattleGo.gameObject.AddComponent<DazhaoExitCheck>();
    }

	public void RunActionWithDazhao(BattleObject casterGo)
	{

		//if(casterBattleGo.shifaNodeEffect !=null)
		//{
		//	casterBattleGo.shifaNodeEffect.HideEffectWithKey(EffectList.dazhaoPreprare);
		//}
		//casterBattleGo.TriggerEvent ("magicDazhaoPrepare_Finish", Time.time, null);
		
		dazhaoSpell = casterBattleGo.unit.GetDazhao ();
		if (dazhaoSpell == null)
		{
			Logger.LogErrorFormat("[SERIOUS]Unit {0}'s dazhao error! No dazhao is configured! Exit dazhao mode!!!", casterBattleGo.guid);
			DazhaoFinished();
			return;
		}
		dazhaoState = DazhaoState.Prepare;

		//隐藏摄像机 
		GameEventMgr.Instance.FireEvent<bool,bool> (GameEventList.SetMirrorModeState, false,false);
		//爆点
		//if (casterGo.shifaNodeEffect != null) 
	//	{
			//casterGo.shifaNodeEffect.ShowEffectWithKey(EffectList.dazhaoReady);
		//}
		casterBattleGo.TriggerEvent ("magicDazhaoReady", Time.time, null);

        StartCoroutine (showOffCo());
	}

	IEnumerator showOffCo()
	{
		yield return new WaitForSeconds (0.0f);

		//if (casterBattleGo.shifaNodeEffect != null) 
		//{
		//	casterBattleGo.shifaNodeEffect.HideEffectWithKey(EffectList.dazhaoReady);
		//}
		//casterBattleGo.TriggerEvent ("magicDazhaoRead_Finish", Time.time, null);

		dazhaoFocusController.ShowoffDazhao (casterBattleGo);
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
		isActionDo = true;
		BattleController.Instance.Process.RunMagicDazhao (magicAction);
		//慢镜头
		//GameSpeedService.Instance.SetTmpSpeed (BattleConst.dazhaoAttackTimeScale, BattleConst.dazhaoAttackTimeLength);
	}

	public void DazhaoAttackFinished(int casterID)
	{
		if (casterBattleGo!=null && casterID == casterBattleGo.guid)
		{
			DazhaoFinished();
		}
	}


	//void OnOverMagicShifaWithResult(int iresult)
	//{
	//	//CloseFazhenUI ();
	//	if (1 == iresult) 
	//	{
	//		//画阵（施法）成功，插入事件
	//		//插入法术事件
	//		BattleController.Instance.Process.InsertAction(magicAction);
	//	}
	//	else 
	//	{
	//		//over
	//		DazhaoFinished();
	//	}
	//}


	public void FinishDazhaoWithAllEnemyDead()
	{
		//CloseFazhenUI ();
		DazhaoFinished ();
	}
	public void FinishDazhaoWithSelfDead()
	{
		//CloseFazhenUI ();
		DazhaoFinished ();
	}
	
	//大招结束
	void DazhaoFinished()
    {
        if (dazhaoState != DazhaoState.Finished)
        {
            //if(casterBattleGo.shifaNodeEffect !=null)
            //{
            //	casterBattleGo.shifaNodeEffect.HideEffectWithKey(EffectList.dazhaoPreprare);
            //}
           // casterBattleGo.TriggerEvent("magicDazhaoPrepare_Finish", Time.time, null);

            dazhaoState = DazhaoState.Finished;

            StartCoroutine(ExitDazhaoCo());
        }
	}

	IEnumerator ExitDazhaoCo()
	{
		yield return new WaitForSeconds (1.0f);

		if (isActionDo)
		{
			isActionDo = false;
			GameEventMgr.Instance.FireEvent<BattleObject> (GameEventList.DazhaoActionOver, casterBattleGo);
		}

		GameEventMgr.Instance.FireEvent(GameEventList.HideDazhaoTip);
		
		//if (null != dazhaoExitCheck)
		//{
		//	Destroy(dazhaoExitCheck);
		//	dazhaoExitCheck = null;
		//}
	}

	private void OnExitByPhyAttacked( int casterID)
	{
		if (dazhaoState == DazhaoState.Finished)
			return;
		if (null == casterBattleGo)
			return;

		if (casterBattleGo.guid != casterID)
		{
			return;
		}
		//CloseFazhenUI ();
		if (dazhaoState == DazhaoState.Prepare || dazhaoState == DazhaoState.Shifa)
		{
			//大招被打断
			//if(casterBattleGo.shifaNodeEffect !=null)
			//{
			//	casterBattleGo.shifaNodeEffect.HideEffectWithKey(EffectList.dazhaoPreprare);
			//}
			//casterBattleGo.TriggerEvent ("magicDazhaoPrepare_Finish", Time.time, null);
			//todo:大招被打断ui提示
			SpellVitalChangeArgs args = new SpellVitalChangeArgs();
            args.vitalType = (int)VitalType.Vital_Type_Interrupt;
			args.triggerTime = Time.time;
			args.casterID = 0;
			args.targetID = casterBattleGo.guid;
			GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.SpellLifeChange, args);

			DazhaoFinished();
			
		}
		//if (null != dazhaoExitCheck)
		//{
		//	Destroy(dazhaoExitCheck);
		//	dazhaoExitCheck = null;
		//}
	}

	public void ClearAll()
	{
		if (dazhaoState == DazhaoState.Finished)
		{
			return;
		}

		dazhaoState = DazhaoState.Finished;
		//CloseFazhenUI ();
		//if (null != dazhaoExitCheck)
		//{
		//	Destroy(dazhaoExitCheck);
		//	dazhaoExitCheck = null;
		//}
		//if(casterBattleGo.shifaNodeEffect !=null)
		//{
		//	casterBattleGo.shifaNodeEffect.HideEffectWithKey(EffectList.dazhaoPreprare);
		//}
		casterBattleGo.TriggerEvent ("magicDazhaoPrepare_Finish", Time.time, null);
	}

	//private void OpenFazhenUI()
	//{
 //       if (UIBattle.Instance != null)
 //       {
 //           UIBattle.Instance.uiFazhen = UIMgr.Instance.OpenUI_(UIFazhen.ViewName) as UIFazhen;
 //       }
	//}

	//private void CloseFazhenUI()
	//{
	//	UIMgr.Instance.CloseUI_(UIFazhen.ViewName);
	//}
}
