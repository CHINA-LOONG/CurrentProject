using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DropItem : MonoBehaviour
{
    public RectTransform iconParent;
    public Text dropInfoText;

    private RectTransform dropItemIconRt = null;

    public static DropItem CreateWith(RewardItemData rewardItemData)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("DropItem");
        DropItem ditem = go.GetComponent<DropItem>();
        if(ditem.RefreshWith(rewardItemData))
        {
            return ditem;
        }
        Destroy(go);
        return null;
    }

    public  bool    RefreshWith(RewardItemData rewardItemData)
    {
        PB.RewardItem subItem = rewardItemData.protocolData;

        switch(subItem.type)
        {
            case (int)PB.itemType.ITEM:
                ItemData itemData = ItemData.valueof(subItem.itemId, (int)subItem.count);
                return  InitItem(itemData);

            case (int)PB.itemType.EQUIP:
                EquipData equipData = EquipData.valueof(subItem.id, subItem.itemId, subItem.stage, subItem.level, -1, new List<PB.GemPunch>());
                return  InitEquip(equipData);

            case (int)PB.itemType.MONSTER:
                PB.HSMonster monster = subItem.monster;
                return  InitMonsterItem(monster);
        }
        return false;
    }

    bool    InitItem(ItemData itemdata)
    {
        ItemIcon subItem = ItemIcon.CreateItemIcon(itemdata,false);

        string desc = "";
        ItemStaticData itemStData = StaticDataMgr.Instance.GetItemData(itemdata.itemId);
        if(itemStData != null)
        {
            desc = itemStData.NameAttr;
        }
        RefreshUi(subItem.transform as RectTransform, desc);
        return true;
    }

    bool InitEquip(EquipData equipData)
    {
        ItemIcon subitem = ItemIcon.CreateItemIcon(equipData,false);

        string desc = "";
        ItemStaticData itemStData = StaticDataMgr.Instance.GetItemData(equipData.equipId);
        if (itemStData != null)
        {
            desc = itemStData.NameAttr;
        }
        RefreshUi(subitem.transform as RectTransform, desc);
        return true;
    }

    bool InitMonsterItem(PB.HSMonster monster)
    {
        MonsterIcon monsterIcon = MonsterIcon.CreateIcon();
        monsterIcon.SetMonsterStaticId(monster.cfgId);
        //monsterIcon.SetLevel(monster.level);
        monsterIcon.SetStage(monster.stage);

        string desc = "";
        UnitData unitData = StaticDataMgr.Instance.GetUnitRowData(monster.cfgId);
        if(unitData != null)
        {
            desc = unitData.NickNameAttr;
        }

        RefreshUi(monsterIcon.transform as RectTransform, desc);
        return true;
    }

    void    RefreshUi(RectTransform itemIcon,string itemDesc)
    {
        if(null != dropItemIconRt)
        {
            Destroy(dropItemIconRt.gameObject);
            dropItemIconRt = null;
        }
        itemIcon.SetParent(iconParent);
        itemIcon.anchoredPosition = new Vector2(0, 0);
        itemIcon.localScale = new Vector3(1, 1, 1);
        dropItemIconRt = itemIcon;

        dropInfoText.text = itemDesc;
    }
}
