using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SociatyOtherItem : MonoBehaviour
{
    public Transform teamPanel;
    public Transform rewardPanel;
   // public GameObject leaderRewardObject;
    public Text rewardText;
    public Text timeLeftText;
    public Button joinButton;

    private PB.AllianceTeamInfo teamInfo;
    private List<SociatyTeamMemberItem> teamMemberItemCatche = new List<SociatyTeamMemberItem>();
    private List<GameObject> rewardItems = new List<GameObject>();
    bool isNeedUpdateTime = false;
    int taskEndTime = 0;
    public static   SociatyOtherItem CreateWith(PB.AllianceTeamInfo teamInfo)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("SociatyOtherItem");
        var item = go.GetComponent<SociatyOtherItem>();
        item.RefreshWith(teamInfo);
        return item;
    }

    // Use this for initialization
	void Start ()
    {
        joinButton.onClick.AddListener(OnJoinButtonClick);
        UIUtil.SetButtonTitle(joinButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_enqueue"));
        rewardText.text = StaticDataMgr.Instance.GetTextByID("sociaty_teamreward");
    }

    int updateCount = 1;
    void Update()
    {
        if(isNeedUpdateTime)
        {
            updateCount++;
            if(updateCount > 20)
            {
                RefreshTimeLeft();
                updateCount = 1;
            }
           
        }
    }
    void OnDisable()
    {
        isNeedUpdateTime = false;
    }

    void Clear()
    {
        foreach(var memberItem in teamMemberItemCatche)
        {
            ResourceMgr.Instance.DestroyAsset(memberItem.gameObject);
        }
        teamMemberItemCatche.Clear();

        foreach(var rewardItem in rewardItems)
        {
            ResourceMgr.Instance.DestroyAsset(rewardItem);
        }
        rewardItems.Clear();
    }

    public void RefreshWith(PB.AllianceTeamInfo teamInfo)
    {
        this.teamInfo = teamInfo;
        int taskId = teamInfo.taskId;
        var taskInfo = StaticDataMgr.Instance.GetSociatyTask(taskId);
        taskEndTime = taskInfo.time * 60 + teamInfo.startTime;

        RefreshMemberItem();
        RefreshReward();
        RefreshTimeLeft();
        isNeedUpdateTime = true;
    }

    void RefreshTimeLeft()
    {
        int hour = 0;
        int minute = 0;
        int second = 0;

        int curTime = GameTimeMgr.Instance.GetServerTimeStamp();
        int leftSecond = taskEndTime - curTime;
       if(leftSecond > 0)
        {
            hour = leftSecond / 3600;
            minute = leftSecond / 60;
            second = leftSecond % 60;
        }
       else
        {
            isNeedUpdateTime = false;
        }
        timeLeftText.text = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_shengyutime"), hour, minute, second);
    }

    void RefreshMemberItem()
    {
        foreach(var catchItem in teamMemberItemCatche)
        {
            catchItem.gameObject.SetActive(false);
        }

        for(int i =0;i<teamInfo.members.Count;++i)
        {
            var itemData = teamInfo.members[i];
            if (i < teamMemberItemCatche.Count)
            {
                teamMemberItemCatche[i].gameObject.SetActive(true);
                teamMemberItemCatche[i].RefreshWith(itemData);
            }
            else
            {
                var item = SociatyTeamMemberItem.CreateWith(itemData);
                item.transform.SetParent(teamPanel);
                item.transform.localScale = new Vector3(1, 1, 1);
                teamMemberItemCatche.Add(item);
            }
        }
    }

    void RefreshReward()
    {
        foreach (var rewardItem in rewardItems)
        {
            ResourceMgr.Instance.DestroyAsset(rewardItem);
        }
        rewardItems.Clear();

        SociatyTask taskData = StaticDataMgr.Instance.GetSociatyTask(teamInfo.taskId);
        if(null != taskData)
        {
            AddLeaderReward(taskData.leaderReward);
            AddTeamReward(taskData.reward);
        }
        
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
       // leaderRewardObject.SetActive(true);

    }
    void AddTeamReward(string rewardid)
    {
        RewardData rewardData = StaticDataMgr.Instance.GetRewardData(rewardid);
        if (rewardData == null || rewardData.itemList == null)
            return;
        foreach (var itemData in rewardData.itemList)
        {
            GameObject go = RewardItemCreator.CreateRewardItem(itemData.protocolData, rewardPanel, true, false);
            if (null != go)
            {
                go.transform.localScale = new Vector3(1, 1, 1);
                rewardItems.Add(go);
            }

        }
    }

    void OnJoinButtonClick()
    {
        if(GameDataMgr.Instance.SociatyDataMgrAttr.taskTeamId >0)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_036"), (int)PB.ImType.PROMPT);
            return;
        }
        if (GameConfig.Instance.sociatyTaskMaxCount <= GameDataMgr.Instance.SociatyDataMgrAttr.taskCount)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_030"), (int)PB.ImType.PROMPT);
            return;
        }

        MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel, StaticDataMgr.Instance.GetTextByID("sociaty_jiarutips"),
            StaticDataMgr.Instance.GetTextByID("sociaty_jiarutips1"), ConformJoinTeam);
    }

    void ConformJoinTeam(MsgBox.PrompButtonClick click)
    {
        if(click == MsgBox.PrompButtonClick.OK)
        {
            GameDataMgr.Instance.SociatyDataMgrAttr.RequestJoinTeam(teamInfo.teamId,OnRequestJoinTeamFinish);
        }
    }
	
    void OnRequestJoinTeamFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        GameDataMgr.Instance.SociatyDataMgrAttr.taskTeamId = teamInfo.teamId;
 
        UISociatyTask.Instance.InitType((int)SociatyTaskContenType.MyTeam);
    }
}
