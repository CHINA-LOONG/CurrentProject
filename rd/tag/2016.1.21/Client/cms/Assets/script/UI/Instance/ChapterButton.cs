﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ChapterButton : MonoBehaviour
{
    public int chapterIndex = 0;
    public float touchScale = 1.15f;

    public Image chapterImage;
    public Text chapterName;
    public Material lockedMaterial;

    private RectTransform rectTrans;
   
	// Use this for initialization
	void Start ()
    {
        rectTrans = transform as RectTransform;

        EventTriggerListener.Get(this.gameObject).onDown = OnTouchEnter;
        EventTriggerListener.Get(this.gameObject).onEnter = OnTouchEnter;
        EventTriggerListener.Get(this.gameObject).onExit = OnTouchExit;
        EventTriggerListener.Get(this.gameObject).onUp = OnTouchExit;
        EventTriggerListener.Get(this.gameObject).onClick = OnClicked;
	}

    void    OnTouchEnter(GameObject go)
    {
        rectTrans.localScale = new Vector3(touchScale, touchScale, touchScale);
    }

    void    OnTouchExit(GameObject go)
    {
        rectTrans.localScale = new Vector3(1, 1, 1);
    }

    void OnClicked(GameObject go)
    {
        InstanceMap.Instance.OnChapterButtonOnClicked(this);
        if(InstanceMapService.Instance.IsChapterOpened(chapterIndex))
        {
            InstanceMap.Instance.OpenInstanceList(chapterIndex);
            return;
        }

        bool isLastFinish = InstanceMapService.Instance.IsChapterFinished(chapterIndex - 1,InstanceDifficulty.Normal);
        if(!isLastFinish)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("instanceselect_open_001"), (int)PB.ImType.PROMPT);
            return;
        }

        Chapter chapter = StaticDataMgr.Instance.GetChapterData(chapterIndex);
        if(null != chapter)
        {
            UIIm.Instance.ShowSystemHints(string.Format(StaticDataMgr.Instance.GetTextByID("instanceselect_open_002"),chapter.normalLevel), (int)PB.ImType.PROMPT);
        }
    }

    public void SetLock(bool isLocked)
    {
        if(isLocked)
        {
            chapterImage.material = lockedMaterial;
        }
        else
        {
            chapterImage.material = null;
        } 
    }
}
