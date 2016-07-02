using UnityEngine;
using System.Collections;

public class FindFinishedWeakpointEffect : MonoBehaviour 
{

	public GameObject effectObject;

	public void Init(string assertName,string prefabName)
	{
		GameObject prefab = ResourceMgr.Instance.LoadAsset (assertName, prefabName);
		effectObject = Instantiate (prefab) as GameObject;
		if (null == effectObject)
			return;
		
		effectObject.transform.SetParent (transform);
		effectObject.transform.localPosition = Vector3.zero;
		effectObject.SetActive (false);
	}
}
