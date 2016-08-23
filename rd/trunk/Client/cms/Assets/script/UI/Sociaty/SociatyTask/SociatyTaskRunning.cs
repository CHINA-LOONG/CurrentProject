using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum SociatyQuestType
{
    CommitCoin = 1,//捐金币
    CommitDrop,//捐道具
    CommitInstance//完成副本
}

public class SociatyTaskRunning : MonoBehaviour
{
    public Text myTeamText;
    public Transform teamTransform;
    public Text teamRewardText;
    public Transform teamRewardPanel;

    public Transform taskPanel;
    public Text timeText;
    public Text destLabelText;
    public Text destValueText;
    public Transform rewardPanel;
    public Text rewardLabelText;
    public Transform subRewardTrans;

    public Transform taskNeedItemParent;
    public GameObject jinbiObj;
    public GameObject instanceObj;
    public Text itemCountText;
    public Text finishText;
    public Button commitButton;

    public Button exitTaskButton;
    public Button recruitButton;
    public Button taskRewardButton;

    private PB.AllianceTeamInfo selfTeamData = null;

    private List<SociatyTeamMemberItem> teamMemberItems = new List<SociatyTeamMemberItem>();
    private List<GameObject> rewardItems = new List<GameObject>();
    private List<SociatySubTaskItem> subTaskItems = new List<SociatySubTaskItem>();

    private SociatySubTaskItem curSelItem = null;
    private List<GameObject> listSubTaskRewardObject = new List<GameObject>();
    bool    isNeedUpdateTime = false;
    GameObject questDestIconObj = null;
    int taskEndTime = 0;

