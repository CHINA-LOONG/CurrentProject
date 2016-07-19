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
    public Text textAnnexnone;  //没有邮件哦

    public Button btnReceive;

    public GameObject annexList;
    public GameObject annexNone;

    public GameObject content;

    public System.Action<PB.HSMail> actionReceiveMail;

    private UIMail uiMail;
    private PB.HSMail info;

    void Start()
    {
        OnLanguageChanged();
        EventTriggerListener.Get(btnReceive.gameObject).onClick = OnClickReveive;
        SetMailContentActive(false);
    }

    public void SetMailContent(PB.HSMail info)
    {
        SetMailContentActive(true);
        this.info = info;
        textTitle.text = info.subject;
        textPlayer.text = info.senderName;
        textSendTime.text = GameTimeMgr.GetTime(info.sendTimeStamp).ToString("yyyy-MM-dd");
        textContent.text = info.content;

        SetReward(info.reward);
    }

    public void SetMailContentActive(bool active)
    {
        content.SetActive(active);
    }

    private List<ObjectItem> items = new List<ObjectItem>();

    void SetReward(List<PB.RewardItem> infos)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            Destroy(items[i].gameObject);
        }
        items.Clear();

        annexNone.SetActive(infos.Count <= 0);
        btnReceive.gameObject.SetActive(infos.Count>0);
        if (infos.Count <= 0) return; 

        for (int i = items.Count; i < infos.Count; i++)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("ObjectItem");
            if (null != go)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(annexList.transform, false);
                ObjectItem item = go.GetComponent<ObjectItem>();
                items.Add(item);
            }
        }
        for (int i = 0; i < infos.Count; i++)
        {
            //TODO： 
        }
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
            Logger.LogError("收取错误");
            return;
        }
        PB.HSMailReceiveRet result = msg.GetProtocolBody<PB.HSMailReceiveRet>();
        if (result.mailId!=info.mailId)
        {
            Logger.LogError("收取错误");
            return;
        }
        info.state = (int)PB.mailState.RECEIVE;
        SetReceiveState();
        actionReceiveMail(info);
    }

    void SetReceiveState()
    {
        foreach (var item in items)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("AnnexTips");
            if (null != go)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(item.transform, false);
            }
        }
        btnReceive.gameObject.SetActive(false);
    }

    void OnLanguageChanged()
    {
        textSend.text = StaticDataMgr.Instance.GetTextByID("mail_laizi");
        textAnnex.text = StaticDataMgr.Instance.GetTextByID("mail_fujian");
        textReceive.text = StaticDataMgr.Instance.GetTextByID("mail_shouqu");
        textAnnexnone.text = StaticDataMgr.Instance.GetTextByID("mail_meiyoufujian");
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
