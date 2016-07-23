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
        imgIcon.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>("baoshiditu_" + info.type);

        if (info.gemId.Equals(BattleConst.invalidGemID))
        {
            if (gemIcon != null)
            {
                gemIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            //TODO: set gem Icon
            if (gemIcon == null)
            {
                Debug.Log("存在宝石");
                gemIcon = ItemIcon.CreateItemIcon(new ItemData() { itemId = info.gemId, count = 0 });
                UIUtil.SetParentReset(gemIcon.transform, (transIcon == null ? transform : transIcon.transform));
                gemIcon.HideExceptIcon();
            }
            else
            {
                gemIcon.gameObject.SetActive(true);
                gemIcon.RefreshWithItemInfo(new ItemData() { itemId = info.gemId, count = 0 });
                gemIcon.HideExceptIcon();
            }
        }
    }

}
