using UnityEngine;
using System.Collections;

public class ScrollViewContainer : MonoBehaviour {

    public ScrollView scrollView; 


    public void AddElement(GameObject element)
    {
        if (scrollView != null)
        {
            scrollView.AddElement(element);
        }
    }
}
