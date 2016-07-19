using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIMailList : MonoBehaviour
{

    public Text textMailnone;   //没有附件哦

    public GameObject content;
    public GameObject mainNone;

    public System.Action<PB.HSMail> actionReadMail;

    //private List<PB.HSMail> infos;
    private List<mailItem> items = new List<mailItem>();
    private List<mailItem> itemsPool = new List<mailItem>();

    void Start()
    {
        OnLanguageChanged();
    }

    public void RefreshList(List<PB.HSMail> list)
    {
        DeleteAllElement();
        //this.infos = list;

        mainNone.SetActive(list.Count<=0);
        if (list.Count<=0)
			return;
        list.Sort(SortMail);

        for (int i = 0; i < list.Count; i++)
        {
            mailItem item = AddElement();
            item.SetMailItem(list[i]);
        }
    }

    public mailItem AddElement()
    {
        mailItem item = null;
        if (itemsPool.Count<=0)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("mailItem");
            if (null != go)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(content.transform, false);
                ScrollViewEventListener.Get(go).onClick = SingleMailClick;
                item = go.GetComponent<mailItem>();
            }
        }
        else
        {
            item = itemsPool[itemsPool.Count - 1];
            item.gameObject.SetActive(true);
            itemsPool.Remove(item);
        }
        items.Add(item);
        return item;
    }

    public mailItem DeleteElement(int mailId)
    {
        mailItem item = null;
        //bool result=false;
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].info.mailId==mailId)
            {
                item = items[i];
                item.gameObject.SetActive(false);
                items.Remove(item);
                itemsPool.Add(item);
                //result = true;
                break;
            }
        }
        return item;
    }

    public void DeleteAllElement()
    {
        mailItem item = null;
        for (int i = items.Count - 1; i >= 0; i--)
        {
            item = items[i];
            item.gameObject.SetActive(false);
            items.Remove(item);
            itemsPool.Add(item);
        }
    }

    void SingleMailClick(GameObject go)
    {
        mailItem item = go.GetComponent<mailItem>();
        PB.HSMail info = item.info;

        if (info.state == (int)PB.mailState.UNREAD)
        {
            PB.HSMailRead param = new PB.HSMailRead();
            param.mailId = item.info.mailId;
            GameApp.Instance.netManager.SendMessage(PB.code.MAIL_READ_C.GetHashCode(), param);
            UINetRequest.Close();
            info.state = (int)PB.mailState.READ;
            item.UpdateMailState();
        }

        actionReadMail(info);
    }

    void OnLanguageChanged()
    {
        //TODO:change language
        textMailnone.text = StaticDataMgr.Instance.GetTextByID("mail_meiyouyoujian");
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
            else
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
            else
            {
                result = 1;
            }
        }
        return result;
    }
}
