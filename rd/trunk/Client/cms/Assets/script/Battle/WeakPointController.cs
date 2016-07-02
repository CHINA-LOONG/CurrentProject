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
		GameEventMgr.Instance.AddListener<int ,string>(GameEventList.FindWeakPoint , OnFindWeakPoint);
		GameEventMgr.Instance.AddListener<GameUnit> (GameEventList.LoadBattleObjectFinished, OnLoadEnemyFinished);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<int,string> (GameEventList.FindWeakPoint, OnFindWeakPoint);
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
			WeakPointData.RowData rowData = StaticDataMgr.Instance.WeakPointData.getRowDataWithID(weakID);
			if(null == rowData)
			{
				continue;
			}

			string colliderName = rowData.collider;

			GameObject colliderGo = Util.FindChildByName(monsterGo,colliderName);
			if(null == colliderGo)
			{
				Debug.LogError("Can't find monster weakCollider " + colliderName);
				continue;
			}
		
			MirrorTarget mTarget = colliderGo.AddComponent<MirrorTarget>();
			mTarget.WeakPointIDAttr = weakID;
			colliderGo.layer = LayerConst.WeakpointLayer;
			BoxCollider bc = colliderGo.AddComponent<BoxCollider>();
		}

	}

	void OnLoadEnemyFinished(GameUnit gu)
	{
		this.AddWeakPoint (gu);
	}

	void OnFindWeakPoint(int slotIndex,string wpID)
	{
		Debug.LogError ("Find weakpoint slot index = " + slotIndex + "   wpid = " + wpID);
	}
}
