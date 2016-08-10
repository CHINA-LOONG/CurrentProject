using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeakPointGroup 
{
	public	Dictionary<string,WeakPointRuntimeData> allWpDic = new Dictionary<string, WeakPointRuntimeData>();

	public	static	WeakPointGroup	CreateWeakpointGroup(BattleObject unit)
	{
		WeakPointGroup wpGroup = new WeakPointGroup ();
		wpGroup.SetBattleObject (unit);
		return wpGroup;
	}
		

	public void SetBattleObject(BattleObject battleObj)
	{
		List<string> weakList = battleObj.unit.weakPointList;
		if (null == weakList || weakList.Count < 1)
		{
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

			WeakPointRuntimeData wpRealData = new WeakPointRuntimeData();
			wpRealData.id = weakID;
			wpRealData.maxHp = rowData.health;
            wpRealData.HpAttr = wpRealData.maxHp;
           wpRealData.wpState = (WeakpointState)rowData.initialStatus;
            if(wpRealData.wpState == WeakpointState.Normal1 ||
                wpRealData.wpState == WeakpointState.Normal2)
            {
                wpRealData.IsFind = true;
            }
            else
            {
                wpRealData.IsFind = false;
            }
			wpRealData.staticData = rowData;
			wpRealData.battleObject = battleObj;

			allWpDic.Add(weakID,wpRealData);

			InitMirrorTarget(wpRealData,battleObj);

			InitWeakPointState(wpRealData, battleObj);

			wpRealData.InitBoxColliders();
			wpRealData.UpdateBoxCollider();
		}
	}

	void  InitMirrorTarget(WeakPointRuntimeData wpRuntimeData,BattleObject bo)
	{
		GameObject monsterGo = bo.gameObject;
		string colliderName = wpRuntimeData.staticData.node;
		
		GameObject colliderGo = Util.FindChildByName(monsterGo,colliderName);
		if(null == colliderGo)
		{
			Logger.LogError("Can't find monster weakCollider " + colliderName);
			return;
		}

		MirrorTarget mTarget = colliderGo.GetComponent<MirrorTarget>();
		if (null == mTarget) 
		{
			mTarget = colliderGo.AddComponent<MirrorTarget>();
		}
		mTarget.WeakPointIDAttr = wpRuntimeData.staticData.id;
		mTarget.battleObject = bo;

		wpRuntimeData.wpMirrorTarget = mTarget;

		//preLoad effect for mirror
		InitWeakPointEffectForMirror (wpRuntimeData, colliderGo);
		
	}
	
	void InitWeakPointEffectForMirror(WeakPointRuntimeData wpData,GameObject effectGo)
	{
		WeakPointData rowData = wpData.staticData;

		wpData.findWpEffect = ParticleEffect.CreateEffect (effectGo.transform, rowData.findWpEffect);
		wpData.appraisalWpStateEffect = ParticleEffect.CreateEffect (effectGo.transform, rowData.appraisalStateEffect);
		wpData.appraisalWpEffect = ParticleEffect.CreateEffect (effectGo.transform, "ruodian_jiandinghou");
		wpData.showoffEffect = ParticleEffect.CreateEffect(effectGo.transform, "ruodian_tishi");//
    }
	//every weakstate have one mesh
	void InitWeakPointState(WeakPointRuntimeData wpRealData,BattleObject bo)
	{
		WeakPointData rowData = wpRealData.staticData;
		GameObject monsterGo = bo.gameObject;
	
		Hashtable ht = null;
		WeakpointState state;
		int stateCount = (int)WeakpointState.Number;
		for (int i =0; i<stateCount; ++i) 
		{
			state = (WeakpointState) i;
			switch(state)
			{
			case WeakpointState.Hide:
				ht = rowData.state0;
				break;
			case WeakpointState.Normal1:
				ht = rowData.state1;
				break;
			case WeakpointState.Normal2:
				ht = rowData.state2;
				break;
			case WeakpointState.Ice:
				ht = rowData.state3;
				break;
			case WeakpointState.Dead:
				ht = rowData.state4;
				break;
			}
			if(null == ht)
				continue;

			//effect
			string effectName = ht["effect"] as string;
			if(!string.IsNullOrEmpty(effectName))
			{
				wpRealData.wpStateEffectDic.Add(state,effectName);
			}

			//mesh
			string meshNodeName = ht["mesh"] as string;
			if(string.IsNullOrEmpty(meshNodeName))
			{
				continue;
			}

			GameObject meshGo = Util.FindChildByName (monsterGo, meshNodeName);
			if(null == meshGo)
			{
				continue;
			}

			meshGo.SetActive(false);

			wpRealData.wpMeshDic.Add(state,meshGo);
		}

        GameObject curStateMesh = wpRealData.GetMeshWithState(wpRealData.wpState);
        if (null != curStateMesh)
        {
            curStateMesh.SetActive(true);
        }
        wpRealData.CheckAndShowAppraisalEffect();
	}


	public	void	ChangeState(string wpid, WeakpointState wpState)
	{
		WeakPointRuntimeData wpRuntime = null;
		if (allWpDic.TryGetValue (wpid, out wpRuntime))
		{
			wpRuntime.ChangeState(wpState);
		}
	}

	public List<string> GetAiCanAttackList()
	{
		List<string> listReturn = new List<string> ();
		foreach (var subItem in allWpDic.Values)
		{
			if(subItem.IsAiCanAttack())
			{
				listReturn.Add(subItem.id);
			}
		}
		return listReturn;
	}

	public	List<string> GetCanFireFocsList()
	{
		List<string> listReturn = new List<string> ();
		foreach (var subItem in allWpDic.Values)
		{
			if(subItem.IsCanFireFocus())
			{
				listReturn.Add(subItem.id);
			}
		}
		return listReturn;
	}

	public List<string> GetCanMirrorList()
	{
		List<string> listReturn = new List<string> ();
		foreach (var subItem in allWpDic.Values)
		{
			if(subItem.IsCanMirror())
			{
				listReturn.Add(subItem.id);
			}
		}
		return listReturn;
	}
}