    public static SociatyTaskRunning instance = null;
    public static SociatyTaskRunning Instance
    {
        get
        {
            if(null == instance)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("SociatyTaskRunning");
                instance = go.GetComponent<SociatyTaskRunning>();
            }
            return instance;
        }
    }

	// Use this for initialization
	void Start ()
    {
        commitButton.onClick.AddListener(OnCommitQuestButtonClick);
        exitTaskButton.onClick.AddListener(OnExitTaskButtonClick);
        recruitButton.onClick.AddListener(OnRecruitButtonClick);
        taskRewardButton.onClick.AddListener(OnTaskRewardButtonClick);

        myTeamText.text = StaticDataMgr.Instance.GetTextByID("sociaty_myteam1");
        teamRewardText.text = StaticDataMgr.Instance.GetTextByID("sociaty_teamreward");

        rewardLabelText.text = StaticDataMgr.Instance.GetTextByID("sociaty_teamreward");
        destLabelText.text = StaticDataMgr.Instance.GetTextByID("sociaty_tasktarget");

        UIUtil.SetButtonTitle(exitTaskButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_giveup"));
        UIUtil.SetButtonTitle(recruitButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_findfriends"));
        UIUtil.SetButtonTitle(taskRewardButton.transform, StaticDataMgr.Instance.GetTextByID("quest_lingqujiangli"));
    }

    int updateCount = 1;
    void Update()
    {
        if (isNeedUpdateTime)
        {
            updateCount++;
            if (updateCount > 20)
            {
                RefreshTimeLeft();
                updateCount = 1;
            }

        }
    }

    void RefreshTimeLeft()
    {
        int hour = 0;
        int minute = 0;
        int second = 0;

        int curTime = GameTimeMgr.Instance.GetServerTimeStamp();
        int leftSecond = taskEndTime - curTime;
        if (leftSecond > 0)
        {
            hour = leftSecond / 3600;
            minute = leftSecond / 60;
            second = leftSecond % 60;
        }
        else
        {
            isNeedUpdateTime = false;
            if (!IsTaskFinish())
            {
                MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("sociaty_taskfail"), OnTaskFaild);
            }
        }
        timeText.text = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_shengyutime"), hour, minute, second);
    }

    void OnTaskFaild(MsgBox.PrompButtonClick click)
    {
        GameDataMgr.Instance.SociatyDataMgrAttr.taskTeamId = 0;
        UISociatyTask.Instance.SetTaskType(SociatyTaskContenType.MyTeam);
    }

    public void Clear()
    {
        foreach (var memberItem in teamMemberItems)
        {
            ResourceMgr.Instance.DestroyAsset(memberItem.gameObject);
        }
        teamMemberItems.Clear();

        foreach (var rewardItem in rewardItems)
        {
            ResourceMgr.Instance.DestroyAsset(rewardItem);
        }
        rewardItems.Clear();

        foreach(var subTaskItem in subTaskItems)
        {
            ResourceMgr.Instance.DestroyAsset(subTaskItem.gameObject);
        }
        subTaskItems.Clear();
        ResetSubTasks();

        if(null != questDestIconObj)
        {
            ResourceMgr.Instance.DestroyAsset(questDestIconObj);
            questDestIconObj = null;
        }

        resetCommitObj();
    }

    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_SELF_TEAM_C.GetHashCode().ToString(), OnRequestMyTeamInfoFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_SELF_TEAM_S.GetHashCode().ToString(), OnRequestMyTeamInfoFinished);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_COMMIT_TASK_C.GetHashCode().ToString(), OnRequestCommitQuestFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_COMMIT_TASK_S.GetHashCode().ToString(), OnRequestCommitQuestFinish);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnReward);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_DISSOVLE_TEAM_C.GetHashCode().ToString(), OnRequestExitTaskFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_DISSOVLE_TEAM_S.GetHashCode().ToString(), OnRequestExitTaskFinish);

        if(GameDataMgr.Instance.SociatyDataMgrAttr.allianceInstanceReward.Count > 0)
        {
            OpenBaoxiangResult.OpenWith(GameDataMgr.Instance.SociatyDataMgrAttr.allianceInstanceReward);
            GameDataMgr.Instance.SociatyDataMgrAttr.allianceInstanceReward.Clear();
        }

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_ACCEPT_TASK_C.GetHashCode().ToString(), OnAcceptInstanceQuestFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_ACCEPT_TASK_S.GetHashCode().ToString(), OnAcceptInstanceQuestFinish);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_TASK_REWARD_C.GetHashCode().ToString(), OnRequestTaskRewardFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_TASK_REWARD_S.GetHashCode().ToString(), OnRequestTaskRewardFinish);
    }
    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_SELF_TEAM_C.GetHashCode().ToString(), OnRequestMyTeamInfoFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_SELF_TEAM_S.GetHashCode().ToString(), OnRequestMyTeamInfoFinished);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_COMMIT_TASK_C.GetHashCode().ToString(), OnRequestCommitQuestFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_COMMIT_TASK_S.GetHashCode().ToString(), OnRequestCommitQuestFinish);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnReward);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_DISSOVLE_TEAM_C.GetHashCode().ToString(), OnRequestExitTaskFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_DISSOVLE_TEAM_S.GetHashCode().ToString(), OnRequestExitTaskFinish);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_ACCEPT_TASK_C.GetHashCode().ToString(), OnAcceptInstanceQuestFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_ACCEPT_TASK_S.GetHashCode().ToString(), OnAcceptInstanceQuestFinish);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_TASK_REWARD_C.GetHashCode().ToString(), OnRequestTaskRewardFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_TASK_REWARD_S.GetHashCode().ToString(), OnRequestTaskRewardFinish);

        isNeedUpdateTime = false;
    }
	
	public void RequestMyTeamInfo()
    {
        PB.HSAllianceSelfTeam param = new PB.HSAllianceSelfTeam();
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_SELF_TEAM_C.GetHashCode(), param);
    }

    void OnRequestMyTeamInfoFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);

            GameDataMgr.Instance.SociatyDataMgrAttr.taskTeamId = 0;
            GameDataMgr.Instance.SociatyDataMgrAttr.selfTeamData = null;
            UISociatyTask.Instance.SetTaskType(SociatyTaskContenType.MyTeam);            
            return;
        }
        PB.HSAllianceSelfTeamRet msgRet = message.GetProtocolBody<PB.HSAllianceSelfTeamRet>();
        GameDataMgr.Instance.SociatyDataMgrAttr.selfTeamData = msgRet.selfTeam;
        selfTeamData = msgRet.selfTeam;
        RefreshUi();
    }
    void RefreshUi()
    {
        if (null == selfTeamData)
            return;

        bool isTaskFinish = IsTaskFinish();

       
        bool isSelfCaptain = SelfIsCaptain();
        bool showExitTaskButton = false;
        if (isSelfCaptain && !isTaskFinish)
        {
            if(selfTeamData.members.Count == 1)
            {
                showExitTaskButton = true;
            }
        }

        exitTaskButton.gameObject.SetActive(showExitTaskButton);
        recruitButton.gameObject.SetActive(isSelfCaptain);
        taskRewardButton.gameObject.SetActive(isTaskFinish);

        finishText.text = "";
        ResetSubTasks();
        resetCommitObj();
        RefreshTeamMember();
        RefreshSubTasks();
        RefreshTaskReward();

        var taskInfo = StaticDataMgr.Instance.GetSociatyTask(selfTeamData.taskId);
        taskEndTime = taskInfo.time * 60 + selfTeamData.startTime;
        RefreshTimeLeft();
        isNeedUpdateTime = true;
    }

    void RefreshTeamMember()
    {
        foreach(var item in teamMemberItems)
        {
            item.gameObject.SetActive(false);
        }

       for(int i=0;i<selfTeamData.members.Count;++i)
        {
            var submemberData = selfTeamData.members[i];
            SociatyTeamMemberItem teamMemberItem = null;
            if (i < teamMemberItems.Count)
            {
                teamMemberItem = teamMemberItems[i];
                teamMemberItem.gameObject.SetActive(true);
                teamMemberItem.RefreshWith(submemberData);
            }
            else
            {
                teamMemberItem = SociatyTeamMemberItem.CreateWith(submemberData);
                teamMemberItems.Add(teamMemberItem);

                teamMemberItem.transform.SetParent(teamTransform);
                teamMemberItem.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
    
    void RefreshSubTasks()
    {
       foreach(var item in subTaskItems)
        {
            item.gameObject.SetActive(false);
        }
        for (int i = 0; i < selfTeamData.questInfos.Count; ++i)
        {
            var subQuestData = selfTeamData.questInfos[i];
            SociatySubTaskItem subItem = null;
            if(i < subTaskItems.Count)
            {
                subItem = subTaskItems[i];
                subItem.gameObject.SetActive(true);
                subItem.RefreshWith(subQuestData);
            }
            else
            {
                subItem = SociatySubTaskItem.CreateWith(subQuestData);
                subTaskItems.Add(subItem);

                subItem.transform.SetParent(taskPanel);
                subItem.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    void RefreshTaskReward()
    {
        foreach (var rewardItem in rewardItems)
        {
            ResourceMgr.Instance.DestroyAsset(rewardItem);
        }
        rewardItems.Clear();

        SociatyTask taskData = StaticDataMgr.Instance.GetSociatyTask(selfTeamData.taskId);
        if (null != taskData)
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

    public void OnSubTaskItemSelected(SociatySubTaskItem selectItem)
    {
        if(curSelItem != null)
        {
            curSelItem.SetSelect(false);
        }
        curSelItem = selectItem;
        curSelItem.SetSelect(true);
        ShowSubTaskReward(selectItem.questInfo);
        RefreshQuestDest(selectItem.questInfo);

        if(curSelItem.questInfo.playerId > 0)
        {
            commitButton.gameObject.SetActive(false);
            finishText.text = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_completename"), curSelItem.questInfo.nickname);
        }
        else
        {
            finishText.text = "";
        }
    }

   void ShowSubTaskReward(PB.AllianceTeamQuestInfo questData)
    {
        ResetSubTasks();

        SociatyQuest questStData = StaticDataMgr.Instance.GetSociatyQuest(questData.questId);
        if (null == questData)
            return;

        RewardData rewardData = StaticDataMgr.Instance.GetRewardData(questStData.rewardId);
        if (rewardData == null || rewardData.itemList == null)
            return;

        RewardItemData subRewardData = null;
        for(int i =0;i < 4 && i<rewardData.itemList.Count;++i)
        {
            subRewardData = rewardData.itemList[i];
            GameObject go = RewardItemCreator.CreateRewardItem(subRewardData.protocolData, subRewardTrans, true, false);
            if (null != go)
            {
                go.transform.localScale = new Vector3(1, 1, 1);
                listSubTaskRewardObject.Add(go);
            }
        }
        destValueText.text = StaticDataMgr.Instance.GetTextByID(questStData.name);
    }
    void ResetSubTasks()
    {
        foreach (var subObj in listSubTaskRewardObject)
        {
            ResourceMgr.Instance.DestroyAsset(subObj);
        }
        listSubTaskRewardObject.Clear();

        destValueText.text = "";
    }

    
    void RefreshQuestDest(PB.AllianceTeamQuestInfo questData)
    {
        SociatyQuest questStData = StaticDataMgr.Instance.GetSociatyQuest(questData.questId);
        if (null == questStData)
            return;
        resetCommitObj();
         SociatyQuestType questType = (SociatyQuestType)questStData.goalType;

        switch(questType)
        {
            case SociatyQuestType.CommitCoin:
                jinbiObj.SetActive(true);
                itemCountText.text = string.Format("{0}/{1}", questStData.goalCount, GameDataMgr.Instance.PlayerDataAttr.coin);
                if(questStData.goalCount > GameDataMgr.Instance.PlayerDataAttr.coin)
                {
                    itemCountText.color = new Color(1, 0, 0);
                }
                else
                {
                    itemCountText.color = new Color(251.0f / 255.0f, 241.0f / 255.0f, 216.0f / 255.0f);
                    commitButton.gameObject.SetActive(true);
                    UIUtil.SetButtonTitle(commitButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_commit"));
                }
                
                break;
            case SociatyQuestType.CommitDrop:
                ItemData goalItemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(questStData.goalParam);
                int itemCount = 0;
                if(goalItemData != null)
                {
                    itemCount = goalItemData.count;
                }
                else
                {
                    goalItemData = ItemData.valueof(questStData.goalParam,0);
                }
                ItemIcon icon = ItemIcon.CreateItemIcon(goalItemData, true, false);
                icon.transform.SetParent(taskNeedItemParent);
                // icon.transform.localPosition
                float parentWith = ((RectTransform)taskNeedItemParent).rect.width;
                float iconWith = ((RectTransform)icon.transform).rect.width;
                float fScale = parentWith / iconWith;

                ((RectTransform)icon.transform).anchoredPosition = new Vector2(0, 0);
                icon.transform.localScale = new Vector3(fScale, fScale, fScale);
                questDestIconObj = icon.gameObject;

                itemCountText.text = string.Format("{0}/{1}", questStData.goalCount, itemCount);
                if (questStData.goalCount > itemCount)
                {
                    itemCountText.color = new Color(1, 0, 0);
                }
                else
                {
                    itemCountText.color = new Color(251.0f / 255.0f, 241.0f / 255.0f, 216.0f / 255.0f);
                    commitButton.gameObject.SetActive(true);
                    UIUtil.SetButtonTitle(commitButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_commit"));
                }
                break;
            case SociatyQuestType.CommitInstance:
                commitButton.gameObject.SetActive(true);
                instanceObj.SetActive(true);
                UIUtil.SetButtonTitle(commitButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_gogogo"));
                break;
        }
    }

    void resetCommitObj()
    {
        jinbiObj.SetActive(false);
        instanceObj.SetActive(false);
        if (null != questDestIconObj)
        {
            ResourceMgr.Instance.DestroyAsset(questDestIconObj);
            questDestIconObj = null;
        }
        itemCountText.text = "";
        commitButton.gameObject.SetActive(false);
    }

    void OnCommitQuestButtonClick()
    {
        if (null == curSelItem)
            return;
        SociatyQuest questStData = StaticDataMgr.Instance.GetSociatyQuest(curSelItem.questInfo.questId);
        if (null == questStData)
            return;
        SociatyQuestType questType = (SociatyQuestType)questStData.goalType;

        if(questType == SociatyQuestType.CommitInstance)
        {
            // UIAdjustBattleTeam.OpenWith(questStData.goalParam, 0, InstanceType.Guild);
            PB.HSAllianceTaskAccept acceptParam  = new PB.HSAllianceTaskAccept();
            acceptParam.questId = curSelItem.questInfo.questId;
            GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_ACCEPT_TASK_C.GetHashCode(), acceptParam);
            return;
        }
        PB.HSAllianceTaskCommit param = new PB.HSAllianceTaskCommit();
        param.questId = curSelItem.questInfo.questId;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_COMMIT_TASK_C.GetHashCode(), param);
    }

    void OnAcceptInstanceQuestFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);

            RequestMyTeamInfo();
            return;
        }

        SociatyQuest questStData = StaticDataMgr.Instance.GetSociatyQuest(curSelItem.questInfo.questId);
        if (null == questStData)
            return;
        SociatyQuestType questType = (SociatyQuestType)questStData.goalType;
        UIAdjustBattleTeam.OpenWith(questStData.goalParam, 0, InstanceType.Guild);
    }

    void OnRequestCommitQuestFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);

            return;
        }
        curSelItem.SetFinish(GameDataMgr.Instance.PlayerDataAttr.playerId);
        OnSubTaskItemSelected(curSelItem);
    }
    void OnReward(ProtocolMessage msg)
    {
        PB.HSRewardInfo reward = msg.GetProtocolBody<PB.HSRewardInfo>();
        if (reward == null )
            return;
        if( reward.hsCode == PB.code.ALLIANCE_COMMIT_TASK_C.GetHashCode() ||
            reward.hsCode == PB.code.ALLIANCE_TASK_REWARD_C.GetHashCode())
        {
            List<PB.HSRewardInfo> listReward = new List<PB.HSRewardInfo>();
            listReward.Add(reward);
            OpenBaoxiangResult.OpenWith(listReward);
        }
        
    }

    void OnExitTaskButtonClick()
    {
        MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel, StaticDataMgr.Instance.GetTextByID("sociaty_giveuptips"), 
            StaticDataMgr.Instance.GetTextByID("sociaty_giveuptips1"), OnConformExitTask);
    }

    void OnConformExitTask(MsgBox.PrompButtonClick click)
    {
        if(click == MsgBox.PrompButtonClick.OK)
        {
            PB.HSAllianceDissolveTeam param = new PB.HSAllianceDissolveTeam();
            GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_DISSOVLE_TEAM_C.GetHashCode(), param);
        }
    }

    void OnRequestExitTaskFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        GameDataMgr.Instance.SociatyDataMgrAttr.taskTeamId = 0;
        UISociatyTask.Instance.SetTaskType(SociatyTaskContenType.MyTeam);
    }

    void OnRecruitButtonClick()
    {
        if(selfTeamData.members.Count >= GameConfig.Instance.sociatyTeamMaxMember)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_044"), (int)PB.ImType.PROMPT);
            return;
        }
        string sendMsg = StaticDataMgr.Instance.GetTextByID("sociaty_record_052");
        bool issend = UIIm.Instance.OnSendMsg(sendMsg, ImMessageType.Msg_Type_Task, GameDataMgr.Instance.SociatyDataMgrAttr.taskTeamId.ToString());
       // if (issend)
       // {
          //  MsgBox.InputConform.Close();
       // }
    }
    void OnTaskRewardButtonClick()
    {
        PB.HSAllianceTaskReward param = new PB.HSAllianceTaskReward();
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_TASK_REWARD_C.GetHashCode(), param);
    }

    void OnRequestTaskRewardFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        GameDataMgr.Instance.SociatyDataMgrAttr.taskTeamId = 0;
        UISociatyTask.Instance.SetTaskType(SociatyTaskContenType.MyTeam);
    }

    bool SelfIsCaptain()
    {
        if (selfTeamData == null)
            return false;
        foreach(var subMember in selfTeamData.members)
        {
            if(subMember.isCaptain)
            {
                return (subMember.playerId == GameDataMgr.Instance.PlayerDataAttr.playerId);
            }
        }
        return false;
    }

    bool IsTaskFinish()
    {
        foreach(var subQuest in selfTeamData.questInfos)
        {
            if (subQuest.playerId < 1)
                return false;
        }
        return true;
    }
}
