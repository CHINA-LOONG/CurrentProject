using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ItemData
{
    public int count;

    public static ItemData valueof(int count) 
    {
        ItemData itemData = new ItemData();
        itemData.count = count;
        return itemData;
    }
}

public class GameItemData
{
    public Dictionary<int, ItemData> itemList = new Dictionary<int, ItemData>();

    public void AddItem(int itemId, int count)
    {
        ItemData itemData;
        if (itemList.TryGetValue(itemId, out itemData))
        {
            itemData.count += count;
        }
        else
        {
            itemList.Add(itemId, ItemData.valueof(count));
        }
    }

    public bool RemoveItem(int itemId, int count)
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
}

