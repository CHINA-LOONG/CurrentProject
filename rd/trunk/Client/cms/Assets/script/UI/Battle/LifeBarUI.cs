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
    float  width = 586;
    public float lifeRatioSpeed = 0.3f;
    public float dangerRatio = 0.3f;
    public Color normalColor = Color.white;
    public Color dangerColor = Color.red;
    public Image shieldImage;//盾
    public GameObject shieldEffect;//护盾满格效果

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
        if (shieldImage!=null)
        {
            shieldImage.enabled = false;
            shieldEffect.SetActive(false);
        }        
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
			RefreshShieldUI((int)(currentLife*lifeTarget.unit.maxLife));
        }
    }

    public void RefreshShieldUI(int currentLife = -1)
    {
        if (lifeTarget==null)
            return;
        if (shieldImage == null)
            return;
        if (lifeTarget.unit.spellMagicShield != 0 || lifeTarget.unit.spellPhyShield != 0)
        {
			int curLife = lifeTarget.unit.curLife;;
            if (currentLife != -1)
            {
                curLife = currentLife;
            }
            float shieldNum = 0.0f;
            if (lifeTarget.unit.spellMagicShield > lifeTarget.unit.spellPhyShield)
            {
                shieldNum = 1.0f / lifeTarget.unit.maxLife * (curLife + lifeTarget.unit.spellMagicShield);
            }
            else
            {
                shieldNum = 1.0f / lifeTarget.unit.maxLife * (curLife + lifeTarget.unit.spellPhyShield);
            }
            if (shieldNum>=1)
            {
                shieldNum = 1.0f;
                shieldEffect.SetActive(true);
            }
            else
            {
                shieldEffect.SetActive(false);
            }
            shieldImage.enabled = true;
            shieldImage.transform.localScale = new Vector3(shieldNum, 1.0f, 1.0f);
        }
        else
        {
            shieldImage.enabled = false;
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
        RefreshShieldUI();
    }
}
