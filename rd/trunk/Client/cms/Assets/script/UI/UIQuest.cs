using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIQuest : UIBase,
                       TabButtonDelegate,
                       IScrollView
{
    public static string ViewName = "UIQuest";

    public Button btn_Close;
    public Text text_Title;

    // quest tab
    public Text text_story;
    public Text text_daily;
    public Text text_other;

    public FixCountScrollView scrollView;
    //not find quest
    //public Text text_tips;

    public Animator animator;

    private int tabIndex = -1;
    private int selIndex = 0;
    public int TabIndex
    {
        get { return tabIndex; }
    }

    private UIQuestInfo uiQuestInfo;

    private TabButtonGroup tabGroup;
    public TabButtonGroup TabGroup
    {
        get
        {
            if (tabGroup == null)
            {
                tabGroup = GetComponentInChildren<TabButtonGroup>();
                tabGroup.InitWithDelegate(this);
            }
            return tabGroup;
        }
    }

    public List<HomeButton> tabControl = new List<HomeButton>();

    private List<QuestData> CurrentList;
    

    void Start()
    {
        OnLanguageChanged();
        EventTriggerListener.Get(btn_Close.gameObject).onClick = ClickCloseButton;
    }

    public void Refresh(int select = -1)
    {
        selIndex = (select == -1 ? selIndex : select);

        #region 控制开放等级

        tabControl[0].gameObject.SetActive(UIUtil.CheckIsStoryQuestOpened());
        tabControl[2].gameObject.SetActive(UIUtil.CheckIsDailyQuestOpened());
        if (selIndex == 2 && !UIUtil.CheckIsDailyQuestOpened())
        {
            selIndex = 0;
        }
        if (selIndex == 0 && !UIUtil.CheckIsStoryQuestOpened())
        {
            selIndex = 1;
        }
        #endregion

        if (tabIndex != selIndex)
        {
            TabGroup.OnChangeItem(selIndex);
        }
        else
        {
            ReLoadData(selIndex);
        }
    }

    void ReLoadData(int index, bool record = false)
    {
        GameQuestData questData = GameDataMgr.Instance.PlayerDataAttr.gameQuestData;
        tabControl[0].ShowTip = questData.StoryFinish;
        tabControl[1].ShowTip = questData.DailyFinish;
        tabControl[2].ShowTip = questData.OtherFinish;

        switch ((QuestType)(index + 1))
        {
            case QuestType.StoryType:
                CurrentList = new List<QuestData>(questData.StoryList);
                break;
            case QuestType.DailyType:
                CurrentList = new List<QuestData>(questData.DailyList);
                break;
            case QuestType.OtherType:
                CurrentList = new List<QuestData>(questData.OtherList);
                break;
            default:
                Logger.LogError("选择类型错误");
                return;
        }
        CurrentList.Sort();
        scrollView.InitContentSize(CurrentList.Count, this, record);
    }

    public void OnQuestChanged()
    {
        ReLoadData(tabIndex, true);
    }

    void OnSubmitReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSQuestSubmitRet result = msg.GetProtocolBody<PB.HSQuestSubmitRet>();

        QuestData quest = GameDataMgr.Instance.PlayerDataAttr.gameQuestData.GetQuestById(result.questId);
        if (quest != null)
        {
            System.Action startEvent = () =>
            {
                CurrentList.Remove(quest);
                scrollView.InitContentSize(CurrentList.Count, this, true);
            };
            System.Action<float> endEvent = (time) =>
            {
                GameEventMgr.Instance.AddListener(GameEventList.QuestChanged, OnQuestChanged);
                int questId = quest.questId;
                GameDataMgr.Instance.PlayerDataAttr.gameQuestData.QuestRemove(new List<int>() { quest.questId });
                CheckPlayerData();
            };
            GameEventMgr.Instance.RemoveListener(GameEventList.QuestChanged, OnQuestChanged);
            uiQuestInfo = UIQuestInfo.Open(quest,playerLevel, startEvent, endEvent);
        }
    }

    void ClickCloseButton(GameObject go)
    {
        if (animator != null)
        {
            animator.SetTrigger("TriggerOut");
        }
        else
        {
            EndAnimOver();
        }
    }
    void EndAnimOver()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void OnEnable()
    {
        BindListener();
    }
    void OnDisable()
    {
        UnBindListener();
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener(GameEventList.QuestChanged, OnQuestChanged);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.QUEST_SUBMIT_C.GetHashCode().ToString(), OnSubmitReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.QUEST_SUBMIT_S.GetHashCode().ToString(), OnSubmitReturn);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener(GameEventList.QuestChanged, OnQuestChanged);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.QUEST_SUBMIT_C.GetHashCode().ToString(), OnSubmitReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.QUEST_SUBMIT_S.GetHashCode().ToString(), OnSubmitReturn);
    }
    void OnLanguageChanged()
    {
        //TODO: change language
        text_Title.text = StaticDataMgr.Instance.GetTextByID("quest_title");

        //text_tips.text = StaticDataMgr.Instance.GetTextByID("tip_not_found_quest");
        text_story.text = StaticDataMgr.Instance.GetTextByID("quest_juqingrenwu");
        text_daily.text = StaticDataMgr.Instance.GetTextByID("quest_richangrenwu");
        text_other.text = StaticDataMgr.Instance.GetTextByID("quest_liezhuanrenwu");
    }

    #region 检测玩家等级疲劳值变化

    private int playerLevel;
    private int playerFatigue;

    void SavePlayerData()
    {
        playerLevel = GameDataMgr.Instance.PlayerDataAttr.LevelAttr;
        playerFatigue = GameDataMgr.Instance.PlayerDataAttr.HuoliAttr;
    }

    void CheckPlayerData()
    {
        if (playerLevel != GameDataMgr.Instance.PlayerDataAttr.LevelAttr)
        {
            LevelUp.OpenWith(playerLevel,
                             GameDataMgr.Instance.PlayerDataAttr.LevelAttr,
                             playerFatigue,
                             GameDataMgr.Instance.PlayerDataAttr.HuoliAttr);
        }
    }

    #endregion

    #region UIBase
    //初始化状态
    public override void Init()
    {
        tabIndex = -1;
        selIndex = 0;
        if (animator != null)
        {
            animator.SetTrigger("TriggerIn");
        }
    }
    //清理资源缓存
    public override void Clean()
    {
        UIMgr.Instance.DestroyUI(uiQuestInfo);
    }

    public override void RefreshOnPreviousUIHide()
    {
        Refresh();
    }
    #endregion

    #region TabButtonDelegate
    public void OnTabButtonChanged(int index, TabButtonGroup tab)
    {
        if (tabIndex==index)
        {
            return;
        }
        selIndex = index;
        tabIndex = selIndex;
        ReLoadData(tabIndex);
    }
    #endregion

    #region IScrollView
    public void IScrollViewReloadItem(FixCountScrollView scrollView, Transform item, int index)
    {
        questItem quest = item.GetComponent<questItem>();
        quest.ReLoadData(CurrentList[index]);
    }
    public Transform IScrollViewCreateItem(FixCountScrollView scrollView, Transform parent)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("questItem");
        if (go!=null)
        {
            questItem quest = go.GetComponent<questItem>();
            quest.onClickTodoit = OnClickTodoit;
            quest.onClickSubmit = OnClickSubmit;
            UIUtil.SetParentReset(go.transform, parent);
            return go.transform;
        }
        return null;
    }
    public void IScrollViewCleanItem(FixCountScrollView scrollView, List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
    }

    void OnClickTodoit(QuestData quest)
    {
        //用于测试领取奖励
        //if (quest != null)
        //{
        //    System.Action startEvent = () =>
        //    {
        //        scrollView.InitContentSize(CurrentList.Count, this, true);
        //    };
        //    System.Action<float> endEvent = (time) =>
        //    {
        //        int questId = quest.questId;
        //        GameDataMgr.Instance.PlayerDataAttr.gameQuestData.QuestRemove(new List<int>() { quest.questId });
        //    };
        //    uiQuestInfo = UIQuestInfo.Open(quest, startEvent, endEvent);
        //    CurrentList.Remove(quest);
        //}
    }
    void OnClickSubmit(QuestData quest)
    {
        PB.HSQuestSubmit param = new PB.HSQuestSubmit();
        param.questId = quest.questId;
        GameApp.Instance.netManager.SendMessage(PB.code.QUEST_SUBMIT_C.GetHashCode(), param);
        SavePlayerData();
    }

    #endregion
}
