using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class MailItemInfo
{
    public PB.HSMail info;
    public System.Action<MailItemInfo> Refresh;
    private bool isSelect = false;
    public bool IsSelect
    {
        get { return isSelect; }
        set
        {
            if (isSelect != value)
            {
                isSelect = value;
                if (Refresh != null)
                {
                    Refresh(this);
                }
            }
        }
    }
}

public class mailItem : MonoBehaviour
{
    public Image imgIcon;
    public Text textTitle;
    public Text textPlayer;
    public Text textSendTime;
    public Image imgIsNew;
    public Image imgSelect;

    public Text textSend;       //来自:

    [HideInInspector]
    public PB.HSMail info;

    private MailItemInfo curData;
    public MailItemInfo CruData
    {
        get { return curData; }
        set
        {
            //if (curData!=null)
            //{
            //    curData.Refresh = null;
            //}
            curData = value;
            curData.Refresh = RefreshInfo;
        }
    }

    void Start()
    {
        textSend.text = StaticDataMgr.Instance.GetTextByID("mail_laizi");
    }

    public void ReloadData(MailItemInfo mailInfo)
    {
        CruData = mailInfo;
        PB.HSMail info = CruData.info;
        string iconName = info.reward.Count > 0 ? "youxiang_baoguo" : "youxiang_youjian";
        imgIcon.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(iconName);
        textTitle.text = info.subject;
        textPlayer.text = info.senderName;
        textSendTime.text = GameTimeMgr.GetTime(info.sendTimeStamp).ToString("MM-dd-yyyy");

        UpdateMailState();
        RefreshInfo(CruData);
    }

    public void RefreshInfo(MailItemInfo info)
    {
        if (info != CruData)
            return;
        imgSelect.gameObject.SetActive(CruData.IsSelect);
    }

    public void UpdateMailState()
    {
        imgIsNew.gameObject.SetActive(CruData.info.state == (int)PB.mailState.UNREAD);
    }
}
