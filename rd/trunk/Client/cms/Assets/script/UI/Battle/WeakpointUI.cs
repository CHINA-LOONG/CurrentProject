using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WeakpointUI : MonoBehaviour
{
    public RectTransform contentParent;
    public List<WpIcon> wpIconList;

    public RectTransform tipsRectTrans;
    public Text wpNameText;
    public Text tipsText;

    public Ease outEaseAni;
    public float outTime = 0.4f;
    public Ease comEaseani;
    public float comeTime = 0.4f;

    private WeakPointGroup wpGroup;

    private bool isPrepareChangeBatch = false;
    private float changeBatchAtTime = 0;
    private Vector2 contentParetnOldPosition;
   
	// Use this for initialization
	void Start ()
    {
        HideAllWpIcon();
        contentParetnOldPosition = contentParent.anchoredPosition;
        Vector2 initPos = new Vector2(contentParetnOldPosition.x - 300, contentParetnOldPosition.y);
        contentParent.anchoredPosition = initPos;
    }

    void HideAllWpIcon()
    {
        foreach (var subWpIcon in wpIconList)
        {
            subWpIcon.gameObject.SetActive(false);
        }
    }

    void    OnEnable()
    {
        GameEventMgr.Instance.AddListener<WeakPointRuntimeData, int>(GameEventList.RefreshWpProgress, UpdateArmorProgress);
    }

    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<WeakPointRuntimeData, int>(GameEventList.RefreshWpProgress, UpdateArmorProgress);
    }
    public  void    ChangeBatch(float delayTime)
    {
        isPrepareChangeBatch = true;
        changeBatchAtTime = Time.time + delayTime;
    }

    void LateUpdate()
    {
       if(isPrepareChangeBatch)
        {
            if(Time.time > changeBatchAtTime)
            {
                isPrepareChangeBatch = false;
                RefreshWithAni(wpGroup);
            }
        }
    }

    private  void    RefreshUI(WeakPointGroup wpGroup)
    {
        this.wpGroup = wpGroup;
        WeakPointRuntimeData subRealData = null;
        int iconCount = 0;
        HideAllWpIcon();
        foreach (var subWpData in wpGroup.allWpDic)
        {
            subRealData = subWpData.Value;
            if(subRealData.wpState != WeakpointState.Ice)
            {
                var subIcon = wpIconList[iconCount];
                subIcon.gameObject.SetActive(true);
                subIcon.RefreshWithWp(subRealData);
                iconCount++;
            }
        }
    }

     public void RefreshWithAni(WeakPointGroup wpGroup)
    {
        this.wpGroup = wpGroup;
        StopAllCoroutines();
        StartCoroutine(PageOutAnimationCo());
        StartCoroutine(PageComeAnimationCo());
    }

    IEnumerator PageOutAnimationCo()
    {
        Vector2 outPos = new Vector2(contentParetnOldPosition.x - 500, contentParetnOldPosition.y);
        contentParent.DOAnchorPos(outPos, outTime).SetEase(outEaseAni);
        yield return new WaitForSeconds(outTime);
        RefreshUI(wpGroup);
    }

    IEnumerator PageComeAnimationCo()
    {
        contentParent.DOAnchorPos(contentParetnOldPosition, comeTime).SetEase(comEaseani);
        yield break;
    }

    public  void    UpdateWeakpointIcon(WeakPointRuntimeData wpRealData)
    {
        foreach(var subIcon in wpIconList)
        {
            if (string.IsNullOrEmpty(subIcon.GetWpId()))
                continue;
            if(subIcon.GetWpId().Equals(wpRealData.id))
            {
                subIcon.RefreshWithWp(wpRealData);
                break;
            }
        }
    }

    void    UpdateArmorProgress(WeakPointRuntimeData wpRealData,int oldHp)
    {
        foreach (var subIcon in wpIconList)
        {
            if (string.IsNullOrEmpty(subIcon.GetWpId()))
                continue;
            if (subIcon.GetWpId().Equals(wpRealData.id))
            {
                subIcon.RefreshProgress(oldHp);
                break;
            }
        }
    }

    public void ShowTips(WpIcon icon,string tips,string wpName)
    {
        tipsRectTrans.gameObject.SetActive(true);
        tipsText.text = tips;
        wpNameText.text = wpName;
        RectTransform iconRt = icon.transform as RectTransform;
        Vector3 iconPosition = UIUtil.GetSpacePos(iconRt, UIMgr.Instance.CanvasAttr, UICamera.Instance.CameraAttr);
        float fscale = UIMgr.Instance.CanvasAttr.scaleFactor;

        tipsRectTrans.anchoredPosition = new Vector2(iconPosition.x / fscale + 75 , iconPosition.y / fscale - 10);
    }

    public void HideTips()
    {
        tipsRectTrans.gameObject.SetActive(false);
        
    }

	public void ShowOffIcon(string wpId,bool isShow)
    {
        foreach (var subIcon in wpIconList)
        {
            if (string.IsNullOrEmpty(subIcon.GetWpId()))
                continue;
            if (subIcon.GetWpId().Equals(wpId))
            {
                subIcon.ShowoffIcon(isShow);
                break;
            }
        }
    }
}
