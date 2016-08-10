using UnityEngine;
using System.Collections;

public delegate void NetMessageDelegate(ProtocolMessage msg);
public class SociatyDataMgr : MonoBehaviour
{
    public int allianceID = 0;
    public PB.AllianceInfo allianceData = new PB.AllianceInfo();
    public PB.AllianceMember allianceSelfData = new PB.AllianceMember();

    private NetMessageDelegate callBack = null;
    // Use this for initialization
    void Start ()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_DATA_S.GetHashCode().ToString(), OnAllianceDataSync);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_SELF_DATA_S.GetHashCode().ToString(),OnSelfAllianceDataSync);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_APPLY_C.GetHashCode().ToString(), OnApplyFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_APPLY_S.GetHashCode().ToString(), OnApplyFinish);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CANCLE_APPLY_C.GetHashCode().ToString(), OnCancelApplyFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_CANCLE_APPLY_S.GetHashCode().ToString(), OnCancelApplyFinish);
    }


    void OnDestroy()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_DATA_S.GetHashCode().ToString(), OnAllianceDataSync);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_SELF_DATA_S.GetHashCode().ToString(),OnSelfAllianceDataSync);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_APPLY_C.GetHashCode().ToString(), OnApplyFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_APPLY_S.GetHashCode().ToString(), OnApplyFinish);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CANCLE_APPLY_C.GetHashCode().ToString(), OnCancelApplyFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_CANCLE_APPLY_S.GetHashCode().ToString(), OnCancelApplyFinish);

        
    }
	
    //打开公会
    public  void    OpenSociaty()
    {
        if(allianceID < 1)
        {
            SociatyList.OpenWith(null);
        }
        else
        {
            SociatyMain.OpenWith();
        }
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
            allianceSelfData = selfData.selfData;
        }
    }


    public  void RequestCancelApply(int allianceId, NetMessageDelegate callBack)
    {
        PB.HSAllianceCancleApply param = new PB.HSAllianceCancleApply();
        param.allianceId = allianceId;
        this.callBack = callBack;

        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_CANCLE_APPLY_C.GetHashCode(), param);
    }

    void OnCancelApplyFinish(ProtocolMessage msg)
    {
        if (null != callBack)
        {
            callBack(msg);
        }
    }

    public  void RequestApply(int allianceId, NetMessageDelegate callBack)
    {
        PB.HSAllianceApply param = new PB.HSAllianceApply();
        param.allianceId = allianceId;
        this.callBack = callBack;

        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_APPLY_C.GetHashCode(), param);
    }

    void OnApplyFinish(ProtocolMessage msg)
    {
        if(null != callBack)
        {
            callBack(msg);
        }
    }
}
