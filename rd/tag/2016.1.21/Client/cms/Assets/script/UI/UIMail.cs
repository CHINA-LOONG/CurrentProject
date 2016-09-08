using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIMail : UIBase//,TabButtonDelegate
{
    public const int maxCount = 300;

    public static string ViewName = "UIMail";

    public Text textCount;      //邮件数量
  
    public Text textTitle;      //界面标题
    public Text textOnekey;     //一键收取
    //public Text textTab1;       //系统邮件

    public Button btnClose;
    public Button btnOnekey;

    //public TabButtonGroup tabGroup;
    public ScrollRect scrollRect;
    public UIMailList mailList;
    public UIMailContent mailContent;

    public Transform coinButtonPos;
    public Transform goldButtonPos;
    private CoinButton coinBtn;
    private CoinButton goldBtn;

    public Text textTips;

    private Dictionary<int, PB.HSMail> allMailList;
    private List<MailItemInfo> sysMailList = new List<MailItemInfo>();
    //private List<PB.HSMail> plyMailList = new List<PB.HSMail>();

    private int tabIndex = 0;
    private MailItemInfo readMail;
    
    private int mailCount
    {
        get { return allMailList.Count; }
    }

	// Use this for initialization
	void Start () 
    {
        mailList.actionReadMail = ActionReadMail;
        mailContent.actionReceiveMail = ActionReceiveMail;
        EventTriggerListener.Get(btnClose.gameObject).onClick = ClickCloseButton;
        EventTriggerListener.Get(btnOnekey.gameObject).onClick = ClickReceiveAll;


        textTitle.text = StaticDataMgr.Instance.GetTextByID("mail_title");
        textOnekey.text = StaticDataMgr.Instance.GetTextByID("mail_yijianlingqu");
        textTips.text = StaticDataMgr.Instance.GetTextByID("mail_zidongshantips");
        //textTab1.text = StaticDataMgr.Instance.GetTextByID("mail_tab1");
    }
    public override void Init()
    {
        if (coinBtn==null)
        {
            coinBtn = CoinButton.CreateWithType(CoinButton.CoinType.Jinbi);
            UIUtil.SetParentReset(coinBtn.transform, coinButtonPos);
        }
        if (goldBtn==null)
        {
            goldBtn = CoinButton.CreateWithType(CoinButton.CoinType.Zuanshi);
            UIUtil.SetParentReset(goldBtn.transform, goldButtonPos);
        }

        //tabGroup.InitWithDelegate(this);
        mailContent.SetMailContentActive(false);
        readMail = null;
        OnMailChanged();
        //scrollRect.verticalNormalizedPosition = 1.0f;

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
        //检测是否读取的同一封
        if (readMail != null && (readMail.info.state == (int)PB.mailState.RECEIVE ||
            (readMail.info.state == (int)PB.mailState.READ && readMail.info.reward.Count <= 0)))
        {
            readMail = null;
        }

        UpdateMailList();
        //OnTabButtonChanged(tabIndex);
        SetMailCount();
    }

    void UpdateMailList()
    {
        allMailList = GameDataMgr.Instance.PlayerDataAttr.gameMailData.mailList;
        sysMailList.Clear();
        foreach (var item in allMailList)
        {
            if (readMail != null && readMail.info.mailId == item.Value.mailId)
            {
                readMail = new MailItemInfo() { info = item.Value, IsSelect = true };
                sysMailList.Add(readMail);
            }
            else
            {
                sysMailList.Add(new MailItemInfo() { info = item.Value, IsSelect = false });
            }
        }
        mailContent.SetMailContentActive(readMail != null);
        sysMailList.Sort(SortMail);
        mailList.RefreshList(sysMailList);
    }

    //public void OnTabButtonChanged(int index)
    //{
    //    mailList.RefreshList(sysMailList);
    //}

    void SetMailCount()
    {
        textCount.text = string.Format("{0}/{1}", mailCount, UIMail.maxCount);
    }

    //读取邮件
    void ActionReadMail(MailItemInfo mailInfo)
    {
        //检测是否读取的同一封
        if (readMail != null)
        {
            if (readMail.info.mailId == mailInfo.info.mailId) return;
            if (readMail.info.state == (int)PB.mailState.RECEIVE ||
                (readMail.info.state == (int)PB.mailState.READ && readMail.info.reward.Count <= 0))
            {
                sysMailList.Remove(readMail);
                mailList.RefreshList(sysMailList);
                SetMailCount();
            }
            else
            {
                readMail.IsSelect = false;
            }
        }
        readMail = mailInfo;
        readMail.IsSelect = true;
        //检测是否需要删除邮件
        if (mailInfo.info.reward.Count <= 0)
        {
            GameDataMgr.Instance.PlayerDataAttr.gameMailData.RemoveMail(mailInfo.info.mailId);
            readMail = mailInfo;
        }
        mailContent.SetMailContent(mailInfo.info);
        GameEventMgr.Instance.FireEvent<int>(GameEventList.MailRead, mailInfo.info.mailId);
    }
    //收取附件
    void ActionReceiveMail(PB.HSMail info)
    {
        PB.HSMailReceive param = new PB.HSMailReceive();
        param.mailId = info.mailId;
        GameApp.Instance.netManager.SendMessage(PB.code.MAIL_RECEIVE_C.GetHashCode(), param);


        GameDataMgr.Instance.PlayerDataAttr.gameMailData.RemoveMail(info.mailId);
        SavePlayerData();
    }
    void OnMailReceiveRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
            if (error.errCode == (int)PB.mailError.MAIL_NOT_EXIST ||
                error.errCode == (int)PB.mailError.MAIL_NONE)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("mail_record_005"), (int)PB.ImType.PROMPT);
            }
            Logger.LogError("收取错误,不存在");
            return;
        }
        PB.HSMailReceiveRet result = msg.GetProtocolBody<PB.HSMailReceiveRet>();
        if (result.mailId != readMail.info.mailId)
        {
            Logger.LogError("收取错误");
            return;
        }

        UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("mail_record_002"), (int)PB.ImType.PROMPT);
        readMail.info.state= (int)PB.mailState.RECEIVE;
        mailContent.SetReceiveState();
        CheckPlayerData();
    }
    //一键收取
    void ClickReceiveAll(GameObject go)
    {
        if (mailCount <= 0)
        {
            if (readMail != null)
            {
                OnMailChanged();
                readMail = null;
            }
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("mail_record_001"), (int)PB.ImType.PROMPT);
            return;
        }
        else
        {
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel,
                                  StaticDataMgr.Instance.GetTextByID("mail_shouquall"),
                                  StaticDataMgr.Instance.GetTextByID("mail_coinmanle"),
                                  OnReceiveAllMsgCallBack);
        }
    }

    void OnReceiveAllMsgCallBack(MsgBox.PrompButtonClick btnParam)
    {
        if (btnParam == MsgBox.PrompButtonClick.OK)
        {
            PB.HSMailReceiveAll param = new PB.HSMailReceiveAll();
            GameApp.Instance.netManager.SendMessage(PB.code.MAIL_RECEIVE_ALL_C.GetHashCode(), param);
            SavePlayerData();
        }
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
        CheckPlayerData();
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
    public static int SortMail(MailItemInfo aInfo, MailItemInfo bInfo)
    {

        PB.HSMail a, b;
        a = aInfo.info;
        b = bInfo.info;
        int result = 0;
        if (a.state == (int)PB.mailState.UNREAD && b.state == (int)PB.mailState.UNREAD)
        {
            if (a.sendTimeStamp > b.sendTimeStamp)
            {
                result = -1;
            }
            else if (a.sendTimeStamp < b.sendTimeStamp)
            {
                result = 1;
            }
        }
        else if (a.state == (int)PB.mailState.UNREAD)
        {
            return result = -1;
        }
        else if (b.state == (int)PB.mailState.UNREAD)
        {
            return result = 1;
        }
        else
        {
            if (a.sendTimeStamp > b.sendTimeStamp)
            {
                result = -1;
            }
            else if (a.sendTimeStamp < b.sendTimeStamp)
            {
                result = 1;
            }
        }
        return result;
    }

    #region 检测玩家等级疲劳值变化

    private int playerLevel;
    private int playerFatigue;

    void SavePlayerData()
    {
        playerLevel = GameDataMgr.Instance.PlayerDataAttr.LevelAttr;
        playerFatigue = GameDataMgr.Instance.PlayerDataAttr.HuoliAttr;
    }

    void CheckPlayerData()
    {
        if (playerLevel!=GameDataMgr.Instance.PlayerDataAttr.LevelAttr)
        {
            LevelUp.OpenWith(playerLevel, 
                             GameDataMgr.Instance.PlayerDataAttr.LevelAttr,
                             playerFatigue,
                             GameDataMgr.Instance.PlayerDataAttr.HuoliAttr);
        }
    }


    #endregion


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

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_S.GetHashCode().ToString(), OnMailReceiveRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_C.GetHashCode().ToString(), OnMailReceiveRet);

        GameEventMgr.Instance.AddListener<int>(GameEventList.MailAdd, OnMailListChanged);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_ALL_C.GetHashCode().ToString(), OnMailReceiveAllRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_ALL_S.GetHashCode().ToString(), OnMailReceiveAllRet);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_S.GetHashCode().ToString(), OnMailReceiveRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MAIL_RECEIVE_C.GetHashCode().ToString(), OnMailReceiveRet);

        GameEventMgr.Instance.RemoveListener<int>(GameEventList.MailAdd, OnMailListChanged);
    }
    
}
