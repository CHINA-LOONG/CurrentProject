using UnityEngine;
using System.Collections;

public class MirrorRaycast : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}


	public void testRayCast(Vector3 startPos)
	{

		Ray ray = BattleCamera.Instance.CameraAttr.ScreenPointToRay (startPos);
	
		RaycastHit rh;
		if (Physics.Raycast (ray, out rh, 100, LayerConst.WeakpointLayer ))
		{
			MirrorTarget target = rh.collider.GetComponent<MirrorTarget>();

			if(target)
			{
				//Debug.LogError("Find Monster Poor ID=" + target.MonsterPoorIDAttr);
				GameEventMgr.Instance.FireEvent<int,string>(GameEventList.FindWeakPoint,1,target.WeakPointIDAttr);
			}
		} 
	}
}
