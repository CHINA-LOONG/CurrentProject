using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnnexItem : MonoBehaviour
{
    public Transform transIcon;
    private ItemIcon itemIcon;

    public GameObject tips;

    private MonoBehaviour icon;


    public void Refresh(PB.RewardItem info)
    {
        //TODO: 修改uiResource后删除
        if (icon!=null)
        {
            ResourceMgr.Instance.DestroyAsset(icon.gameObject);
        }
        if (info.type==(int)PB.itemType.ITEM)
        {
            icon = ItemIcon.CreateItemIcon(new ItemData() { itemId = info.itemId, count = info.count });
            UIUtil.SetParentReset(icon.transform, transIcon);
        }
        else if (info.type == (int)PB.itemType.EQUIP)
        {
            EquipData equipData = EquipData.valueof(0, info.itemId, info.stage, info.level, BattleConst.invalidMonsterID, null);
            icon = ItemIcon.CreateItemIcon(equipData);
            UIUtil.SetParentReset(icon.transform, transIcon);
        }
        else if(info.type==(int)PB.itemType.PLAYER_ATTR)
        {
            icon = changeTypeIcon.CreateIcon((PB.changeType)(int.Parse(info.itemId)), info.count);
            UIUtil.SetParentReset(icon.transform, transIcon);
        }
        else
        {
            Logger.LogError("雷神配错了---找雷神");
        }
    }


    public void SetReceive(bool isReceive)
    {
        tips.SetActive(isReceive);
    }


}
