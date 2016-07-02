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
			subUnit = listEnemy[i];
			if(null == subUnit)
			{
				continue;
			}
			Dictionary<string,GameObject> weakpointDumpDic = subUnit.weakPointDumpDic;

			GameObject subEnemy = null;
			foreach(KeyValuePair<string,GameObject> subWeak in weakpointDumpDic)
			{
				subEnemy = subWeak.Value;
				Vector2	dumpPos = RectTransformUtility.WorldToScreenPoint(BattleCamera.Instance.CameraAttr,subEnemy.transform.position);
				float distane = Vector2.Distance(startPos,dumpPos);
				if(distane < GameConfig.Instance.MirrorRadius)
				{
					MirrorTarget mTarget = subEnemy.GetComponent<MirrorTarget>();
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
		}

		return findTarget;
	}
}
