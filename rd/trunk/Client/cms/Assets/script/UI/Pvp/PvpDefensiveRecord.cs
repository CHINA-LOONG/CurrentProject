using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PvpDefensiveRecord : UIBase, IScrollView
{
    public static string ViewName = "PvpDefensiveRecord";

    public Button closeButton;
    public Text titleText;
    public Text[] listTitleText;
    public FixCountScrollView scrollView;

    private List<PB.PVPDefenceRecordData> defensiveData = new List<PB.PVPDefenceRecordData>();

    public static   PvpDefensiveRecord Open()
    {
        PvpDefensiveRecord record = UIMgr.Instance.OpenUI_(ViewName) as PvpDefensiveRecord;

        return record;
    }

    public override void Init()
    {
        RequestRecordData();
    }

    public override void Clean()
    {
        
    }
    // Use this for initialization
    void Start ()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick); 
    }

    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }

   void RequestRecordData()
    {
        GameDataMgr.Instance.PvpDataMgrAttr.RequestPvpDefenseRecord(OnRequestRecordDataFinished);
    }

    void OnRequestRecordDataFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            PvpErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        PB.HSPVPDefenceRecordRet msgRet = message.GetProtocolBody<PB.HSPVPDefenceRecordRet>();
        defensiveData.Clear();
        defensiveData.AddRange(msgRet.pvpDefenceRecordList);
        scrollView.InitContentSize(defensiveData.Count, this, true);
    }

    #region IScrollView
    public void IScrollViewReloadItem(FixCountScrollView scrollView, Transform item, int index)
    {
        DefensiveRecordItem subRecordItem = item.GetComponent<DefensiveRecordItem>();
        subRecordItem.RefreshWith(defensiveData[index]);
    }
    public Transform IScrollViewCreateItem(FixCountScrollView scrollView, Transform parent)
    {
        DefensiveRecordItem subItem = DefensiveRecordItem.Create();
        UIUtil.SetParentReset(subItem.transform, parent);
        return subItem.transform;
    }
    public void IScrollViewCleanItem(FixCountScrollView scrollView, List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
    }
    #endregion
}
