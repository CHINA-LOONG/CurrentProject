using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScrollView : MonoBehaviour {

    public GridLayoutGroup gridLayout;

	private List<GameObject> listItems = new List<GameObject>();

	// Use this for initialization
	void Start () {
           
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddElement(GameObject go)
    {
        go.transform.SetParent(gridLayout.gameObject.transform, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

		listItems.Add (go);
    }

	public	void ClearAllElement()
	{
		for (int i =0; i < listItems.Count; ++i)
		{
			ResourceMgr.Instance.DestroyAsset(listItems[i]);
		}
		listItems.Clear ();
	}

}
