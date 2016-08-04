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
	public	WeakpointState wpState;
	public	bool	IsFind;//是否被鉴定

	public float damageRate
	{
        get
        {
            if (staticData == null)
                return 0;

            //if(wpState == WeakpointState.Normal1)
            //return staticData.damageRate1;

            if (wpState == WeakpointState.Normal2)
                return staticData.damageRate2;

            return staticData.damageRate1;

            //return 0;
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
    
	public  Dictionary<WeakpointState,string> wpStateEffectDic = new Dictionary<WeakpointState, string> ();
	public	WeakPointData	staticData;

	public void ChangeState(WeakpointState state)
	{
		WeakpointState lastWpstate = wpState;
		wpState = state;
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
}

public class WeakPointDeadArgs : EventArgs
{
	public float triggerTime;
	public int targetID;
	public string wpID;
}