using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class BagItem : MonoBehaviour 
{
	public	static	BagItem	Create(ItemData itemData, BagType bagType,BagState bstate)
	{
		GameObject go = ResourceMgr.Instance.LoadAsset ("bagItem");
		BagItem item = go.GetComponent<BagItem> ();
		item.RefreshBagItem (itemData, bagType, bstate);
		return	item;
	}

	//general
	public	ItemIcon	itemIcon;
	public	Text	itemName;

	//useButton
	public	Button	useButton;

	//baoxiang need item
	public	Transform	baoXiangNeedPanel;
	public	Text	needItemName;
	public	Text	needItemCount;

	//baoshi need 
	public	Transform baoshiNeedPanel;
	public	Text baoshiProperty1;
	public	Text baoshiValue1;
	public	Text baoshiProperty2;
	public	Text baoshiValue2;

	//sell panel
	public	Transform sellPanel;
	public	Button	sellSelectButton;
	public	Image	sellSelectImage;
	public	Text	sellPrice;
	public	Text	sellCount;
	public	int		inSellCount =0;

	//bag item data--------
	ItemData itemData =  null;
	BagType bagType ;

	int	leftKeyCount = 0;//剩余钥匙数量 
	string needKeyId = null;


	// Use this for initialization
	void Start () 
	{
		EventTriggerListener.Get (useButton.gameObject).onClick = OnUseButtonClicked;
		EventTriggerListener.Get (sellSelectButton.gameObject).onClick = OnSellSelectButtonClicked;
	}

	public	void	RefreshBagItem(ItemData itemData,BagType bagType, BagState bstate)
	{
		ItemStaticData itemStaticData = StaticDataMgr.Instance.GetItemData (itemData.itemId);
		if(null == itemStaticData)
		{
			Logger.LogError(string.Format("can't find itemid = {0} for refresh bagItem",itemData.itemId));
			return;
		}
		useButton.GetComponentInChildren<Text> ().text = StaticDataMgr.Instance.GetTextByID ("exp_use");
		this.bagType = bagType;
		this.itemData = itemData;
		baoXiangNeedPanel.gameObject.SetActive (bagType == BagType.BaoXiang);
		baoshiNeedPanel.gameObject.SetActive (bagType == BagType.Baoshi);
		useButton.gameObject.SetActive (bstate == BagState.Norml && (bagType == BagType.BaoXiang || bagType == BagType.Xiaohao));
		sellPanel.gameObject.SetActive (bstate == BagState.Sell && itemStaticData.sellPrice > 0);

		itemIcon.RefreshWithItemInfo (itemData);
		UIUtil.SetStageColor (itemName, itemStaticData.NameAttr, itemStaticData.grade);

		if (bagType == BagType.BaoXiang)
		{
			SetBaoxiangProperty(itemStaticData);
		}
		else if (bagType == BagType.Baoshi)
		{
			SetBaoshiProperty(itemStaticData);
		}

		if (bstate == BagState.Sell) 
		{
			if(itemStaticData.sellPrice > 0)
			{
				SetSellCount(0);
			}
		}
	}

	private	void	SetBaoxiangProperty(ItemStaticData stdata)
	{
		if (string.IsNullOrEmpty (stdata.needItem)) 
		{
			leftKeyCount = -1;//no need key
			needItemName.text = "";
			needItemCount.text = "";
			return;
		}
		string [] szstr = stdata.needItem.Split ('_');
		needKeyId = szstr [1];

		ItemStaticData needItemStData = StaticDataMgr.Instance.GetItemData (needKeyId);
		if(null == needItemStData)
		{
			Logger.LogError(string.Format("can't find itemid = {0} for baoxiang need item", stdata.needItem));
			return;
		}
		UIUtil.SetStageColor (needItemName, needItemStData.NameAttr, needItemStData.grade);

		int itemCount = 0;

		ItemData gItemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem (needKeyId);
		if (null != gItemData) 
		{
			itemCount = gItemData.count;
		}
		leftKeyCount = itemCount;
		needItemCount.text = string.Format (StaticDataMgr.Instance.GetTextByID ("bag_shengyu"), itemCount);
	}

	private	void	SetBaoshiProperty(ItemStaticData stdata)
	{
		EquipLevelData attr = StaticDataMgr.Instance.GetEquipLevelData (stdata.gemId);
		if (null != attr)
		{
			UIUtil.SetDisPlayAttr (attr, baoshiProperty1, baoshiValue1, baoshiProperty2, baoshiValue2);
		}
	}

	public void SetSellCount(int sellCount)
	{
		inSellCount = sellCount;
		string selectImgName = "";
		string sellMsg = "";
		if (sellCount > 0)
		{
			//sellMsg = string.Format (StaticDataMgr.Instance.GetTextByID ("出售{0}"), sellCount);
			selectImgName = "beibao_duigou_1";
		}
		else
		{
			selectImgName = "beibao_duigou";
		}
		this.sellCount.text = sellMsg;

		ItemStaticData itemStaticData = StaticDataMgr.Instance.GetItemData (itemData.itemId);
		sellPrice.text = itemStaticData.sellPrice.ToString ();

		//sellSelectImage
		Sprite sp = ResourceMgr.Instance.LoadAssetType<Sprite> (selectImgName);
		if (null != sp)
		{
			sellSelectImage.sprite = sp;
		}
	}

	void	OnUseButtonClicked(GameObject go)
	{
		if (BagType.BaoXiang == bagType) 
		{
			OpenBox ();
		}
		else if (BagType.Xiaohao == bagType) 
		{
			UIBag.Instance.UseItem(itemData);
		}
	}

	void OpenBox()
	{
		//leftKeyCount < 0  no need key
		if (leftKeyCount == 0) 
		{
			BuyItem.BuyItemParam param = new BuyItem.BuyItemParam();
			
			param.itemId = needKeyId;
			param.defaultbuyCount = 1;
			param.maxCount = GameConfig.Instance.maxBuyItemCount;
			param.isShowCoinButton = true;
			
			BuyItem.OpenWith(param);
		}
		else
		{
			int maxOpenCount = itemData.count;
			if(leftKeyCount > 0 && leftKeyCount < maxOpenCount)
			{
				maxOpenCount = leftKeyCount;
			}
			maxOpenCount = maxOpenCount < GameConfig.Instance.maxOpenBoxCount?maxOpenCount:GameConfig.Instance.maxOpenBoxCount;
			OpenBaoxiangDlg.OpenWith(itemData,maxOpenCount);
		}
	}

	void	OnSellSelectButtonClicked(GameObject go)
	{
		if (itemData.count == 1)
		{
			if(inSellCount ==0)
				OnSetSellCount (1);
			else
				OnSetSellCount(0);
		}
		else 
		{
			int defaultValue = inSellCount;
			if(inSellCount == 0)
			{
				defaultValue = itemData.count;
			}
			AdjustCountForSell.OpenWith (itemData.count, defaultValue , transform as RectTransform, OnSetSellCount);
		}
	}

	void OnSetSellCount(int sellCount)
	{
		SetSellCount(sellCount);
		UIBag.Instance.UpdateSellItems (itemData.itemId, sellCount);
	}

}
