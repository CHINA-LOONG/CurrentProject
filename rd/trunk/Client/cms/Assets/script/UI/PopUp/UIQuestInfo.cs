using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIQuestInfo : UIBase
{

    public static string ViewName = "UIQuestInfo";

    //public static void Open(int questId)
    public static void Open(QuestInfo quest)
    {
        //questItem quest = StaticDataMgr.Instance.GetQuestData(questId);

        GameObject go = UIMgr.Instance.OpenUI(UIQuestInfo.ViewName);
        UIQuestInfo mInfo = go.GetComponent<UIQuestInfo>();
        mInfo.info = quest;
        mInfo.ShowWithData(quest);
    }




    public Button btn_confirm;
    public Text text_Title;
    public Text text_Finish;
    public Text text_Quest;

    public Transform rewardParent;


    private QuestInfo info;
    private List<rewardItemIcon> rewards = new List<rewardItemIcon>();


    void Start()
    {
        //TODO:
        OnLanguageChanged();
        EventTriggerListener.Get(btn_confirm.gameObject).onClick = ClickConfirmButton;
    }

    void ShowWithData(QuestInfo info)
    {
        text_Quest.text =StaticDataMgr.Instance.GetTextByID(info.staticData.name);
        SetReward(info.staticData.rewardId);
    }

    void ClickConfirmButton(GameObject go)
    {
        Logger.Log("click uiQuestInfo confirm Button");

#if XL_DEBUG
        Destroy(gameObject);
#else
        UIMgr.Instance.CloseUI(this);
        if (!string.IsNullOrEmpty(info.staticData.speechId))
        { 
            UISpeech.Open(info.staticData.speechId); 
        }
#endif
    }



    void SetReward(string rewardId)
    {
        List<RewardItemData> list = new List<RewardItemData>(StaticDataMgr.Instance.GetRewardData(rewardId).itemList);
        list.Sort(SortReward);
        for (int i = 0; i < rewards.Count; i++)
        {
            if (i >= list.Count) rewards[i].gameObject.SetActive(false);
            else rewards[i].gameObject.SetActive(true); ;
        }
        for (int i = rewards.Count; i < list.Count; i++)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("rewardItemIcon");
            if (go != null)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(rewardParent, false);
                rewardItemIcon item = go.GetComponent<rewardItemIcon>();
                rewards.Add(item);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].type == (int)PB.itemType.PLAYER_ATTR &&
                int.Parse(list[i].itemId) == (int)PB.changeType.CHANGE_PLAYER_EXP)
            {
                rewards[i].SetItem(list[i], info.staticData.expK, info.staticData.expB);
            }
            else
            {
                rewards[i].SetItem(list[i]);
            }
        }


    }

    public static int SortReward(RewardItemData a, RewardItemData b)
    {
        int result = 0;
        //    显示顺序为：钻石、金币、经验、道具（道具图标按照道具表中的顺序）	
        if ((a.type <= (int)PB.itemType.MONSTER_ATTR && b.type <= (int)PB.itemType.MONSTER_ATTR)
            || (a.type > (int)PB.itemType.MONSTER_ATTR && b.type > (int)PB.itemType.MONSTER_ATTR))
        {
            if (int.Parse(a.itemId) < int.Parse(b.itemId))
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
            if (a.type < b.type)
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



    void OnLanguageChanged()
    {
        //TODO: change language
        text_Title.text = StaticDataMgr.Instance.GetTextByID("quest_reward_title");
        text_Finish.text = StaticDataMgr.Instance.GetTextByID("quest_reward_wancheng");
    }
}
