using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopItem : MonoBehaviour 
{
	public	Text	itemName;
	public	ItemIcon	itemIcon;
	public	Image	coinImage;
    public Image oldCoinImage;
	public	Text	priceText;

	public	Button	buyButton;
	public	Transform	hasBuyPanel;

    public Transform cheapPanel;
    public Text offText;
    public Text oldPrice;


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

        if(itemData.count > 1)
        {
            itemName.text = string.Format("{0}*{1}", itemStaticData.NameAttr, itemData.count);
        }
        else
        {
            itemName.text = itemStaticData.NameAttr;
        }
		

		priceText.text = string.Format("{0}", Mathf.CeilToInt(itemData.price * itemData.count* shopItemData.discount));

		if (IsCoinEnough (itemData,false)) 
		{
			priceText.color = ColorConst.text_color_Enough;
		}
		else
		{
			priceText.color = ColorConst.text_color_nReq;
		}

		//coinImage.sprite = 
		Sprite coinSp = ResourceMgr.Instance.LoadAssetType<Sprite>(GetCoinImageName(shopType,itemData.priceType)) as Sprite;
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
        if(shopItemData.discount > 0.9999f)
        {
            cheapPanel.gameObject.SetActive(false);
        }
        else
        {
            cheapPanel.gameObject.SetActive(true);
            oldPrice.text = (itemData.price * itemData.count).ToString();
            offText.text = string.Format("{0}%", Mathf.CeilToInt(shopItemData.discount * 100));

            Sprite oldSp = ResourceMgr.Instance.LoadAssetType<Sprite>(GetCoinImageName(shopType, itemData.priceType)) as Sprite;
            if (null != oldSp)
            {
                oldCoinImage.sprite = oldSp;
            }
        }
    }

	string GetCoinImageName(int stype, int buyType)
	{
        switch (stype)
        {
            case (int)PB.shopType.NORMALSHOP:
                if (buyType == (int)PB.moneyType.MONEY_COIN)
                {
                    return "icon_jinbi";//2
                }
                else
                {
                    return "icon_zuanshi";//1
                }
            case (int)PB.shopType.ALLIANCESHOP:
                return "icon_gonghuibi";
            case (int)PB.shopType.TOWERSHOP:
                return "icon_towercoin";
            case (int)PB.shopType.PVPSHOP:
                return "icon_rongyudian";
            default:
                return "";
        }
    }

	bool IsCoinEnough(PB.ShopItem itemData,bool showNotEnoughTip)
	{
        bool isEnough = false;
        int costCoin = Mathf.CeilToInt(itemData.price * shopItemData.count * shopItemData.discount);
        switch (shopType)
        {
            case (int)PB.shopType.NORMALSHOP:
                if (itemData.priceType == (int)PB.moneyType.MONEY_COIN)
                {
                    isEnough = costCoin <= GameDataMgr.Instance.PlayerDataAttr.coin;//2
                    if (showNotEnoughTip && !isEnough)
                    {
                        GameDataMgr.Instance.ShopDataMgrAttr.JinbiNoEnough();
                    }
                }
                else
                {
                    isEnough = costCoin <= GameDataMgr.Instance.PlayerDataAttr.gold;//1
                    if (showNotEnoughTip && !isEnough)
                    {
                        GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
                    }
                }
                break;
            case (int)PB.shopType.ALLIANCESHOP:
                isEnough = costCoin <= GameDataMgr.Instance.PlayerDataAttr.GonghuiCoinAttr;
                if(showNotEnoughTip &&!isEnough)
                {
                    UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("shop_record_003"), (int)PB.ImType.PROMPT);
                }
                break;
            case (int)PB.shopType.TOWERSHOP:
                isEnough = costCoin <= GameDataMgr.Instance.PlayerDataAttr.TowerCoinAttr;
                if (showNotEnoughTip && !isEnough)
                {
                    UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("shop_record_003"), (int)PB.ImType.PROMPT);
                }
                break;
            case (int)PB.shopType.PVPSHOP:
                isEnough = costCoin <= GameDataMgr.Instance.PlayerDataAttr.HonorAtr;
                if(showNotEnoughTip && !isEnough)
                {
                    UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("shop_record_003"), (int)PB.ImType.PROMPT);
                }
                break;
            default:
                return false;
        }
        return isEnough;
    }

	private	void	RefreshItemIcon(ItemStaticData itemStaticData)
	{
		if (shopItemData.type == (int)PB.itemType.EQUIP)
        {
            EquipData equipData = EquipData.valueof(0, shopItemData.itemId, shopItemData.stage, shopItemData.level, BattleConst.invalidMonsterID, null);
			itemIcon.RefreshWithEquipInfo (equipData,true,false);
		}
		else 
		{
			ItemData itemData = ItemData.valueof(shopItemData.itemId,0);

			itemIcon.RefreshWithItemInfo(itemData,true,false);
		}
	}


	private	void	OnBuyItemClick(GameObject go)
	{
        if(IsCoinEnough(shopItemData,true))
        {
            GameDataMgr.Instance.ShopDataMgrAttr.BuyShopItem(shopItemData, shopId, shopType);
        }
	}

}
