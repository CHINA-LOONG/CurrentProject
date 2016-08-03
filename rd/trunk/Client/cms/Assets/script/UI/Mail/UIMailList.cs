using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIMailList : MonoBehaviour,IScrollView
{

    //public Text textMailnone;   //没有附件哦
    public FixCountScrollView scrollView;
    //public GameObject mainNone;

    public Action<PB.HSMail> actionReadMail;
    
    private List<PB.HSMail> infos;
    private List<mailItem> items = new List<mailItem>();
    private List<mailItem> itemsPool = new List<mailItem>();
    

    public void Clean()
    {
        scrollView.CleanContent();
    }

    public void RefreshList(List<PB.HSMail> list)
    {
        this.infos = list;
        if (infos.Count<=0)
			return;
        infos.Sort(SortMail);

        scrollView.InitContentSize(infos.Count, this);
    }

    public void RemoveItem()
    {
        scrollView.InitContentSize(infos.Count, this);
    }
    
    void SingleMailClick(GameObject go)
    {
        mailItem item = go.GetComponent<mailItem>();
        PB.HSMail info = item.info;

        if (info.state == (int)PB.mailState.UNREAD)
        {
            PB.HSMailRead param = new PB.HSMailRead();
            param.mailId = item.info.mailId;
            GameApp.Instance.netManager.SendMessage(PB.code.MAIL_READ_C.GetHashCode(), param,false);

            info.state = (int)PB.mailState.READ;
            item.UpdateMailState();
        }

        actionReadMail(info);
    }

    public static int SortMail(PB.HSMail a, PB.HSMail b)
    {
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

    public void ReloadData(Transform item, int index)
    {
        mailItem mail = item.GetComponent<mailItem>();
        mail.ReloadData(infos[index]);
    }

    public Transform CreateData(Transform parent, int index = 0)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("mailItem");
        UIUtil.SetParentReset(go.transform, parent);
        ScrollViewEventListener.Get(go).onClick = SingleMailClick;
        return go.transform;
    }

    public void CleanData(List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
        itemList.Clear();
    }
}
