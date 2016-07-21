using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class ItemIcon : MonoBehaviour 
{
	protected	enum IconType:int
	{
		Equip = 0,
		GemTool,
		UseTool,
		Material_Common,
		Material_Fragment,
		TypeCount
	}

	#region ----------------Create Method

	public	static	ItemIcon	CreateItemIcon (ItemData	itemInfo)
	{ 
		GameObject go = ResourceMgr.Instance.LoadAsset("ItemIcon");
		ItemIcon itemIcon = go.GetComponent<ItemIcon>();
		itemIcon.RefreshWithItemInfo (itemInfo);
		return itemIcon;
	}
	
	public	static	ItemIcon	CreateItemIcon (EquipData	equipInfo)
	{
		GameObject go = ResourceMgr.Instance.LoadAsset("ItemIcon");
		ItemIcon itemIcon = go.GetComponent<ItemIcon>();
		itemIcon.RefreshWithEquipInfo (equipInfo);
		return itemIcon;
	}
	#endregion


	public	Image 	frameImage;
	public	Image	itemImage;
	public	Image	pieceImage;
	public	Text	equipLevelText;
	public	Text	itemCountText;
	public	Button	iconButton;

	private	IconType	iconType;
	ItemData			itemInfo;
    EquipData equipInfo;

	#region --------------public接口----------------------

	public	bool	RefreshWithItemInfo(ItemData	itemInfo)
	{
		string itemStaticId = itemInfo.itemId;

		ItemStaticData itemStaticData = StaticDataMgr.Instance.GetItemData (itemStaticId);
		if (null == itemStaticData)
			return false;

		iconType = GetIconType (itemStaticData.type);
		this.itemInfo = itemInfo;

		SetItemIconUi (itemStaticData);
		return true;
	}

	public	bool	RefreshWithEquipInfo(EquipData	equipInfo)
	{
		ItemStaticData itemStaticData =  StaticDataMgr.Instance.GetItemData (equipInfo.equipId);
		if (null == itemStaticData)
			return false;

		iconType = IconType.Equip;
		this.equipInfo = equipInfo;

		SetItemIconUi (itemStaticData);
		return true;
	}

	public	void	HideItemCountText()
	{
		itemCountText.text = "";
	}
	#endregion

	private	IconType GetIconType(int itemType)
	{
		switch (itemType)
		{
		case (int)PB.toolType.EQUIPTOOL:
			return IconType.Equip;

		case (int)PB.toolType.GEMTOOL:
			return IconType.GemTool;

		case (int)PB.toolType.BOXTOOL:
		case (int)PB.toolType.USETOOL:
			return IconType.UseTool;

		case (int)PB.toolType.COMMONTOOL:
			return IconType.Material_Common;
		case (int)PB.toolType.FRAGMENTTOOL:
			return	IconType.Material_Fragment;			
		}

		return IconType.TypeCount;
	}

	private	void	SetItemIconUi(ItemStaticData itemStaticData)
	{
		int stage = 0;
		int equipLevel = 0;
		int itemCount = 0;
		bool showFragmentImg = false;

		string itemIcon = itemStaticData.asset;
		SetIconImage (itemIcon);

		if (iconType == IconType.Equip) 
		{
			stage = equipInfo.stage;
			equipLevel = equipInfo.level;

			SetEquipLevelText(equipLevel,stage);
		}
		else 
		{
			stage = itemStaticData.grade;
			itemCount = itemInfo.count;

			SetItemCountText(itemCount);

			if(iconType == IconType.Material_Fragment)
			{
				showFragmentImg = true;
			}
		}
		SetFrameImage(stage);

		SetFragmentImage (showFragmentImg);
	}

	private void SetIconImage(string iconName)
	{
		Sprite iconImg = ResourceMgr.Instance.LoadAssetType<Sprite>(iconName) as Sprite;
		if (null != iconImg)
			itemImage.sprite = iconImg;
	}

	private void SetFrameImage(int stage)
	{
		string assetname = "grade_" + stage.ToString ();
		Sprite headImg = ResourceMgr.Instance.LoadAssetType<Sprite>(assetname) as Sprite;
		if (null != headImg)
			frameImage.sprite = headImg;
	}

	private void SetEquipLevelText(int equipLevel,int stage)
	{
		if (equipLevel < 1) 
		{
			equipLevelText.text = "";
			return;
		}
		equipLevelText.text = string.Format ("<color={0}>+{1}</color>", GetColorWithStage(stage), equipLevel);
	}

	private void SetItemCountText(int itemCount)
	{
		string strCount = "";
		if (itemCount > 1) 
		{
			strCount = string.Format("{0}",itemCount);
		}
		itemCountText.text = strCount;
	}

	private	void SetFragmentImage(bool bShow)
	{
		pieceImage.gameObject.SetActive (bShow);
	}

	private	string GetColorWithStage(int stage)
	{
		switch(stage)
		{
		case 1:
			return "#dededeff";
		case 2:
			return "#5cc639ff";
		case 3:
			return "#1b85ffff";
		case 4:
			return "#d130a5ff";
		case 5:
			return "#ef7131ff";
		case 6:
			return "#d31c1dff";
		}
		return "#dededeff";
	}
}
