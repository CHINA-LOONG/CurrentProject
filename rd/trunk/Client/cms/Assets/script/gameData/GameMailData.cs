using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public class MailData
//{

//    //public PB.HSMail 
//    public int mailId;
//    public int state;
//    public int sendTimeStamp;
//    public int senderId;
//    public string senderName;
//    public string subject;
//    public string content;
//    public List<RewardItemData> annex;

//    public MailData(int mailId,
//                   int state,
//                   int sendTimeStamp,
//                   int senderId,
//                   string senderName,
//                   string subject,
//                   string content,
//                   List<RewardItemData> annex)
//    {
//        this.mailId = mailId;
//        this.state = state;
//        this.sendTimeStamp = sendTimeStamp;
//        this.senderId = senderId;
//        this.senderName = senderName;
//        this.subject = subject;
//        this.content = content;
//        this.annex = annex;
//    }

//}


public class GameMailData
{
    public Dictionary<int, PB.HSMail> mailList = new Dictionary<int, PB.HSMail>();

    public void ClearMail()
    {
        mailList.Clear();
    }

    public void AddMail(PB.HSMail mailData)
    {
        int mailId = mailData.mailId;
        if (mailList.ContainsKey(mailId))
        {
            mailList[mailId] = mailData;
        }
        else
        {
            mailList.Add(mailId, mailData);
        }
    }

    public bool RemoveMail(int mailId)
    {
        return mailList.Remove(mailId);
    }

    public PB.HSMail GetMail(int mailId)
    {
        PB.HSMail mailData;
        if (mailList.TryGetValue(mailId,out mailData))
        {
            return mailData;
        }
        return null;
    }

}
