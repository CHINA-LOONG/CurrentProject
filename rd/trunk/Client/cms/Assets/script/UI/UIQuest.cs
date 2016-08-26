using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIQuest : UIBase, 
                       TabButtonDelegate,
                       IScrollView
{
    public static string ViewName = "UIQuest";

    enum QuestType
    {
        StoryType,
        DailyType,
        OtherType
    }

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

    private List<questItem> items = new List<questItem>();

    private Dictionary<int, QuestItemInfo> QuestList = new Dictionary<int, QuestItemInfo>();
    private List<QuestItemInfo> StoryList = new List<QuestItemInfo>();
    private List<QuestItemInfo> DailyList = new List<QuestItemInfo>();
    private List<QuestItemInfo> OtherList = new List<QuestItemInfo>();
    private List<QuestItemInfo> CurrentList;

    void Start()
    {
        OnLanguageChanged();
        EventTriggerListener.Get(btn_Close.gameObject).onClick = ClickCloseButton;
    }

    public void Refresh(int select=-1)
    {
        selIndex = (select == -1 ? selIndex : select);

        if (tabIndex!=selIndex)
        {
            TabGroup.OnChangeItem(selIndex);
        }
        else
        {
            ReLoadData(selIndex);
        }
    }

    void ReLoadData(int index,bool record=false)
    {
        switch ((QuestType)index)
        {
            case QuestType.StoryType:
                CurrentList = new List<QuestItemInfo>(StoryList);
                break;
            case QuestType.DailyType:
                CurrentList = new List<QuestItemInfo>(DailyList);
                break;
            case QuestType.OtherType:
                CurrentList = new List<QuestItemInfo>(OtherList);
                break;
            default:
                Logger.LogError("选择类型错误");
                return;
        }
        scrollView.InitContentSize(CurrentList.Count, this, record);
    }

    void RefreshScrollList()
    {
        scrollView.InitContentSize(CurrentList.Count, this);
    }

    public void OnQuestChanged()
    {
        Dictionary<int, QuestData> list = GameDataMgr.Instance.PlayerDataAttr.gameQuestData.questList;
        QuestList.Clear();
        StoryList.Clear();
        DailyList.Clear();
        OtherList.Clear();
        Dictionary<int, QuestItemInfo> groupUsedTime = new Dictionary<int, QuestItemInfo>();
        foreach (KeyValuePair<int, QuestData> item in list)
        {
            QuestItemInfo info = new QuestItemInfo();
            info.serverData = item.Value;
            info.staticData = StaticDataMgr.Instance.GetQuestData(item.Value.questId);

            if (info.staticData == null) continue;
            QuestList.Add(info.serverData.questId, info);

            TimeStaticData beginTime = StaticDataMgr.Instance.GetTimeData(info.staticData.timeBeginId);
            TimeStaticData endTime = StaticDataMgr.Instance.GetTimeData(info.staticData.timeEndId);
            if (beginTime != null && endTime != null)
            {
                if (endTime < GameTimeMgr.Instance.GetServerTime()) continue;//任务已经结束
                //保证每个时间任务组仅且只显示一个任务
                if (groupUsedTime.ContainsKey(info.staticData.group))
                {
                    if (beginTime < StaticDataMgr.Instance.GetTimeData(groupUsedTime[info.staticData.group].staticData.timeBeginId))
                    {
                        groupUsedTime[info.staticData.group] = info;
                    }
                }
                else
                {
                    groupUsedTime.Add(info.staticData.group, info);
                }
                continue;//时间任务不计入其他任务类型
            }

            if ((QuestType)(info.staticData.type - 1) == QuestType.StoryType)
                StoryList.Add(info);
            else if ((QuestType)(info.staticData.type - 1) == QuestType.DailyType)
                DailyList.Add(info);
            else if ((QuestType)(info.staticData.type - 1) == QuestType.OtherType)
                OtherList.Add(info);
        }
        //把包含时间的任务分发到对应的任务类型中去
        foreach (QuestItemInfo item in groupUsedTime.Values)
        {
            if ((QuestType)(item.staticData.type - 1) == QuestType.StoryType)
                StoryList.Add(item);
            else if ((QuestType)(item.staticData.type - 1) == QuestType.DailyType)
                DailyList.Add(item);
            else if ((QuestType)(item.staticData.type - 1) == QuestType.OtherType)
                OtherList.Add(item);
        }
    }
    
    void OnSubmitReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null||msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSQuestSubmitRet result = msg.GetProtocolBody<PB.HSQuestSubmitRet>();
        QuestItemInfo item = null;
        QuestList.TryGetValue(result.questId, out item);
        if (item != null)
        {
            uiQuestInfo = UIQuestInfo.Open(QuestList[result.questId], SubmitStartEvent,SubmitEndEvent);
            CurrentList.Remove(item);
            GameDataMgr.Instance.PlayerDataAttr.gameQuestData.RemoveQuest(result.questId);
            GameEventMgr.Instance.FireEvent(GameEventList.QuestChanged);
        }
    }
    void SubmitStartEvent()
    {
        scrollView.InitContentSize(CurrentList.Count, this,true);
    }
    void SubmitEndEvent(float delay)
    {
        ReLoadData(tabIndex,true);
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

    #region UIBase
    //初始化状态
    public override void Init()
    {
        OnQuestChanged();

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
    #endregion

    #region TabButtonDelegate
    public void OnTabButtonChanged(int index)
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
    public void ReloadData(Transform item, int index)
    {
        questItem quest = item.GetComponent<questItem>();
        quest.ReLoadData(CurrentList[index]);
    }

    public Transform CreateData(Transform parent, int index = 0)
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

    public void CleanData(List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
    }

    void OnClickTodoit(QuestItemInfo quest)
    {
        uiQuestInfo = UIQuestInfo.Open(quest, SubmitStartEvent, SubmitEndEvent);
        CurrentList.Remove(quest);
    }
    void OnClickSubmit(QuestItemInfo quest)
    {
        PB.HSQuestSubmit param = new PB.HSQuestSubmit();
        param.questId = quest.serverData.questId;
        GameApp.Instance.netManager.SendMessage(PB.code.QUEST_SUBMIT_C.GetHashCode(), param);
    }

    #endregion

    public static int SortQuest(QuestItemInfo a, QuestItemInfo b)
    {
        int result = 0;
        if (((a.serverData.progress >= a.staticData.goalCount) && (b.serverData.progress >= b.staticData.goalCount)) ||
            ((a.serverData.progress < a.staticData.goalCount) && (b.serverData.progress < b.staticData.goalCount)))
        {
            if (a.serverData.questId < b.serverData.questId)
            {
                result = -1;
            }
            else
            {
                result = 1;
            }
        }
        else
        {
            if (a.serverData.progress >= a.staticData.goalCount)
            {
                result = -1;
            }
            else
            {
                result = 1;
            }
        }
        return result;
    }
}
