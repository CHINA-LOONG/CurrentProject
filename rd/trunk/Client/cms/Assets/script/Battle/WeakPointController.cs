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
	static WeakPointController Instance
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
		GameEventMgr.Instance.AddListener<MirrorTarget,MirrorTarget>(GameEventList.FindWeakPoint , OnFindWeakPoint);
		GameEventMgr.Instance.AddListener<MirrorTarget>(GameEventList.FindFinishedWeakPoint , OnFindFinishedWeakPoint);
		GameEventMgr.Instance.AddListener<MirrorTarget>(GameEventList.MirrorOutWeakPoint , OnMirrorOutFromWeakPoint);
		GameEventMgr.Instance.AddListener<GameUnit> (GameEventList.LoadBattleObjectFinished, OnLoadEnemyFinished);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<MirrorTarget,MirrorTarget> (GameEventList.FindWeakPoint, OnFindWeakPoint);
		GameEventMgr.Instance.RemoveListener<MirrorTarget> (GameEventList.FindFinishedWeakPoint, OnFindFinishedWeakPoint);
		GameEventMgr.Instance.RemoveListener<MirrorTarget> (GameEventList.MirrorOutWeakPoint, OnMirrorOutFromWeakPoint);
		GameEventMgr.Instance.RemoveListener<GameUnit> (GameEventList.LoadBattleObjectFinished, OnLoadEnemyFinished);
	}

	void AddWeakPoint(GameUnit gu)
	{
		List<string> weakList = gu.weakPointList;
		if (null == weakList || weakList.Count < 1)
		{
			return;
		}

		GameObject monsterGo = gu.gameObject;
		MirrorTarget weakpoint = monsterGo.GetComponent<MirrorTarget> ();
		if (null != weakpoint) 
		{
			//已经挂接了脚本
			return;
		}
		string weakID;
		for (int i =0; i<weakList.Count; ++i) 
		{
			weakID = weakList[i];
			Logger.LogFormat("weakpointid = {0}",weakID);
			WeakPointData rowData = StaticDataMgr.Instance.GetWeakPointData(weakID);
			if(null == rowData)
			{
				continue;
			}

			InitWeakPointCollider(rowData,gu);
			InitWeakPointEffect(rowData,gu);
			InitWeakPointMesh(rowData,gu);

		}

	}

	void  InitWeakPointCollider(WeakPointData rowData,GameUnit gu)
	{
		GameObject monsterGo = gu.gameObject;
		string colliderName = rowData.collider;
		
		GameObject colliderGo = Util.FindChildByName(monsterGo,colliderName);
		if(null == colliderGo)
		{
			Debug.LogError("Can't find monster weakCollider " + colliderName);
			return;
		}
		
		MirrorTarget mTarget = colliderGo.AddComponent<MirrorTarget>();
		mTarget.WeakPointIDAttr = rowData.id;
		colliderGo.layer = LayerConst.WeakpointLayer;
		if(null == 	colliderGo.GetComponent<BoxCollider>())
		{
			BoxCollider bc = colliderGo.AddComponent<BoxCollider>();
		}
		
		MeshRenderer mr = colliderGo.GetComponentInChildren<MeshRenderer>();
		if(mr!=null)
		{
			//mr.enabled = false;
		}
	}

	void InitWeakPointEffect(WeakPointData rowData,GameUnit gu)
	{
		GameObject monsterGo = gu.gameObject;
		string effectNodeName = rowData.node;
		GameObject effectGo = Util.FindChildByName (monsterGo, effectNodeName);
		if (effectGo != null) 
		{
			gu.weakPointEffectDic[rowData.id] = effectGo;

			if(null == effectGo.GetComponent<FindWeakpointEffect>())
			{
				FindWeakpointEffect fEffect = effectGo.AddComponent<FindWeakpointEffect>();
				fEffect.Init();
				
				string prefabName = rowData.asset;
				FindFinishedWeakpointEffect finishEffect = effectGo.AddComponent<FindFinishedWeakpointEffect>();
				finishEffect.Init("effect/battle",prefabName);
			}
		}
	}

	void InitWeakPointMesh(WeakPointData rowData,GameUnit gu)
	{
		GameObject monsterGo = gu.gameObject;
		string meshNodeName = rowData.mesh;
		GameObject meshGo = Util.FindChildByName (monsterGo, meshNodeName);
		//Debug.LogError ("meshName = " + meshNodeName);
		if (meshGo != null) 
		{
			//Debug.LogError("find meshName initialStatus = " + rowData.initialStatus );
			gu.weakPointEffectDic[rowData.id] = meshGo;
			if(1 != rowData.initialStatus)
			{
				meshGo.SetActive(false);
			}
		}
	}

	void OnLoadEnemyFinished(GameUnit gu)
	{
		//return;
		Logger.LogFormat ("On Load Enemy finished!");
		this.AddWeakPoint (gu);
	}

	void OnFindWeakPoint(MirrorTarget findTarget,MirrorTarget lastTarget)
	{
		//Debug.LogError("weakpoint  find");
		if (null != lastTarget)
		{
			GameUnit lastGameUnit = getGameUnit(lastTarget);
			string lastWeakpointName = lastTarget.WeakPointIDAttr;

			ShowOrHideFindEffect(lastGameUnit,lastWeakpointName,false);
			ShowOrHideFindFinishedEffect(lastGameUnit,lastWeakpointName,false);
		}

		GameUnit curGameUnit = getGameUnit(findTarget);
		string curWeakPointName = findTarget.WeakPointIDAttr;
		
		ShowOrHideFindEffect(curGameUnit,curWeakPointName,true);
		ShowOrHideFindFinishedEffect(curGameUnit,curWeakPointName,false);

		Debug.LogError ("-----find a new weakPoint ");
	}

	void OnFindFinishedWeakPoint(MirrorTarget target)
	{
		GameUnit lastGameUnit = getGameUnit(target);
		string lastWeakpointName = target.WeakPointIDAttr;
		
		ShowOrHideFindEffect(lastGameUnit,lastWeakpointName,false);
		ShowOrHideFindFinishedEffect(lastGameUnit,lastWeakpointName,true);
		Debug.LogError ("finish ----------- weakPoint ");
	}

	void OnMirrorOutFromWeakPoint(MirrorTarget target)
	{
		GameUnit lastGameUnit = getGameUnit(target);
		string lastWeakpointName = target.WeakPointIDAttr;
		
		ShowOrHideFindEffect(lastGameUnit,lastWeakpointName,false);
		ShowOrHideFindFinishedEffect(lastGameUnit,lastWeakpointName,false);
		Debug.LogError ("out weakPoint -----------");
	}

	GameUnit getGameUnit(MirrorTarget target)
	{
		var battleObject = target.GetComponentInParent<BattleObject> () as BattleObject;
		return battleObject.unit;
	}

	void ShowOrHideFindEffect(GameUnit gUnit,string weakpointID,bool bshow)
	{
		GameObject effectGo = gUnit.weakPointEffectDic[weakpointID];
		if(effectGo)
		{
			FindWeakpointEffect fe = effectGo.GetComponent<FindWeakpointEffect>();
			if(fe)
			{
				fe.effectObject.SetActive(bshow);
			}
		}
	}

	void ShowOrHideFindFinishedEffect(GameUnit gUnit,string weakpointID,bool bshow)
	{
		GameObject effectGo = gUnit.weakPointEffectDic[weakpointID];
		if (effectGo) 
		{
			FindFinishedWeakpointEffect ffe = effectGo.GetComponent<FindFinishedWeakpointEffect> ();
			if (ffe)
			{
				ffe.effectObject.SetActive (bshow);
			}
		} 
		else 
		{
			Debug.LogError("Error to find finished effectobj with  weakpointID " + weakpointID);
		}
	}
}
