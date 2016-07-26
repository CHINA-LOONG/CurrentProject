using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EquipMaterialItem : MonoBehaviour
{
    public Text textCount;
    private ItemIcon itemIcon;

    public void Refresh(string itemId,int count)
    {
        ItemData data = new ItemData() { itemId = itemId, count = 1 };
        if (itemIcon == null)
        {
            itemIcon = ItemIcon.CreateItemIcon(data);
            UIUtil.SetParentReset(itemIcon.transform, transform);
        }
        else
        {
            itemIcon.gameObject.SetActive(true);
            itemIcon.RefreshWithItemInfo(data);
        }

        ItemData mineItem = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(itemId);
        if (mineItem==null)
        {
            mineItem = new ItemData() { itemId = itemId, count = 0 };
        }
        Color color;
        if (mineItem.count<count)
        {
            color = ColorConst.text_color_nReq;
        }
        else
        {
            color = ColorConst.text_color_Req;
        }
        textCount.text = "<color=" + ColorConst.colorTo_Hstr(color) + ">" + (mineItem.count > 9999 ? 9999 : mineItem.count) + "</color>/" + count;
    }

}
