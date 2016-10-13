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

    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<long>(GameEventList.CoinChanged, OnCoinChanged);
        GameEventMgr.Instance.AddListener<int>(GameEventList.ZuanshiChanged, OnZuanshiChanged);
    }
    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<long>(GameEventList.CoinChanged, OnCoinChanged);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.ZuanshiChanged, OnZuanshiChanged);
    }
    void OnCoinChanged(long coin)
    {
        InitStoreItems();
    }
    void OnZuanshiChanged(int zuanshi)
    {
        InitStoreItems();
    }
    void OnCloseButtonClick()
    {
        RequestCloseUi();
    }
    void OnPayButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
        UIMall.OpenWith(true);
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
