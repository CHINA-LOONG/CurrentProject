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

    public static ItemIcon CreateItemIcon(ItemData itemInfo, bool showTips = true)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("ItemIcon");
        ItemIcon itemIcon = go.GetComponent<ItemIcon>();
        itemIcon.RefreshWithItemInfo(itemInfo,showTips);
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

    public Image backGround;
	public	Image 	frameImage;
	public	Image	itemImage;
	public	Image	pieceImage;
	public	Text	equipLevelText;
	public	Text	itemCountText;
	public	Button	iconButton;

	private	IconType	iconType;
	ItemData			itemInfo;
    EquipData equipInfo;

    private bool showTips = false;

    public bool ShowTips
    {
        get { return showTips; }
        set 
        {
            if (showTips != value)
            {
                showTips = value;
                if (showTips)
                {
                    ScrollViewEventListener.Get(iconButton.gameObject).onClick = OnClickIconBtn;
                }
                else
                {
                    ScrollViewEventListener.Get(iconButton.gameObject).onClick = null;
                }
            }
        }
    }

	#region --------------public接口----------------------

    public bool RefreshWithItemInfo(ItemData itemInfo, bool showTips = true)
    {
        ShowTips = showTips;
        string itemStaticId = itemInfo.itemId;

        ItemStaticData itemStaticData = StaticDataMgr.Instance.GetItemData(itemStaticId);
        if (null == itemStaticData)
            return false;

        iconType = GetIconType(itemStaticData.type);
        this.itemInfo = itemInfo;

        SetItemIconUi(itemStaticData);
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

    public void HideExceptIcon()
    {
        backGround.gameObject.SetActive(false);
        frameImage.gameObject.SetActive(false);
        pieceImage.gameObject.SetActive(false);
        equipLevelText.gameObject.SetActive(false);
        itemCountText.gameObject.SetActive(false);
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
			SetItemCountText(-1);
		}
		else 
		{
			SetEquipLevelText(-1,0);
			stage = itemStaticData.grade;
			itemCount = itemInfo.count;

			SetItemCountText(itemCount);

			if(iconType == IconType.Material_Fragment)
			{
				showFragmentImg = true;
			}
		}
		SetFrameImage(stage);

		SetFragmentImage (showFragmentImg, stage);
	}

	private void SetIconImage(string iconName)
	{
		if (string.IsNullOrEmpty (iconName))
			return;

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
		equipLevelText.text = string.Format ("+{0}",equipLevel);
		equipLevelText.color = ColorConst.GetStageTextColor (stage);

		Outline ol = equipLevelText.gameObject.GetComponent<Outline> ();
		if (null != ol)
		{
			ol.effectColor = ColorConst.GetStageOutLineColor(stage);
		}
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

	private	void SetFragmentImage(bool bShow, int stage)
	{
		pieceImage.gameObject.SetActive (bShow);
		string assetname = "suipian_" + stage.ToString ();
		Sprite headImg = ResourceMgr.Instance.LoadAssetType<Sprite>(assetname) as Sprite;
		if (null != headImg)
			pieceImage.sprite = headImg;
	}

    void OnClickIconBtn(GameObject go)
    {
        string itemStaticId;
        if(iconType == IconType.Equip)
        {
            //itemStaticId = equipInfo.equipId;
            return;
        }
        else
        {
            itemStaticId = itemInfo.itemId;
        }
        UIPropTips tips = UIPropTips.openPropTips(itemStaticId);
    }

}
