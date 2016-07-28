using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIMail : UIBase,TabButtonDelegate
{
    public const int maxCount = 300;

    public static string ViewName = "UIMail";

    public Text textCount;      //邮件数量
  
    public Text textTitle;      //界面标题
    public Text textOnekey;     //一键收取
    public Text textTab1;       //系统邮件

    public Button btnClose;
    public Button btnOnekey;

    public TabButtonGroup tabGroup;
    public ScrollRect scrollRect;
    public UIMailList mailList;
    public UIMailContent mailContent;

    private Dictionary<int, PB.HSMail> allMailList;
    private List<PB.HSMail> sysMailList = new List<PB.HSMail>();
    private List<PB.HSMail> plyMailList = new List<PB.HSMail>();

    private int tabIndex = 0;
    private PB.HSMail readMail;

    private int mailCount
    {
        get { return allMailList.Count; }
    }

	// Use this for initialization
	void Start () 
    {
        OnLanguageChanged();
        mailList.actionReadMail = ActionReadMail;
        mailContent.actionReceiveMail = ActionReceiveMail;
        EventTriggerListener.Get(btnClose.gameObject).onClick = ClickCloseButton;
        EventTriggerListener.Get(btnOnekey.gameObject).onClick = ClickReceiveAll;
	}
    public override void Init()
    {
        tabGroup.InitWithDelegate(this);
        mailContent.SetMailContentActive(false);
        OnMailChanged();
        scrollRect.verticalNormalizedPosition = 1.0f;
        readMail = null;

        if (mailCount >= UIMail.maxCount)
			MsgBox.PromptMsg.Open (MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID ("mail_youxiangyiman"));
        else if (mailCount >= (UIMail.maxCount-20))
			MsgBox.PromptMsg.Open (MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID ("mail_youxiangjiangman"));
    }
    public override void Clean()
    {
        mailList.Clean();
        mailContent.Clean();
    }

    void OnMailChanged()
    {
        UpdateMailList();
        OnTabButtonChanged(tabIndex);
        SetMailCount();

    }

    void UpdateMailList()
    {
        allMailList = GameDataMgr.Instance.PlayerDataAttr.gameMailData.mailList;
        sysMailList.Clear();
        foreach (var item in allMailList)
        {
            sysMailList.Add(item.Value);
        }
    }

    public void OnTabButtonChanged(int index)
    {
        List<PB.HSMail> list = sysMailList;

        mailList.RefreshList(list);
    }

    void SetMailCount()
    {
        textCount.text = string.Format("{0}/{1}", mailCount, UIMail.maxCount);
    }

    //读取邮件
    void ActionReadMail(PB.HSMail info)
    {
        //检测是否读取的同一封
        if (readMail != null)
        {
            if (readMail.mailId == info.mailId) return;
            if (readMail.state == (int)PB.mailState.RECEIVE ||
                (readMail.state == (int)PB.mailState.READ && readMail.reward.Count <= 0))
            {
                sysMailList.Remove(readMail);
                mailList.DeleteElement(readMail.mailId);
                SetMailCount();
            }
        }
        readMail = info;

        //检测是否需要删除邮件
        if (info.reward.Count <= 0)
        {
            GameDataMgr.Instance.PlayerDataAttr.gameMailData.RemoveMail(info.mailId);
            readMail = info;
        }
        mailContent.SetMailContent(info);
        GameEventMgr.Instance.FireEvent<int>(GameEventList.MailRead,info.mailId);
    }
    //收取附件
    void ActionReceiveMail(PB.HSMail info)
    {
        GameDataMgr.Instance.PlayerDataAttr.gameMailData.RemoveMail(info.mailId);
    }
    //收取返回
    void OnMailReceiveAllRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("收取错误");
            return;
        }
        PB.HSMailReceiveAllRet result = msg.GetProtocolBody<PB.HSMailReceiveAllRet>();
        if (result.status==0)
        {
            GameDataMgr.Instance.PlayerDataAttr.gameMailData.ClearMail();
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("mail_CollectedAll"));
        }
        else
        {
            if (result.status == (int)PB.mailError.MAIL_COIN_FULL)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("mail_record_003"), (int)PB.ImType.PROMPT);
            }
            else if (result.status == (int)PB.mailError.MAIL_GOLD_FULL)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("mail_record_004"), (int)PB.ImType.PROMPT);
            }
            foreach (var mailId in result.receiveMailId)
            {
                GameDataMgr.Instance.PlayerDataAttr.gameMailData.RemoveMail(mailId);
            }
        }
        GameEventMgr.Instance.FireEvent<int>(GameEventList.MailRead,0);
        mailContent.SetMailContentActive(false);
        OnMailChanged();
        readMail = null;
    }
    //一键收取
    void ClickReceiveAll(GameObject go)
    {
        if (mailCount <= 0)
        {
            if (readMail!=null)
            {
                mailContent.SetMailContentActive(false);
                OnMailChanged();
                readMail = null;
            }
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("mail_record_001"), (int)PB.ImType.PROMPT);
            return;
        }

        PB.HSMailReceiveAll param = new PB.HSMailReceiveAll();
        GameApp.Instance.netManager.SendMessage(PB.code.MAIL_RECEIVE_ALL_C.GetHashCode(), param);
    }
    //点击关闭
    void ClickCloseButton(GameObject go)
    {
        CloseUIMail();
    }
    //界面关闭处理
    void CloseUIMail()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    //同步邮件列表，收取新邮件
    void OnMailListChanged(int mailId)
    {
        OnMailChanged();
        scrollRect.verticalNormalizedPosition = 1.0f;
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
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_ALL_C.GetHashCode().ToString(), OnMailReceiveAllRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_ALL_S.GetHashCode().ToString(), OnMailReceiveAllRet);

        GameEventMgr.Instance.AddListener<int>(GameEventList.MailAdd, OnMailListChanged);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_ALL_C.GetHashCode().ToString(), OnMailReceiveAllRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_ALL_S.GetHashCode().ToString(), OnMailReceiveAllRet);

        GameEventMgr.Instance.RemoveListener<int>(GameEventList.MailAdd, OnMailListChanged);
    }


    void OnLanguageChanged()
    {
        textTitle.text = StaticDataMgr.Instance.GetTextByID("mail_title");
        textOnekey.text = StaticDataMgr.Instance.GetTextByID("mail_yijianlingqu");
        textTab1.text = StaticDataMgr.Instance.GetTextByID("mail_tab1");
    }

}
