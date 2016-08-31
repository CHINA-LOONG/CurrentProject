using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EquipGemItem : MonoBehaviour
{
    public Image imgIcon;
    public Transform transIcon;
    private ItemIcon gemIcon;

    public void Refresh(GemInfo info)
    {
        imgIcon.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(UIUtil.GetBaoshidituByType(info.type));
        if (info.gemId.Equals(BattleConst.invalidGemID))
        {
            if (gemIcon != null)
            {
                gemIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            ItemData itemData = new ItemData() { itemId = info.gemId, count = 0 };
            if (gemIcon == null)
            {
                Debug.Log("存在宝石");
                gemIcon = ItemIcon.CreateItemIcon(itemData,false);
                UIUtil.SetParentReset(gemIcon.transform, (transIcon == null ? transform : transIcon.transform));
                gemIcon.HideExceptIcon();
            }
            else
            {
                gemIcon.gameObject.SetActive(true);
                gemIcon.RefreshWithItemInfo(itemData,false);
                gemIcon.HideExceptIcon();
            }
        }
    }

}
