using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuideDataMgr : MonoBehaviour
{
    public List<int> guideFinished = new List<int>();

    public delegate void GuideFinishDelegate(bool succ);
    GuideFinishDelegate callBack;
    List<int> requestFinishList = new List<int>();
    void Start()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.GUIDE_FINISH_C.GetHashCode().ToString(), OnGuideNetCallback);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.GUIDE_FINISH_S.GetHashCode().ToString(), OnGuideNetCallback);
    }
    void OnDestroy()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.GUIDE_FINISH_C.GetHashCode().ToString(), OnGuideNetCallback);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.GUIDE_FINISH_S.GetHashCode().ToString(), OnGuideNetCallback);
    }
    public void ClearData()
    {
        guideFinished.Clear();
    }
    public bool  IsGuideFinished(int groupId)
    {
        for(int i =0;i<guideFinished.Count;++i)
        {
            if(groupId == guideFinished[i])
            {
                return true;
            }
        }
        return false;
    }

    public void RequestFinishGuide(List<int> listFinish, GuideFinishDelegate callBack)
    {
        requestFinishList.Clear();
        requestFinishList.AddRange(listFinish);
        this.callBack = callBack;
        PB.HSGuideFinish param = new PB.HSGuideFinish();
        param.guideId.AddRange(listFinish);
        GameApp.Instance.netManager.SendMessage(PB.code.GUIDE_FINISH_C.GetHashCode(), param, false);
    }

    void OnGuideNetCallback(ProtocolMessage message)
    {
        bool succ = true;
        //UINetRequest.Close();
        if(message.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            succ = false;
        }
        else
        {
            guideFinished.AddRange(requestFinishList);
        }
        if(null != callBack)
        {
            callBack(succ);
        }
    }
}
