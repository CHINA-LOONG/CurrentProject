using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RewardItemCreator
{
    public  static  GameObject  CreateRewardItem(PB.RewardItem rewardItemData,Transform parent,bool isTips = false)
    {
       // PB.RewardItem subItem = rewardItemData.protocolData;

        GameObject go = null;
        switch (rewardItemData.type)
        {
            case (int)PB.itemType.ITEM:
                ItemData itemData = ItemData.valueof(rewardItemData.itemId, rewardItemData.count);
                go = InitItem(itemData,isTips);
                break;

            case (int)PB.itemType.EQUIP:
                EquipData equipData = EquipData.valueof(rewardItemData.id, rewardItemData.itemId, rewardItemData.stage, rewardItemData.level, -1, new List<PB.GemPunch>());
                go = InitEquip(equipData,isTips);
                break;

            case (int)PB.itemType.MONSTER:
                PB.HSMonster monster = rewardItemData.monster;
                go = InitMonsterItem(monster);
                break;
        }
        if(null != go)
        {
            float parentWith = ((RectTransform)parent).rect.width;
            float iconWith = ((RectTransform)go.transform).rect.width;
            float fScale = parentWith / iconWith;

            go.transform.SetParent(parent);
            ((RectTransform)go.transform).anchoredPosition = new Vector2(0, 0);
            go.transform.localScale = new Vector3(fScale, fScale, fScale);
        }

        return go;
    }

    static  GameObject InitItem(ItemData itemdata,bool isTips)
    {
        ItemIcon subItem = ItemIcon.CreateItemIcon(itemdata, isTips);

        return subItem.gameObject;
    }

   static GameObject InitEquip(EquipData equipData, bool isTips)
    {
        ItemIcon subitem = ItemIcon.CreateItemIcon(equipData, isTips);

        return subitem.gameObject;
    }

   static GameObject InitMonsterItem(PB.HSMonster monster)
    {
        MonsterIcon monsterIcon = MonsterIcon.CreateIcon();
        monsterIcon.SetMonsterStaticId(monster.cfgId);
        //monsterIcon.SetLevel(monster.level);
        monsterIcon.SetStage(monster.stage);
        return monsterIcon.gameObject;
    }
}
