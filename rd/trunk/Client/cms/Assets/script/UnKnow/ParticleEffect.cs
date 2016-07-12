using UnityEngine;
using System.Collections;

public class ParticleEffect : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}

	public	static	ParticleEffect	CreateEffect(Transform parent,string prefabName)
	{
		GameObject go = ResourceMgr.Instance.LoadAsset (prefabName);
		if (null == go)
			return null;
		go.name = prefabName;
		go.transform.SetParent (parent);

		go.transform.localScale = Vector3.one;
		go.transform.localPosition = Vector3.zero;

		ParticleEffect pEffect = go.AddComponent<ParticleEffect> ();
		pEffect.Show (false);
		return pEffect; 
	}

	public void Show(bool bshow)
	{
		gameObject.SetActive (bshow);
	}
}
