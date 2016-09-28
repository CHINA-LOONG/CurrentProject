using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MirrorRaycast : MonoBehaviour 
{
	public List<MirrorTarget> WeakpointRayCast(Vector3 startPosInScreen)
	{
		List<MirrorTarget> returnList = new List<MirrorTarget> ();
		List<BattleObject> listEnemy = BattleController.Instance.BattleGroup.EnemyFieldList;

		BattleObject subUnit = null;
		for (int i =0; i< listEnemy.Count; ++i)
		{
			subUnit = listEnemy [i];
			if (null == subUnit) 
			{
				continue;
			}
			MirrorTarget bestTarget = null;
			List<MirrorTarget> listFind = RaycastWeakPoint(subUnit,startPosInScreen,GameConfig.Instance.MirrorRadius*UIMgr.Instance.CanvasAttr.scaleFactor,out bestTarget);
			returnList.AddRange(listFind);
		}
		return returnList;
	}

	private  static	List<MirrorTarget> RaycastWeakPoint(BattleObject bo,Vector3 startPosInScreen,float maxDistance,out MirrorTarget bestTarget)
	{
		List<MirrorTarget> allFindTarget  = new List<MirrorTarget>();
		bestTarget = null;
		GameObject subWeakpointObj = null;

		foreach(KeyValuePair<string,WeakPointRuntimeData> subWeak in bo.wpGroup.allWpDic)
		{
			WeakPointRuntimeData wpRuntimeData = subWeak.Value;
			if(!wpRuntimeData.IsCanMirror())
			{
				continue;
			}
			subWeakpointObj = wpRuntimeData.wpMirrorTarget.gameObject;
			MirrorTarget mTarget = wpRuntimeData.wpMirrorTarget;


			Vector2	dumpPos = RectTransformUtility.WorldToScreenPoint(BattleCamera.Instance.CameraAttr,subWeakpointObj.transform.position);
			float distane = Vector2.Distance(startPosInScreen,dumpPos);
			if(distane < maxDistance)
			{
				
				mTarget.DistanceToMirror = distane;
				if(bestTarget == null)
				{
					bestTarget = mTarget;
				}
				else
				{
					if(mTarget.DistanceToMirror < bestTarget.DistanceToMirror)
					{
						bestTarget = mTarget;
					}
				}
				allFindTarget.Add(mTarget);
			}
		}
		 
		return allFindTarget;
	}
}
