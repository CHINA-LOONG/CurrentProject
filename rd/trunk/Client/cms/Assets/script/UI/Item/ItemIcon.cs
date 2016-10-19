using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoadItemIconEventArgs : System.EventArgs
{
    public LoadItemIconEventArgs(
        AssetLoadedCallBack assetCallBack,
        ItemData itemData,
        EquipData equipData,
        bool showTips,
        bool showGetby
        )
    {
        this.assetCallBack = assetCallBack;
        this.showTips = showTips;
        this.equipData = equipData;
        this.showGetby = showGetby;
        this.itemData = itemData;
    }

    public ItemData itemData;
    public EquipData equipData;
    public bool showTips;
    public bool showGetby;
    public AssetLoadedCallBack assetCallBack;
}
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
    public static void CreateItemIconIconAsync(
        ItemData itemInfo,
        bool showTips = true,
        bool showGetby = true,
        AssetLoadedCallBack callback = null
        )
    {
        AssetRequest requestUI = new AssetRequest("ItemIcon");
        requestUI.assetCallBack = CreateItemIconCallback;
        requestUI.args = new LoadItemIconEventArgs(callback, itemInfo, null, showTips, showGetby);
        ResourceMgr.Instance.LoadAssetAsyn(requestUI);
    }
    public static void CreateItemIconIconAsync(
        EquipData equipInfo,
        bool showTips = true,
        bool showGetby = true,
        AssetLoadedCallBack callback = null
        )
    {
        AssetRequest requestUI = new AssetRequest("ItemIcon");
        requestUI.assetCallBack = CreateItemIconCallback;
        requestUI.args = new LoadItemIconEventArgs(callback, null, equipInfo, showTips, showGetby);
        ResourceMgr.Instance.LoadAssetAsyn(requestUI);
    }
    public static void CreateItemIconCallback(GameObject itemIcon, System.EventArgs args)
    {
        LoadItemIconEventArgs itemIconArgs = args as LoadItemIconEventArgs;
        if (null != itemIcon && itemIconArgs != null)
        {
            ItemIcon icon = itemIcon.GetComponent<ItemIcon>();
            icon.RefreshWithItemInfo(itemIconArgs.itemData, itemIconArgs.showTips, itemIconArgs.showGetby);
            if (itemIconArgs.assetCallBack != null)
            {
                itemIconArgs.assetCallBack(itemIcon, args);
            }
        }
    }
    /// <summary>
    /// 创建物品ICON  设置点击处理
    /// </summary>
    /// <param name="itemInfo">要创建icon的物品</param>
    /// <param name="showTips">点击是否显示tips</param>
    /// <param name="showGetby">tips中是否显示获取途径</param>
    /// <returns></returns>
    public static ItemIcon CreateItemIcon(ItemData itemInfo, bool showTips = true, bool showGetby = true)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("ItemIcon");
        ItemIcon itemIcon = go.GetComponent<ItemIcon>();
        itemIcon.RefreshWithItemInfo(itemInfo, showTips, showGetby);
        return itemIcon;
    }
    /// <summary>
    /// 创建装备icon    设置icon点击事件
    /// </summary>
    /// <param name="equipInfo">要创建icon的装备</param>
    /// <param name="showTips">点击是否显示装备tips</param>
    /// <param name="showGetby">tips中是否显示获取途径</param>
    /// <returns></returns>
    public static ItemIcon CreateItemIcon(EquipData equipInfo, bool showTips = true, bool showGetby = true)
	{
		GameObject go = ResourceMgr.Instance.LoadAsset("ItemIcon");
		ItemIcon itemIcon = go.GetComponent<ItemIcon>();
		itemIcon.RefreshWithEquipInfo (equipInfo,showTips,showGetby);
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

    //小龙添加 2015-10-29 20:20:16
    public ItemData ItemInfo
    {
        get { return itemInfo; }
    }

	private	IconType	iconType;
    private bool showGetby;
    ItemData itemInfo;

    EquipData equipInfo;

    private bool showTips = false;

    public bool ShowTips
    {
        get { return showTips; }
        set 
        {
            //if (showTips != value)
           // {
                showTips = value;
                if (showTips)
                {
                    ScrollViewEventListener.Get(iconButton.gameObject).onClick = OnClickIconBtn;
                }
                else
                {
                    ScrollViewEventListener.Get(iconButton.gameObject).onClick = null;
                }
           // }
        }
    }

	#region --------------public接口----------------------

    public bool RefreshWithItemInfo(ItemData itemInfo, bool showTips = true, bool showGetby = true)
    {
        ShowTips = showTips;
        string itemStaticId = itemInfo.itemId;
        this.showGetby = showGetby;

        ItemStaticData itemStaticData = StaticDataMgr.Instance.GetItemData(itemStaticId);
        if (null == itemStaticData)
            return false;

        iconType = GetIconType(itemStaticData.type);
        this.itemInfo = itemInfo;

        SetItemIconUi(itemStaticData);
        return true;
    }

    public bool RefreshWithEquipInfo(EquipData equipInfo, bool showTips = true, bool showGetby = true)
    {
        ShowTips = showTips;
        this.showGetby = showGetby;
        ItemStaticData itemStaticData = StaticDataMgr.Instance.GetItemData(equipInfo.equipId);
        if (null == itemStaticData)
            return false;

        iconType = IconType.Equip;
        this.equipInfo = equipInfo;

        SetItemIconUi(itemStaticData);
        return true;
    }

    public void HideItemCountText()
    {
        itemCountText.text = "";
    }

    public void HideItemButton(bool hide = true)
    {
        iconButton.gameObject.SetActive(!hide);
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

    public void OnClickIconBtn(GameObject go)
    {
        string itemStaticId;
        if(iconType == IconType.Equip)
        {
            UIEquipTips.OpenEquipTips(equipInfo, showGetby);
            //UIEquipDetails.openEquipTips(equipInfo);
        }
        else
        {
            itemStaticId = itemInfo.itemId;
            UIPropTips tips = UIPropTips.openPropTips(itemStaticId, showGetby);
        }
    }

}
