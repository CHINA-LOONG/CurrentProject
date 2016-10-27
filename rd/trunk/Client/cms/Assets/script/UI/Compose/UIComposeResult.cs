using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIComposeResult : UIBase
{
    public static string ViewName = "UIComposeResult";

    public Button btnClose;
    public Transform content;

    public Text text_title;
    public Text text_btnText;


    [HideInInspector]
    public List<ItemIcon> items = new List<ItemIcon>();
    [HideInInspector]
    public List<ItemIcon> itemPool = new List<ItemIcon>();

    void Start()
    {
        EventTriggerListener.Get(btnClose.gameObject).onClick = OnClickClose;
        text_title.text = StaticDataMgr.Instance.GetTextByID("compose_success");
        text_btnText.text = StaticDataMgr.Instance.GetTextByID("ui_queding");
    }

    public override void Init(bool forbidGuide = false)
    {
        base.Init();
    }

    public override void Clean()
    {
        base.Clean();
    }

    public void ReloadData(List<PB.RewardItem> rewardInfos)
    {
        RemoveAllElement();
        for (int i = 0; i < rewardInfos.Count; i++)
        {
            if (rewardInfos[i].type == (int)PB.itemType.ITEM)
            {
                GetElement(new ItemData() { itemId = rewardInfos[i].itemId, count = (int)rewardInfos[i].count });
            }
        }
    }

    public ItemIcon GetElement(ItemData data)
    {
        ItemIcon item = null;
        if (itemPool.Count <= 0)
        {
            item = ItemIcon.CreateItemIcon(data, true, false);
            UIUtil.SetParentReset(item.transform, content);
        }
        else
        {
            item = itemPool[itemPool.Count - 1];
            item.gameObject.SetActive(true);
            itemPool.Remove(item);
            item.RefreshWithItemInfo(data,true,false);
        }
        item.transform.SetAsLastSibling();
        items.Add(item);
        return item;
    }
    public void RemoveElement(ItemIcon item)
    {
        if (items.Contains(item))
        {
            item.gameObject.SetActive(false);
            itemPool.Add(item);
            items.Remove(item);
        }
    }
    public void RemoveAllElement()
    {
        items.ForEach(delegate(ItemIcon item) { item.gameObject.SetActive(false); });
        itemPool.AddRange(items);
        items.Clear();
    }

    void OnClickClose(GameObject go)
    {
        this.RequestCloseUi();
    }


}
