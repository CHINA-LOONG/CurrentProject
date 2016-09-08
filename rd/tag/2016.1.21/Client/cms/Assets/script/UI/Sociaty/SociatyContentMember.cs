using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SociatyContentMember : SociatyContentBase
{
    public Text[] titleTextArray;
    public ScrollView scrollView;
    public Button exitSociatyButton;
    public Button newApplayButton;

    private SociatyDataMgr sociatyDataMgr = null;

    private List<MemberItem> listMemberItemCatch = new List<MemberItem>();

    public static SociatyContentMember Instance = null;
	// Use this for initialization
	void Start ()
    {
        Instance = this;
        exitSociatyButton.onClick.AddListener(OnExitSociatyButtonClick);
        newApplayButton.onClick.AddListener(OnNewApplayButtonClick);

        titleTextArray[0].text = StaticDataMgr.Instance.GetTextByID("sociaty_membername");
        titleTextArray[1].text = StaticDataMgr.Instance.GetTextByID("sociaty_level");
        titleTextArray[2].text = StaticDataMgr.Instance.GetTextByID("sociaty_job");
        titleTextArray[3].text = StaticDataMgr.Instance.GetTextByID("sociaty_contributionall");
        titleTextArray[4].text = StaticDataMgr.Instance.GetTextByID("sociaty_last");
        titleTextArray[5].text = StaticDataMgr.Instance.GetTextByID("sociaty_songhuo");

        UIUtil.SetButtonTitle(exitSociatyButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_quit"));
        UIUtil.SetButtonTitle(newApplayButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_newapply"));
    }

    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_MEMBERS_C.GetHashCode().ToString(), OnRequestMemberDataFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_MEMBERS_S.GetHashCode().ToString(), OnRequestMemberDataFinish);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_MEMBER_LEAVE_C.GetHashCode().ToString(), OnRequestExitSociatyFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_MEMBER_LEAVE_S.GetHashCode().ToString(), OnRequestExitSociatyFinish);
    }

    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_MEMBERS_C.GetHashCode().ToString(), OnRequestMemberDataFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_MEMBERS_S.GetHashCode().ToString(), OnRequestMemberDataFinish);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_MEMBER_LEAVE_C.GetHashCode().ToString(), OnRequestExitSociatyFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_MEMBER_LEAVE_S.GetHashCode().ToString(), OnRequestExitSociatyFinish);
    }
	
    public override void RefreshUI()
    {
        sociatyDataMgr = GameDataMgr.Instance.SociatyDataMgrAttr;
        RequestMemberData();
        //CheckAndRequestMemberData();
    }

    void CheckAndRequestMemberData()
    {
        if(sociatyDataMgr.allianceMemberList.Count == 0)
        {
            RequestMemberData();
        }
        else
        {
            int curTime = GameTimeMgr.Instance.TimeStamp();
            if(curTime - sociatyDataMgr.lastSyncAllianceMemberTime > 600)
            {
                RequestMemberData();
            }
            else
            {
                UpdateMemberItems();
            }
        }
    }

    public  void RequestMemberData()
    {
        PB.HSAllianceMembers param = new PB.HSAllianceMembers();
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_MEMBERS_C.GetHashCode(), param);
    }

    void OnRequestMemberDataFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSAllianceMembersRet memberRet = message.GetProtocolBody<PB.HSAllianceMembersRet>();
        sociatyDataMgr.allianceMemberList.Clear();
        sociatyDataMgr.allianceMemberList.AddRange(memberRet.memberList);
        sociatyDataMgr.lastSyncAllianceMemberTime = GameTimeMgr.Instance.TimeStamp();
        UpdateMemberItems();
    }

    public  void UpdateMemberItems()
    {
        HideAllMemberItem();
        sociatyDataMgr.allianceMemberList.Sort(SortMembers);
        for(int i =0; i<sociatyDataMgr.allianceMemberList.Count;++i)
        {
            CreateMemberItem(i, sociatyDataMgr.allianceMemberList[i]);
        }
    }
    int SortMembers(PB.AllianceMember itemA,PB.AllianceMember itemB)
    {
        if(itemA.postion > itemB.postion)
        {
            return -1;
        }
        else if(itemA.postion < itemB.postion)
        {
            return 1;
        }
        else
        {
           if( itemA.totalContribution > itemB.totalContribution)
            {
                return -1;
            }
           else if(itemA.totalContribution < itemB.totalContribution)
            {
                return 1;
            }
        }
        return 0;
    }

    void HideAllMemberItem()
    {
        foreach(var subItem in listMemberItemCatch)
        {
            subItem.gameObject.SetActive(false);
        }
    }

    MemberItem  CreateMemberItem(int index,PB.AllianceMember memberData)
    {
        MemberItem item = null;
        if (index < listMemberItemCatch.Count )
        {
            item = listMemberItemCatch[index];
            item.RefreshWith(memberData);
            item.gameObject.SetActive(true);
        }
        else
        {
            item = MemberItem.CreateWith(memberData);
            scrollView.AddElement(item.gameObject);
            listMemberItemCatch.Add(item);
        }
        return item;
    }
    ///-------------------------------------------------------------------------------------------------
    void OnExitSociatyButtonClick()
    {
        if(sociatyDataMgr.allianceSelfData.postion ==2 &&
            sociatyDataMgr.allianceMemberList.Count > 1)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_019"), (int)PB.ImType.PROMPT);
            return;
        }
        string title = StaticDataMgr.Instance.GetTextByID("sociaty_tips6");
        string msg = StaticDataMgr.Instance.GetTextByID("sociaty_tips2");
        MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel,title, msg, OnConformExitSociaty);
    }

    void OnConformExitSociaty(MsgBox.PrompButtonClick click)
    {
        if(click == MsgBox.PrompButtonClick.OK)
        {
            RequestExitSociaty();
        }
    }

    void RequestExitSociaty()
    {
        PB.HSAllianceLeave param = new PB.HSAllianceLeave();
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_MEMBER_LEAVE_C.GetHashCode(), param);
    }

    void OnRequestExitSociatyFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(error.errCode);
            return;
        }
        sociatyDataMgr.ClearData();
        SociatyMain.Instance.Close();
        UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_020"), (int)PB.ImType.PROMPT);
    }

    ///-------------------------------------------------------------------------------------------------
    void OnNewApplayButtonClick()
    {
        MemberApply.Open();
    }
}
