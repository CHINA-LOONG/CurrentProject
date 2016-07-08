using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollView : MonoBehaviour {

    public GridLayoutGroup gridLayout;

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
    }

}
