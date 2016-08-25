using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIQuestInfo : UIBase
{

    public static string ViewName = "UIQuestInfo";

    //public static void Open(int questId)
    public static UIQuestInfo Open(QuestInfo quest)
    {
        //questItem quest = StaticDataMgr.Instance.GetQuestData(questId);

        UIQuestInfo mInfo = UIMgr.Instance.OpenUI_(UIQuestInfo.ViewName)as UIQuestInfo;
        mInfo.info = quest;
        mInfo.ShowWithData(quest);
        return mInfo;
    }

    public Button btn_confirm;
    public Text text_Title;
    public Text text_Finish;
    public Text text_Quest;

    public Transform rewardParent;


    private QuestInfo info;
    private List<rewardItemIcon> items = new List<rewardItemIcon>();


    void Start()
    {
        //TODO:
        OnLanguageChanged();
        EventTriggerListener.Get(btn_confirm.gameObject).onClick = ClickConfirmButton;
    }

    public override void Init()
    {

    }

    public override void Clean()
    {
        for (int i = items.Count - 1; i >= 0; i++)
        {
            ResourceMgr.Instance.DestroyAsset(items[i].gameObject);
        }
        items.Clear();
    }

    void ShowWithData(QuestInfo info)
    {
        text_Quest.text =StaticDataMgr.Instance.GetTextByID(info.staticData.name);
        SetReward(info.staticData.rewardId);
    }

    void ClickConfirmButton(GameObject go)
    {
        Logger.Log("click uiQuestInfo confirm Button");

        UIMgr.Instance.CloseUI_(this);
        if (!string.IsNullOrEmpty(info.staticData.speechId))
        { 
            UISpeech.Open(info.staticData.speechId); 
        }
    }

    void SetReward(string rewardId)
    {
        List<RewardItemData> list = new List<RewardItemData>(StaticDataMgr.Instance.GetRewardData(rewardId).itemList);
        list.Sort(QuestItem.SortReward);
        for (int i = 0; i < items.Count; i++)
        {
            if (i >= list.Count) items[i].gameObject.SetActive(false);
            else items[i].gameObject.SetActive(true); ;
        }
        for (int i = items.Count; i < list.Count; i++)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("rewardItemIcon");
            if (go != null)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(rewardParent, false);
                rewardItemIcon item = go.GetComponent<rewardItemIcon>();
                items.Add(item);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].protocolData.type == (int)PB.itemType.PLAYER_ATTR &&
                int.Parse(list[i].protocolData.itemId) == (int)PB.changeType.CHANGE_PLAYER_EXP)
            {
                items[i].SetItem(list[i], info.staticData.expK, info.staticData.expB);
            }
            else
            {
                items[i].SetItem(list[i]);
            }
        }
    }
    void OnLanguageChanged()
    {
        //TODO: change language
        text_Title.text = StaticDataMgr.Instance.GetTextByID("quest_reward_title");
        text_Finish.text = StaticDataMgr.Instance.GetTextByID("ui_queding");
    }
}
