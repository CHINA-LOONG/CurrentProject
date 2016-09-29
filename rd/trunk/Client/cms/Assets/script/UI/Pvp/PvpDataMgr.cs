using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PvpDataMgr : MonoBehaviour
{
    public List<string> defenseTeamList = new List<string>();
    private NetMessageDelegate callBack = null;

    void Start ()
    {
        for (int i = 0; i < 5; ++i)
        {
            defenseTeamList.Add("");
        }
	}
    void OnDestroy()
    {

    }

    public  void ClearData()
    {
	
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

    public void RequestSaveDefensePosition(List<string> defenseList, NetMessageDelegate callBack)
    {
        this.callBack = callBack;
      //  GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_CANCLE_APPLY_C.GetHashCode(), param);
    }

    public void RequestSearchPvpOpponent(NetMessageDelegate callBack)
    {
        this.callBack = callBack;
        callBack(null);
    }

    void OnReceivPvpMessage(ProtocolMessage msg)
    {
        if (null != callBack)
        {
            callBack(msg);
        }
    }

}
