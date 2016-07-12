using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public	class WeakpointTarget:MonoBehaviour
{
	void Start()
	{
	}

	public	BattleObject battleObject = null;
	public	string wpId = null;
}

public	enum WeakpointState:int
{
	Hide = 0,
	Normal1 ,
	Normal2 ,
	Ice,
	Dead ,
	Number
}

public class WeakPointRuntimeData
{
	public BattleObject battleObject;
	public string id;
	public int maxHp;
	public int hp;
	public	WeakpointState	wpState;
	public	bool	IsFind;//是否被鉴定

	public float damageRate
	{
		get
		{
			if(staticData == null)
				return 0;

			if(wpState == WeakpointState.Normal1)
				return staticData.damageRate1;

			if(wpState == WeakpointState.Normal2)
				return staticData.damageRate2;

			return 0;
		}
	}

	public int property
	{
		get
		{
			int tempProperty = -1;

			if(wpState == WeakpointState.Normal1)
				tempProperty = staticData.property1;

			if(wpState == WeakpointState.Normal2)
				tempProperty =  staticData.property2;

			if(-1 == tempProperty)
			{
				tempProperty = battleObject.unit.property;
			}
			return tempProperty;
		}
	}

	public  MirrorTarget wpMirrorTarget;//
	public	Dictionary<WeakpointState,GameObject> wpMeshDic = new Dictionary<WeakpointState, GameObject>();
	public  Dictionary<WeakpointState,string> wpStateEffectDic = new Dictionary<WeakpointState, string> ();
	// 照妖镜鉴定效果
	public	ParticleEffect	findWpEffect = null;// 
	public	ParticleEffect	appraisalWpStateEffect = null;
	public	ParticleEffect	appraisalWpEffect = null;

	//boxCollider
	public List<WeakpointTarget>  listBoxCollider = new List<WeakpointTarget> ();

	public	WeakPointData	staticData;

	public void ChangeState(WeakpointState state)
	{
		WeakpointState lastWpstate = wpState;
		wpState = state;

		GameObject lastMesh = GetMeshWithState (lastWpstate);
		if (lastMesh != null)
		{
			lastMesh.SetActive(false);
		}

		GameObject curMesh = GetMeshWithState (wpState);
		if (curMesh != null) 
		{
			curMesh.SetActive(true);
		}

		string effectId = "";
		wpStateEffectDic.TryGetValue(wpState,out effectId);
		if(!string.IsNullOrEmpty(effectId))
		{
			battleObject.TriggerEvent(effectId,Time.time,null);
		}

		if (lastWpstate == WeakpointState.Dead) 
		{
			hp = maxHp;
			IsFind = false;
		}
		if (lastWpstate == WeakpointState.Normal1 &&
			wpState == WeakpointState.Normal2) 
		{
			IsFind = false;
		}
		if (lastWpstate == WeakpointState.Normal2 &&
			wpState == WeakpointState.Normal1)
		{
			IsFind = false;
		}
		//检测是否取消集火
		string fireFocuWpName = BattleController.Instance.Process.fireAttackWpName;
		if (!string.IsNullOrEmpty (fireFocuWpName))
		{
			if(fireFocuWpName.EndsWith(id))
			{
				BattleController.Instance.Process.HideFireFocus();
			}
		}
		UpdateBoxCollider ();
	}

	public GameObject GetMeshWithState(WeakpointState state)
	{
		GameObject go = null;
		wpMeshDic.TryGetValue (state, out go);
		return go;
	}

	public bool IsAiCanAttack()
	{
		if (wpState == WeakpointState.Dead ||
			wpState == WeakpointState.Hide ||
			wpState == WeakpointState.Ice) 
		{
			return false;
		}

		if(staticData.isTarget == 1)
			return true;

		return false;

	}

	public bool IsCanMirror()
	{
		if (wpState == WeakpointState.Hide ||
		    wpState == WeakpointState.Normal1 ||
		    wpState == WeakpointState.Normal2) 
		{
			return true;
		}
		
		return false;
	}

	public bool IsCanFireFocus()
	{
		if (wpState == WeakpointState.Dead ||
			wpState == WeakpointState.Hide ||
			wpState == WeakpointState.Ice)
		{
			return false;
		}
		return true;
	}

	public void InitBoxColliders()
	{
		if (!battleObject.unit.isBoss)
			return;

		if (string.IsNullOrEmpty (staticData.boxColliders))
			return;

		ArrayList colliders = MiniJsonExtensions.arrayListFromJson (staticData.boxColliders) as ArrayList;
		if (null == colliders)
			return;
		listBoxCollider.Clear ();

		foreach (string subCollider in colliders) 
		{
			GameObject boxGo = Util.FindChildByName(battleObject.gameObject,subCollider);
			if(null == boxGo)
				continue;
			WeakpointTarget wpTarget = boxGo.GetComponent<WeakpointTarget>();
			if(null == wpTarget)
			{
				wpTarget = boxGo.AddComponent<WeakpointTarget>();
			}
			wpTarget.wpId = id;
			wpTarget.battleObject = battleObject;

			var bc = boxGo.GetComponent<BoxCollider>();
			if(null == bc)
				boxGo.AddComponent<BoxCollider>();
		}
	}

	public void UpdateBoxCollider()
	{
		foreach (var wpTarget in listBoxCollider) 
		{
			if(IsCanFireFocus())
			{
				wpTarget.gameObject.SetActive(true);
			}
			else
			{
				wpTarget.gameObject.SetActive(false);
			}
		}
	}
}

public class WeakPointDeadArgs : EventArgs
{
	public float triggerTime;
	public int targetID;
	public string wpID;
}