using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SociatyPray : UIBase
{
    public static string ViewName = "SociatyPray";

    public Text mTitle;
    public RectTransform mPrayItemList;
    public Text mSelfContributeText;
    public Text mSelfContributeValue;
    public Text mSociatyLiveText;
    public Text mSociatyLiveValue;
    public Button mBackBtn;
    //---------------------------------------------------------------------------------------------
    void Start()
    {
        EventTriggerListener.Get(mBackBtn.gameObject).onClick = OnClose;
        StaticDataMgr sdMgr = StaticDataMgr.Instance;
        mTitle.text = sdMgr.GetTextByID("sociaty_pray");
        mSelfContributeText.text = sdMgr.GetTextByID("sociaty_onecontribution");
        mSociatyLiveText.text = sdMgr.GetTextByID("sociaty_comcontribution");
        //3 max pray count
        for (int i = 1; i <= 3; ++i)
        {
            SociatyPrayItem item = SociatyPrayItem.Create();
            item.transform.SetParent(mPrayItemList.transform, false);
            item.SetPrayData(i, this);
        }

        Refresh();
    }
    //---------------------------------------------------------------------------------------------
    void OnClose(GameObject go)
    {
        UIMgr.Instance.DestroyUI(this);
    }
    //---------------------------------------------------------------------------------------------
    public void Refresh()
    {
        SociatyDataMgr mgr = GameDataMgr.Instance.SociatyDataMgrAttr;
        mSelfContributeValue.text = mgr.allianceSelfData.contribution.ToString();
        mSociatyLiveValue.text = mgr.allianceData.contribution.ToString();
    }
    //---------------------------------------------------------------------------------------------
    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_PRAY_C.GetHashCode().ToString(), OnPrayResult);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_PRAY_S.GetHashCode().ToString(), OnPrayResult);

    }
    //---------------------------------------------------------------------------------------------
    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_PRAY_C.GetHashCode().ToString(), OnPrayResult);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_PRAY_S.GetHashCode().ToString(), OnPrayResult);
    }
    //---------------------------------------------------------------------------------------------
    void OnPrayResult(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
            SociatyErrorMsg.ShowImWithErrorCode(error.errCode);
        }
        else
        {
            GameDataMgr.Instance.SociatyDataMgrAttr.allianceParyCount = 1;
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_029"), (int)PB.ImType.PROMPT);
            PB.HSAlliancePrayRet playerSync = msg.GetProtocolBody<PB.HSAlliancePrayRet>();
            if (playerSync != null)
            {
                SociatyDataMgr mgr = GameDataMgr.Instance.SociatyDataMgrAttr;
                mgr.allianceSelfData.contribution = playerSync.selfContribution;
                mSelfContributeValue.text = playerSync.selfContribution.ToString();
                mSociatyLiveValue.text = playerSync.allianceContribution.ToString();
            }
        }
    }
    //---------------------------------------------------------------------------------------------
}
