using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class mailItem : MonoBehaviour
{
    public Image imgIcon;
    public Text textTitle;
    public Text textPlayer;
    public Text textSendTime;
    public Image imgIsNew;

    public Text textSend;       //来自:

    [HideInInspector]
    public PB.HSMail info;

    void Start()
    {
        OnLanguageChanged();
    }

    public void SetMailItem(PB.HSMail info)
    {
        this.info = info;
        textTitle.text = info.subject;
        textPlayer.text = info.senderName;
        textSendTime.text = GameTimeMgr.GetTime(info.sendTimeStamp).ToString("yyyy-MM-dd");

        UpdateMailState();
    }

    public void UpdateMailState()
    {
        imgIsNew.gameObject.SetActive(info.state == (int)PB.mailState.UNREAD);
    }

    void OnLanguageChanged()
    {
        textSend.text = StaticDataMgr.Instance.GetTextByID("mail_laizi");
    }

}
