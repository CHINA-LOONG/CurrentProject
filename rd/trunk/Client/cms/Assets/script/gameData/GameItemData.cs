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
        itemData.count = (int)count;
        return itemData;
    }
}

public class GameItemData
{
    public Dictionary<string, ItemData> itemList = new Dictionary<string, ItemData>();
    private Dictionary<string, PB.ItemState> itemStateDic = new Dictionary<string, PB.ItemState>();

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

    public  void    SynItemState(List<PB.ItemState> listItemState)
    {
        itemStateDic.Clear();
        if(listItemState != null)
        {
            foreach (var subItemState in listItemState)
            {
                itemStateDic.Add(subItemState.itemId, subItemState);
            }
        }
    }

    public void UpdateItemState(string id, int useCountDaily)
    {
        PB.ItemState subState = null;
        if(itemStateDic.TryGetValue(id,out subState))
        {
            subState.useCountDaily = useCountDaily;
        }
        else
        {
            subState = new PB.ItemState();
            subState.itemId = id;
            subState.useCountDaily = useCountDaily;
            itemStateDic.Add(id, subState);
        }
    }

    public  int GetItemUsedCountDaily(string itemid)
    {
        PB.ItemState itemState = null;

        if(itemStateDic.TryGetValue(itemid, out itemState))
        {
            return itemState.useCountDaily;
        }
        return 0;
    }

    public int GetItemUseLimitTimes(string itemId)
    {
        ItemStaticData itemStData = StaticDataMgr.Instance.GetItemData(itemId);
        return itemStData.times;
    }

    public void ClearData()
    {
        itemList.Clear();
        itemStateDic.Clear();
    }
}

public class ItemDataInfo
{
    public ItemData itemData;
    public ItemStaticData staticData;
}
