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
    //not find quest
    public Text text_tips;

    private List<questItem> quests = new List<questItem>();

    private List<QuestInfo> StoryList = new List<QuestInfo>();
    private List<QuestInfo> DailyList = new List<QuestInfo>();
    private List<QuestInfo> OtherList = new List<QuestInfo>();

    // Use this for initialization
    void Start()
    {
        //TODO:
        OnLanguageChanged();

        GetComponentInChildren<TabButtonGroup>().InitWithDelegate(this);
        OnTabButtonChanged(0);
    }

    public void OnTabButtonChanged(int index)
    {
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
            GameObject go = ResourceMgr.Instance.LoadAsset("ui/quest", "questItem");
            if (go != null)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(list_Content.transform, false);
                questItem item = go.GetComponent<questItem>();
                quests.Add(item);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            quests[i].SetQuest(list[i]);
        }
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
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener(GameEventList.QuestChanged, OnQuestChanged);
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
        QuestInfo group4 = null;
        Logger.Log("Dictionary<int, QuestData> list:" + list.Count);
        foreach (KeyValuePair<int,QuestData> item in list)
        {
            QuestInfo info = new QuestInfo();
            info.serverData = item.Value;
            info.staticData = StaticDataMgr.Instance.GetQuestData(item.Value.questId);
            if (info.staticData == null)  continue;
            if (info.staticData.group==4)
            {
                TimeStaticData beginTime = StaticDataMgr.Instance.GetTimeData(info.staticData.timeBeginId);
                TimeStaticData endTime = StaticDataMgr.Instance.GetTimeData(info.staticData.timeEndId);
                Logger.Log(beginTime + "      " + endTime);
                if (beginTime == null || endTime == null) continue;
                if (endTime < GameTimeMgr.Instance.GetTime())
                    continue;
                if (group4 != null)
                {
                    if (beginTime < StaticDataMgr.Instance.GetTimeData(group4.staticData.timeBeginId))
                    {
                        group4 = info;
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    group4 = info; 
                    continue;
                }
            }

            if ((QuestType)(info.staticData.type - 1) == QuestType.StoryType)
                StoryList.Add(info);
            else if ((QuestType)(info.staticData.type - 1) == QuestType.DailyType)
                DailyList.Add(info);
            else if ((QuestType)(info.staticData.type - 1) == QuestType.OtherType)
                OtherList.Add(info);
        }

        if (group4 != null)
        {
            if ((QuestType)(group4.staticData.type - 1) == QuestType.StoryType)
                StoryList.Add(group4);
            else if ((QuestType)(group4.staticData.type - 1) == QuestType.DailyType)
                DailyList.Add(group4);
            else if ((QuestType)(group4.staticData.type - 1) == QuestType.OtherType)
                OtherList.Add(group4);
        }

    }

    void OnLanguageChanged()
    {
        text_story.text = "剧情任务";
        text_daily.text = "日常任务";
        text_other.text = "列传任务";
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
