using UnityEngine;
using System.Collections;

public class LifeBarUI : MonoBehaviour
{
    public RectTransform bar;
    public float value = 1;

    float width = 586;

    void Start()
    {
        //width = GetComponent<RectTransform>().sizeDelta.x;
    }

    void Update()
    {
        var size = bar.sizeDelta;
        size.x =  width *(0.2f+ value*0.8f);
        bar.sizeDelta = size;
    }
}
