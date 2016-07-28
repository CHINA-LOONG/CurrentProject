using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIMailContent : MonoBehaviour
{
    public Text textTitle;      //邮件标题
    public Text textPlayer;     //发送人
    public Text textSendTime;   //发送时间
    public Text textContent;    //邮件内容

    public Text textSend;       //来自：
    public Text textAnnex;      //附件
    public Text textReceive;    //收取
    //public Text textAnnexnone;  //没有邮件哦

    public Button btnReceive;

    public GameObject annexList;
    //public GameObject annexNone;

    public GameObject content;

    public System.Action<PB.HSMail> actionReceiveMail;

    private PB.HSMail info;

    private List<AnnexItem> items = new List<AnnexItem>();

    void Start()
    {
        OnLanguageChanged();
        EventTriggerListener.Get(btnReceive.gameObject).onClick = OnClickReveive;
    }

    public void Clean()
    {
        CleanRewards();
    }

    public void SetMailContent(PB.HSMail info)
    {
        SetMailContentActive(true);
        this.info = info;
        textTitle.text = info.subject;
        textPlayer.text = info.senderName;
        textSendTime.text = GameTimeMgr.GetTime(info.sendTimeStamp).ToString("MM-dd-yyyy");
        textContent.text = info.content;

        SetReward(info.reward);
    }

    public void SetMailContentActive(bool active)
    {
        content.SetActive(active);
    }


    void SetReward(List<PB.RewardItem> infos)
    {
        CleanRewards();

        //annexNone.SetActive(infos.Count <= 0);
        btnReceive.gameObject.SetActive(infos.Count>0);
        if (infos.Count <= 0) 
            return;

        for (int i = items.Count; i < infos.Count; i++)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("AnnexItem");
            if (null != go)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(annexList.transform, false);
                AnnexItem item = go.GetComponent<AnnexItem>();
                item.Refresh(infos[i]);
                item.SetReceive(false);
                items.Add(item);
            }
        }
    }

    void CleanRewards()
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            ResourceMgr.Instance.DestroyAsset(items[i].gameObject);
        }
        items.Clear();
    }

    void OnClickReveive(GameObject go)
    {
        PB.HSMailReceive param = new PB.HSMailReceive();
        param.mailId = info.mailId;
        GameApp.Instance.netManager.SendMessage(PB.code.MAIL_RECEIVE_C.GetHashCode(), param);
    }

    void OnMailReceiveRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType()==(int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
            if (error.errCode==(int)PB.mailError.MAIL_NOT_EXIST||
                error.errCode==(int)PB.mailError.MAIL_NONE)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("mail_record_005"), (int)PB.ImType.PROMPT);
            }
            Logger.LogError("收取错误,不存在");
            return;
        }
        PB.HSMailReceiveRet result = msg.GetProtocolBody<PB.HSMailReceiveRet>();
        if (result.mailId!=info.mailId)
        {
            Logger.LogError("收取错误");
            return;
        }
        //TODO： 系统提示

        info.state = (int)PB.mailState.RECEIVE;
        SetReceiveState();
        actionReceiveMail(info);
        UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("mail_record_002"), (int)PB.ImType.PROMPT);
    }

    void SetReceiveState()
    {
        foreach (var item in items)
        {
            item.SetReceive(true);
        }
        btnReceive.gameObject.SetActive(false);
    }

    void OnLanguageChanged()
    {
        textSend.text = StaticDataMgr.Instance.GetTextByID("mail_laizi");
        textAnnex.text = StaticDataMgr.Instance.GetTextByID("mail_fujian");
        textReceive.text = StaticDataMgr.Instance.GetTextByID("mail_shouqu");
    }


    void OnEnable()
    {
        BindListener();
    }

    void OnDisable()
    {
        UnBindListener();
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_S.GetHashCode().ToString(), OnMailReceiveRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_C.GetHashCode().ToString(), OnMailReceiveRet);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_S.GetHashCode().ToString(), OnMailReceiveRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_C.GetHashCode().ToString(), OnMailReceiveRet);
    }
}
