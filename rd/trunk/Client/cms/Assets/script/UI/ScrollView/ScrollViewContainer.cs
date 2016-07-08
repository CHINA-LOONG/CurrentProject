using UnityEngine;
using System.Collections;

public class ScrollViewContainer : MonoBehaviour {

    public ScrollView scrollView; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddElement(GameObject element)
    {
        if (scrollView != null)
        {
            scrollView.AddElement(element);
        }
    }
}
