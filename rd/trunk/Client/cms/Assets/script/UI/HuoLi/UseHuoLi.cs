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

    private ItemData[] szItemData = new ItemData[3];
    private string curUseItemId = null;

    public  static  void    Open()
    {
        UseHuoLi ui = (UseHuoLi)UIMgr.Instance.OpenUI_(ViewName);
    }

	// Use this for initialization
	void Start ()
    {
        closeButton.onClick.AddListener(OnClosebuttonClick);
        szUseButton[0].onClick.AddListener(OnUseButton1Click);
        szUseButton[1].onClick.AddListener(OnUseButton2Click);
        szUseButton[2].onClick.AddListener(OnUseButton3Click);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_USE_C.GetHashCode().ToString(), OnUseItemFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_USE_S.GetHashCode().ToString(), OnUseItemFinished);
    }

    public override void Init()
    {
        RefreshUi();
    }
    //删除界面，对子对象的清理操作
    public override void Clean()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ITEM_USE_C.GetHashCode().ToString(), OnUseItemFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ITEM_USE_S.GetHashCode().ToString(), OnUseItemFinished);
    }

    public  void RefreshUi()
    {
        RefreshSubItem(0, HuoLiDataMgr.Instance.huoliyao_1);
        RefreshSubItem(1, HuoLiDataMgr.Instance.huoliyao_2);
        RefreshSubItem(2, HuoLiDataMgr.Instance.huoliyao_3);
    }

    private void    RefreshSubItem(int index,string itemId)
    {
        ItemData itemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(itemId);

        if(itemData == null)
        {
            itemData = ItemData.valueof(itemId, 0);
        }
        szItemData[index] = itemData;
        szItemIcon[index].RefreshWithItemInfo(itemData);

        int leftTime = HuoLiDataMgr.Instance.GetHuoliYaoLeftTime(itemId);

        if(leftTime == 0)
        {
            szDescText[index].text = StaticDataMgr.Instance.GetTextByID("energy_times_enough");
            UIUtil.SetButtonTitle(szUseButton[index].transform, StaticDataMgr.Instance.GetTextByID("exp_use"));
            return;
        }

        szDescText[index].text = string.Format(StaticDataMgr.Instance.GetTextByID("energy_times"), leftTime);
        if(itemData.count > 0)
        {
            UIUtil.SetButtonTitle(szUseButton[index].transform, StaticDataMgr.Instance.GetTextByID("exp_use"));
        }
        else
        {
            UIUtil.SetButtonTitle(szUseButton[index].transform, StaticDataMgr.Instance.GetTextByID("energy_button_buy&use"));
        } 
    }
    

    void    OnClosebuttonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void     OnUseButton1Click()
    {
        useButtonClick(0);
    }

    void    OnUseButton2Click()
    {
        useButtonClick(1);
    }

    void    OnUseButton3Click()
    {
        useButtonClick(2);
    }

    void    useButtonClick(int index)
    {
        int leftTime = HuoLiDataMgr.Instance.GetHuoliYaoLeftTime(HuoLiDataMgr.Instance.GetHuoliyaoId(index));

        if(leftTime == 0)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("energy_times_enough"), (int)PB.ImType.PROMPT);
            return;
        }

        ItemData subItem = szItemData[index];
        if(subItem.count > 0)
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

        ItemStaticData stData = StaticDataMgr.Instance.GetItemData(curUseItemId);

        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
            if (error.errCode == (int)PB.itemError.ITEM_EXP_LEFT_TIMES)
            {
                string errMsg = "bag_record_002";
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(errMsg),
                                               (int)PB.ImType.PROMPT);
            }
            Logger.LogError("use items Error.....");
            return;
        }
        string succMsg = "bag_record_001";
        UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(succMsg), (int)PB.ImType.PROMPT);
        RefreshUi();
    }

    void    BuyAndUseItem(string itemId)
    {
        if(HuoLiDataMgr.Instance.showHuoliBuyDlg)
        {
            BuyHuoliDlg.OpenWith(StaticDataMgr.Instance.GetTextByID("要买吗?"),
                StaticDataMgr.Instance.GetTextByID("购买物品"),
                null);
        }
        else
        {
            //直接买
            OnBuyAndUseItem(MsgBox.PrompButtonClick.OK);
        }
    }

    void OnBuyAndUseItem(MsgBox.PrompButtonClick click)
    {
        if(click == MsgBox.PrompButtonClick.OK)
        {

        }
    }
}
