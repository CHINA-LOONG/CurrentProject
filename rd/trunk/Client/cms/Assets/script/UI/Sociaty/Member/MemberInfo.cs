using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MemberInfo : UIBase
{
    public static string ViewName = "MemberInfo";
    public Image headImage;
    public Text nameText;
    public Text levelText;

    public Button closebutton;
    public Button transferCaptionButton;
    public Button appointmentButton;
    public Button kickButton;

    private PB.AllianceMember memberData;
    private PB.AllianceMember selfData;

    public static void OpenWith(PB.AllianceMember allianceMember)
    {
        MemberInfo memberInfo = (MemberInfo)UIMgr.Instance.OpenUI_(ViewName);
        memberInfo.InitWith(allianceMember);
    }

	// Use this for initialization
	void Start ()
    {
        closebutton.onClick.AddListener(OnCloseButtonClick);
        transferCaptionButton.onClick.AddListener(OnTransferButtonClick);
        appointmentButton.onClick.AddListener(OnAppointmentButtonClick);
        kickButton.onClick.AddListener(OnKichButton);

        UIUtil.SetButtonTitle(transferCaptionButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_yield"));
        
        UIUtil.SetButtonTitle(kickButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_expel"));
    }

    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CHANGE_OWNER_C.GetHashCode().ToString(), OnRequestTransferFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CHANGE_OWNER_S.GetHashCode().ToString(), OnRequestTransferFinished);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CHANGE_POS_C.GetHashCode().ToString(), OnRequestAppointmentFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CHANGE_POS_S.GetHashCode().ToString(), OnRequestAppointmentFinished);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_MEMBER_KICK_C.GetHashCode().ToString(), OnRequestKichFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_MEMBER_KCIK_S.GetHashCode().ToString(), OnRequestKichFinished);
    }

    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CHANGE_OWNER_C.GetHashCode().ToString(), OnRequestTransferFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CHANGE_OWNER_S.GetHashCode().ToString(), OnRequestTransferFinished);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CHANGE_POS_C.GetHashCode().ToString(), OnRequestAppointmentFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CHANGE_POS_S.GetHashCode().ToString(), OnRequestAppointmentFinished);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_MEMBER_KICK_C.GetHashCode().ToString(), OnRequestKichFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_MEMBER_KCIK_S.GetHashCode().ToString(), OnRequestKichFinished);
    }

    public void InitWith(PB.AllianceMember allianceMember)
    {
        memberData = allianceMember;
        selfData = GameDataMgr.Instance.SociatyDataMgrAttr.allianceSelfData;
        if(memberData.postion == 0 || memberData.postion == 2)
        {
            UIUtil.SetButtonTitle(appointmentButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_yieldfu"));
        }
        else
        {
            UIUtil.SetButtonTitle(appointmentButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_yieldfujie"));
        }
        nameText.text = memberData.name;
        if(null != levelText)
             levelText.text = memberData.level.ToString();
    }

    void OnCloseButtonClick()
    {
        RequestCloseUi();
    }
    ///-----------------------------------------------------------------------------------
    void OnTransferButtonClick()
    { 
        string msg = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_tips3"), memberData.name);
        MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel, msg, OnConformTransfer);
    }

    void OnConformTransfer(MsgBox.PrompButtonClick click)
    {
        if(click == MsgBox.PrompButtonClick.OK)
        {
            if (selfData.postion != 2)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_018"), (int)PB.ImType.PROMPT);
                return;
            }
            RequestTransfer();
        }
    }

    void RequestTransfer()
    {
        PB.HSAllianceChangeOwner param = new PB.HSAllianceChangeOwner();
        param.targetId = memberData.id;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_CHANGE_OWNER_C.GetHashCode(), param);
    }

    void OnRequestTransferFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(error.errCode);
        }
        else
        {
            selfData.postion = 0;
            memberData.postion = 2;
            GameDataMgr.Instance.SociatyDataMgrAttr.SetMemberPosition(selfData.id, 0);
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_013"), (int)PB.ImType.PROMPT);
            SociatyContentMember.Instance.UpdateMemberItems();
            OnCloseButtonClick();
        }
    }


    ///-----------------------------------------------------------------------------------
    void OnAppointmentButtonClick()
    {
        string msg = "";
        if(memberData.postion == 1)
        {
            msg = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_jiechu"), memberData.name);
        }
        else
        {
            msg = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_tips4"), memberData.name);
        }
        
        MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel, msg, OnConformAppointment);
    }
    
    void OnConformAppointment(MsgBox.PrompButtonClick click)
    {
        if(click == (int) MsgBox.PrompButtonClick.OK)
        {
            if (selfData.postion != 2)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_018"), (int)PB.ImType.PROMPT);
                return;
            }

            RequestAppointment();
        }
    }

    void RequestAppointment()
    {
        PB.HSAllianceChangePos param = new PB.HSAllianceChangePos();
        param.playerId = memberData.id;
        if(memberData.postion == 0)
        {
            param.postion = 1;
        }
        else
        {
            param.postion = 0;
        }

        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_CHANGE_POS_C.GetHashCode(), param);
    }

    void OnRequestAppointmentFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(error.errCode);
        }
        else
        {
            if (memberData.postion == 0)
            {
                memberData.postion = 1;
                string msg = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_record_016"), memberData.name);
                UIIm.Instance.ShowSystemHints(msg, (int)PB.ImType.PROMPT);
            }
            else
            {
                memberData.postion = 0;
                string msg = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_record_041"), memberData.name);
                UIIm.Instance.ShowSystemHints(msg, (int)PB.ImType.PROMPT);
            }
           
            SociatyContentMember.Instance.UpdateMemberItems();
            OnCloseButtonClick();
        }
    }
    ///-----------------------------------------------------------------------------------
    void OnKichButton()
    {
        string msg = string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_tips5"), memberData.name);
        MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel, msg, OnKichAppointment);
    }
    void OnKichAppointment(MsgBox.PrompButtonClick click)
    {
        if(click == MsgBox.PrompButtonClick.OK)
        {
            if (selfData.postion == 1
           && memberData.postion != 0)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_018"), (int)PB.ImType.PROMPT);
                return;
            }
            RequestKich();
        }
    }

    void RequestKich()
    {
        PB.HSAllianceMemKick param = new PB.HSAllianceMemKick();
        param.targetId = memberData.id;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_MEMBER_KICK_C.GetHashCode(), param);
    }

    void OnRequestKichFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(error.errCode);
        }
        else
        {
            UIIm.Instance.ShowSystemHints(string.Format(StaticDataMgr.Instance.GetTextByID("sociaty_record_017"),memberData.name), (int)PB.ImType.PROMPT);
            GameDataMgr.Instance.SociatyDataMgrAttr.allianceMemberList.Remove(memberData);
            
            SociatyContentMember.Instance.UpdateMemberItems();
            OnCloseButtonClick();
        }
    }

}
