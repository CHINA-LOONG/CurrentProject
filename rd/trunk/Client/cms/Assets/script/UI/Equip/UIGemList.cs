using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public interface IGemListCallBack
{
    void OnMosaicReturn(ItemData data);
}


public class UIGemList : MonoBehaviour,IXiangQianCallBack
{

    public const string AssetName = "UIGemList";
    public static UIGemList CreateEquipInlay()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset(AssetName);
        UIGemList uiGemList = go.GetComponent<UIGemList>();
        return uiGemList;
    }

    public Transform content;
    public IGemListCallBack ICallBackDeletage;


    private List<ItemDataInfo> infos = new List<ItemDataInfo>();

    private List<GemListItem> items = new List<GemListItem>();
    private List<GemListItem> itemPool = new List<GemListItem>();

    public void Refresh(int gemType)
    {
        Dictionary<string, ItemData> itemList = GameDataMgr.Instance.PlayerDataAttr.gameItemData.itemList;
        ItemStaticData itemStatic;
        infos.Clear();
        foreach (var item in itemList)
        {
            itemStatic = StaticDataMgr.Instance.GetItemData(item.Value.itemId);
            if (itemStatic==null)
            {
                Logger.LogError("缺少物品配置:"+item.Value.itemId);
                continue;
            }
            if (itemStatic.type==3&&itemStatic.gemType==gemType)
            {
                infos.Add(new ItemDataInfo() { itemData = item.Value, staticData = itemStatic });
            }
        }
        RemoveAllElement();
        for (int i = 0; i < infos.Count; i++)
        {
            GemListItem item = GetElement();
            item.OnReload(infos[i]);
        }


    }

    public GemListItem GetElement()
    {
        GemListItem item = null;

        if (itemPool.Count <= 0)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("GemListItem");
            if (go != null)
            {
                UIUtil.SetParentReset(go.transform, content);
                item = go.GetComponent<GemListItem>();
                item.XiangqianDelegate = this;
            }
        }
        else
        {
            item = itemPool[itemPool.Count - 1];
            item.gameObject.SetActive(true);
            itemPool.Remove(item);
        }
        items.Add(item);
        return item;
    }
    public void RemoveElement(GemListItem item)
    {
        if (items.Contains(item))
        {
            item.gameObject.SetActive(false);
            items.Remove(item);
            itemPool.Add(item);
        }
    }
    public void RemoveAllElement()
    {
        items.ForEach(delegate(GemListItem item) { item.gameObject.SetActive(false); });
        itemPool.AddRange(items);
        items.Clear();
    }
    
    public void OnMosaicReturn(ItemData data)
    {
        if (ICallBackDeletage!=null)
        {
            ICallBackDeletage.OnMosaicReturn(data);
        }
    }
}
