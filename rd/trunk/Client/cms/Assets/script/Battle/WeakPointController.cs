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
		GameEventMgr.Instance.AddListener<BattleObject> (GameEventList.LoadBattleObjectFinished, OnLoadEnemyFinished);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener< List<MirrorTarget> > (GameEventList.FindWeakPoint, OnFindWeakPoint);
		GameEventMgr.Instance.RemoveListener< List<MirrorTarget> > (GameEventList.FindFinishedWeakPoint, OnFindFinishedWeakPoint);
		GameEventMgr.Instance.RemoveListener< List<MirrorTarget> > (GameEventList.MirrorOutWeakPoint, OnMirrorOutFromWeakPoint);
        GameEventMgr.Instance.RemoveListener<BattleObject>(GameEventList.LoadBattleObjectFinished, OnLoadEnemyFinished);
	}

	void AddWeakPoint(BattleObject bo)
	{
		List<string> weakList = bo.unit.weakPointList;
		if (null == weakList || weakList.Count < 1)
		{
			return;
		}

		GameObject monsterGo = bo.gameObject;
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

			InitWeakPointCollider(rowData,bo);
            InitWeakPointEffect(rowData, bo);
            InitWeakPointMesh(rowData, bo);

		}

	}

	void  InitWeakPointCollider(WeakPointData rowData,BattleObject bo)
	{
		GameObject monsterGo = bo.gameObject;
		string colliderName = rowData.node;
		
		GameObject colliderGo = Util.FindChildByName(monsterGo,colliderName);
		if(null == colliderGo)
		{
			Debug.LogError("Can't find monster weakCollider " + colliderName);
			return;
		}

		bo.unit.weakPointDumpDic [rowData.id] = colliderGo;
		MirrorTarget mTarget = colliderGo.AddComponent<MirrorTarget>();
		mTarget.WeakPointIDAttr = rowData.id;
		mTarget.isSelf = rowData.isSelf == 1;

		MeshRenderer mr = colliderGo.GetComponentInChildren<MeshRenderer>();
		if(mr!=null)
		{
			mr.enabled = false;
		}

		BoxCollider bc = colliderGo.GetComponent<BoxCollider> ();
		if (bc != null)
		{
			bc.enabled = false;
		}

	}

	void InitWeakPointEffect(WeakPointData rowData,BattleObject bo)
	{
		GameObject monsterGo = bo.gameObject;
		string effectNodeName = rowData.node;
		GameObject effectGo = Util.FindChildByName (monsterGo, effectNodeName);
		if (effectGo != null) 
		{
			bo.unit.weakPointEffectDic[rowData.id] = effectGo;

			if(null == effectGo.GetComponent<FindWeakpointEffect>())
			{
				FindWeakpointEffect fEffect = effectGo.AddComponent<FindWeakpointEffect>();
				if(rowData.isSelf == 1)
				{
					fEffect.Init("effect/battle", "monster_ghost");
				}
				else
				{
					fEffect.Init("effect/battle", "findweakpointEffect");
				}

				
				string prefabName = rowData.asset;
				FindFinishedWeakpointEffect finishEffect = effectGo.AddComponent<FindFinishedWeakpointEffect>();
				finishEffect.Init("effect/battle",prefabName);

				WeakpointFireEffect fireEffect = effectGo.AddComponent<WeakpointFireEffect>();
				fireEffect.Init();
			}
		}
	}

	void InitWeakPointMesh(WeakPointData rowData,BattleObject bo)
	{
		GameObject monsterGo = bo.gameObject;
		string meshNodeName = rowData.mesh;
		GameObject meshGo = Util.FindChildByName (monsterGo, meshNodeName);
		//Debug.LogError ("meshName = " + meshNodeName);
		if (meshGo != null) 
		{
			//Debug.LogError("find meshName initialStatus = " + rowData.initialStatus );
			bo.unit.weakPointMeshDic[rowData.id] = meshGo;
			if(1 != rowData.initialStatus)
			{
				meshGo.SetActive(false);
			}

		}
		string deadMeshName = rowData.deadMesh;
		if (!string.IsNullOrEmpty (deadMeshName)) 
		{
			GameObject deadMeshGo = Util.FindChildByName (monsterGo, rowData.deadMesh);
			if(deadMeshGo != null)
			{
				bo.unit.weakPointDeadMeshDic[rowData.id] = deadMeshGo;
				deadMeshGo.SetActive(false);
			}
		}
	}

	void OnLoadEnemyFinished(BattleObject bo)
	{
		Logger.LogFormat ("On Load Enemy finished!");
		this.AddWeakPoint (bo);
	}

	void OnFindWeakPoint( List<MirrorTarget> newFindList)
	{
		foreach (MirrorTarget findTarget in newFindList) 
		{

			GameUnit curGameUnit = getGameUnit (findTarget);
			string curWeakPointName = findTarget.WeakPointIDAttr;

			bool isFinished = curGameUnit.findWeakPointlist.Contains (curWeakPointName);

			ShowOrHideFindEffect (curGameUnit, curWeakPointName, !isFinished);
			ShowOrHideFindFinishedEffect (curGameUnit, curWeakPointName, isFinished);

			//Debug.LogError ("-----find a new weakPoint ");
		}
	}

	void OnFindFinishedWeakPoint(List<MirrorTarget> finishList)
	{
		foreach (MirrorTarget target in finishList)
		{
			GameUnit unit = getGameUnit (target);
			string wpName = target.WeakPointIDAttr;

			GameObject wpMeshGo = null;
			bool isMeshActived = true;
			if(unit.weakPointMeshDic.TryGetValue(wpName,out wpMeshGo))
			{
				isMeshActived = wpMeshGo.activeSelf;
				wpMeshGo.SetActive(true);
			}

			if(target.isSelf)
			{
				unit.isVisible = true;
                GameEventMgr.Instance.FireEvent<int>(GameEventList.ShowHideMonster, unit.pbUnit.guid);
			}

			if (!unit.findWeakPointlist.Contains (wpName)) 
			{
				unit.findWeakPointlist.Add (wpName);
				//光效果
				if(!target.isSelf || !isMeshActived)
				{
					ShowWeakpointFireEffect(unit, wpName);
				}
			}
		

			ShowOrHideFindEffect (unit, wpName, false);
			if(!target.isSelf )
			{
				ShowOrHideFindFinishedEffect (unit, wpName, true);
			}
		}
	}

	void OnMirrorOutFromWeakPoint(List<MirrorTarget> outList)
	{
		foreach (MirrorTarget target in outList) 
		{
			GameUnit lastGameUnit = getGameUnit (target);
			string lastWeakpointName = target.WeakPointIDAttr;
		
			ShowOrHideFindEffect (lastGameUnit, lastWeakpointName, false);
			ShowOrHideFindFinishedEffect (lastGameUnit, lastWeakpointName, false);
		//	Debug.LogError ("out weakPoint -----------");
		}
	}

	public GameUnit getGameUnit(MirrorTarget target)
	{
		if (null == target)
			return null;
		var battleObject = target.GetComponentInParent<BattleObject> () as BattleObject;
		return battleObject.unit;
	}

	public int GetProperty(MirrorTarget target)
	{
		int property = -1;
		WeakPointData wpData = StaticDataMgr.Instance.GetWeakPointData (target.WeakPointIDAttr);
		if (wpData != null) 
		{
			property = wpData.property;
		}
		if (-1 == property) 
		{
			property = getGameUnit(target).property;
		}
		return property;
	}

	void ShowOrHideFindEffect(GameUnit gUnit,string weakpointID,bool bshow)
	{
		if (null == gUnit) 
			return;

		GameObject effectGo = null;
		if (gUnit.weakPointEffectDic.TryGetValue (weakpointID, out effectGo))
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
		if (null == gUnit) 
			return;

		GameObject effectGo = null;
		if (gUnit.weakPointEffectDic.TryGetValue (weakpointID, out effectGo))
		{
			FindFinishedWeakpointEffect fe = effectGo.GetComponent<FindFinishedWeakpointEffect>();
			if(fe)
			{
				fe.effectObject.SetActive(bshow);
			}
		}
	}

	void ShowWeakpointFireEffect(GameUnit gUnit,string weakpointID)
	{
		GameObject effectGo = gUnit.weakPointEffectDic[weakpointID];
		if (effectGo) 
		{
			WeakpointFireEffect ffe = effectGo.GetComponent<WeakpointFireEffect> ();
			if (ffe)
			{
				ffe.Show();
			}
		}  
	}

	// -----------------------------------------------------------------------
	public List<string> GetAiCanAttackWeakpointList(GameUnit unit)
	{
		List<string> weakPointList = unit.weakPointList;
		List<string> canAttackList = new List<string> ();
		string subWp;
		for (int i = 0; i< weakPointList.Count; ++i)
		{	
			subWp = weakPointList[i];

			WeakPointData wpData = StaticDataMgr.Instance.GetWeakPointData(subWp);
			if(null == wpData )
			{
				continue;
			}
			
			if(wpData.isTarget != 1)
				continue;

			WeakPointRuntimeData tempData = unit.wpHpList[subWp];

			if(tempData.hp < 1)
				continue;

			if(wpData.initialStatus ==1)
			{
				canAttackList.Add(subWp);
			}
			else
			{
				if(unit.findWeakPointlist.Contains(subWp))
				{
					canAttackList.Add(subWp);
				}
			}
		}

		return canAttackList;
	}

	// -----------------------------------------------------------------------
	public List<string> GetFireFocusAttackWeakpointList(GameUnit unit)
	{
		List<string> weakPointList = unit.weakPointList;
		List<string> canAttackList = new List<string> ();
		string subWp;
		for (int i = 0; i< weakPointList.Count; ++i)
		{	
			subWp = weakPointList[i];
			
			WeakPointData wpData = StaticDataMgr.Instance.GetWeakPointData(subWp);
			if(null == wpData || wpData.isSelf == 1)
			{
				continue;
			}
			
			WeakPointRuntimeData tempData = unit.wpHpList[subWp];
			
			if(tempData.hp < 1)
				continue;
			
			if(wpData.initialStatus ==1)
			{
				canAttackList.Add(subWp);
			}
			else
			{
				if(unit.findWeakPointlist.Contains(subWp))
				{
					canAttackList.Add(subWp);
				}
			}
		}
		
		return canAttackList;
	}
}
