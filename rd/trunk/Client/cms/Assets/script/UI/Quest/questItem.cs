using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class questItem : MonoBehaviour
{
    public Image img_QuestIcon;
    public Image img_Bground;
    public Text text_Name;
    public Text text_Desc;
    public Text text_progress;

    public Button btn_Todoit;
    public Button btn_Submit;
    public Text text_Cause;

    public Transform rewardParent;
    private List<rewardItemIcon> rewards = new List<rewardItemIcon>();
    private QuestInfo info;
    void Start()
    {
        EventTriggerListener.Get(btn_Todoit.gameObject).onClick = OnClickTodoit;
        EventTriggerListener.Get(btn_Submit.gameObject).onClick = OnClickSubmit;

        //TODO:Debug
        btn_Todoit.gameObject.SetActive(true);
    }

    public void SetQuest(QuestInfo info)
    {
        this.info = info;
        text_Name.text = info.staticData.name+info.staticData.id;
        //TODO: extend
        #region Desc

        string desc;
        switch (info.staticData.goalType)
        {
            case "difficulty":
                desc = "通关特定难度的副本"; break;
            case "3stars":
                desc = "通关特定星级的副本"; break;
            case "normal":
                desc = "通关普通难度的副本X次"; break;
            case "hard":
                desc = "通关精英难度的副本X次"; break;
            case "all":
                desc = "通关副本X次"; break;
            case "level":
                desc = "提升等级到X级"; break;
            case "petquality":
                desc = "携带的固定品级的伙伴数量达到N个"; break;
            case "petlevel":
                desc = "携带的伙伴中最高等级达到X级"; break;
            case "arena":
                desc = "完成一定次数的竞技场"; break;
            case "time":
                desc = "完成一定次数的时光之穴"; break;
            case "petmix":
                desc = "完成一定次数的炼妖炉"; break;
            case "adventure":
                desc = "完成一定次数的大冒险"; break;
            case "bossrush":
                desc = "完成一定次数的bossrush"; break;
            case "explore":
                desc = "完成一定次数的稀有探索"; break;
            case "skill":
                desc = "升级一定次数的宠物技能"; break;
            case "equip":
                desc = "升级一定次数的宠物装备"; break;
            case "buycoin":
                desc = "进行一定次数的钻石买钱操作"; break;
            default:
                desc = "类型扩展没有做……";
                break;
        }
        text_Desc.text = desc;

        #endregion

        text_progress.text = info.serverData.progress + "/" + info.staticData.goalCount;

        if (info.staticData.group == 4 && 
            StaticDataMgr.Instance.GetTimeData(info.staticData.timeBeginId) < GameTimeMgr.Instance.GetTime())
        {
            SetState(false, false, "时间未到");
        }
        else
        {
            SetState((info.serverData.progress >= info.staticData.goalCount), true, "");
        }

        SetReward(info.staticData.rewardId);

    }

    void SetState(bool finish, bool accept=true, string cause = "")
    {
        btn_Todoit.gameObject.SetActive(false);
        btn_Submit.gameObject.SetActive(false);
        text_Cause.gameObject.SetActive(false);
        if (!accept) { text_Cause.gameObject.SetActive(true); text_Cause.text = cause; }
        else if (finish) btn_Submit.gameObject.SetActive(true);
        else btn_Todoit.gameObject.SetActive(true);
    }

    void SetReward(int rewardId)
    {
        List<RewardItemData> list =new List<RewardItemData>(StaticDataMgr.Instance.GetRewardData(rewardId).itemList);
        list.Sort(SortReward);
        for (int i = 0; i < rewards.Count; i++)
        {
            if (i >= list.Count) rewards[i].gameObject.SetActive(false);
            else rewards[i].gameObject.SetActive(true); ;
        }
        for (int i = rewards.Count; i < list.Count; i++)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("ui/quest", "rewardItemIcon");
            if (go!=null)
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


    void OnClickTodoit(GameObject go)
    {
        //TODO:  
        Logger.Log("因为提交任务逻辑已经做了，由于无法完成任务。此处为了测试提交任务的弹窗");

        UIQuestInfo.Open(this.info);
    }
    void OnClickSubmit(GameObject go)
    {
        //TODO:
        PB.HSQuestSubmit param = new PB.HSQuestSubmit();
        param.questId = info.serverData.questId;
        GameApp.Instance.netManager.SendMessage(PB.code.QUEST_SUBMIT_C.GetHashCode(), param);
    }


    void BindListener()
    {
        //TODO:
        //GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_ENTER_S);
    }

    void UnBindListener()
    {
        //GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_ENTER_S.GetHashCode().ToString(), OnRequestEnterInstanceFinished);
    }


    //sort reward item

    public static int SortReward(RewardItemData a, RewardItemData b)
    {
        int result=0;
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
