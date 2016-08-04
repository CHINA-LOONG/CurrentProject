using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuyHuoliDlg : UIBase
{
    public Text huafeiDesc;
    public Text zuanshiText;

    public Button cancelButton;
    public Button conformButton;
    public Toggle nextToggle;
    public Text nextToggleText;

    private MsgBox.PromptMsg.PrompDelegate callBack;

    public static string ViewName = "BuyHuoliDlg";
    public static void    OpenWith(string desc1,int zuanshi, MsgBox.PromptMsg.PrompDelegate callBack)
    {
        BuyHuoliDlg huoliDlg = (BuyHuoliDlg)UIMgr.Instance.OpenUI_(ViewName);
        huoliDlg.InitWith(desc1, zuanshi, callBack);
    }
    
    void    Start()
    {
        cancelButton.onClick.AddListener(OnCancelButtonClick);
        conformButton.onClick.AddListener(OnConformButtonClick);
        nextToggleText.text = StaticDataMgr.Instance.GetTextByID("energy_hint_unhint");
        UIUtil.SetButtonTitle(cancelButton.transform, StaticDataMgr.Instance.GetTextByID("ui_quxiao"));
        UIUtil.SetButtonTitle(conformButton.transform, StaticDataMgr.Instance.GetTextByID("ui_queding"));
    }

    public  void InitWith(string desc1, int zuanshi, MsgBox.PromptMsg.PrompDelegate callBack)
    {
        this.callBack = callBack;
        huafeiDesc.text = desc1;
        zuanshiText.text = zuanshi.ToString();
        nextToggle.isOn = !HuoLiDataMgr.Instance.showHuoliBuyDlg;
        
    }
    void    OnCancelButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void    OnConformButtonClick()
    {
        if(callBack != null)
        {
            callBack(MsgBox.PrompButtonClick.OK);
        }
        HuoLiDataMgr.Instance.showHuoliBuyDlg = !nextToggle.isOn;
        UIMgr.Instance.CloseUI_(this);
    }

}
