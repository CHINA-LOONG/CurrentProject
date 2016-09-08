using UnityEngine;
using System.Collections;

public class EnegyBarUI : MonoBehaviour
{
    public RectTransform mask;
    //public RectTransform thumb;

    float curValue;
    public float value 
    {
        get { return curValue; }
        set { curValue = value; SetMask(curValue); }
    }

    //float startY = -45;
    //float baseHeight = 5;
    float height = 45;

    //// Use this for initialization
    //void Start()
    //{
    //    startY = thumb.anchoredPosition.y;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    var pos = mask.anchoredPosition;
    //    pos.y = startY - startY * value;
    //    thumb.anchoredPosition = pos;

    //    var size = thumb.sizeDelta;
    //    size.y = baseHeight + height * value;
    //    mask.sizeDelta = size;
    //}

    void Awake()
    {
        height = mask.sizeDelta.y;
    }
    void SetMask(float value)
    {
        Vector2 size = mask.sizeDelta;
        size.y = height * (1 - value);
        mask.sizeDelta = size;
    }
}
