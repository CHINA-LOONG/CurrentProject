using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemParseType
{
    StageItemType,

    StageItemNum
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

    public bool initStageByString(string info)
    {
        if (info != null && info.Equals("") == false)
        {
            string[] items = info.Split('_');
            if (items.Length < 3)
            {
                return false;
            }

            type = int.Parse(items[0]);
            itemId = items[1];
            count = int.Parse(items[2]);
            if (items.Length == 4)
            {
                stage = int.Parse(items[3]);
            }
            return true;
        }

        return false;
    }

    public static ItemInfo valueof(string info, ItemParseType type)
    {
        ItemInfo itemInfo = new ItemInfo();
        switch (type)
        {
            case ItemParseType.StageItemType:
                itemInfo.initStageByString(info);
        	    break;
        }

        return itemInfo;
    }

    public static List<ItemInfo> getItemInfoList(string info, ItemParseType type)
    {        
        List<ItemInfo> itemInfos = new List<ItemInfo>();
		if (string.IsNullOrEmpty (info))
			return itemInfos;

        string[] items = info.Split(',');
        for (int i = 0; i < items.Length; i++)
        {
            itemInfos.Add(ItemInfo.valueof(items[i], type));
        }

        return itemInfos;
    }

}
