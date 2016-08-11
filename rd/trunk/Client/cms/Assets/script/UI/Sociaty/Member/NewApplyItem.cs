using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewApplyItem : MonoBehaviour
{
    public Text nameText;
    public Text levelText;
    public Button refuseButton;
    public Button acceptButton;

    private PB.AllianceApply applyData;

    public static NewApplyItem CreateWith(PB.AllianceApply applyData)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("NewApplyItem");
        var item = go.GetComponent<NewApplyItem>();
        item.RefreshWith(applyData);
        return item;
    }

	// Use this for initialization
	void Start ()
    {
        refuseButton.onClick.AddListener(OnRefuseButtonClick);
        acceptButton.onClick.AddListener(OnAcceptButtonClick);

        UIUtil.SetButtonTitle(refuseButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_refuse"));
        UIUtil.SetButtonTitle(acceptButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_approve"));
    }

    public void RefreshWith(PB.AllianceApply applyData)
    {
        this.applyData = applyData;

        nameText.text = applyData.nickname;
        levelText.text = applyData.level.ToString();
    }

    void OnRefuseButtonClick()
    {
        GameDataMgr.Instance.SociatyDataMgrAttr.RequestApplyOperate(applyData.playerId, false, false, OnRefruseRequestFinished);
    }

    void OnRefruseRequestFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        PB.HSAllianceHanleApplyRet retmsg = message.GetProtocolBody<PB.HSAllianceHanleApplyRet>();
        if(null != retmsg)
        {
            GameDataMgr.Instance.SociatyDataMgrAttr.newApplyList.Remove(applyData);
            MemberApply.Instance.RefreshUi();
        }
    }

    void OnAcceptButtonClick()
    {
        GameDataMgr.Instance.SociatyDataMgrAttr.RequestApplyOperate(applyData.playerId, true, false, OnAcceptRequestFinished);
    }

    void OnAcceptRequestFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
           if(errorCode.errCode == (int)PB.allianceError.ALLIANCE_TARGET_ALREADY_JOIN)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_045"), (int)PB.ImType.PROMPT);
            }

            return;
        }

        GameDataMgr.Instance.SociatyDataMgrAttr.newApplyList.Remove(applyData);
        MemberApply.Instance.RefreshUi();
        SociatyContentMember.Instance.RequestMemberData();
    }
	
}
