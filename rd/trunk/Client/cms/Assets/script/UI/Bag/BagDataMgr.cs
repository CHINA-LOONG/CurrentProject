using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BagDataMgr 
{
	static	BagDataMgr instance = null;
	public	static BagDataMgr Instance
	{
		get
		{
			if(null == instance)
			{
				instance = new BagDataMgr();
			}
			return instance;
		}
	}

	public	List<ItemData>	GetBagItemsWithType(BagType bagType,BagState bagState)
	{
		GameItemData gameItemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData;

		List<ItemData> allItems = new List<ItemData> ();
		foreach (ItemData subItem in gameItemData.itemList.Values)
		{
			ItemStaticData stData = StaticDataMgr.Instance.GetItemData (subItem.itemId);
			if(bagType == GetItemBagType(stData))
			{
				if(bagState == BagState.Sell)
				{
					if(stData.sellPrice < 1)
						continue;
				}
				allItems.Add(subItem);
			}
		}

		allItems.Sort (SortBagItem);

		return allItems;
	}

	public	static	BagType	GetItemBagType(ItemStaticData stData)
	{
		if(null == stData)
			return BagType.Unknow;

		switch (stData.type)
		{
		case (int)PB.toolType.COMMONTOOL://1材料
			return BagType.Cailiao;
			break;
		case (int)PB.toolType.GEMTOOL://3宝石
			return BagType.Baoshi;
			break;
		case (int)PB.toolType.BOXTOOL://4宝箱
			return BagType.BaoXiang;
			break;
		case (int)PB.toolType.USETOOL://5消耗品
			if(stData.classType == 1)
			{
				return BagType.Xiaohao;
			}
			break;
		}
		return BagType.Unknow;
	}
	
	public static	int SortBagItem(ItemData itemA, ItemData itemB)
	{
		ItemStaticData stDataA = StaticDataMgr.Instance.GetItemData (itemA.itemId);
		ItemStaticData stDataB = StaticDataMgr.Instance.GetItemData (itemB.itemId);
		if (null == stDataA || null == stDataB)
			return 0;

		// order 1..2
		if (stDataA.classType > stDataB.classType)
		{
			return 1;
		}
		else if (stDataA.classType < stDataB.classType)
		{
			return -1;
		}

		//order 2..1
		if (stDataA.grade < stDataB.grade)
		{
			return 1;
		}
		else if (stDataA.grade > stDataB.grade)
		{
			return -1;
		}

		//orecer 2..1
		if (itemA.count < itemB.count)
		{
			return 1;
		}
		else if (itemA.count > itemB.count)
		{
			return -1;
		}

		return 0;
	}
}
