using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopItem : MonoBehaviour 
{
	public	Text	itemName;
	public	ItemIcon	itemIcon;
	public	Image	coinImage;
	public	Text	priceText;

	public	Button	buyButton;
	public	Transform	hasBuyPanel;


	//data
	PB.ShopItem	shopItemData;
	int 	shopId = -1;
	int shopType = -1;

	public	static	ShopItem  CreateShopItem()
	{
		GameObject go = ResourceMgr.Instance.LoadAsset ("ShopItem");
		return go.GetComponent<ShopItem> ();
	}

	public	void	RefreshShopItem(PB.ShopItem itemData,int shopId,int shopType)
	{
		shopItemData = itemData;
		this.shopId = shopId;
		this.shopType = shopType;

		ItemStaticData itemStaticData = StaticDataMgr.Instance.GetItemData (itemData.itemId);
		if (null == itemStaticData)
		{
			Logger.LogError("shop error: can't find item id = " + itemData.itemId);
			return;
		}

		RefreshItemIcon (itemStaticData);

		itemName.text = itemStaticData.NameAttr;
		priceText.text = itemStaticData.buyPrice.ToString();
		//coinImage.sprite = 
		Sprite coinSp = ResourceMgr.Instance.LoadAssetType<Sprite>(GetCoinImageName(shopType,itemStaticData.sellType)) as Sprite;
		if(null != coinSp)
		{
			coinImage.sprite = coinSp;
		}

		if (shopItemData.hasBuy) 
		{
			hasBuyPanel.gameObject.SetActive(true);
			EventTriggerListener.Get (buyButton.gameObject).onClick = null;
		}
		else
		{
			hasBuyPanel.gameObject.SetActive(false);
			EventTriggerListener.Get(buyButton.gameObject).onClick = OnBuyItemClick;
		}
	}

	string GetCoinImageName(int stype, int sellType)
	{
		switch (stype) 
		{
		case (int)PB.shopType.NORMALSHOP:
			if(sellType == 2)
			{
				return "icon_jinbi";//2
			}
			else
			{
				return "icon_zuanshi";//1
			}
		case (int)PB.shopType.ALLIANCESHOP:
			return "icon_gonghuibi";
		default:
			return "";
		}
	}

	private	void	RefreshItemIcon(ItemStaticData itemStaticData)
	{
		if (shopItemData.type == (int)PB.itemType.EQUIP) 
		{
			EquipData equipData = new EquipData ();
			equipData.equipId = shopItemData.itemId;
			equipData.level = shopItemData.level;
			equipData.stage = shopItemData.stage;

			itemIcon.RefreshWithEquipInfo (equipData);
		}
		else 
		{
			ItemData itemData = ItemData.valueof(shopItemData.itemId,shopItemData.count);

			itemIcon.RefreshWithItemInfo(itemData);
		}
	}


	private	void	OnBuyItemClick(GameObject go)
	{
		Logger.LogError ("buy item");

		GameDataMgr.Instance.ShopDataMgrAttr.BuyShopItem (shopItemData, shopId,shopType);
	}

}
