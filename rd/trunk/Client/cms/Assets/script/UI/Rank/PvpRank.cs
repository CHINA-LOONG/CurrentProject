using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PvpRank : MonoBehaviour, IScrollView
{
    public Text[] titlesText;
    public FixCountScrollView scrollView;

    private static PvpRank instance = null;
    public static PvpRank Instance
    {
        get
        {
            if(null == instance)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("PvpRank");
                instance = go.GetComponent<PvpRank>();
            }
            return instance;
        }
    }

    private List<PB.PVPRankData> listPvpRankData = new List<PB.PVPRankData>();
    
     void RequestPvpRank()
    {
        GameDataMgr.Instance.PvpDataMgrAttr.RequestPvpRank(OnRequestPvpRankFinished);
    }

    void OnRequestPvpRankFinished(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = message.GetProtocolBody<PB.HSErrorCode>();
            PvpErrorMsg.ShowImWithErrorCode(errorCode.errCode);
            return;
        }
        PB.HSPVPRankRet msgRet = message.GetProtocolBody<PB.HSPVPRankRet>();
        listPvpRankData.Clear();
        listPvpRankData.AddRange(msgRet.pvpRankList);
        scrollView.InitContentSize(listPvpRankData.Count, this, true);
    }


    #region IScrollView
    public void IScrollViewReloadItem(FixCountScrollView scrollView, Transform item, int index)
    {
         PvpRankItem rankItem = item.GetComponent<PvpRankItem>();
        rankItem.InitWith(listPvpRankData[index]);
    }
    public Transform IScrollViewCreateItem(FixCountScrollView scrollView, Transform parent)
    {
        PvpRankItem subItem = PvpRankItem.Create();
        UIUtil.SetParentReset(subItem.transform, parent);
        return subItem.transform;
    }
    public void IScrollViewCleanItem(FixCountScrollView scrollView, List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
    }
    #endregion
}
