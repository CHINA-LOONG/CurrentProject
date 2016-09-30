using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIQuestInfo : UIBase
{

    public static string ViewName = "UIQuestInfo";

    //public static void Open(int questId)
    public static UIQuestInfo Open(QuestData quest, System.Action StartEvent = null, System.Action<float> EndEvent = null)
    {
        //questItem quest = StaticDataMgr.Instance.GetQuestData(questId);

        UIQuestInfo mInfo = UIMgr.Instance.OpenUI_(UIQuestInfo.ViewName)as UIQuestInfo;
        mInfo.ShowWithData(quest,StartEvent,EndEvent);
        return mInfo;
    }

    public Button btn_confirm;
    public Text text_Name;
    public Text text_Desc;
    public Text text_Confrim;

    public Transform rewardParent;

    public Animator animator;

    private QuestData info;
    public System.Action StartEvent;
    public System.Action<float> EndEvent;

    private List<GameObject> rewardItems = new List<GameObject>();

    void Start()
    {
        animator.SetTrigger("PopupIn");
        OnLanguageChanged();
        EventTriggerListener.Get(btn_confirm.gameObject).onClick = ClickConfirmButton;
    }
    void ShowWithData(QuestData info, System.Action StartEvent, System.Action<float> EndEvent)
    {
        this.info = info;
        this.StartEvent = StartEvent;
        this.EndEvent = EndEvent;
        text_Name.text =StaticDataMgr.Instance.GetTextByID(info.staticData.name);
        text_Desc.text = StaticDataMgr.Instance.GetTextByID(info.staticData.desc);
        SetReward(info.staticData.rewardId);
    }
    //创建奖励对象
    void SetReward(string rewardId)
    {
        RewardData rewardData = StaticDataMgr.Instance.GetRewardData(rewardId);
        if (rewardData == null)
        {
            Logger.Log("奖励没有配置：" + rewardId);
            return;
        }
        List<RewardItemData> list = new List<RewardItemData>(rewardData.itemList);
        list.Sort(questItem.SortReward);
        CleanReward();
        PB.RewardItem info;
        GameObject reward;
        for (int i = 0; i < list.Count; i++)
        {
            info = list[i].protocolData;
            if (info.type == (int)PB.itemType.ITEM)
            {
                ItemIcon icon = ItemIcon.CreateItemIcon(new ItemData() { itemId = info.itemId, count = (int)info.count },true,false);
                UIUtil.SetParentReset(icon.transform, rewardParent);
                reward = icon.gameObject;
            }
            else if (info.type == (int)PB.itemType.EQUIP)
            {
                EquipData equipData = EquipData.valueof(0, info.itemId, info.stage, info.level, BattleConst.invalidMonsterID, null);
                ItemIcon icon = ItemIcon.CreateItemIcon(equipData,true,false);
                UIUtil.SetParentReset(icon.transform, rewardParent);
                reward = icon.gameObject;
            }
            else if (info.type == (int)PB.itemType.PLAYER_ATTR)
            {
                changeTypeIcon icon = changeTypeIcon.CreateIcon((PB.changeType)(int.Parse(info.itemId)), (int)info.count);
                UIUtil.SetParentReset(icon.transform, rewardParent);
                reward = icon.gameObject;
            }
            else
            {
                Logger.LogError("配置错误，雷神知道怎么配"); reward = null;
            }
            rewardItems.Add(reward);
        }
    }    
    //清理奖励列表对象
    public void CleanReward()
    {
        for (int i = 0; i < rewardItems.Count; i++)
        {
            ResourceMgr.Instance.DestroyAsset(rewardItems[i]);
        }
        rewardItems.Clear();
    }

    void ClickConfirmButton(GameObject go)
    {
        Logger.Log("click uiQuestInfo confirm Button");

        UIMgr.Instance.CloseUI_(this);
        if (!string.IsNullOrEmpty(info.staticData.speechId))
        {
            StartEvent();
            UISpeech.Open(info.staticData.speechId, EndEvent);
        }
        else
        {
            EndEvent(0.0f);
        }
    }
    void OnLanguageChanged()
    {
        //TODO: change language
        text_Confrim.text = StaticDataMgr.Instance.GetTextByID("ui_queding");
    }

    #region UIBase
    public override void Init()
    {
        //effect.SetActive(true);
    }

    public override void Clean()
    {

    }

    #endregion

}
