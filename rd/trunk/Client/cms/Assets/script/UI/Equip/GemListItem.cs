using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public interface IXiangQianCallBack
{
    void OnMosaicReturn(ItemData data);
}

public class GemListItem : MonoBehaviour
{
    public Transform transIconPos;
    private ItemIcon itemIcon;

    public Text textName;
    public Text text_Attr1;
    public Text textAttr1;
    public Text text_Attr2;
    public Text textAttr2;

    public Button btnXiangqian;

    public IXiangQianCallBack XiangqianDelegate;

    public UIGemList.GemInfo curData;

    void Start()
    {
        btnXiangqian.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("equip_inlay_xiangqian");
        ScrollViewEventListener.Get(btnXiangqian.gameObject).onClick = OnClickXiangqian;
    }

    public void OnReload(UIGemList.GemInfo data)
    {
        this.curData = data;
        if (itemIcon==null)
        {
            itemIcon = ItemIcon.CreateItemIcon(curData.itemData);
            UIUtil.SetParentReset(itemIcon.transform, transIconPos);
        }
        else
        {
            itemIcon.RefreshWithItemInfo(curData.itemData);
        }
        textName.text = StaticDataMgr.Instance.GetTextByID(curData.itemInfo.name);
        EquipLevelData attr = StaticDataMgr.Instance.GetEquipLevelData(curData.itemInfo.gemId);
        UIUtil.SetDisPlayAttr(attr, text_Attr1, textAttr1, text_Attr2, textAttr2);
    }


    void OnClickXiangqian(GameObject go)
    {
        if (XiangqianDelegate!=null)
        {
            XiangqianDelegate.OnMosaicReturn(curData.itemData);
        }
    }


}
