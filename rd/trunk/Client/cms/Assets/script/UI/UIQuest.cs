using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class QuestInfo
{
    public QuestData serverData;
    public QuestStaticData staticData;
}

public class UIQuest : UIBase, TabButtonDelegate
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

    //quest list
    public GameObject list_Content;
    public ScrollRect scrollView;
    //not find quest
    public Text text_tips;

    private UIQuestInfo uiQuestInfo;
    public UIQuestInfo UIQuestInfo
    {
        get { return uiQuestInfo; }
    }

    private TabButtonGroup tabGroup;
    private List<QuestItem> items = new List<QuestItem>();

    private Dictionary<int, QuestInfo> QuestList = new Dictionary<int, QuestInfo>();
    private List<QuestInfo> StoryList = new List<QuestInfo>();
    private List<QuestInfo> DailyList = new List<QuestInfo>();
    private List<QuestInfo> OtherList = new List<QuestInfo>();

    private int tabIndex = 0;

    void Start()
    {
        OnLanguageChanged();
        EventTriggerListener.Get(btn_Close.gameObject).onClick = ClickCloseButton;
    }
    //初始化状态
    public override void Init()
    {
        UIMgr.Instance.CloseUI_(UIQuestInfo);
        if (tabGroup==null)
        {
            tabGroup = GetComponentInChildren<TabButtonGroup>();
            tabGroup.InitWithDelegate(this);
        }
        if (tabGroup==null)
        {
            return;
        }
        OnQuestChanged();
        tabGroup.OnChangeItem(0);
    }
    //清理资源缓存
    public override void Clean()
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            items[i].Clean();
            ResourceMgr.Instance.DestroyAsset(items[i].gameObject);
        }
        items.Clear();
        UIMgr.Instance.DestroyUI(UIQuestInfo);
    }

    public void OnQuestChanged()
    {
        Dictionary<int, QuestData> list = GameDataMgr.Instance.PlayerDataAttr.gameQuestData.questList;
        QuestList.Clear();
        StoryList.Clear();
        DailyList.Clear();
        OtherList.Clear();
        Logger.Log("Dictionary<int, QuestData> list:" + list.Count);
        Dictionary<int, QuestInfo> groupUsedTime = new Dictionary<int, QuestInfo>();
        foreach (KeyValuePair<int, QuestData> item in list)
        {
            QuestInfo info = new QuestInfo();
            info.serverData = item.Value;
            info.staticData = StaticDataMgr.Instance.GetQuestData(item.Value.questId);
            if (info.staticData == null) continue;
            QuestList.Add(info.serverData.questId, info);
            TimeStaticData beginTime = StaticDataMgr.Instance.GetTimeData(info.staticData.timeBeginId);
            TimeStaticData endTime = StaticDataMgr.Instance.GetTimeData(info.staticData.timeEndId);
            if (beginTime != null && endTime != null)
            {
                if (endTime < GameTimeMgr.Instance.GetTime()) continue;
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
                continue;
            }

            if ((QuestType)(info.staticData.type - 1) == QuestType.StoryType)
                StoryList.Add(info);
            else if ((QuestType)(info.staticData.type - 1) == QuestType.DailyType)
                DailyList.Add(info);
            else if ((QuestType)(info.staticData.type - 1) == QuestType.OtherType)
                OtherList.Add(info);
        }
        foreach (QuestInfo item in groupUsedTime.Values)
        {
            if ((QuestType)(item.staticData.type - 1) == QuestType.StoryType)
                StoryList.Add(item);
            else if ((QuestType)(item.staticData.type - 1) == QuestType.DailyType)
                DailyList.Add(item);
            else if ((QuestType)(item.staticData.type - 1) == QuestType.OtherType)
                OtherList.Add(item);
        }
        OnTabButtonChanged(tabIndex);
    }

    public void OnTabButtonChanged(int index)
    {
        tabIndex = index;
        List<QuestInfo> list;
        if ((QuestType)index == QuestType.StoryType) list = StoryList;
        else if ((QuestType)index == QuestType.DailyType) list = DailyList;
        else if ((QuestType)index == QuestType.OtherType) list = OtherList;
        else { Logger.Log("error: tabview bug"); return; }

        list.Sort(SortQuest);
        for (int i = 0; i < items.Count; i++)
        {
            if (i >= list.Count) items[i].gameObject.SetActive(false);
            else items[i].gameObject.SetActive(true);
        }

        for (int i = items.Count; i < list.Count; i++)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("questItem");
            if (go != null)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(list_Content.transform, false);
                QuestItem item = go.GetComponent<QuestItem>();
                items.Add(item);
                //TODO:
                LanguageMgr.Instance.SetLanguageFont(go);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            items[i].SetQuest(list[i]);
        }

        scrollView.verticalNormalizedPosition = 1.0f;
        text_tips.gameObject.SetActive(list.Count <= 0);
    }

    void ClickCloseButton(GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void OnSubmitReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        if (msg == null)
            return;
        PB.HSQuestSubmitRet result = msg.GetProtocolBody<PB.HSQuestSubmitRet>();
        QuestInfo item = null;
        QuestList.TryGetValue(result.questId, out item);
        if (item != null)
        {
            uiQuestInfo = UIQuestInfo.Open(QuestList[result.questId]);
            GameDataMgr.Instance.PlayerDataAttr.gameQuestData.RemoveQuest(result.questId);
            OnQuestChanged();
        }
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
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.QUEST_SUBMIT_S.GetHashCode().ToString(), OnSubmitReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.QUEST_SUBMIT_C.GetHashCode().ToString(), OnSubmitReturn);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener(GameEventList.QuestChanged, OnQuestChanged);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.QUEST_SUBMIT_S.GetHashCode().ToString(), OnSubmitReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.QUEST_SUBMIT_C.GetHashCode().ToString(), OnSubmitReturn);
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

    public static int SortQuest(QuestInfo a, QuestInfo b)
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
