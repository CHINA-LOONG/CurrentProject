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

    public Action<MailItemInfo> actionReadMail;

    public Text textEmptytips;
    
    private List<MailItemInfo> infos=new List<MailItemInfo>();
    private List<mailItem> items = new List<mailItem>();
    private List<mailItem> itemsPool = new List<mailItem>();

    void Start()
    {
        textEmptytips.text = StaticDataMgr.Instance.GetTextByID("list_empty");
    }

    public void Clean()
    {
        scrollView.CleanContent();
    }

    public void RefreshList(List<MailItemInfo> list)
    {
        this.infos = list;

        textEmptytips.gameObject.SetActive(infos.Count <= 0);
        //infos.Sort(SortMail);

        scrollView.InitContentSize(infos.Count, this);
    }

    public void RemoveItem()
    {
        scrollView.InitContentSize(infos.Count, this);
    }
    
    void SingleMailClick(GameObject go)
    {
        mailItem item = go.GetComponent<mailItem>();
        MailItemInfo mailInfo = item.CruData;

        if (mailInfo.info.state == (int)PB.mailState.UNREAD)
        {
            PB.HSMailRead param = new PB.HSMailRead();
            param.mailId = item.CruData.info.mailId;
            GameApp.Instance.netManager.SendMessage(PB.code.MAIL_READ_C.GetHashCode(), param,false);

            mailInfo.info.state = (int)PB.mailState.READ;
            item.UpdateMailState();
        }

        actionReadMail(mailInfo);
    }

    public void IScrollViewReloadItem(Transform item, int index)
    {
        mailItem mail = item.GetComponent<mailItem>();
        mail.ReloadData(infos[index]);
    }

    public Transform IScrollViewCreateItem(Transform parent, int index = 0)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("mailItem");
        UIUtil.SetParentReset(go.transform, parent);
        ScrollViewEventListener.Get(go).onClick = SingleMailClick;
        return go.transform;
    }

    public void IScrollViewCleanItem(List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
        itemList.Clear();
    }
}
