using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LifeBarUI : MonoBehaviour
{
    public RectTransform bar;
    public float value = 1;

    float width = 586;

    private List<SpellVitalChangeArgs> vitalEventList;
    private BattleObject lifeTarget;
    public BattleObject LifeTarget
    {
        set 
        {
            if (lifeTarget != value)
            {
                lifeTarget = value;
                if (vitalEventList != null)
                    vitalEventList.Clear();
            }
        }
    }

    void Start()
    {
        //width = GetComponent<RectTransform>().sizeDelta.x;
        //TODO: use record manager to record event
        vitalEventList = new List<SpellVitalChangeArgs>();
        vitalEventList.Clear();
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpellLifeChange, OnLifeChange);
    }

    void OnDestroy()
    {
        lifeTarget = null;
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpellLifeChange, OnLifeChange);
    }

    void Update()
    {
        var size = bar.sizeDelta;
        size.x =  width *(0.2f+ value*0.8f);
        bar.sizeDelta = size;

        for (int i = vitalEventList.Count - 1; i >= 0; --i)
        {
            if (vitalEventList[i].triggerTime <= Time.time)
            {
                value = vitalEventList[i].vitalCurrent/(float)vitalEventList[i].vitalMax;
                vitalEventList.RemoveAt(i);
            }
        }
    }

    public void SetTargetLife(int targetValue, int maxValue)
    {
        value = targetValue / maxValue;
    }

    public void OnLifeChange(EventArgs args)
    {
        SpellVitalChangeArgs vitalArgs = args as SpellVitalChangeArgs;
        if (lifeTarget != null && vitalArgs.targetID == lifeTarget.guid)
        {
            vitalEventList.Add(vitalArgs);
        }
    }
}
