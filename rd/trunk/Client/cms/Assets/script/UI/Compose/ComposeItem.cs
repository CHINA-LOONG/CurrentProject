using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

    public ItemDataInfo curData;

    private System.Action<ComposeItem> onClickBack;
    private int selectCount = 0;
    public int SelectCount
    {
        get { return selectCount; }
        set 
        { 
            selectCount = value;
            Refresh();
        }
    }

    void Start()
    {
        ScrollViewEventListener.Get(btnSelect.gameObject).onClick = OnClickItem;
    }

    public void ReloadData(ItemDataInfo data, System.Action<ComposeItem> clickBack=null)
    {
        this.curData = data;
        this.onClickBack = clickBack;
        this.SelectCount = 0;

        UIUtil.SetStageColor(textName, curData.staticData);
        if (curData.staticData.type == (int)PB.toolType.GEMTOOL)
        {
            objAttr.SetActive(false);
            EquipLevelData attr = StaticDataMgr.Instance.GetEquipLevelData(curData.staticData.gemId);
            UIUtil.SetDisPlayAttr(attr, text_Attr1, textAttr1, text_Attr2, textAttr2);
        }
        else
        {
            objAttr.SetActive(false);
        }
    }

    void Refresh()
    {
        ItemData tempData = new ItemData() { itemId = curData.itemData.itemId, count = curData.itemData.count - SelectCount };
        if (itemIcon == null)
        {
            itemIcon = ItemIcon.CreateItemIcon(tempData);
            UIUtil.SetParentReset(itemIcon.transform, transPos);
        }
        else
        {
            itemIcon.gameObject.SetActive(true);
            itemIcon.RefreshWithItemInfo(tempData);
        }
        if (tempData.count<=0)
        {
            SetDisable(true);
        }
    }

    public void SetDisable(bool disable)
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
