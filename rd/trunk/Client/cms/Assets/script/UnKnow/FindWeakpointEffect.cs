using UnityEngine;
using System.Collections;

public class FindWeakpointEffect : MonoBehaviour 
{

	public	GameObject effectObject;
	public void Init(string assertName,string prefabName)
	{
		GameObject prefab = ResourceMgr.Instance.LoadAsset (assertName,prefabName);//
		effectObject = Instantiate (prefab) as GameObject;
		if (null == effectObject)
		{
			Logger.LogError("can't load findweakpointEffect");
			return;
		}

		effectObject.transform.SetParent (transform);
		effectObject.transform.localPosition = Vector3.zero;
		effectObject.SetActive (false);
	}
}
