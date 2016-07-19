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
    public Text text_Todoit;
    public Text text_Submit;
    public Text text_Cause;
    private string causeId="";

    public Transform rewardParent;
    private List<rewardItemIcon> items = new List<rewardItemIcon>();
    private QuestInfo info;
    void Start()
    {
        OnLanguageChanged();
        EventTriggerListener.Get(btn_Todoit.gameObject).onClick = OnClickTodoit;
        EventTriggerListener.Get(btn_Submit.gameObject).onClick = OnClickSubmit;
    }
    //清理资源对象
    public void Clean()
    {
        for (int i = 0; i < items.Count; i++)
        {
            ResourceMgr.Instance.DestroyAsset(items[i].gameObject);
        }
        items.Clear();
    }

    public void SetQuest(QuestInfo info)
    {
        this.info = info;
        img_QuestIcon.sprite =ResourceMgr.Instance.LoadAssetType<Sprite>(info.staticData.icon) as Sprite;
        text_Name.text = StaticDataMgr.Instance.GetTextByID(info.staticData.name);
        //TODO: extend  need to modify
        #region Desc
        string param="";
        if (string.IsNullOrEmpty(info.staticData.descType))
        {
            param = info.staticData.goalParam;
        }
        text_Desc.text = string.Format(StaticDataMgr.Instance.GetTextByID(info.staticData.desc), param);
        #endregion

        text_progress.text = info.serverData.progress + "/" + info.staticData.goalCount;
        if (StaticDataMgr.Instance.GetTimeData(info.staticData.timeBeginId) != null &&
            GameTimeMgr.Instance.GetTime() < StaticDataMgr.Instance.GetTimeData(info.staticData.timeBeginId))
        {
            //TODO： test message；
            SetState(false, false, "quest_shijianweidao");
        }
        else
        {
            SetState((info.serverData.progress >= info.staticData.goalCount), true);
        }

        SetReward(info.staticData.rewardId);

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
        else if (finish) btn_Submit.gameObject.SetActive(true);
        else btn_Todoit.gameObject.SetActive(true);
    }

    void SetReward(string rewardId)
    {
        List<RewardItemData> list =new List<RewardItemData>(StaticDataMgr.Instance.GetRewardData(rewardId).itemList);
        list.Sort(SortReward);
        for (int i = 0; i < items.Count; i++)
        {
            if (i >= list.Count) items[i].gameObject.SetActive(false);
            else items[i].gameObject.SetActive(true); ;
        }
        for (int i = items.Count; i < list.Count; i++)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("rewardItemIcon");
            if (go!=null)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(rewardParent, false);
                rewardItemIcon item = go.GetComponent<rewardItemIcon>();
                items.Add(item);
                LanguageMgr.Instance.SetLanguageFont(go);
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

    void OnClickTodoit(GameObject go)
    {
        //TODO:  
    }
    void OnClickSubmit(GameObject go)
    {
        PB.HSQuestSubmit param = new PB.HSQuestSubmit();
        param.questId = info.serverData.questId;
        GameApp.Instance.netManager.SendMessage(PB.code.QUEST_SUBMIT_C.GetHashCode(), param);
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
            else
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
        text_Todoit.text = StaticDataMgr.Instance.GetTextByID("quest_lijiqianwang");
        text_Submit.text = StaticDataMgr.Instance.GetTextByID("quest_lingqujiangli");
        text_Cause.text = StaticDataMgr.Instance.GetTextByID(causeId); 
    }
}
