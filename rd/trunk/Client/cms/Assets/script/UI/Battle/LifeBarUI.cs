using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class LifeBarUI : MonoBehaviour
{
    public Image currentBarImage;
    public Image targetBarImage;
    public Image shieldImage;//盾
    RectTransform currentBar;
    RectTransform targetBar;
    float  width = 586;
    public float lifeRatioSpeed = 0.3f;
    public float dangerRatio = 0.3f;
    //public Color normalColor = Color.white;
    //public Color dangerColor = Color.red;
    //modify:xuelong 2015-8-30 19:02:09
    public Sprite normalSprite;
    public Sprite dangerSprite;

    RectTransform shieldRect;
    float shieldWidth = 0.0f;
    //add xuelong 2015-8-31 09:20:26
    int shieldMax = 0;
    public Image shieldEffect;//护盾满格效果

    public float targetLife = 1.0f;
    float currentLife = 1.0f;
    //bool isDanger = false;
    //int lifeSpeed = 1000;

    //private List<SpellVitalChangeArgs> vitalEventList;
    private BattleObject lifeTarget=new BattleObject();
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
            shieldRect = shieldImage.transform as RectTransform;
            shieldWidth = shieldRect.rect.width;
            shieldImage.gameObject.SetActive(false);
			//shieldEffect.SetActive(false);
            //modify xiaolong 2015-8-30 17:36:57
            shieldEffect.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
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
            //modify:xiaolong 2015-8-27 14:23:05
            //size.x = width * (0.2f + currentLife * 0.8f);
            size.x = width * currentLife;
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
        if (lifeTarget.unit.spellMagicShield > 0 || lifeTarget.unit.spellPhyShield > 0)
        {
			int curLife = lifeTarget.unit.curLife;
            if (currentLife != -1)
            {
                curLife = currentLife;
            }
            var shieldNum = shieldRect.sizeDelta;
            float life = 0.0f;
            if (lifeTarget.unit.spellMagicShield > lifeTarget.unit.spellPhyShield)
            {
                life = width / lifeTarget.unit.maxLife * lifeTarget.unit.spellMagicShield;
                if (targetBar.sizeDelta.x + life > width)
                    shieldNum.x = width + 100;
                else
                    shieldNum.x = targetBar.sizeDelta.x + life;              
            }
            else
            {
                life = width / lifeTarget.unit.maxLife *  lifeTarget.unit.spellPhyShield;
                if (targetBar.sizeDelta.x + life > width)
                    shieldNum.x = width + 100;
                else
                    shieldNum.x = targetBar.sizeDelta.x + life;
            }

            if (shieldNum.x >= shieldWidth)
            {
                shieldNum.x = shieldWidth;

                //shieldEffect.SetActive(true);
                //modify:xiaolong 2015-8-27 14:23:05
                SetShildMaxImage(true);
            }
            else
            {
                //shieldEffect.SetActive(false);
                //modify:xiaolong 2015-8-27 14:23:05
                SetShildMaxImage(false);
            }
            shieldImage.gameObject.SetActive(true);
            shieldRect.sizeDelta = shieldNum;
           
        }
        else
        {
            shieldImage.gameObject.SetActive(false);
			//shieldEffect.SetActive(false);
            //modify:xiaolong 2015-8-27 14:23:05
            SetShildMaxImage(false);
        }
    }
    //add xuelong 2015-8-30 17:37:29
    void SetShildMaxImage(bool isMax)
    {
        if (shieldMax==(isMax?1:-1))
        {
            return;
        }
        float curAlpha = shieldEffect.color.a;
        if (isMax)
        {
            shieldMax = 0;
            curAlpha += Time.deltaTime / 0.5f;
            if (curAlpha>=1.0f)
	        {
                shieldMax=1;
	        }
        }
        else
        {
            shieldMax = 0;
            curAlpha -= Time.deltaTime / 0.5f;
            if (curAlpha <= 0.0f)
            {
                shieldMax = -1;
            }
        }
        curAlpha=Mathf.Clamp01(curAlpha);
        Color color = shieldEffect.color;
        color.a = curAlpha;
        shieldEffect.color = color;
    }

    public void SetTargetLife(int targetValue, int maxValue)
    {
        targetLife = targetValue / (float)maxValue;
        targetLife = Mathf.Clamp01(targetLife);
        var size = targetBar.sizeDelta;
        //modify:xiaolong 2015-8-27 14:22:05
        //size.x = width * (0.2f + targetLife * 0.8f);
        size.x = width * targetLife;
        targetBar.sizeDelta = size;
        //加血直接加
        if (targetLife > currentLife)
        {
            currentLife = targetLife;
            currentBar.sizeDelta = size;
        }

        if (targetLife <= dangerRatio)
        {
            //targetBarImage.color = dangerColor;
            targetBarImage.sprite = dangerSprite;
        }
        else
        {
            //targetBarImage.color = normalColor;
            targetBarImage.sprite = normalSprite;
        }
        RefreshShieldUI();
    }
}
