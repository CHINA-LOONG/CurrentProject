using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SociatyTaskOther : MonoBehaviour
{
    public Text timesText;
    public ScrollView scrollView;

    private SociatyDataMgr sociatyDataMgr;

    private List<SociatyOtherItem> listOtherTaskCatche = new List<SociatyOtherItem>();

    private static SociatyTaskOther instance = null;
    private int topOtherTeamId = -1;

    public void SetTopTeamId(int oherTeamId)
    {
        topOtherTeamId = oherTeamId;
    }
    public static SociatyTaskOther Instance
    {
        get
        {
            if(null == instance)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("SociatyTaskOther");
                SociatyTaskOther taskOther = go.GetComponent<SociatyTaskOther>();
                instance = taskOther;
            }
            return instance;
        }
    }

	// Use this for initialization
	void Start ()
    {
        sociatyDataMgr = GameDataMgr.Instance.SociatyDataMgrAttr;
	}
	void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_TEAM_LIST_C.GetHashCode().ToString(), OnRequestTeamListFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_TEAM_LIST_S.GetHashCode().ToString(), OnRequestTeamListFinish);

     //   GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_JOIN_TEAM_C.GetHashCode().ToString(), OnRequestJoinTeamFinish);
       // GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_JOIN_TEAM_S.GetHashCode().ToString(), OnRequestJoinTeamFinish);
    }
    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_TEAM_LIST_C.GetHashCode().ToString(), OnRequestTeamListFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_TEAM_LIST_S.GetHashCode().ToString(), OnRequestTeamListFinish);

       // GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_JOIN_TEAM_C.GetHashCode().ToString(), OnRequestJoinTeamFinish);
       // GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_JOIN_TEAM_S.GetHashCode().ToString(), OnRequestJoinTeamFinish);
    }

    public  void RequestTeamList()
    {
        PB.HSAllianceTeamList param = new PB.HSAllianceTeamList();
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_TEAM_LIST_C.GetHashCode(), param);
    }

    void OnRequestTeamListFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
           PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        PB.HSAllianceTeamListRet msgRet = message.GetProtocolBody<PB.HSAllianceTeamListRet>();
        sociatyDataMgr.teamList.Clear();
        sociatyDataMgr.teamList.AddRange(msgRet.allianceTeams);
        //排序
        sociatyDataMgr.teamList.Sort(SortTeamInfo);
        //置顶
        if(topOtherTeamId > -1 )
        {
            PB.AllianceTeamInfo topTeam = null;
            for(int i = 0;i<sociatyDataMgr.teamList.Count;++i)
            {
                if (sociatyDataMgr.teamList[i].teamId == topOtherTeamId)
                {
                    topTeam = sociatyDataMgr.teamList[i];
                    sociatyDataMgr.teamList.Remove(topTeam);
                    sociatyDataMgr.teamList.Insert(0, topTeam);
                    break;
                }
            }
            topOtherTeamId = -1;
        }
        RefreshUi();
    }
    int SortTeamInfo(PB.AllianceTeamInfo itemA, PB.AllianceTeamInfo itemB)
    {
        if (itemA.startTime > itemB.startTime)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    void RefreshUi()
    {
        timesText.text = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_tasknum"),
            GameConfig.Instance.sociatyTaskMaxCount - GameDataMgr.Instance.SociatyDataMgrAttr.taskCount);

        HideAllOtherTask();
        List<PB.AllianceTeamInfo> teamList = sociatyDataMgr.teamList;
        for(int i =0;i<teamList.Count;++i)
        {
            SociatyOtherItem subItem = null;
            if (i < listOtherTaskCatche.Count)
            {
                subItem = listOtherTaskCatche[i];
                subItem.gameObject.SetActive(true);
                subItem.RefreshWith(teamList[i]);
            }
            else
            {
                subItem = SociatyOtherItem.CreateWith(teamList[i]);
                scrollView.AddElement(subItem.gameObject);
                listOtherTaskCatche.Add(subItem);
            }
        }
    }

    void HideAllOtherTask()
    {
        foreach(var subItem in listOtherTaskCatche)
        {
            subItem.gameObject.SetActive(false);
        }
    }
/*
    //////////////////////////////////////////////////////////////////
    void RequestJoinTeam()
    {
        PB.HSAllianceJoinTeam param = new PB.HSAllianceJoinTeam();
        param.teamId = 0;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_TEAM_LIST_C.GetHashCode().ToString(), param);
    }

    void OnRequestJoinTeamFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            // PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();

            return;
        }
    }
    */
}
