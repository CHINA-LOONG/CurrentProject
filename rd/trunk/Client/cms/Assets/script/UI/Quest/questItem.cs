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
    private List<rewardItemIcon> rewards = new List<rewardItemIcon>();
    private QuestInfo info;
    void Start()
    {
        OnLanguageChanged();
        EventTriggerListener.Get(btn_Todoit.gameObject).onClick = OnClickTodoit;
        EventTriggerListener.Get(btn_Submit.gameObject).onClick = OnClickSubmit;
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
        for (int i = 0; i < rewards.Count; i++)
        {
            if (i >= list.Count) rewards[i].gameObject.SetActive(false);
            else rewards[i].gameObject.SetActive(true); ;
        }
        for (int i = rewards.Count; i < list.Count; i++)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("rewardItemIcon");
            if (go!=null)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(rewardParent, false);
                rewardItemIcon item = go.GetComponent<rewardItemIcon>();
                rewards.Add(item);
                LanguageMgr.Instance.SetLanguageFont(go);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].itemType == (int)PB.itemType.PLAYER_ATTR &&
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


    void OnClickTodoit(GameObject go)
    {
        //TODO:  

        GameEventMgr.Instance.FireEvent<ProtocolMessage>(PB.code.QUEST_SUBMIT_S.GetHashCode().ToString(),null);
    }
    void OnClickSubmit(GameObject go)
    {
        PB.HSQuestSubmit param = new PB.HSQuestSubmit();
        param.questId = info.serverData.questId;
        GameApp.Instance.netManager.SendMessage(PB.code.QUEST_SUBMIT_C.GetHashCode(), param);
    }



    void OnEnable()
    {
        BindListener();
    }
    void OnDisable()
    {
        UnBindListener();
    }

    void BindListener()
    {
    }

    void UnBindListener()
    {
    }
    //sort reward item
    public static int SortReward(RewardItemData a, RewardItemData b)
    {
        int result=0;
        //    显示顺序为：钻石、金币、经验、道具（道具图标按照道具表中的顺序）	
        if ((a.itemType <= (int)PB.itemType.MONSTER_ATTR && b.itemType <= (int)PB.itemType.MONSTER_ATTR)
            || (a.itemType > (int)PB.itemType.MONSTER_ATTR && b.itemType > (int)PB.itemType.MONSTER_ATTR))
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




    void OnLanguageChanged()
    {
        //TODO: change language
        text_Todoit.text = StaticDataMgr.Instance.GetTextByID("quest_lijiqianwang");
        text_Submit.text = StaticDataMgr.Instance.GetTextByID("quest_lingqujiangli");
        text_Cause.text = StaticDataMgr.Instance.GetTextByID(causeId); 
    }
}
