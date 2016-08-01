using UnityEngine;
using System.Collections;

public class ComposeField : MonoBehaviour
{
    public Transform transPos;
    [HideInInspector]
    public ItemIcon itemIcon;
    public GameObject objMask;
    [HideInInspector]
    public ItemData curData;
    private System.Action<ComposeField> onClickBack;

    public void Initialize()
    {
        if (itemIcon!=null)
        {
            itemIcon.gameObject.SetActive(false);
        }
        SetDisable(false);
        curData = null;
    }


    public ItemIcon SetItemIcon(ItemData data, System.Action<ComposeField> clickBack = null)
    {
        curData = data;
        onClickBack = clickBack;
        if (curData == null)
        {
            if (itemIcon!=null)
            {
                itemIcon.gameObject.SetActive(false);
            }
            return null;
        }
        else
        {
            ItemData tempData = new ItemData() { itemId = data.itemId, count = 0 };
            if (itemIcon == null)
            {
                itemIcon = ItemIcon.CreateItemIcon(tempData,false);
                EventTriggerListener.Get(itemIcon.iconButton.gameObject).onClick = OnClickItem;
                UIUtil.SetParentReset(itemIcon.transform, transPos);
            }
            else
            {
                itemIcon.gameObject.SetActive(true);
                itemIcon.RefreshWithItemInfo(tempData,false);
            }
            return itemIcon;
        }
    }

    public void SetDisable(bool disable)
    {
        objMask.SetActive(disable);
    }

    void OnClickItem(GameObject go)
    {
        if (onClickBack != null)
        {
            onClickBack(this);
        }
    }

}
