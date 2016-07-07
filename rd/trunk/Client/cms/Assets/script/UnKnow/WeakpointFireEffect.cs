using UnityEngine;
using System.Collections;

public class WeakpointFireEffect : MonoBehaviour
{

	public	GameObject effectObject;
	public void Init()
	{
		GameObject prefab = ResourceMgr.Instance.LoadAsset ("effect/battle", "baozha_fire");
		effectObject = Instantiate (prefab) as GameObject;
		if (null == effectObject)
		{
			Logger.LogError("can't load weakpointFire");
			return;
		}
		
		effectObject.transform.SetParent (transform);
		effectObject.transform.localPosition = Vector3.zero;
		effectObject.SetActive (false);
	}

	public void Show()
	{
		effectObject.SetActive (true);
	}
}
