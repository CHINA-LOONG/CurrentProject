using UnityEngine;
using System.Collections;

public class MirrorRaycast : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public	void	testRayCast(Vector3 startPos)
	{

		Ray ray = BattleCamera.Instance.CameraAttr.ScreenPointToRay (startPos);
	
		RaycastHit rh;
		if (Physics.Raycast (ray, out rh))
		{
			MirrorTarget target = rh.collider.GetComponent<MirrorTarget>();

			if(target)
			{
				Debug.LogError("Find Monster Poor ID=" + target.MonsterPoorIDAttr);
			}
		} 
	}
}
