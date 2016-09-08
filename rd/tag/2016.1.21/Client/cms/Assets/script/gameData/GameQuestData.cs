using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum QuestType
{
    StoryType = 1,
    DailyType = 2,
    OtherType = 3
}
public class QuestData:IComparable
{
    public int questId;
    public int progress;
    public QuestStaticData staticData;
    private TimeEventWrap timeEvent;
    public TimeEventWrap TimeEvent
    {
        get
        {
            return timeEvent;
        }
        set
        {
            if (timeEvent!=null)
            {
                timeEvent.RemoveTimeEvent();
            }
            timeEvent = value;
        }
    }

    public static QuestData valueof(int questId, int progress = 0)
    {
        QuestData questData = new QuestData();
        questData.questId = questId;
        questData.progress = progress;
        questData.staticData = StaticDataMgr.Instance.GetQuestData(questData.questId);
        return questData;
    }

    public int CompareTo(object obj)
    {
        QuestData a = this;
        QuestData b = (QuestData)obj;
        int result = 0;
        if (((a.progress >= a.staticData.goalCount) && (b.progress >= b.staticData.goalCount)) ||
            ((a.progress < a.staticData.goalCount) && (b.progress < b.staticData.goalCount)))
        {
            if (a.questId < b.questId)
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
            if (a.progress >= a.staticData.goalCount)
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


public class GameQuestData
{
    public Dictionary<int, QuestData> questList = new Dictionary<int, QuestData>();

    //時間任務沒有處理---在檢測時需要檢測時間
    public List<QuestData> StoryList = new List<QuestData>();
    public List<QuestData> DailyList = new List<QuestData>();
    public List<QuestData> OtherList = new List<QuestData>();

    bool storyFinish = false;
    bool dailyFinish = false;
    bool otherFinish = false;

    public bool StoryFinish { get { return storyFinish; } }
    public bool DailyFinish { get { return dailyFinish; } }
    public bool OtherFinish { get { return otherFinish; } }


    public QuestData GetQuestById(int questId)
    {
        QuestData quest = null;
        questList.TryGetValue(questId, out quest);
        return quest;
    }

    public void ClearData()
    {
        questList.Clear();

        StoryList.Clear();
        DailyList.Clear();
        OtherList.Clear();
    }
    //同步所有任务
    public void QuestInfoSync(List<PB.HSQuest> questSync)
    {
        ClearData();

        Dictionary<int, QuestData> groupUsedTime = new Dictionary<int, QuestData>();
        foreach (var questInfo in questSync)
        {
            AddQuest(questInfo.questId, questInfo.progress);
        }

        RefreshQuest(QuestType.StoryType);
        RefreshQuest(QuestType.DailyType);
        RefreshQuest(QuestType.OtherType);
        
        GameEventMgr.Instance.FireEvent(GameEventList.QuestChanged);
    }
    //获取一个新任务
    public void QuestAccept(List<PB.HSQuest> questList)
    {
        bool storyChange = false;
        bool dailyChange = false;
        bool otherChange = false;
        foreach (var questInfo in questList)
        {
            QuestData quest = AddQuest(questInfo.questId, questInfo.progress);
            switch ((QuestType)quest.staticData.type)
            {
                case QuestType.StoryType:
                    storyChange = true;
                    break;
                case QuestType.DailyType:
                    dailyChange = true;
                    break;
                case QuestType.OtherType:
                    otherChange = true;
                    break;
            }
        }
        if (storyChange)
        {
            RefreshQuest(QuestType.StoryType);
        }
        if (dailyChange)
        {
            RefreshQuest(QuestType.DailyType);
        }
        if (otherChange)
        {
            RefreshQuest(QuestType.OtherType);
        }
        GameEventMgr.Instance.FireEvent(GameEventList.QuestChanged);
    }
    //删除一个任务
    public void QuestRemove(List<int> questList)
    {
        bool storyChange = false;
        bool dailyChange = false;
        bool otherChange = false;

        foreach (int questId in questList)
        {
            QuestData quest=RemoveQuest(questId);
            switch ((QuestType)quest.staticData.type)
            {
                case QuestType.StoryType:
                    storyChange = true;
                    break;
                case QuestType.DailyType:
                    dailyChange = true;
                    break;
                case QuestType.OtherType:
                    otherChange = true;
                    break;
            }
        }
        if (storyChange)
        {
            RefreshQuest(QuestType.StoryType);
        }
        if (dailyChange)
        {
            RefreshQuest(QuestType.DailyType);
        }
        if (otherChange)
        {
            RefreshQuest(QuestType.OtherType);
        }
        GameEventMgr.Instance.FireEvent(GameEventList.QuestChanged);
    }
    //任务进度更新
    public void QuestUpdate(List<PB.HSQuest> questList)
    {
        bool storyChange = false;
        bool dailyChange = false;
        bool otherChange = false;
        foreach (PB.HSQuest questInfo in questList)
        {
            QuestData quest = AddQuest(questInfo.questId, questInfo.progress);
            switch ((QuestType)quest.staticData.type)
            {
                case QuestType.StoryType:
                    storyChange = true;
                    break;
                case QuestType.DailyType:
                    dailyChange = true;
                    break;
                case QuestType.OtherType:
                    otherChange = true;
                    break;
            }
        }
        if (storyChange)
        {
            RefreshQuest(QuestType.StoryType);
        }
        if (dailyChange)
        {
            RefreshQuest(QuestType.DailyType);
        }
        if (otherChange)
        {
            RefreshQuest(QuestType.OtherType);
        }
        GameEventMgr.Instance.FireEvent(GameEventList.QuestChanged);
    }

    public void RefreshQuest(QuestType type)
    {
        #region 获取要刷新的列表

        List<QuestData> list;
        switch (type)
        {
            case QuestType.StoryType:
                list = StoryList;
                break;
            case QuestType.DailyType:
                list = DailyList;
                break;
            case QuestType.OtherType:
                list = OtherList;
                break;
            default:
                return;
        }
        #endregion

        list.Clear();

        #region 添加到对应类型表
        
        Dictionary<int, QuestData> groupUsedTime = new Dictionary<int, QuestData>();
        foreach (var item in questList)
        {
            QuestData info = item.Value;
            if (info.staticData.type == (int)type)
            {
                TimeStaticData beginTime = StaticDataMgr.Instance.GetTimeData(info.staticData.timeBeginId);
                TimeStaticData endTime = StaticDataMgr.Instance.GetTimeData(info.staticData.timeEndId);
                if (beginTime != null && endTime != null)
                {
                    if (endTime < GameTimeMgr.Instance.GetServerTime())
                        continue;//任务已经结束
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
                    continue;
                }
                list.Add(info);
            }
        }
        #region 获取系统当天时间

        DateTime curDateTime = GameTimeMgr.Instance.GetServerDateTime();
        
        #endregion

        foreach (var item in groupUsedTime)
        {
            QuestData info = item.Value;

            TimeStaticData beginTime = StaticDataMgr.Instance.GetTimeData(info.staticData.timeBeginId);
            TimeStaticData endTime = StaticDataMgr.Instance.GetTimeData(info.staticData.timeEndId);
            
            if (beginTime > GameTimeMgr.Instance.GetServerTime())//任务未开始
            {
                Action endEvent = () =>
                {
                    RefreshQuest((QuestType)info.staticData.type);
                };
                info.TimeEvent = new TimeEventWrap(GameTimeMgr.GetTimeStamp(new DateTime(curDateTime.Year,curDateTime.Month,curDateTime.Day,beginTime.hour,beginTime.minute,0)), endEvent);
            }
            else if(endTime> GameTimeMgr.Instance.GetServerTime())//任务已开始
            {
                Action endEvent = () =>
                {
                    RefreshQuest((QuestType)info.staticData.type);
                };
                info.TimeEvent = new TimeEventWrap(GameTimeMgr.GetTimeStamp(new DateTime(curDateTime.Year, curDateTime.Month, curDateTime.Day, endTime.hour, endTime.minute, 0)), endEvent);
            }
            else
            {
                continue;//任务已经结束
            }
        }
        
        list.AddRange(groupUsedTime.Values);

        #endregion

        #region 检测对应类型状态
        switch (type)
        {
            case QuestType.StoryType:
                storyFinish = CheckIsFinished(list);
                break;
            case QuestType.DailyType:
                dailyFinish = CheckIsFinished(list);
                break;
            case QuestType.OtherType:
                otherFinish = CheckIsFinished(list);
                break;
        }

        #endregion
    }

    bool CheckIsFinished(List<QuestData> questList)
    {
        for (int i = 0; i < questList.Count; i++)
        {
            QuestData info = questList[i];
            TimeStaticData beginTime = StaticDataMgr.Instance.GetTimeData(info.staticData.timeBeginId);
            TimeStaticData endTime = StaticDataMgr.Instance.GetTimeData(info.staticData.timeEndId);
            if (beginTime != null && endTime != null)
            {
                if (beginTime < GameTimeMgr.Instance.GetServerTime() && endTime > GameTimeMgr.Instance.GetServerTime())
                {
                    if (info.progress >= info.staticData.goalCount)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (info.progress >= info.staticData.goalCount)
                {
                    return true;
                }
            }
        }
        return false;
    }
    QuestData AddQuest(int questId, int progress)
    {
        QuestData questData;
        if (questList.TryGetValue(questId, out questData))
        {
            questData.questId = questId;
            questData.progress = progress;
        }
        else
        {
            questData = QuestData.valueof(questId, progress);
            questList.Add(questId, questData);
        }
        return questData;
    }
    QuestData RemoveQuest(int questId)
    {
        QuestData questData;
        questList.TryGetValue(questId, out questData);
        if (questData!=null)
        {
            questData.TimeEvent = null;
            questList.Remove(questId);
        }
        return questData;
    }
    
}
