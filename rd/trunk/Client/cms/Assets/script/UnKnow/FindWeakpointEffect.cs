using UnityEngine;
using System.Collections;

public class FindWeakpointEffect : MonoBehaviour 
{

	public	GameObject effectObject;
	public void Init()
	{
		GameObject prefab = ResourceMgr.Instance.LoadAsset ("effect/battle", "findweakpointEffect");
		effectObject = Instantiate (prefab) as GameObject;
		if (null == effectObject)
		{
			Debug.LogError("can't load findweakpointEffect");
			return;
		}

		effectObject.transform.SetParent (transform);
		effectObject.transform.position = Vector3.zero;
		effectObject.SetActive (false);
	}
}
