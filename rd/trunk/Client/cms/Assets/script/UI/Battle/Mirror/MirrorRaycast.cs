using UnityEngine;
using System.Collections;

public class MirrorRaycast : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}


	public MirrorTarget WeakpointRayCast(Vector3 startPos)
	{
		Vector3 [] rayArray = new Vector3[5];
		float radius = GameConfig.Instance.MirrorRadius;
		rayArray [0] = new Vector3 (startPos.x, startPos.y, startPos.z);;
		rayArray [1] = new Vector3 (startPos.x + radius, startPos.y, startPos.z);
		rayArray [2] = new Vector3 (startPos.x - radius , startPos.y, startPos.z);
		rayArray [3] = new Vector3 (startPos.x, startPos.y + radius, startPos.z);
		rayArray [4] = new Vector3 (startPos.x, startPos.y - radius, startPos.z);

		//for (int i = 0; i< rayArray.Length; ++i) 
		//{
		Ray ray = BattleCamera.Instance.CameraAttr.ScreenPointToRay (startPos);
			
			RaycastHit rh;
			if (Physics.Raycast (ray, out rh, 100, LayerConst.WeakpointLayerMask))
			{
				MirrorTarget target = rh.collider.GetComponent<MirrorTarget>();
				
				return target;
				
			} 
		//}


		return null;
	}
}
