using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDropManager : MonoBehaviour {
	
	static ItemDropManager mInst = null;
    List<GameObject> dropItemList = new List<GameObject>();
	public static ItemDropManager Instance
	{
		get
		{
			if (mInst == null)
			{
				GameObject go = new GameObject("ItemeDropMgr");
				mInst = go.AddComponent<ItemDropManager>();
			}
			return mInst;
		}
	}

	public void Fall(int dropType, Transform node)
	{
        if (dropType == 0)
            return;
        GameObject clone = null;//接收克隆
        float randomNum = Random.Range(-20f, 90f);//生成随机-20~90随机角度

        if (dropType == 1 || dropType == 2)//掉落钱或钻石
        {
            clone = ResourceMgr.Instance.LoadAsset("money");
        }
        else if (dropType == 3 || dropType == 4)//掉落道具或装备
        {
            clone = ResourceMgr.Instance.LoadAsset("prop");
        }

        if (clone != null)
        {
            clone.transform.position = node.position;
            clone.transform.rotation = Quaternion.Euler(0, randomNum, Random.Range(-5f, 5f));
            dropItemList.Add(clone);
        }
	}

    public void DestroyDropItem(GameObject go)
    {
        ResourceMgr.Instance.DestroyAsset(go);
        dropItemList.Remove(go);
    }

    public void ClearDropItem()
    {
        DropItems dropItems;
        for (int i = 0; i < dropItemList.Count; ++i)
        {
            dropItems = dropItemList[i].GetComponent<DropItems>();
            if (dropItems != null)
            {
                dropItems.OnHit();
            }
        }

        dropItemList.Clear();
    } 
}