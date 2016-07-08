using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIQuestInfo : UIBase
{

    public static string ViewName = "UIQuestInfo";
    public static string AssertName = "ui/quest";

    //public static void Open(int questId)
    public static void Open(QuestInfo quest)
    {
        //questItem quest = StaticDataMgr.Instance.GetQuestData(questId);

        GameObject go = UIMgr.Instance.OpenUI(UIQuestInfo.AssertName, UIQuestInfo.ViewName);
        UIQuestInfo mInfo = go.GetComponent<UIQuestInfo>();
        mInfo.info = quest;
        mInfo.ShowWithData(quest);
    }




    public Button btn_confirm;
    public Text text_title;

    public Transform rewardParent;


    private QuestInfo info;
    private List<rewardItemIcon> rewards = new List<rewardItemIcon>();


    void Start()
    {
        EventTriggerListener.Get(btn_confirm.gameObject).onClick = ClickConfirmButton;
    }

    void ShowWithData(QuestInfo info)
    {
        text_title.text = info.staticData.name;
        SetReward(info.staticData.rewardId);
    }




    void ClickConfirmButton(GameObject go)
    {
        Logger.Log("click uiQuestInfo confirm Button");

#if XL_DEBUG
        Destroy(gameObject);
#else
        UIMgr.Instance.CloseUI(this);
#endif
    }



    void SetReward(int rewardId)
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
            GameObject go = ResourceMgr.Instance.LoadAsset("ui/quest", "rewardItemIcon");
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
            rewards[i].SetItem(list[i]);
        }


    }

    public static int SortReward(RewardItemData a, RewardItemData b)
    {
        int result = 0;
        //    显示顺序为：钻石、金币、经验、道具（道具图标按照道具表中的顺序）	
        if ((a.itemType <= (int)PB.itemType.MONSTER_ATTR && b.itemType <= (int)PB.itemType.MONSTER_ATTR)
            || (a.itemType > (int)PB.itemType.MONSTER_ATTR && b.itemType > (int)PB.itemType.MONSTER_ATTR))
        {
            if (a.itemId < b.itemId)
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
            if (a.itemType < b.itemType)
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
