using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface IUsedExpCallBack
{
 
}


public class EXPListItem : MonoBehaviour
{
    public Transform transIconPos;
    private ItemIcon itemIcon;

    public Text textName;
    public Text textCount;
    public Text textExp;

    public Button btnUsed;

    private ItemStaticData itemData;
    private ItemData itemInfo;

    void Start()
    {
        //ScrollViewEventListener.Get(btnUsed.gameObject).onClick
    }

    public void OnReload(ItemStaticData staticData)
    {
        itemData = staticData;
        itemInfo = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(itemData.id);

        ItemData tempInfo = new ItemData() { itemId = itemData.id, count = 0 };
        if (itemIcon==null)
        {
            itemIcon = ItemIcon.CreateItemIcon(tempInfo);
            UIUtil.SetParentReset(itemIcon.transform, transIconPos);
        }
        else
        {
            itemIcon.RefreshWithItemInfo(tempInfo);
        }

        UIUtil.SetStageColor(textName, itemData);
        textCount.text = "*" + (itemInfo == null ? 0 : itemInfo.count);
        textExp.text = string.Format("{0}+{1}",StaticDataMgr.Instance.GetTextByID("Exp"),itemData.addAttrValue);
    }


}
