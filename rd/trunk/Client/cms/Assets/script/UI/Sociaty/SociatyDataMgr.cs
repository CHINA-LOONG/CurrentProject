using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void NetMessageDelegate(ProtocolMessage msg);
public delegate void AllianceBaseMonsterDelegage(List<PB.AllianceBaseMonster> listData);
public class SociatyDataMgr : MonoBehaviour
{
    public int allianceID = 0;
    public int allianceParyCount = 0;
    public PB.AllianceInfo allianceData = new PB.AllianceInfo();
    public PB.AllianceMember allianceSelfData = new PB.AllianceMember();
    public bool[] hasReceivContributionReword = new bool[3];

    public List<PB.AllianceMember> allianceMemberList = new List<PB.AllianceMember>();
    public int lastSyncAllianceMemberTime = 0;

    public List<PB.AllianceApply> newApplyList = new List<PB.AllianceApply>();

    //task
    public int taskTeamId = 0;
    public PB.AllianceTeamInfo selfTeamData = null;
    public int taskCount = 0;
    public List<PB.AllianceTeamInfo> teamList = new List<PB.AllianceTeamInfo>();
    public List<PB.HSRewardInfo> allianceInstanceReward = new List<PB.HSRewardInfo>();

    //jidi
    public List<PB.AllianceBaseMonster> allianceBaseMonster = new List<PB.AllianceBaseMonster>();
    private int syncBaseMonsterTime = 0;

