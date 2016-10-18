using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ComposeField : MonoBehaviour
{
    public Transform transPos;
    [HideInInspector]
    public ItemIcon itemIcon;
    public Image imageField;
    [HideInInspector]
    public ItemData curData;
    private System.Action<ComposeField> onClickBack;
    private Sprite fieldActive;
    private Sprite fieldUnactive;

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
        if (disable)
        {
            if (fieldUnactive==null)
            {
                fieldUnactive = ResourceMgr.Instance.LoadAssetType<Sprite>("hecheng_tubiaokuang1");
            }
            imageField.sprite = fieldUnactive;
        }
        else
        {
            if (fieldActive==null)
            {
                fieldActive = ResourceMgr.Instance.LoadAssetType<Sprite>("hecheng_tubiaokuang2");
            }
            imageField.sprite = fieldActive;
        }
    }

    void OnClickItem(GameObject go)
    {
        if (onClickBack != null)
        {
            onClickBack(this);
        }
    }

}
