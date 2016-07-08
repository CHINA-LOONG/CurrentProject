using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeakPointController : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	static WeakPointController instance = null;
	public static WeakPointController Instance
	{
		get
		{
			return instance;
		}
	}

	public	void Init()
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
		GameEventMgr.Instance.AddListener< List<MirrorTarget> >(GameEventList.FindWeakPoint , OnFindWeakPoint);
		GameEventMgr.Instance.AddListener< List<MirrorTarget> >(GameEventList.FindFinishedWeakPoint , OnFindFinishedWeakPoint);
		GameEventMgr.Instance.AddListener< List<MirrorTarget> >(GameEventList.MirrorOutWeakPoint , OnMirrorOutFromWeakPoint);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener< List<MirrorTarget> > (GameEventList.FindWeakPoint, OnFindWeakPoint);
		GameEventMgr.Instance.RemoveListener< List<MirrorTarget> > (GameEventList.FindFinishedWeakPoint, OnFindFinishedWeakPoint);
		GameEventMgr.Instance.RemoveListener< List<MirrorTarget> > (GameEventList.MirrorOutWeakPoint, OnMirrorOutFromWeakPoint);   
	}

	void OnFindWeakPoint( List<MirrorTarget> newFindList)
	{
		foreach (MirrorTarget findTarget in newFindList) 
		{
			GameUnit curGameUnit = findTarget.battleObject.unit;
			string curWeakPointName = findTarget.WeakPointIDAttr;

			WeakPointRuntimeData wpRuntime = null;
			findTarget.battleObject.wpGroup.allWpDic.TryGetValue(curWeakPointName,out wpRuntime);

			if(null == wpRuntime)
				continue;

			bool isFinished = wpRuntime.IsFind;

			if(wpRuntime.findWpEffect != null)
			{
				wpRuntime.findWpEffect.Show(!isFinished);
			}

			if(wpRuntime.appraisalWpStateEffect != null)
			{
				wpRuntime.appraisalWpStateEffect.Show(isFinished);
			}
		}
	}

	void OnFindFinishedWeakPoint(List<MirrorTarget> finishList)
	{
		foreach (MirrorTarget target in finishList)
		{
			BattleObject bo = target.battleObject;
			string wpName = target.WeakPointIDAttr;

			WeakPointRuntimeData  wpRuntime = null;

			if(!bo.wpGroup.allWpDic.TryGetValue(wpName,out wpRuntime))
			{
				continue;
			}

			if(  !wpRuntime.IsFind )
			{
				wpRuntime.IsFind = true;
				if(null != wpRuntime.appraisalWpEffect)
				{
					wpRuntime.appraisalWpEffect.Show(true);
				}
			}

			if(wpRuntime.appraisalWpStateEffect != null)
			{
				wpRuntime.appraisalWpStateEffect.Show(true);
			}

			if(wpRuntime.findWpEffect != null)
			{
				wpRuntime.findWpEffect.Show(false);
			}

			if(WeakpointState.Hide == wpRuntime.wpState)
			{
				wpRuntime.ChangeState(WeakpointState.Normal1);
			}


			bo.unit.isVisible =  true;
		}
	}

	void OnMirrorOutFromWeakPoint(List<MirrorTarget> outList)
	{
		foreach (MirrorTarget target in outList) 
		{
			BattleObject bo = target.battleObject;
			string lastWeakpointName = target.WeakPointIDAttr;

			WeakPointRuntimeData wpRuntime = null;
			if(!bo.wpGroup.allWpDic.TryGetValue(lastWeakpointName,out wpRuntime))
			{
				continue;
			}
		
			if(wpRuntime.findWpEffect != null)
			{
				wpRuntime.findWpEffect.Show(false);
			}
			
			if(wpRuntime.appraisalWpStateEffect != null)
			{
				wpRuntime.appraisalWpStateEffect.Show(false);
			}
		}
	}

}
