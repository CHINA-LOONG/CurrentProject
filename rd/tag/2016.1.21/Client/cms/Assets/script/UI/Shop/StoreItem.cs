using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoreItem : MonoBehaviour
{
    public Text itemName;
    public ItemIcon itemIcon;
    public Text priceText;

    public Button buyButton;
    public Transform cheapPanel;
    public Text offText;
    public Text oldPrice;

    private StoreStaticData storeItemData;

    public static   StoreItem CreateWith(StoreStaticData sellItemData)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("StoreItem");
        var storeItem = go.GetComponent<StoreItem>();
        storeItem.InitWith(sellItemData);
        return storeItem;
    }

    void Start()
    {
        buyButton.onClick.AddListener(OnBuyButtonClick);
    }

    public  void InitWith(StoreStaticData sellItemData)
    {
        storeItemData = sellItemData;
        ItemStaticData itemStaticData = StaticDataMgr.Instance.GetItemData(storeItemData.itemId);
        if (null == itemStaticData)
        {
            Logger.LogError("StoreItem error: can't find item id = " + storeItemData.itemId);
            return;
        }
        RefreshItemIcon(itemStaticData);
        itemName.text = itemStaticData.NameAttr;
        priceText.text = string.Format("{0}", Mathf.Ceil(storeItemData.price  * storeItemData.discount));

        if (IsCoinEnough())
        {
            priceText.color = ColorConst.text_color_Enough;
        }
        else
        {
            priceText.color = ColorConst.text_color_nReq;
        }

        if (storeItemData.discount > 0.9999f)
        {
            cheapPanel.gameObject.SetActive(false);
        }
        else
        {
            cheapPanel.gameObject.SetActive(true);
            oldPrice.text = (storeItemData.price).ToString();
            offText.text = offText.text = string.Format("{0}%", Mathf.CeilToInt(storeItemData.discount * 100));
        }
    }
    private void RefreshItemIcon(ItemStaticData itemStaticData)
    {
        if (itemStaticData.type == (int)PB.toolType.EQUIPTOOL)
        {
            Logger.LogError("Config Error for StoreItem, no suppot equip in store with id = " + storeItemData.itemId);
           // EquipData equipData = EquipData.valueof(0, itemStaticData.id, itemStaticData.stage, itemStaticData.level, BattleConst.invalidMonsterID, null);
            //itemIcon.RefreshWithEquipInfo(equipData);
        }
        else
        {
            ItemData itemData = ItemData.valueof(storeItemData.itemId, 0);

            itemIcon.RefreshWithItemInfo(itemData);
        }
    }
    bool IsCoinEnough( )
    {
        return GameDataMgr.Instance.PlayerDataAttr.gold >= Mathf.CeilToInt(storeItemData.price * storeItemData.discount);
    }

    void OnBuyButtonClick()
    {
        BuyItem.BuyItemParam param = new BuyItem.BuyItemParam();
        param.itemId = storeItemData.itemId;
        param.defaultbuyCount = 1;
        param.maxCount = 50;
        param.isShowCoinButton = false;
        BuyItem.OpenWith(param);
    }

}
