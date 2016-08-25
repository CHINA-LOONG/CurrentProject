using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIStore : UIBase
{
    public static string ViewName = "UIStore";
    public Text storeName;
    public Button closeButton;
    public Button payButton;
    public ScrollView scrollView;
	// Use this for initialization
	void Start ()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        payButton.onClick.AddListener(OnPayButtonClick);
        storeName.text = StaticDataMgr.Instance.GetTextByID("shop_store");
        UIUtil.SetButtonTitle(payButton.transform, StaticDataMgr.Instance.GetTextByID("shop_chongzhi"));
        InitStoreItems();
	}
    public override void Clean()
    {
        scrollView.ClearAllElement();
    }

    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }
    void OnPayButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
        UIMgr.Instance.OpenUI_(UIMall.ViewName, false);
    }

    void InitStoreItems()
    {
        scrollView.ClearAllElement();
        List<StoreStaticData> listItemsData = StaticDataMgr.Instance.GetStoreStaticData();
        foreach(var subItemData in listItemsData)
        {
            var subItemUi = StoreItem.CreateWith(subItemData);
            scrollView.AddElement(subItemUi.gameObject);
        }
    }
}
