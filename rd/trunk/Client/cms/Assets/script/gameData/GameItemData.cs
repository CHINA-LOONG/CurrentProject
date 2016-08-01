using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ItemData
{
    public string itemId;
    public int count;

    public static ItemData valueof(string itemId, int count) 
    {
        ItemData itemData = new ItemData();
        itemData.itemId = itemId;
        itemData.count = count;
        return itemData;
    }
}

public class GameItemData
{
    public Dictionary<string, ItemData> itemList = new Dictionary<string, ItemData>();

    public void AddItem(string itemId, int count)
    {
        ItemData itemData;
        if (itemList.TryGetValue(itemId, out itemData))
        {
            itemData.count += count;
        }
        else
        {
            itemList.Add(itemId, ItemData.valueof(itemId, count));
        }
    }

    public bool RemoveItem(string itemId, int count)
    {
        ItemData itemData;
        if (itemList.TryGetValue(itemId, out itemData))
        {
            itemData.count -= count;
            if (itemData.count == 0)
            {
                itemList.Remove(itemId);
            }

            return true;
        }

        return false;
    }

    public ItemData getItem(string itemId)
    {
        ItemData itemData;
        if (itemList.TryGetValue(itemId, out itemData))
        {
            return itemData;
        }

        return null;
    }
}

public class ItemDataInfo
{
    public ItemData itemData;
    public ItemStaticData staticData;
}
