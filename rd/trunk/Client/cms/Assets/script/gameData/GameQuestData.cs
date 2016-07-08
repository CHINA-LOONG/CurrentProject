using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestData
{
    public int questId;
    public int progress;
    public static QuestData valueof(int questId, int progress = 0)
    {
        QuestData questData = new QuestData();
        questData.questId = questId;
        questData.progress = progress;
        return questData;
    }
}


public class GameQuestData
{
    public Dictionary<int, QuestData> questList = new Dictionary<int, QuestData>();

    public List<QuestData> GetAllQuest()
    {
        List<QuestData> allQuest = new List<QuestData>();
        allQuest.AddRange(questList.Values);
        return allQuest;
    }

    public void ClearQuest()
    {
        questList.Clear();
    }

    public void AddQuest(int questId, int progress)
    {
        //Logger.Log("questId:" + questId + "\n progress:" + progress);
        QuestData questData;
        if (questList.TryGetValue(questId, out questData))
        {
            questData.questId = questId;
            questData.progress = progress;
        }
        else
        {
            questList.Add(questId, QuestData.valueof(questId, progress));
        }
    }

    public bool RemoveQuest(int questId)
    {
        if (questList.ContainsKey(questId))
        {
            questList.Remove(questId);
            return true;
        }
        return false;
    }


}
