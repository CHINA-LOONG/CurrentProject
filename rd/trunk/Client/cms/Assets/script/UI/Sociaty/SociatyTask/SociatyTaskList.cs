using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SociatyTaskList : MonoBehaviour
{
    public Text rewardText;
    public Transform rewardPanel;
    public GameObject leaderRewardObject;
    public Button openButton;
    public Text costText;
    public Text leftTimes;
    public ScrollView scrollView;

    private SociatyTaskItem curSelItem = null;
    private List<GameObject> rewardItems = new List<GameObject>();

    private static SociatyTaskList instance = null;
    public static   SociatyTaskList Instance
    {
        get
        {
            if(null == instance)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("SociatyTaskList");
                SociatyTaskList taskList = go.GetComponent<SociatyTaskList>();
                //taskList.RefreshTaskList();
                instance = taskList;
            }
            return instance;
        }
    }
    // Use this for initialization
    void Start ()
    {
        openButton.onClick.AddListener(OnOpenButtonClick);
        rewardText.text = StaticDataMgr.Instance.GetTextByID("sociaty_teamreward");
	}

    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CREATE_TEAM_C.GetHashCode().ToString(), OnRequestOpenTaskFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CREATE_TEAM_S.GetHashCode().ToString(), OnRequestOpenTaskFinished);
    }

    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CREATE_TEAM_C.GetHashCode().ToString(), OnRequestOpenTaskFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CREATE_TEAM_S.GetHashCode().ToString(), OnRequestOpenTaskFinished);
    }

    public void Clear()
    {
        scrollView.ClearAllElement();
        ClearReward();
    }

    void ClearReward()
    {
        leaderRewardObject.SetActive(false);
        foreach (var item in rewardItems)
        {
            ResourceMgr.Instance.DestroyAsset(item);
        }
        rewardItems.Clear();
    }

    public  void    RefreshTaskList()
    {
        Clear();
        List<SociatyTask> sociatyList = StaticDataMgr.Instance.GetSociatyTaskList();
        for(int i = 0;i<sociatyList.Count;++i)
        {
            var itemData = sociatyList[i];
            var itemUi = SociatyTaskItem.CreateWith(itemData);
            scrollView.AddElement(itemUi.gameObject);
            if (i == 0)
            {
                OnItemSelected(itemUi);
            }
        }
        RefreshLeftTimes();
    }

    void RefreshLeftTimes()
    {
        leftTimes.text = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_tasknum"),
            GameConfig.Instance.sociatyTaskMaxCount - GameDataMgr.Instance.SociatyDataMgrAttr.taskCount);
    }

    public void OnItemSelected(SociatyTaskItem selItem)
    {
        if(curSelItem != null)
        {
            curSelItem.SetSelected(false);
        }
        leaderRewardObject.SetActive(false);
        selItem.SetSelected(true);
        curSelItem = selItem;
        costText.text = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_start"), selItem.sociatyTaskData.taskStart);
        ClearReward();
        AddLeaderReward(selItem.sociatyTaskData.leaderReward);
        AddTeamReward(selItem.sociatyTaskData.reward);
    }
    void AddLeaderReward(string rewardid)
    {
        RewardData rewardData = StaticDataMgr.Instance.GetRewardData(rewardid);
        if (rewardData == null || rewardData.itemList == null)
            return;
        RewardItemData itemData = rewardData.itemList[0];
        GameObject go = RewardItemCreator.CreateRewardItem(itemData.protocolData, rewardPanel, true, false);
        go.transform.localScale = new Vector3(1, 1, 1);
        rewardItems.Add(go);
        leaderRewardObject.SetActive(true);

    }
    void AddTeamReward(string rewardid)
    {
        RewardData rewardData = StaticDataMgr.Instance.GetRewardData(rewardid);
        if (rewardData == null || rewardData.itemList == null)
            return;
        foreach(var itemData in rewardData.itemList)
        {
            GameObject go = RewardItemCreator.CreateRewardItem(itemData.protocolData, rewardPanel, true, false);
            if(null != go)
            {
                go.transform.localScale = new Vector3(1, 1, 1);
                rewardItems.Add(go);
            }
        }
    }
	
    void OnOpenButtonClick()
    {
        if(null == curSelItem)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_058"), (int)PB.ImType.PROMPT);
            return;
        }
        if (GameConfig.Instance.sociatyTaskMaxCount <= GameDataMgr.Instance.SociatyDataMgrAttr.taskCount)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_030"), (int)PB.ImType.PROMPT);
            return;
        }
        MsgBox.PrompCostMsg.Open(curSelItem.sociatyTaskData.taskStart, StaticDataMgr.Instance.GetTextByID("sociaty_spendtips"), "", ConformOpenTask);
    }

    void ConformOpenTask(MsgBox.PrompButtonClick click)
    {
        if (click == MsgBox.PrompButtonClick.Cancle)
            return;

        if(curSelItem.sociatyTaskData.minLevel > GameDataMgr.Instance.PlayerDataAttr.LevelAttr)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_031"), (int)PB.ImType.PROMPT);
            return;
        }
        if (curSelItem.sociatyTaskData.taskStart > GameDataMgr.Instance.PlayerDataAttr.gold)
        {
            GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
            return;
        }

        PB.HSAllianceCreateTeam param = new PB.HSAllianceCreateTeam();
        param.taskId = curSelItem.sociatyTaskData.id;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_CREATE_TEAM_C.GetHashCode(), param);
    }

    void OnRequestOpenTaskFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        PB.HSAllianceCreateTeamRet msgRet = message.GetProtocolBody<PB.HSAllianceCreateTeamRet>();

        GameDataMgr.Instance.SociatyDataMgrAttr.taskTeamId = msgRet.teamId;
        GameDataMgr.Instance.SociatyDataMgrAttr.taskCount++;
        RefreshLeftTimes();
        UISociatyTask.Instance.SetTaskType(SociatyTaskContenType.MyTeam);
    }
}
