using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class InstanceList : UIBase
{
    public static string ViewName = "InstanceList";
    public Dropdown difficultyDropDown;
    public Button dropButton;
    public Text chaptrName;
    public Text getStartText;
    public GameObject boxButton;
    public Image closeImage;
    public GameObject closeButton;

    public RectTransform rootRt;
    private Vector2 oldPosition;


    public ScrollView instanceScrollView;
    public ScrollRect instanceScrollRect;

    private InstanceDifficulty insDifficulty = InstanceDifficulty.Normal;
    private int chapterIndex = 0;

    private List<InstanceEntryRuntimeData> listInstanceRuntime = new List<InstanceEntryRuntimeData>();
    private int allStar = 0;
    private int getStar = 0;

    private List<InstanceItem> listInstanceItemCache = new List<InstanceItem>();

    PB.HSRewardInfo boxReward = null;

    public static void OpenWith(int chapterId)
    {
        InstanceList instanceUi = (InstanceList)UIMgr.Instance.OpenUI_(ViewName);
        instanceUi.RefreshWithChapterId(chapterId);
    }

    public override void Init()
    {

    }
    //删除界面，对子对象的清理操作
    public override void Clean()
    {
        UnBindListener();
    }

    void    BindListener()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.CHAPTER_BOX_C.GetHashCode().ToString(), OnRequestReceivBoxFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.CHAPTER_BOX_S.GetHashCode().ToString(), OnRequestReceivBoxFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnReward);
    }

    void    UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.CHAPTER_BOX_C.GetHashCode().ToString(), OnRequestReceivBoxFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.CHAPTER_BOX_S.GetHashCode().ToString(), OnRequestReceivBoxFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnReward);
    }

    bool isFirst = true;
    void FirstInit ()
    {
        EventTriggerListener.Get(dropButton.gameObject).onClick = OnDropButtonClicked;
        EventTriggerListener.Get(boxButton).onClick = OnboxButtonClicked;
        oldPosition = rootRt.anchoredPosition;
        difficultyDropDown.onValueChanged.AddListener(OnDifficultyValueChanged);
        difficultyDropDown.options.Clear();
        difficultyDropDown.options.Add(new Dropdown.OptionData(StaticDataMgr.Instance.GetTextByID("diffculty_normal")));
        difficultyDropDown.options.Add(new Dropdown.OptionData(StaticDataMgr.Instance.GetTextByID("diffculty_hard")));
        difficultyDropDown.itemText.text = StaticDataMgr.Instance.GetTextByID("diffculty_normal");

        InstanceItem[] szItem = instanceScrollView.GetComponentsInChildren<InstanceItem>();
        for (int i = 0; i < szItem.Length; ++i)
        {
            listInstanceItemCache.Add(szItem[i]);
            szItem[i].gameObject.SetActive(false);
        }
        EventTriggerListener.Get(closeImage.gameObject).onClick = OnClose;
        EventTriggerListener.Get(closeButton).onClick = OnClose;

        BindListener();
    }

    public  static  void    Close()
    {
        UIMgr.Instance.CloseUI_(InstanceList.ViewName);
    }

    void OnClose(GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }

    public  void    RefreshWithChapterId(int chapterIndex, InstanceDifficulty difficulty = InstanceDifficulty.Normal)
    {
        if(isFirst)
        {
            isFirst = false;
            FirstInit();
        }
        listInstanceRuntime.Clear();
        this.insDifficulty = difficulty;
        this.chapterIndex = chapterIndex;

        int selValue = difficultyDropDown.value;
        if(selValue == (int)difficulty)
        {
            UpdateUI();
            return;
        }
        difficultyDropDown.value = (int)difficulty;
    }

    void    UpdateUI(bool animation = true)
    {
        List<InstanceEntryRuntimeData> listData = InstanceMapService.Instance.GetRuntimeInstance(insDifficulty, chapterIndex);
        listInstanceRuntime.Clear();
        if (null != listData)
        {
            listInstanceRuntime.AddRange(listData);
        }

        Chapter chapter = StaticDataMgr.Instance.GetChapterData(chapterIndex);
        if (null != chapter)
        {
            chaptrName.text = StaticDataMgr.Instance.GetTextByID(chapter.name);
        }

        allStar = listInstanceRuntime.Count * 3;
        getStar = 0;

        for (int i = 0; i < listInstanceItemCache.Count; ++i)
        {
            var subItem = listInstanceItemCache[i];

            if (i < listInstanceRuntime.Count)
            {
                var subData = listInstanceRuntime[i];
                getStar += subData.star;
                subItem.gameObject.SetActive(true);
                subItem.RefreshWith(subData);
            }
            else
            {
                subItem.gameObject.SetActive(false);
            }
        }
        getStartText.text = string.Format("{0}/{1}", getStar, allStar);
        
        if(animation)
        {
            Vector2 startPos = oldPosition;
            startPos.x = 0;
            rootRt.anchoredPosition = startPos;
            rootRt.DOAnchorPos(oldPosition, 1.0f);
        }
        else
        {
            rootRt.anchoredPosition = oldPosition;
        }


        ChapterBoxState boxState = InstanceMapService.Instance.GetChapterBoxState(chapterIndex, insDifficulty);
        if(boxState == ChapterBoxState.CanNotReceiv && getStar == allStar)
        {
            //修改为可领取
            InstanceMapService.Instance.SetChapterBoxState(chapterIndex, insDifficulty,ChapterBoxState.CanReceiv);
            boxState = ChapterBoxState.CanReceiv;
        }
        boxButton.gameObject.SetActive(boxState != ChapterBoxState.HasReceiv);
    }

    private void    OnDropButtonClicked(GameObject go)
    {
        Chapter chapter = StaticDataMgr.Instance.GetChapterData(chapterIndex);
        if (null != chapter)
        {
            string dropId = chapter.normalDrop;
            if(insDifficulty == InstanceDifficulty.Hard)
            {
                dropId = chapter.hardDrop;
            }
            DropInfo.OpenWith(dropId);
        }
      
    }

    private void OnboxButtonClicked(GameObject go)
    {
        ChapterBoxState boxState = InstanceMapService.Instance.GetChapterBoxState(chapterIndex, insDifficulty);

        if(boxState == ChapterBoxState.CanReceiv)
        {
            RequestReceivBox();
            return;
        }
        else if (boxState == ChapterBoxState.CanNotReceiv)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("instanceselect_reward_001"), (int)PB.ImType.PROMPT);
        }
    }

    void RequestReceivBox()
    {
        PB.HSChapterBox param = new PB.HSChapterBox();
        param.chapterId = chapterIndex;
        param.difficulty = (int)insDifficulty;

        GameApp.Instance.netManager.SendMessage(PB.code.CHAPTER_BOX_C.GetHashCode(), param);
    }

    void    OnRequestReceivBoxFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = message.GetProtocolBody<PB.HSErrorCode>();
          //  Logger.LogError("RequestShopData Error errorCode: " + error.errCode);
            return;
        }
        PB.HSChapterBoxRet msgRet = message.GetProtocolBody<PB.HSChapterBoxRet>();
        InstanceMapService.Instance.SetChapterBoxState(chapterIndex, insDifficulty, ChapterBoxState.HasReceiv);
        if (boxReward!=null)
        {
            List<PB.HSRewardInfo> listReward = new List<PB.HSRewardInfo>();
            listReward.Add(boxReward);
            OpenBaoxiangResult.OpenWith(listReward);
        }
    }

    void OnReward(ProtocolMessage message )
    {
        boxReward = null;
        PB.HSRewardInfo reward = message.GetProtocolBody<PB.HSRewardInfo>();
        if (reward == null || reward.hsCode != PB.code.CHAPTER_BOX_C.GetHashCode())
            return;
        boxReward = reward;
    }

    private void OnDifficultyValueChanged(int index)
    {
        insDifficulty = (InstanceDifficulty)index;
       if(index == (int)InstanceDifficulty.Hard)
        {
            if(!InstanceMapService.Instance.IsHardChapterOpend(chapterIndex))
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("未开启。。。"), (int)PB.ImType.PROMPT);
                difficultyDropDown.value = 0;
                return;
            }
        }
        UpdateUI(false);
    }
}
