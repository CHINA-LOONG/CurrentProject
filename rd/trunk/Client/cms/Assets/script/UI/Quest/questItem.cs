using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//public class QuestItemInfo
//{
//    public QuestData serverData;
//    public QuestStaticData staticData;
//}

public class questItem : MonoBehaviour
{
    public Text text_Name;
    public Text text_Desc;
    public Text text_progress;

    public Transform rewardParent;

    public Button btn_Todoit;
    public Button btn_Submit;
    public Text text_Todoit;
    public Text text_Submit;

    public Text text_Cause;
    private string causeId="";

    //private QuestItemInfo curData;
    private QuestData curData;
    public List<GameObject> rewardItems = new List<GameObject>();

    public System.Action<QuestData> onClickTodoit;
    public System.Action<QuestData> onClickSubmit;

    private System.Action onClickEvent = null;
    void Start()
    {
        OnLanguageChanged();
        EventTriggerListener.Get(btn_Todoit.gameObject).onClick = OnClickTodoit;
        EventTriggerListener.Get(btn_Submit.gameObject).onClick = OnClickSubmit;
    }

    public void ReLoadData(QuestData info)
    {
        curData = info;
        text_Name.text = StaticDataMgr.Instance.GetTextByID(info.staticData.name);
        text_Desc.text = StaticDataMgr.Instance.GetTextByID(info.staticData.desc);

        text_progress.text = info.progress + "/" + info.staticData.goalCount;
        SetReward(info.staticData.rewardId);

        if (StaticDataMgr.Instance.GetTimeData(info.staticData.timeBeginId) != null &&
            GameTimeMgr.Instance.GetServerTime() < StaticDataMgr.Instance.GetTimeData(info.staticData.timeBeginId))
        {
            //TODO： test message；
            SetState(false, false, "quest_shijianweidao");
        }
        else
        {
            SetState((info.progress >= info.staticData.goalCount), true);
        }
        if (curData.staticData.PathList != null)
        {
            ParseBase parse = ParseFactory.CreateParse(curData.staticData.PathList);
            string name;
            bool condition;
            parse.GetResult(curData.staticData.PathList, out name, out onClickEvent, out condition);
        }
    }
    void SetState(bool finish, bool accept=true, string cause = "")
    {
        btn_Todoit.gameObject.SetActive(false);
        btn_Submit.gameObject.SetActive(false);
        text_Cause.gameObject.SetActive(false);
        if (!accept) 
        { 
            text_Cause.gameObject.SetActive(true);
            causeId = cause;
            text_Cause.text = StaticDataMgr.Instance.GetTextByID(causeId); 
        }
        else if (finish)
            btn_Submit.gameObject.SetActive(true);
        else if (!string.IsNullOrEmpty(curData.staticData.path))
            btn_Todoit.gameObject.SetActive(true);
    }

    void SetReward(string rewardId)
    {
        RewardData rewardData = StaticDataMgr.Instance.GetRewardData(rewardId);
        if (rewardData==null)
        {
            Logger.Log("奖励没有配置："+rewardId);
            return;
        }
        List<RewardItemData> list =new List<RewardItemData>(rewardData.itemList);
        list.Sort(SortReward);
        CleanReward();
        PB.RewardItem info;
        GameObject reward;
        for (int i = 0; i < list.Count; i++)
        {
            info = list[i].protocolData;
            if (info.type == (int)PB.itemType.ITEM)
            {
                ItemIcon icon = ItemIcon.CreateItemIcon(new ItemData() { itemId = info.itemId, count = (int)info.count });
                UIUtil.SetParentReset(icon.transform, rewardParent);
                reward = icon.gameObject;
            }
            else if (info.type == (int)PB.itemType.EQUIP)
            {
                EquipData equipData = EquipData.valueof(0, info.itemId, info.stage, info.level, BattleConst.invalidMonsterID, null);
                ItemIcon icon = ItemIcon.CreateItemIcon(equipData);
                UIUtil.SetParentReset(icon.transform, rewardParent);
                reward = icon.gameObject;
            }
            else if (info.type == (int)PB.itemType.PLAYER_ATTR)
            {
                changeTypeIcon icon;
                if ((PB.changeType)(int.Parse(info.itemId)) == PB.changeType.CHANGE_PLAYER_EXP)
                {
                    icon = changeTypeIcon.CreateIcon((PB.changeType)(int.Parse(info.itemId)), (int)(info.count * curData.staticData.expK + curData.staticData.expB));
                }
                else
                {
                    icon = changeTypeIcon.CreateIcon((PB.changeType)(int.Parse(info.itemId)), (int)info.count);
                }
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

    void OnClickTodoit(GameObject go)
    {
        if (onClickEvent!=null)
        {
            onClickEvent();
        }
        if (onClickTodoit!=null)
        {
            onClickTodoit(curData);
        }
    }
    void OnClickSubmit(GameObject go)
    {
        if (onClickSubmit!=null)
        {
            onClickSubmit(curData);
        }
    }

    //sort reward item
    public static int SortReward(RewardItemData a, RewardItemData b)
    {
        int result=0;
        //    显示顺序为：钻石、金币、经验、道具（道具图标按照道具表中的顺序）	
        if ((a.protocolData.type <= (int)PB.itemType.MONSTER_ATTR && b.protocolData.type <= (int)PB.itemType.MONSTER_ATTR)
            || (a.protocolData.type > (int)PB.itemType.MONSTER_ATTR && b.protocolData.type > (int)PB.itemType.MONSTER_ATTR))
        {
            if (int.Parse(a.protocolData.itemId) < int.Parse(b.protocolData.itemId))
            {
                result = -1;
            }
            else if (int.Parse(a.protocolData.itemId) > int.Parse(b.protocolData.itemId))
            {
                result = 1;
            }
        }
        else
        {
            if (a.protocolData.type < b.protocolData.type)
            {
                result = -1;
            }
            else if (a.protocolData.type > b.protocolData.type)
            {
                result = 1;
            }
        }
        return result;
    }

    void OnLanguageChanged()
    {
        //TODO: change language
        text_Todoit.text = StaticDataMgr.Instance.GetTextByID("quest_lijiqianwang");
        text_Submit.text = StaticDataMgr.Instance.GetTextByID("quest_lingqujiangli");
        text_Cause.text = StaticDataMgr.Instance.GetTextByID(causeId); 
    }
}
