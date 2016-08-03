using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ComposeItemInfo
{
    public ItemDataInfo itemInfo;
    private int selectCount = 0;
    private bool isDisable=false;
    public System.Action Refresh;

    public int SelectCount
    {
        get{return selectCount;}
        set
        {
            if (selectCount != value)
            {
                selectCount = value;
                if (Refresh != null)
                {
                    Refresh();
                }
            }
        }
    }
    public bool IsDisable
    {
        get{return isDisable; }
        set
        {
            if (isDisable != value)
            {
                isDisable = value;
                if (Refresh != null)
                {
                    Refresh();
                }
            }
        }
    }

    public ComposeItemInfo(ItemDataInfo data)
    {
        itemInfo = data;
        SelectCount = 0;
        IsDisable = false;
    }
}

public class ComposeItem : MonoBehaviour
{
    public Transform transPos;
    private ItemIcon itemIcon;

    public Text textName;

    //宝石属性  物品隐藏
    public GameObject objAttr;
    public Text text_Attr1;
    public Text textAttr1;
    public Text text_Attr2;
    public Text textAttr2;

    public Button btnSelect;

    //禁用蒙板
    public GameObject objMask;

    private System.Action<ComposeItem> onClickBack;

    private ComposeItemInfo curData;
    public ComposeItemInfo CurData
    {
        get{return curData;}
        set
        {
            if (curData!=null)
            {
                curData.Refresh = null;
            }
            curData = value;
            curData.Refresh = RefreshInfo;
        }
    }

    void Start()
    {
        ScrollViewEventListener.Get(btnSelect.gameObject).onClick = OnClickItem;
    }

    public void Init(System.Action<ComposeItem> clickBack)
    {
        this.onClickBack = clickBack;
    }

    public void ReloadData(ComposeItemInfo info)
    {
        this.CurData = info;

        UIUtil.SetStageColor(textName, CurData.itemInfo.staticData);
        if (CurData.itemInfo.staticData.type==(int)PB.toolType.GEMTOOL)
        {
            objAttr.SetActive(true);
            EquipLevelData attr = StaticDataMgr.Instance.GetEquipLevelData(CurData.itemInfo.staticData.gemId);
            UIUtil.SetDisPlayAttr(attr, text_Attr1, textAttr1, text_Attr2, textAttr2);
        }
        else
        {
            objAttr.SetActive(false);
        }
        RefreshInfo();
    }

    public void RefreshInfo()
    {
        SetSelCount(CurData.SelectCount);
        SetDisable(CurData.IsDisable);
    }

    void SetSelCount(int select)
    {
        ItemData tempData = new ItemData() { itemId = CurData.itemInfo.itemData.itemId, count = CurData.itemInfo.itemData.count - CurData.SelectCount };
        if (itemIcon == null)
        {
            itemIcon = ItemIcon.CreateItemIcon(tempData);
            UIUtil.SetParentReset(itemIcon.transform, transPos);
        }
        else
        {
            itemIcon.RefreshWithItemInfo(tempData);
        }
        if (tempData.count<=0)
        {
            curData.IsDisable = true;
        }
    }
    void SetDisable(bool disable)
    {
        objMask.SetActive(disable);
    }

    void OnClickItem(GameObject go)
    {
        if (onClickBack!=null)
        {
            onClickBack(this);
        }
    }

}
