using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RewardItemCreator
{
    public  static  GameObject  CreateRewardItem(PB.RewardItem rewardItemData,Transform parent,bool isTips ,bool showGetby )
    {
       // PB.RewardItem subItem = rewardItemData.protocolData;

        GameObject go = null;
        switch (rewardItemData.type)
        {
            case (int)PB.itemType.ITEM:
                ItemData itemData = ItemData.valueof(rewardItemData.itemId, (int)rewardItemData.count);
                go = InitItem(itemData,isTips, showGetby);
                break;

            case (int)PB.itemType.EQUIP:
                EquipData equipData = EquipData.valueof(rewardItemData.id, rewardItemData.itemId, rewardItemData.stage, rewardItemData.level, -1, new List<PB.GemPunch>());
                go = InitEquip(equipData,isTips, showGetby);
                break;

            case (int)PB.itemType.MONSTER:
                PB.HSMonster monster = rewardItemData.monster;
                go = InitMonsterItem(monster);
                break;
            case (int)PB.itemType.PLAYER_ATTR:
                ItemData itemDataPlayerAttr = RewardItemData.GetItemData(rewardItemData);
                if (itemDataPlayerAttr != null)
                {
                    go = InitItem(itemDataPlayerAttr, isTips, showGetby);
                }
                else
                {
                    go = changeTypeIcon.CreateIcon((PB.changeType)(int.Parse(rewardItemData.itemId)), (int)rewardItemData.count).gameObject;
                    UIUtil.SetParentReset(go.transform, parent);
                }
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

    static  GameObject InitItem(ItemData itemdata,bool isTips,bool showGetby)
    {
        ItemIcon subItem = ItemIcon.CreateItemIcon(itemdata, isTips, showGetby);

        return subItem.gameObject;
    }

   static GameObject InitEquip(EquipData equipData, bool isTips, bool showGetby)
    {
        ItemIcon subitem = ItemIcon.CreateItemIcon(equipData, isTips, showGetby);

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
