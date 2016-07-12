using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class QuestInfo
{
    public QuestData serverData;
    public QuestStaticData staticData;
}

public class UIQuestList : MonoBehaviour, TabButtonDelegate
{
    enum QuestType
    {
        StoryType,
        DailyType,
        OtherType
    }

    // quest tab
    public Text text_story;
    public Text text_daily;
    public Text text_other;

    //quest list
    public GameObject list_Content;
    public ScrollRect scrollView;
    //not find quest
    public Text text_tips;

    private List<questItem> quests = new List<questItem>();

    private Dictionary<int, QuestInfo> QuestList = new Dictionary<int, QuestInfo>();
    private List<QuestInfo> StoryList = new List<QuestInfo>();
    private List<QuestInfo> DailyList = new List<QuestInfo>();
    private List<QuestInfo> OtherList = new List<QuestInfo>();

    private int tabIndex = 0;
    // Use this for initialization
    void Start()
    {
        //TODO:
        OnLanguageChanged();
        GetComponentInChildren<TabButtonGroup>().InitWithDelegate(this);
        //OnTabButtonChanged(tabIndex);
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
        for (int i = 0; i < quests.Count; i++)
        {
            if (i >= list.Count) quests[i].gameObject.SetActive(false);
            else quests[i].gameObject.SetActive(true);
        }

        for (int i = quests.Count; i < list.Count; i++)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("questItem");
            if (go != null)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(list_Content.transform, false);
                questItem item = go.GetComponent<questItem>();
                quests.Add(item);

                LanguageMgr.Instance.SetLanguageFont(go);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            quests[i].SetQuest(list[i]);
        }
        if (list.Count <= 0) { text_tips.gameObject.SetActive(true); return; }
        else { text_tips.gameObject.SetActive(false); }
        scrollView.verticalNormalizedPosition = 1.0f;
    }

    void OnEnable()
    {
        Logger.Log("OnEnable");
        OnQuestChanged();
        BindListener();
    }

    void OnDisable()
    {
        Logger.Log("OnDisable");
        UnBindListener();
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener(GameEventList.QuestChanged, OnQuestChanged);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.QUEST_SUBMIT_S.GetHashCode().ToString(), OnSubmitReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.QUEST_SUBMIT_C.GetHashCode().ToString(), OnSubmitError);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener(GameEventList.QuestChanged, OnQuestChanged);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.QUEST_SUBMIT_S.GetHashCode().ToString(), OnSubmitReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.QUEST_SUBMIT_C.GetHashCode().ToString(), OnSubmitError);
    }

    void OnSubmitReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null)
            return;
        PB.HSQuestSubmitRet result = msg.GetProtocolBody<PB.HSQuestSubmitRet>();
        QuestInfo item=null;
        QuestList.TryGetValue(result.questId, out item);
        if (item != null)
        {
            UIQuestInfo.Open(QuestList[result.questId]);
            GameDataMgr.Instance.PlayerDataAttr.gameQuestData.RemoveQuest(result.questId);
            Logger.Log("任务数量："+GameDataMgr.Instance.PlayerDataAttr.gameQuestData.questList.Count);
            OnQuestChanged();
        }
    }
    void OnSubmitError(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
    }

    void OnQuestChanged()
    {

#if XL_DEBUG
        
        #region XL:quest test

        Dictionary<int, QuestData> list = new Dictionary<int, QuestData>();
        list.Add(10001, new QuestData() { questId = 10001, progress = 0 });
        list.Add(20011, new QuestData() { questId = 20011, progress = 2 });
        list.Add(10002, new QuestData() { questId = 10002, progress = 1 });
        list.Add(30008, new QuestData() { questId = 30008, progress = 0 });
        list.Add(10003, new QuestData() { questId = 10003, progress = 0 });
        list.Add(30012, new QuestData() { questId = 30012, progress = 2 });
        list.Add(20005, new QuestData() { questId = 20005, progress = 2 });
        #endregion
#else
        Dictionary<int,QuestData> list= GameDataMgr.Instance.PlayerDataAttr.gameQuestData.questList;
#endif
        QuestList.Clear();
        StoryList.Clear();
        DailyList.Clear();
        OtherList.Clear();
        Logger.Log("Dictionary<int, QuestData> list:" + list.Count);
        Dictionary<int, QuestInfo> groupUsedTime = new Dictionary<int, QuestInfo>();
        foreach (KeyValuePair<int,QuestData> item in list)
        {
            QuestInfo info = new QuestInfo();
            info.serverData = item.Value;
            info.staticData = StaticDataMgr.Instance.GetQuestData(item.Value.questId);
            if (info.staticData == null)  continue;
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

    void OnLanguageChanged()
    {
        text_tips.text = StaticDataMgr.Instance.GetTextByID("tip_NotFoundQuest");
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
            if (a.serverData.progress>=a.staticData.goalCount)
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
