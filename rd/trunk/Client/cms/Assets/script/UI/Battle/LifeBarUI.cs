using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class LifeBarUI : MonoBehaviour
{
    public Image currentBarImage;
    public Image targetBarImage;
    RectTransform currentBar;
    RectTransform targetBar;
    float width = 586;
    public float lifeRatioSpeed = 0.3f;
    public float dangerRatio = 0.3f;
    public Color normalColor = Color.white;
    public Color dangerColor = Color.red;

    float targetLife = 1.0f;
    float currentLife = 1.0f;
    //bool isDanger = false;
    //int lifeSpeed = 1000;

    //private List<SpellVitalChangeArgs> vitalEventList;
    private BattleObject lifeTarget;
    public BattleObject LifeTarget
    {
        set 
        {
            if (lifeTarget != value)
            {
                lifeTarget = value;
                //if (vitalEventList != null)
                //    vitalEventList.Clear();
                if (lifeTarget != null)
                {
                    SetTargetLife(lifeTarget.unit.curLife, lifeTarget.unit.maxLife);
                    currentLife = targetLife;
                    currentBar.sizeDelta = targetBar.sizeDelta;
                    //lifeSpeed = (int)(lifeTarget.unit.maxLife * lifeRatioSpeed);
                }
                else
                {
                    var size = targetBar.sizeDelta;
                    size.x = 0;
                    currentBar.sizeDelta = targetBar.sizeDelta = size;
                }
            }
        }
    }

    void Awake()
    {
        currentBar = currentBarImage.transform as RectTransform;
        targetBar = targetBarImage.transform as RectTransform;

        width = currentBar.rect.width;
    }

    void OnDestroy()
    {
        lifeTarget = null;
    }

    void Update()
    {
        if (targetLife != currentLife)
        {
            currentLife -= lifeRatioSpeed * Time.deltaTime;
            if (currentLife < targetLife)
            {
                currentLife = targetLife;
            }

            var size = currentBar.sizeDelta;
            size.x = width * (0.2f + currentLife * 0.8f);
            currentBar.sizeDelta = size;
        }

    }

    public void SetTargetLife(int targetValue, int maxValue)
    {
        targetLife = targetValue / (float)maxValue;
        var size = targetBar.sizeDelta;
        size.x = width * (0.2f + targetLife * 0.8f);
        targetBar.sizeDelta = size;

        //加血直接加
        if (targetLife > currentLife)
        {
            currentLife = targetLife;
            currentBar.sizeDelta = size;
        }

        if (targetLife <= dangerRatio)
        {
            targetBarImage.color = dangerColor;
        }
        else
        {
            targetBarImage.color = normalColor;
        }
    }
}
