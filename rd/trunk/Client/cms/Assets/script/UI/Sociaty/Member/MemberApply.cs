using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MemberApply : UIBase
{
    public static string ViewName = "MemberApply";

    public Text titleText;
    public Text nameText;
    public Text levelText;
    public Text actionText;

    public Button closeButton;
    public Button settingButton;
    public Button acceptAllButton;
    public Button refuseAllButton;

    public ScrollView scrollView;

    private SociatyDataMgr sociatyDataMgr = null;

    private List<NewApplyItem> newApplyItemCatch = new List<NewApplyItem>();

    public static MemberApply Instance = null;

    public static void Open()
    {
        MemberApply apply = (MemberApply)UIMgr.Instance.OpenUI_(ViewName);
        apply.RequestMemberApplyList();
    }

	// Use this for initialization
	void Start ()
    {
        Instance = this;
        titleText.text = StaticDataMgr.Instance.GetTextByID("sociaty_newapply");

        nameText.text = StaticDataMgr.Instance.GetTextByID("sociaty_membername");
        levelText.text = StaticDataMgr.Instance.GetTextByID("sociaty_level");
        actionText.text = StaticDataMgr.Instance.GetTextByID("sociaty_operate");

        closeButton.onClick.AddListener(OnCloseButtonClick);
        settingButton.onClick.AddListener(OnSettingButtonClick);
        acceptAllButton.onClick.AddListener(OnAcceptAllButtonClick);
        refuseAllButton.onClick.AddListener(OnRefuseAllbuttonClick);

        UIUtil.SetButtonTitle(settingButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_setup"));
        UIUtil.SetButtonTitle(acceptAllButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_approveall"));
        UIUtil.SetButtonTitle(refuseAllButton.transform, StaticDataMgr.Instance.GetTextByID("sociaty_refuseall"));
    }

    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_APPLYS_C.GetHashCode().ToString(), OnRequestMemberApplyListFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_APPLYS_S.GetHashCode().ToString(), OnRequestMemberApplyListFinish);
    }

    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_APPLYS_C.GetHashCode().ToString(), OnRequestMemberApplyListFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_APPLYS_S.GetHashCode().ToString(), OnRequestMemberApplyListFinish);
    }

    public override void Clean()
    {
        scrollView.ClearAllElement();
        newApplyItemCatch.Clear();
    }

    public void RequestMemberApplyList()
    {
        sociatyDataMgr = GameDataMgr.Instance.SociatyDataMgrAttr;
        PB.HSAllianceApplyList param = new PB.HSAllianceApplyList();
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_APPLYS_C.GetHashCode(), param);
    }

    void OnRequestMemberApplyListFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        PB.HSAllianceApplyListRet retMsg = message.GetProtocolBody<PB.HSAllianceApplyListRet>();
        if(null != retMsg)
        {
            sociatyDataMgr.newApplyList.Clear();
            sociatyDataMgr.newApplyList.AddRange(retMsg.apply);
        }
        RefreshUi();
    }

    public void RefreshUi()
    {
        HideAllItem();

        for(int i =0;i<sociatyDataMgr.newApplyList.Count;++i)
        {
            var itemData = sociatyDataMgr.newApplyList[i];
            if(i < newApplyItemCatch.Count)
            {
                var cacheItem = newApplyItemCatch[i];
                cacheItem.gameObject.SetActive(true);
                cacheItem.RefreshWith(itemData);
            }
            else
            {
                var newItem = NewApplyItem.CreateWith(itemData);
                newApplyItemCatch.Add(newItem);
                scrollView.AddElement(newItem.gameObject);
            }
        }
    }

    void HideAllItem()
    {
        foreach(var item in newApplyItemCatch)
        {
            item.gameObject.SetActive(false);
        }
    }

    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void OnSettingButtonClick()
    {
        SociatyApplySetting.OpenWith(sociatyDataMgr.allianceData.autoAccept, sociatyDataMgr.allianceData.minLevel);
    }

    void OnAcceptAllButtonClick()
    {
        if (sociatyDataMgr.newApplyList.Count < 1)
            return;

        sociatyDataMgr.RequestApplyOperate(0, true, true, OnAcceptAllRequestFinish);
    }

    void OnAcceptAllRequestFinish(ProtocolMessage message)
    {
        UINetRequest.Close();

        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        PB.HSAllianceHanleApplyRet retmsg = message.GetProtocolBody<PB.HSAllianceHanleApplyRet>();
        if (null != retmsg)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_022"), (int)PB.ImType.PROMPT);
            GameDataMgr.Instance.SociatyDataMgrAttr.newApplyList.Clear();
            RefreshUi();
            SociatyContentMember.Instance.RequestMemberData();
        }
    }

    void OnRefuseAllbuttonClick()
    {
        if (sociatyDataMgr.newApplyList.Count < 1)
            return;

        sociatyDataMgr.RequestApplyOperate(0, false, true, OnRefuseAllRequestFinish);
    }

    void OnRefuseAllRequestFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }

        PB.HSAllianceHanleApplyRet retmsg = message.GetProtocolBody<PB.HSAllianceHanleApplyRet>();
        if (null != retmsg)
        {
            GameDataMgr.Instance.SociatyDataMgrAttr.newApplyList.Clear();
            MemberApply.Instance.RefreshUi();
        }
    }
}
