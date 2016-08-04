using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UseHuoLi : UIBase
{
    public static string ViewName = "UseHuoLi";
    public ItemIcon[] szItemIcon;
    public Text[] szDescText;
    public Button[] szUseButton;
    public Button closeButton;
    public Text title;

    private ItemData[] szItemData = new ItemData[3];
    private string curUseItemId = null;
    private string buyAndUseItem = null;

    public static void Open()
    {
        UseHuoLi ui = (UseHuoLi)UIMgr.Instance.OpenUI_(ViewName,true);
    }

    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_USE_C.GetHashCode().ToString(), OnUseItemFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_USE_S.GetHashCode().ToString(), OnUseItemFinished);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_BUY_AND_USE_C.GetHashCode().ToString(), OnItemBuyAndUseFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_BUY_AND_USE_S.GetHashCode().ToString(), OnItemBuyAndUseFinished);

        GameEventMgr.Instance.AddListener(GameEventList.RefreshUseHuoliWithZeroClock, RefreshUi);
    }

    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ITEM_USE_C.GetHashCode().ToString(), OnUseItemFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ITEM_USE_S.GetHashCode().ToString(), OnUseItemFinished);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ITEM_BUY_AND_USE_C.GetHashCode().ToString(), OnItemBuyAndUseFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ITEM_BUY_AND_USE_S.GetHashCode().ToString(), OnItemBuyAndUseFinished);

        GameEventMgr.Instance.RemoveListener(GameEventList.RefreshUseHuoliWithZeroClock, RefreshUi);
    }
    // Use this for initialization
    void Start()
    {
        closeButton.onClick.AddListener(OnClosebuttonClick);
        szUseButton[0].onClick.AddListener(OnUseButton1Click);
        szUseButton[1].onClick.AddListener(OnUseButton2Click);
        szUseButton[2].onClick.AddListener(OnUseButton3Click);
        title.text = StaticDataMgr.Instance.GetTextByID("energy_supple");
    }

    public override void Init()
    {
        RefreshUi();
    }
    //删除界面，对子对象的清理操作
    public override void Clean()
    {
    }

    public void RefreshUi()
    {
        RefreshSubItem(0, HuoLiDataMgr.Instance.huoliyao_1);
        RefreshSubItem(1, HuoLiDataMgr.Instance.huoliyao_2);
        RefreshSubItem(2, HuoLiDataMgr.Instance.huoliyao_3);
    }

    private void RefreshSubItem(int index, string itemId)
    {
        ItemData itemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(itemId);

        if (itemData == null)
        {
            itemData = ItemData.valueof(itemId, 0);
        }
        szItemData[index] = itemData;
        szItemIcon[index].RefreshWithItemInfo(itemData);

        int leftTime = HuoLiDataMgr.Instance.GetHuoliYaoLeftTime(itemId);

        if (itemData.count > 0)
        {
            UIUtil.SetButtonTitle(szUseButton[index].transform, StaticDataMgr.Instance.GetTextByID("exp_use"));
        }
        else
        {
            UIUtil.SetButtonTitle(szUseButton[index].transform, StaticDataMgr.Instance.GetTextByID("energy_button_buy&use"));
        }

        if (leftTime == 0)
        {
            szDescText[index].text = StaticDataMgr.Instance.GetTextByID("energy_times_enough");
            return;
        }

        szDescText[index].text = string.Format(StaticDataMgr.Instance.GetTextByID("energy_times"), leftTime);
        
    }


    void OnClosebuttonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void OnUseButton1Click()
    {
        useButtonClick(0);
    }

    void OnUseButton2Click()
    {
        useButtonClick(1);
    }

    void OnUseButton3Click()
    {
        useButtonClick(2);
    }

    void useButtonClick(int index)
    {
        int leftTime = HuoLiDataMgr.Instance.GetHuoliYaoLeftTime(HuoLiDataMgr.Instance.GetHuoliyaoId(index));

        if (leftTime == 0)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("energy_times_enough"), (int)PB.ImType.PROMPT);
            return;
        }

        ItemData subItem = szItemData[index];
        if (subItem.count > 0)
        {
            //use item
            curUseItemId = subItem.itemId;
            UseItem(curUseItemId);
        }
        else
        {
            //buy and use item
            curUseItemId = subItem.itemId;
            BuyAndUseItem(curUseItemId);
        }
    }

    void UseItem(string itemId)
    {
        PB.HSItemUse param = new PB.HSItemUse();
        param.itemId = itemId;
        param.itemCount = 1;
        GameApp.Instance.netManager.SendMessage(PB.code.ITEM_USE_C.GetHashCode(), param);
    }

    void OnUseItemFinished(ProtocolMessage msg)
    {
        UINetRequest.Close();

       // ItemStaticData stData = StaticDataMgr.Instance.GetItemData(curUseItemId);

        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
            if (error.errCode == (int)PB.itemError.ITEM_EXP_LEFT_TIMES)
            {
                string errMsg = "bag_record_002";
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(errMsg),
                                               (int)PB.ImType.PROMPT);
            }
            else if (error.errCode == (int)PB.PlayerError.GOLD_NOT_ENOUGH)
            {
                GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
            }
            else if (error.errCode == (int)PB.PlayerError.FATIGUE_LIMIT)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("im_recordhuoli_002"),(int)PB.ImType.PROMPT);
            }
            Logger.LogError("use items Error.....");
            return;
        }
        string succMsg = "bag_record_001";
        UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(succMsg), (int)PB.ImType.PROMPT);
        RefreshUi();
        GameEventMgr.Instance.FireEvent(GameEventList.RefreshInstanceList);
    }

    void BuyAndUseItem(string itemId)
    {
        buyAndUseItem = itemId;
        if (HuoLiDataMgr.Instance.showHuoliBuyDlg)
        {
            ItemStaticData stData = StaticDataMgr.Instance.GetItemData(itemId);
            string msg = string.Format(StaticDataMgr.Instance.GetTextByID("energy_hint_buy"), stData.NameAttr);
            BuyHuoliDlg.OpenWith(msg, stData.buyPrice, OnBuyAndUseItem);
        }
        else
        {
            //直接买
            OnBuyAndUseItem(MsgBox.PrompButtonClick.OK);
        }
    }

    void OnBuyAndUseItem(MsgBox.PrompButtonClick click)
    {
        if (click == MsgBox.PrompButtonClick.OK)
        {
            PB.HSItemBuyAndUse param = new PB.HSItemBuyAndUse();
            param.itemId = buyAndUseItem;
            param.itemCount = 1;

            GameApp.Instance.netManager.SendMessage(PB.code.ITEM_BUY_AND_USE_C.GetHashCode(), param);
        }
    }

    void OnItemBuyAndUseFinished(ProtocolMessage msg)
    {
        UINetRequest.Close();

        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
            if (error.errCode == (int)PB.itemError.ITEM_EXP_LEFT_TIMES)
            {
                string errMsg = "bag_record_002";
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(errMsg),
                                               (int)PB.ImType.PROMPT);
            }
            else if (error.errCode == (int)PB.PlayerError.GOLD_NOT_ENOUGH)
            {
                GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
            }
            else if (error.errCode == (int)PB.PlayerError.FATIGUE_LIMIT)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("im_recordhuoli_002"), (int)PB.ImType.PROMPT);
            }
            Logger.LogError("use items Error.....");
            return;
        }

        PB.HSItemBuyAndUseRet useReturn = msg.GetProtocolBody<PB.HSItemBuyAndUseRet>();
        GameDataMgr.Instance.PlayerDataAttr.gameItemData.UpdateItemState(useReturn.itemId, useReturn.useCountDaily);
        RefreshUi();
        GameEventMgr.Instance.FireEvent(GameEventList.RefreshInstanceList);
    }
}

   
