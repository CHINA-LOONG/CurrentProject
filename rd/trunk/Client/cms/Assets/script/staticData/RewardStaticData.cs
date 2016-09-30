using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RewardStaticData
{
    public string id;
    public string reward;
}

public class RewardItemData
{
    public PB.RewardItem protocolData=new PB.RewardItem();
    public float prob;

    private ItemData itemData;
    public ItemData ItemData
    {
        get
        {
            if (itemData == null && (protocolData.type == (int)PB.itemType.ITEM || protocolData.type == (int)PB.itemType.PLAYER_ATTR))
            {
                itemData = RewardItemData.GetItemData(protocolData);
            }
            return itemData;
        }
    }

    public static ItemData GetItemData(PB.RewardItem protocolData)
    {
        ItemData itemData = null;
        if (protocolData.type == (int)PB.itemType.ITEM || protocolData.type == (int)PB.itemType.PLAYER_ATTR)
        {
            if (protocolData.type == (int)PB.itemType.ITEM)
            {
                itemData = new ItemData() { itemId = protocolData.itemId, count = (int)protocolData.count };
            }
            else
            {
                switch ((PB.changeType)(int.Parse(protocolData.itemId)))
                {
                    case PB.changeType.CHANGE_GOLD:
                        itemData = new ItemData() { itemId = "90002", count = (int)protocolData.count };
                        break;
                    case PB.changeType.CHANGE_COIN:
                        itemData = new ItemData() { itemId = "90001", count = (int)protocolData.count };
                        break;
                    default:
                        break;
                }
            }
        }
        return itemData;
    }

    public RewardItemData(int type, string itemId, int count, float prob)
    {
        this.protocolData.type = type;
        this.protocolData.itemId = itemId;
        this.protocolData.count = count;
        this.prob = prob;
    }

    public RewardItemData(int type, string itemId, int count, int stage, int level, float prob)
    {
        this.protocolData.type = type;
        this.protocolData.itemId = itemId;
        this.protocolData.count = count;
        this.protocolData.stage = stage;
        this.protocolData.level = level;
        this.prob = prob;

        if(type == (int)PB.itemType.MONSTER)
        {
            PB.HSMonster monster = new PB.HSMonster();
            monster.cfgId = itemId;
            monster.stage = stage;
            monster.monsterId = -1;
            monster.level = level;

            this.protocolData.monster = monster;
        }
    }

}


public class RewardData
{
    public string id;
    public List<RewardItemData> itemList;
}