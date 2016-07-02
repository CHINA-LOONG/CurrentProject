using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MirrorRaycast : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}


	public MirrorTarget WeakpointRayCast(Vector2 startPos)
	{
		List<GameUnit> listEnemy = BattleController.Instance.BattleGroup.EnemyFieldList;

		MirrorTarget findTarget = null;
		GameUnit subUnit = null;
		for (int i =0; i< listEnemy.Count; ++i)
		{
			subUnit = listEnemy [i];
			if (null == subUnit) 
			{
				continue;
			}
			MirrorTarget subfindTarget = RaycastAllWeakpoint(subUnit,startPos,GameConfig.Instance.MirrorRadius);
			if(null == subfindTarget)
			{
				continue;
			}

			if(findTarget == null)
			{
				findTarget = subfindTarget;
			}
			else
			{
				if(subfindTarget.DistanceToMirror < findTarget.DistanceToMirror)
				{
					findTarget = subfindTarget;
				}
			}
		}
		return findTarget;
	}

	public static	MirrorTarget RaycastCanAttackWeakpoint( GameUnit gameUnit,Vector2 uiPos,float maxDistance)
	{
		Dictionary<string,GameObject> weakpointDumpDic = new Dictionary<string, GameObject> ();
		List<string> attackWpList = WeakPointController.Instance.GetCanAttackWeakpointList (gameUnit);
		if (null != attackWpList)
		{
			foreach(string subWp in attackWpList)
			{
				weakpointDumpDic.Add(subWp, gameUnit.weakPointDumpDic[subWp]);
			}
		}
		return RaycastWeakpoint (gameUnit, uiPos, maxDistance, weakpointDumpDic);
	}

	public static	MirrorTarget RaycastAllWeakpoint(GameUnit gameUnit, Vector2 uiPos, float maxDistance)
	{
		Dictionary<string,GameObject> weakpointDumpDic = gameUnit.weakPointDumpDic;
		return RaycastWeakpoint (gameUnit, uiPos, maxDistance, weakpointDumpDic);
	}

	private  static	MirrorTarget RaycastWeakpoint(GameUnit gameUnit,Vector2 uiPos,float maxDistance,Dictionary<string,GameObject> weakpointDumpDic)
	{
		MirrorTarget findTarget = null;
		GameObject subWeakpointObj = null;
		foreach(KeyValuePair<string,GameObject> subWeak in weakpointDumpDic)
		{
			WeakPointRuntimeData wpHp = null;
			if(gameUnit.wpHpList.TryGetValue(subWeak.Key, out wpHp))
			{
				if(wpHp.hp < 1)
				{
					continue;
				}
			}

			subWeakpointObj = subWeak.Value;
			Vector2	dumpPos = RectTransformUtility.WorldToScreenPoint(BattleCamera.Instance.CameraAttr,subWeakpointObj.transform.position);
			float distane = Vector2.Distance(uiPos,dumpPos);
			if(distane < maxDistance)
			{
				MirrorTarget mTarget = subWeakpointObj.GetComponent<MirrorTarget>();
				mTarget.DistanceToMirror = distane;
				if(findTarget == null)
				{
					findTarget = mTarget;
				}
				else
				{
					if(mTarget.DistanceToMirror < findTarget.DistanceToMirror)
					{
						findTarget = mTarget;
					}
				}
				
			}
		}
		
		return findTarget;
	}
}
