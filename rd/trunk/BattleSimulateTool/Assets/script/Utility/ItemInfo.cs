using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemParseType
{
    DemandItemType,

    DemandMonsterType
}

public class ItemInfo 
{
    public int type;
    public string itemId;
    public int count;
    public int stage;
    public int level;

    public ItemInfo()
    {
        this.type = 0;
        this.itemId = null;
        this.count = 0;
        this.stage = 0;
        this.level = 0;
    }

    public ItemInfo(int type, string itemId, int count, int stage, int level)
    {
        this.type = type;
        this.itemId = itemId;
        this.count = count;
        this.stage = stage;
        this.level = level;
    }

    public ItemInfo(int type, string itemId, int count, int stage)
    {
        this.type = type;
        this.itemId = itemId;
        this.count = count;
        this.stage = stage;
        this.level = 0;
    }

    public ItemInfo(int type, string itemId, int count)
    {
        this.type = type;
        this.itemId = itemId;
        this.count = count;
        this.stage = 0;
        this.level = 0;
    }

    public static ItemInfo valueof(string info, ItemParseType type)
    {
        if (info != null && info.Equals("") == false)
        {
            ItemInfo itemInfo = new ItemInfo();
            string[] items = info.Split('_');
            if (items.Length < 3)
            {
                return null;
            }

            itemInfo.type = int.Parse(items[0]);
            itemInfo.itemId = items[1];
            itemInfo.count = int.Parse(items[2]);

            switch (type)
            {
                case ItemParseType.DemandMonsterType:
                    if (items.Length == 4)
                    {
                        itemInfo.stage = int.Parse(items[3]);
                    }
                    else
                    {
                        return null;
                    } 
                                      
                    break;
                default:
                    break;
            }
            return itemInfo;
        }

        return null;
    }

    public static List<ItemInfo> getItemInfoList(string info, ItemParseType type)
    {        
        List<ItemInfo> itemInfos = new List<ItemInfo>();
        getItemInfoList1(itemInfos, info, type);
        return itemInfos;
    }

    public static void getItemInfoList1(List<ItemInfo> itemInfos, string info, ItemParseType type)
    {
        if (string.IsNullOrEmpty(info) == false)
        { 
            string[] items = info.Split(',');
            for (int i = 0; i < items.Length; i++)
            {
                itemInfos.Add(ItemInfo.valueof(items[i], type));
            }
        }
    }
}
