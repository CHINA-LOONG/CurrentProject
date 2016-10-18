using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class InstanceMap : UIBase
{
    public static string ViewName = "InstanceMap";
    public RectTransform scrollViewRt;
    public RectTransform scrollContentRt;

    public ScrollRect mapScrollRect;
    public float scrollLeftValue = -0.2f;
    public float scrollRightValue = 1.5f;

    public RectTransform chapterButtonCankao;
    public Button backButton;
    public Button addHuoliButton;
    public Text huoliText;

    public static InstanceMap Instance;
    private float scrollMaxValueEqualOne = 1.0f;

    private Dictionary<int, ChapterButton> allChapterButtonDic = new Dictionary<int, ChapterButton>();
    private InstanceList mInstanceList = null;

    public  static  void    Open()
    {
        UIMgr.Instance.OpenUI_(ViewName);
    }
    public static void OpenMapAndInstanceList(int chapter,InstanceDifficulty diffType= InstanceDifficulty.Normal)
    {
        InstanceMap insMap = (InstanceMap)UIMgr.Instance.OpenUI_(ViewName);
        insMap.OpenInstanceList(chapter, diffType);
        insMap.FocusOnChapter(chapter);
    }

    bool isFirst = true;
    public override void Init()
    {
        if(isFirst)
        {
            isFirst = false;
            FirstInit();
        }
        RefreshChapterButtons();
        OnHuoliChanged(GameDataMgr.Instance.PlayerDataAttr.HuoliAttr);
        AudioSystemMgr.Instance.PlayMusicByName("Instancemusic");
    }
    public override void RefreshOnPreviousUIHide()
    {
        base.RefreshOnPreviousUIHide();
        RefreshChapterButtons();
        OnHuoliChanged(GameDataMgr.Instance.PlayerDataAttr.HuoliAttr);
    }
    public override void Clean()
    {
        UIMgr.Instance.DestroyUI(mInstanceList);
    }

    void    OnEnable()
    {
        BindListener();
    }
    void OnDisable()
    {
        UnBindListener();
        UIBuild uiBuild = UIMgr.Instance.GetUI(UIBuild.ViewName) as UIBuild;
        if (uiBuild != null && (uiBuild.uiAdjustBattleTeam != null && uiBuild.uiAdjustBattleTeam.gameObject.activeSelf))
        {
            AudioSystemMgr.Instance.PlayMusicByName("Entermusic");
        }
        else
        {
            AudioSystemMgr.Instance.PlayMusicByName("Homemusic");
        }
    }
    void BindListener()
    {
        GameEventMgr.Instance.AddListener<int>(GameEventList.OpenNewChapter, OnOpenNewChapter);
        //GameEventMgr.Instance.AddListener<string>(GameEventList.ShowInstanceList, OnShowInstanceList);
        GameEventMgr.Instance.AddListener<int>(GameEventList.HuoliChanged, OnHuoliChanged);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.OpenNewChapter, OnOpenNewChapter);
      //  GameEventMgr.Instance.RemoveListener<string>(GameEventList.ShowInstanceList, OnShowInstanceList);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.HuoliChanged, OnHuoliChanged);
    }

    public void OpenInstanceList(int chapterIndex,InstanceDifficulty diffType = InstanceDifficulty.Normal)
    {
        mInstanceList = InstanceList.OpenWith(chapterIndex, diffType);
    }
    private void FirstInit()
    {
        chapterButtonCankao.gameObject.SetActive(false);
        mapScrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
        EventTriggerListener.Get(backButton.gameObject).onClick = OnBackButtonClicked;
        EventTriggerListener.Get(addHuoliButton.gameObject).onClick = OnHuoliButtonClicked;
        Instance = this;

        scrollMaxValueEqualOne = scrollContentRt.sizeDelta.x - scrollViewRt.rect.width;
        InitChapterButtonDic();
    }

   private  void InitChapterButtonDic()
    {
        ChapterButton[] szChapter = scrollContentRt.GetComponentsInChildren<ChapterButton>();
        for(int i =0;i<szChapter.Length;++i)
        {
            var subBtn = szChapter[i];
            allChapterButtonDic.Add(subBtn.chapterIndex, subBtn);
        }
    }

    private void RefreshChapterButtons()
    {
        Dictionary<int, Chapter> allChapter = StaticDataMgr.Instance.GetAllStaticChapter();

        foreach(var subChapter in allChapter)
        {
            ChapterButton subBtn = null;
            if(allChapterButtonDic.TryGetValue(subChapter.Key,out subBtn))
            {
                subBtn.chapterName.text = StaticDataMgr.Instance.GetTextByID(subChapter.Value.name);
                bool isOpen = InstanceMapService.Instance.IsChapterOpened(subChapter.Key);
                subBtn.SetLock(!isOpen);
            }
        }
    }

    void OnOpenNewChapter(int newChapter)
    {
        RefreshChapterButtons();
    }
    
    public  void OpenInstanceList(string instanceId)
    {
        if (GameDataMgr.Instance.curInstanceType != (int)InstanceType.Normal)
            return;

        InstanceEntryRuntimeData subInstance = InstanceMapService.Instance.GetRuntimeInstance(instanceId);
        if (null == subInstance)
            return;
        int chapterIndex = subInstance.staticData.chapter;
        if (InstanceMapService.Instance.IsChapterOpened(chapterIndex))
        {
            InstanceMap.Instance.OpenInstanceList(chapterIndex);
        }
    }
    

    void OnHuoliChanged(int newHuoli)
    {
        huoliText.text = string.Format("{0}/{1}", newHuoli, GameDataMgr.Instance.PlayerDataAttr.MaxHuoliAttr);
    }

    void    OnScrollRectValueChanged(Vector2 value)
    {
       
        if(value.x < scrollLeftValue)
        {
            value.x = scrollLeftValue;
        }
        if(value.x > scrollRightValue)
        {
            value.x = scrollRightValue;
        }
        mapScrollRect.normalizedPosition = value;
    }

    void    OnBackButtonClicked(GameObject go)
    {
        InstanceList.Close();
        UIMgr.Instance.CloseUI_(this);
    }

    void OnHuoliButtonClicked(GameObject go)
    {
        UseHuoLi.Open();
    }

    public  void    OnChapterButtonOnClicked(ChapterButton chapterButton)
    {
        Vector2 chapterButtonPos = (chapterButton.transform as RectTransform).anchoredPosition + scrollContentRt.anchoredPosition;
        Vector2 cankaoPos = chapterButtonCankao.anchoredPosition;

        float deltha = (chapterButtonPos.x - cankaoPos.x);
        deltha /= scrollMaxValueEqualOne;

        if(Mathf.Abs(deltha) > 0.005)
        {
            Vector2 scrollPosValue = mapScrollRect.normalizedPosition;
            scrollPosValue.x += deltha;
            DOTween.To(GetScrollNormlizePosition, SetScrollNormlizePosition, scrollPosValue.x, 1.0f);
        }
    }

    public void FocusOnChapterButton(string instanceID)
    {
        InstanceEntryRuntimeData subInstance = InstanceMapService.Instance.GetRuntimeInstance(instanceID);
        if (null == subInstance)
            return;
        FocusOnChapter(subInstance.staticData.chapter);
    }

    public void FocusOnChapter(int chapter)
    {
        ChapterButton cb;
        if (allChapterButtonDic.TryGetValue(chapter, out cb) == true)
        {
            OnChapterButtonOnClicked(cb);
        }
    }

    float GetScrollNormlizePosition()
    {
        return mapScrollRect.normalizedPosition.x;
    }

    void SetScrollNormlizePosition(float value)
    {
        Vector2 pos = mapScrollRect.normalizedPosition;
        pos.x = value;
       mapScrollRect.normalizedPosition = pos;
    }


    public void ReOpenCurrentInstance(string curInstanceID)
    {
        InstanceEntryRuntimeData data = InstanceMapService.Instance.GetRuntimeInstance(curInstanceID);
        OpenInstanceInternal(data);
    }

    public void OpenNextInstance(string curInstanceID)
    {
        //TODO:duplicate code
        InstanceEntryRuntimeData data = InstanceMapService.Instance.GetNextRuntimeInstance(curInstanceID);
        OpenInstanceInternal(data);
    }

    private void OpenInstanceInternal(InstanceEntryRuntimeData data)
    {
        //TODO:duplicate code
        if (data != null)
        {
            UIAdjustBattleTeam.OpenWith(data.instanceId, data.star,true);
            /*
            UIAdjustBattleTeam adjustUi = UIMgr.Instance.OpenUI_(UIAdjustBattleTeam.ViewName) as UIAdjustBattleTeam;
            UIBuild uiBuild = UIMgr.Instance.GetUI(UIBuild.ViewName) as UIBuild;
            if (uiBuild != null)
            {
                uiBuild.uiAdjustBattleTeam = adjustUi;
            }
            adjustUi.SetData(data.instanceId, data.star);
            */
        }
    }

    //IEnumerator ScrollRectAnimationCo(Vector2 normalizedPos)
    //{
    //    yield break;
    //}
    
}
