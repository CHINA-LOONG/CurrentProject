using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PetDetailRightFeed : PetDetailRightBase
{

    public Transform content;
    GameUnit m_unit;
    private List<ItemStaticData> infos = new List<ItemStaticData>();

    private List<EXPListItem> items = new List<EXPListItem>();
    private List<EXPListItem> itemPool = new List<EXPListItem>();


    public override void ReloadData(PetRightParamBase param)
    {
        m_unit = param.unit;
        StaticDataMgr.Instance.GetItemData(PB.changeType.CHANGE_MONSTER_EXP, ref infos);

        RemoveAllElement();
        for (int i = 0; i < infos.Count; i++)
        {
            EXPListItem item = GetElement();
            item.OnReload(infos[i]);
        }

    }

    public EXPListItem GetElement()
    {
        EXPListItem item = null;
        if (itemPool.Count <= 0)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("EXPListItem");
            if (go != null)
            {
                UIUtil.SetParentReset(go.transform, content);
                item = go.GetComponent<EXPListItem>();
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

    public void RemoveElement(EXPListItem item)
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
        items.ForEach(delegate(EXPListItem item) { item.gameObject.SetActive(false); });
        itemPool.AddRange(items);
        items.Clear();
    }

}
