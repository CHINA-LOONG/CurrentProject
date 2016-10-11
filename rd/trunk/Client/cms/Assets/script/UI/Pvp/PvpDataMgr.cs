using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PvpDataMgr : MonoBehaviour
{
    public int selfPvpTiems;
    public int selfPvpTimesBeginTime;

    private int selfPvpPoint;
    public int SelfPvpPointAttr
    {
        get { return selfPvpPoint; }
        set
        {
            selfPvpPoint = value;
            selfPvpStage = GetPvpStageWithPoint(selfPvpPoint);
        }
    }

    public int selfPvpStage;
    public int selfPvpRank;
    public List<string> defenseTeamList = new List<string>();
    private NetMessageDelegate callBack = null;

    void Start ()
    {
        ClearDefensePosition();
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_SET_DEFENCE_MONSTERS_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_SET_DEFENCE_MONSTERS_S.GetHashCode().ToString(), OnReceivPvpMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_MATCH_TARGET_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_MATCH_TARGET_S.GetHashCode().ToString(), OnReceivPvpMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_GET_MY_INFO_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_GET_MY_INFO_S.GetHashCode().ToString(), OnReceivPvpMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_DEFENCE_RECORD_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_DEFENCE_RECORD_S.GetHashCode().ToString(), OnReceivPvpMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_RANK_LIST_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_RANK_LIST_S.GetHashCode().ToString(), OnReceivPvpMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_GET_RANK_DEFENCE_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_GET_RANK_DEFENCE_S.GetHashCode().ToString(), OnReceivPvpMessage);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_ENTER_ROOM_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PVP_ENTER_ROOM_S.GetHashCode().ToString(), OnReceivPvpMessage);
    }
    void OnDestroy()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_SET_DEFENCE_MONSTERS_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_SET_DEFENCE_MONSTERS_S.GetHashCode().ToString(), OnReceivPvpMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_MATCH_TARGET_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_MATCH_TARGET_S.GetHashCode().ToString(), OnReceivPvpMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_GET_MY_INFO_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_GET_MY_INFO_S.GetHashCode().ToString(), OnReceivPvpMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_DEFENCE_RECORD_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_DEFENCE_RECORD_S.GetHashCode().ToString(), OnReceivPvpMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_RANK_LIST_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_RANK_LIST_S.GetHashCode().ToString(), OnReceivPvpMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_GET_RANK_DEFENCE_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_GET_RANK_DEFENCE_S.GetHashCode().ToString(), OnReceivPvpMessage);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_ENTER_ROOM_C.GetHashCode().ToString(), OnReceivPvpMessage);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PVP_ENTER_ROOM_S.GetHashCode().ToString(), OnReceivPvpMessage);
    }

    public  void ClearData()
    {
        ClearDefensePosition();
        selfPvpTiems = 0;
        selfPvpTimesBeginTime = 0;
    }
    public void ClearDefensePosition()
    {
        defenseTeamList.Clear();
        for (int i = 0; i < 5; ++i)
        {
            defenseTeamList.Add("");
        }
    }

    public int GetRereshCost(int changeTimes)
    {
        int k = 10;
        int b = 5;
        int rTimes = changeTimes + 1;

        int result = k * rTimes * rTimes + b;
        return result;
    }

    public int GetPvpStageWithPoint(int point)
    {
        //计算段位
        PvpStaticData subData = null;
        for (int i = 1; i <= 12; ++i)
        {
            subData = StaticDataMgr.Instance.GetPvpStaticDataWithStage(i);
            if (subData != null)
            {
                if (i == 12)
                {
                    return 12;
                }
                else
                {
                    if (point <= subData.point)
                    {
                        return i;
                    }
                }
            }
        }
        return 1;
    }

    public bool IsSelfHaveDefensePositon()
    {
        for (int i = 0; i < defenseTeamList.Count; ++i)
        {
            if (!string.IsNullOrEmpty(defenseTeamList[i]))
                return true;
        }
        return false;
    }

    public int GetBpWithGuidList(List<string> petList)
    {
        int bpValue = 0;
        int guid = 0;
        GameUnit subUnit = null;
        for (int i = 0; i < petList.Count; ++i)
        {
            if (!string.IsNullOrEmpty(petList[i]))
            {
                guid = int.Parse(petList[i]);
                subUnit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(guid);
                if (null != subUnit)
                {
                    bpValue += subUnit.mBp;
                }
            }
        }
        return bpValue;
    }

    public string GetStageNameWithId(int stageId)
    {
        switch(stageId)
        {
            case 1:
                return StaticDataMgr.Instance.GetTextByID("pvp_copper3");
            case 2:
                return StaticDataMgr.Instance.GetTextByID("pvp_copper2");
            case 3:
                return StaticDataMgr.Instance.GetTextByID("pvp_copper1");
            case 4:
                return StaticDataMgr.Instance.GetTextByID("pvp_silver3");
            case 5:
                return StaticDataMgr.Instance.GetTextByID("pvp_silver2");
            case 6:
                return StaticDataMgr.Instance.GetTextByID("pvp_silver1");
            case 7:
                return StaticDataMgr.Instance.GetTextByID("pvp_gold3");
            case 8:
                return StaticDataMgr.Instance.GetTextByID("pvp_gold2");
            case 9:
                return StaticDataMgr.Instance.GetTextByID("pvp_gold1");
            case 10:
                return StaticDataMgr.Instance.GetTextByID("pvp_master3");
            case 11:
                return StaticDataMgr.Instance.GetTextByID("pvp_master2");
            case 12:
                return StaticDataMgr.Instance.GetTextByID("pvp_master1");
        }
        return "";
    }

    public void RequestSaveDefensePosition(List<string> defenseList, NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSSetPVPDefenceMonster param = new PB.HSSetPVPDefenceMonster();
        for(int i=0;i<defenseList.Count;++i)
        {
            if (!string.IsNullOrEmpty(defenseList[i]))
            {
                param.monsterId.Add(int.Parse(defenseList[i]));
            }
        }
        GameApp.Instance.netManager.SendMessage(PB.code.PVP_SET_DEFENCE_MONSTERS_C.GetHashCode(), param);
    }

    public void RequestSearchPvpOpponent(NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSPVPMatchTarget param = new PB.HSPVPMatchTarget();
        param.changeTarget = false;
        GameApp.Instance.netManager.SendMessage(PB.code.PVP_MATCH_TARGET_C.GetHashCode(), param);
    }

    public void RequestChangeOpponent(NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSPVPMatchTarget param = new PB.HSPVPMatchTarget();
        param.changeTarget = true;
        GameApp.Instance.netManager.SendMessage(PB.code.PVP_MATCH_TARGET_C.GetHashCode(), param);
    }

    public void RequestPvpInfo(NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSPVPInfo param = new PB.HSPVPInfo();
        GameApp.Instance.netManager.SendMessage(PB.code.PVP_GET_MY_INFO_C.GetHashCode(),param);
    }

    public void RequestPvpDefenseRecord(NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSPVPDefenceRecord param = new PB.HSPVPDefenceRecord();
        GameApp.Instance.netManager.SendMessage(PB.code.PVP_DEFENCE_RECORD_C.GetHashCode(), param);
    }

    public void RequestPvpRank(NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSPVPRank param = new PB.HSPVPRank();
        GameApp.Instance.netManager.SendMessage(PB.code.PVP_RANK_LIST_C.GetHashCode(), param);
    }

    public void RequestPlayerDefense(int playerId,NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSPVPRankDefence param = new PB.HSPVPRankDefence();
        param.playerId = playerId;
        GameApp.Instance.netManager.SendMessage(PB.code.PVP_GET_RANK_DEFENCE_C.GetHashCode(), param);
    }

    public void RequestPvpFight(NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        PB.HSPVPEnter param = new PB.HSPVPEnter();
        GameApp.Instance.netManager.SendMessage(PB.code.PVP_ENTER_ROOM_C.GetHashCode(), param);
    }
    void OnReceivPvpMessage(ProtocolMessage msg)
    {
        if (null != callBack)
        {
            callBack(msg);
        }
    }

}