    private NetMessageDelegate callBack = null;
    private AllianceBaseMonsterDelegage baseMonsterCallback = null;
    // Use this for initialization
    void Start ()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_DATA_S.GetHashCode().ToString(), OnAllianceDataSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_SELF_DATA_S.GetHashCode().ToString(),OnSelfAllianceDataSync);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_APPLY_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_APPLY_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CANCLE_APPLY_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CANCLE_APPLY_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_NOTICE_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_NOTICE_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CONTRI_REWARD_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CONTRI_REWARD_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_HANDLE_APPLY_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_HANDLE_APPLY_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_FATIGUE_GIVE_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_FATIGUE_GIVE_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_JOIN_TEAM_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_JOIN_TEAM_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_JOIN_N_S.GetHashCode().ToString(), OnAllianceJoin_N_S);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_LEAVE_N_S.GetHashCode().ToString(), OnAllianceLeave_N_S);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_TASK_TIMEOUT_N_S.GetHashCode().ToString(), OnTaskTimeOut_N_S);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_BASE_LIST_C.GetHashCode().ToString(), OnRequestBaseMOnstersFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_BASE_LIST_S.GetHashCode().ToString(), OnRequestBaseMOnstersFinish);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_BASE_SEND_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_BASE_SEND_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_BASE_RECALL_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_BASE_RECALL_S.GetHashCode().ToString(), OnReceivSociatyMessage);
    }


    void OnDestroy()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_DATA_S.GetHashCode().ToString(), OnAllianceDataSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_SELF_DATA_S.GetHashCode().ToString(),OnSelfAllianceDataSync);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_APPLY_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_APPLY_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CANCLE_APPLY_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CANCLE_APPLY_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_NOTICE_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_NOTICE_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CONTRI_REWARD_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CONTRI_REWARD_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_HANDLE_APPLY_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_HANDLE_APPLY_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_FATIGUE_GIVE_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_FATIGUE_GIVE_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_JOIN_TEAM_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_JOIN_TEAM_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_JOIN_N_S.GetHashCode().ToString(), OnAllianceJoin_N_S);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_LEAVE_N_S.GetHashCode().ToString(), OnAllianceLeave_N_S);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_TASK_TIMEOUT_N_S.GetHashCode().ToString(), OnTaskTimeOut_N_S);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_BASE_LIST_C.GetHashCode().ToString(), OnRequestBaseMOnstersFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_BASE_LIST_S.GetHashCode().ToString(), OnRequestBaseMOnstersFinish);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_BASE_SEND_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_BASE_SEND_S.GetHashCode().ToString(), OnReceivSociatyMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_BASE_RECALL_C.GetHashCode().ToString(), OnReceivSociatyMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_BASE_RECALL_S.GetHashCode().ToString(), OnReceivSociatyMessage);
    }
	
    //打开公会
    public  void    OpenSociaty(string search = null)
    {
        if(allianceID < 1)
        {
            SociatyList.OpenWith(search);
        }
        else
        {
            if(string.IsNullOrEmpty(search))
            {
                SociatyMain.OpenWith();
            }
            else
            {
                SociatyMain.OpenWith(SociatyContenType.OtherSociaty,search);
            }
        }
    }

    public void OpenSociatyTaskWithTeam(SociatyTaskContenType taskType,string otherTeamId = null)
    {
        UISociatyTask.Open(taskType, otherTeamId);
    }

    public void ClearSociaty()
    {
        allianceMemberList.Clear();
        allianceID = 0;
    }

    void OnAllianceDataSync(ProtocolMessage msg)
    {
        PB.HSAllianceDataRet data = msg.GetProtocolBody<PB.HSAllianceDataRet>();
        if(null != data)
        {
            allianceData = data.allianceData;
        }
    }

    void OnSelfAllianceDataSync(ProtocolMessage msg)
    {
        PB.HSAllianceSelfDataRet selfData = msg.GetProtocolBody<PB.HSAllianceSelfDataRet>();
        if(selfData != null)
        {
            allianceParyCount = selfData.prayCount;
            allianceSelfData = selfData.selfData;
            hasReceivContributionReword[0] = (selfData.contributionReward & 1) == 1;
            hasReceivContributionReword[1] = (selfData.contributionReward & 2) == 2;
            hasReceivContributionReword[2] = (selfData.contributionReward & 4) == 4;

            taskTeamId = selfData.teamID;
            taskCount = selfData.taskCount;
        }
    }


    void OnReceivSociatyMessage(ProtocolMessage msg)
    {
        if(null != callBack)
        {
            callBack(msg);
        }
    }

    public  void RequestCancelApply(int allianceId, NetMessageDelegate callBack)
    {
        PB.HSAllianceCancleApply param = new PB.HSAllianceCancleApply();
        param.allianceId = allianceId;
        this.callBack = callBack;

        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_CANCLE_APPLY_C.GetHashCode(), param);
    }


    public  void RequestApply(int allianceId, NetMessageDelegate callBack)
    {
        PB.HSAllianceApply param = new PB.HSAllianceApply();
        param.allianceId = allianceId;
        this.callBack = callBack;

        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_APPLY_C.GetHashCode(), param);
    }

    public void RequestModifyNotify(string newNotify, NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSAllianceNotice param = new PB.HSAllianceNotice();
        param.notice = newNotify;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_NOTICE_C.GetHashCode(), param);
    }

    public  void    RequestContributionReward(int index, NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSAllianceContriReward param = new PB.HSAllianceContriReward();
        param.index = index;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_CONTRI_REWARD_C.GetHashCode(), param);
    }

    public void RequestApplyOperate(int playerId,bool accept,bool isAll, NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSAllianceHanleApply param = new PB.HSAllianceHanleApply();
        param.accept = accept;
        param.isAll = isAll;
        if(!isAll)
        {
            param.playerId = playerId;
        }
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_HANDLE_APPLY_C.GetHashCode(), param);
    }

    public void RequestSendHuoli(int playerId, NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSAllianceFatigueGive param = new PB.HSAllianceFatigueGive();
        param.targetId = playerId;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_FATIGUE_GIVE_C.GetHashCode(), param);
    }

    public void RequestJoinTeam(int teamId,NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSAllianceJoinTeam param = new PB.HSAllianceJoinTeam();
        param.teamId = teamId;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_JOIN_TEAM_C.GetHashCode(), param);
    }

    void OnAllianceJoin_N_S(ProtocolMessage message)
    {
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSAllianceJoinNotify msgRet = message.GetProtocolBody<PB.HSAllianceJoinNotify>();
        GameDataMgr.Instance.SociatyDataMgrAttr.allianceID = msgRet.allianceId;
    }

    void OnAllianceLeave_N_S(ProtocolMessage message)
    {
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        GameDataMgr.Instance.SociatyDataMgrAttr.allianceID = 0;
    }
    void OnTaskTimeOut_N_S(ProtocolMessage message)
    {
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        GameDataMgr.Instance.SociatyDataMgrAttr.taskTeamId = 0;
    }

    public  void SetMemberPosition(int playerId,int position)
    {
        foreach(var subMember in allianceMemberList)
        {
            if(subMember.id == playerId)
            {
                subMember.postion = position;
                break;
            }
        }
    }

    public string GetPositionDesc(int position)
    {
        switch(position)
        {
            case 0:
                return StaticDataMgr.Instance.GetTextByID("sociaty_member1");
            case 1:
                return StaticDataMgr.Instance.GetTextByID("sociaty_vicechairman");
            case 2:
                return StaticDataMgr.Instance.GetTextByID("sociaty_chairman");
        }
        return "";
    }

    #region --------公会基地----------
    public void GetJidiBaseMonstersAsyn(AllianceBaseMonsterDelegage callBack)
    {
        baseMonsterCallback = callBack;
        if(allianceBaseMonster.Count > 0)
        {
            if(GameTimeMgr.Instance.GetServerTimeStamp() - syncBaseMonsterTime > 3600)//1 hour
            {
                RequestBaseMonsters();
                return;
            }
        }
        if (null != baseMonsterCallback)
        {
            baseMonsterCallback(allianceBaseMonster);
        }
    }

    void RequestBaseMonsters()
    {
        PB.HSAllianceBaseList param = new PB.HSAllianceBaseList();
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_BASE_LIST_C.GetHashCode(), param);
    }
    void OnRequestBaseMOnstersFinish(ProtocolMessage message)
    {
        if (message.GetMessageType() != (int)PB.sys.ERROR_CODE)
        {
            PB.HSAllianceBaseListRet msgRet = message.GetProtocolBody<PB.HSAllianceBaseListRet>();
            if (null != msgRet)
            {
                allianceBaseMonster.Clear();
                allianceBaseMonster.AddRange(msgRet.monsterInfo);
                syncBaseMonsterTime = GameTimeMgr.Instance.GetServerTimeStamp();
            }
        }

        if (null != baseMonsterCallback)
        {
            baseMonsterCallback(allianceBaseMonster);
        }
    }

    public  void RequestZhushou(int monsterId,int position, NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSAllianceBaseSendMonster param = new PB.HSAllianceBaseSendMonster();
        param.monsterId = monsterId;
        param.position = position;//
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_BASE_SEND_C.GetHashCode(), param);
    }

    public void RequestRecallZhushou(int position,NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSAllianceBaseRecallMonster param = new PB.HSAllianceBaseRecallMonster();
        param.position = position;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_BASE_RECALL_C.GetHashCode(), param);
    }

    #endregion
}
