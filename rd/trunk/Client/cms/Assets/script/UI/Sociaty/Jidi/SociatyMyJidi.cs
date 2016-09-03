using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SociatyMyJidi : MonoBehaviour
{
    
    public JidiPetPosition[] petPositionArray;

    private PB.AllianceBaseMonster[] baseMonsterArray = new PB.AllianceBaseMonster[3];
    private static SociatyMyJidi instance = null;
    public static SociatyMyJidi Instance
    {
        get
        {
            if(null == instance)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("SociatyMyJidi");
                instance = go.GetComponent<SociatyMyJidi>();
            }
            return instance;
        }
    }

    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_MY_BASE_LIST_C.GetHashCode().ToString(), OnRequestJidiInfoFinish);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ALLIANCE_MY_BASE_LIST_S.GetHashCode().ToString(), OnRequestJidiInfoFinish);
    }
    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_MY_BASE_LIST_C.GetHashCode().ToString(), OnRequestJidiInfoFinish);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ALLIANCE_MY_BASE_LIST_S.GetHashCode().ToString(), OnRequestJidiInfoFinish);
    }
    
    public void Clear()
    {
        for(int i = 0;i < petPositionArray.Length;++i)
        {
            petPositionArray[i].petImageView.CleanImageView();
        }
    }
    public void UpdatePositionData(int position,PB.AllianceBaseMonster monster)
    {
        if(position < baseMonsterArray.Length)
        {
            baseMonsterArray[position] = monster;
        }
    }
    public void RequestJidiInfo()
    {
        PB.HSAllianceMyBaseList param = new PB.HSAllianceMyBaseList();
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_MY_BASE_LIST_C.GetHashCode(), param);
    }

    void OnRequestJidiInfoFinish(ProtocolMessage message)
    {
        UINetRequest.Close();
        if (message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSAllianceMyBaseListRet msgRet = message.GetProtocolBody<PB.HSAllianceMyBaseListRet>();
        for(int i =0;i<baseMonsterArray.Length;++i)
        {
            baseMonsterArray[i] = null;
        }
        PB.AllianceBaseMonster subMonster = null;
        for(int i =0;i < msgRet.monsterInfo.Count;++i)
        {
            subMonster = msgRet.monsterInfo[i];
            int positionIndex = subMonster.position;
            baseMonsterArray[positionIndex] = subMonster;
        }
        RefreshUi();
    }

    void RefreshUi()
    {
        for (int i = 0; i < petPositionArray.Length; ++i)
        {
            JidiPetPosition subPosition = petPositionArray[i];
            subPosition.SetPetData(baseMonsterArray[i]);
        }
    }
}
