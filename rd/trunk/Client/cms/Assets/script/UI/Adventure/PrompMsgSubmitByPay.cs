using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MsgBox;

public class PrompMsgSubmitByPay : UIBase
{
    public static string ViewName = "PrompMsgSubmitByPay";

    public Text textTitle;
    public Text textDesc;
    public Text textCost;

    public Button btnCancel;
    public Button btnConfirm;

    private string content;
    private TimeEventWrap timeWrap;
    private PromptMsg.PrompDelegate callBack;
    private bool autoClose;
    //private bool autoFinish;

    private int price = 0;

    public static PrompMsgSubmitByPay Open(string title, string content, TimeEventWrap timeWrap, PromptMsg.PrompDelegate callback = null, bool autoClose = true)
    {
        PrompMsgSubmitByPay mInfo = UIMgr.Instance.OpenUI_(ViewName, false) as PrompMsgSubmitByPay;
        mInfo.SetData(title, content, timeWrap, callback, autoClose);
        return mInfo;
    }

    void Start()
    {
        btnCancel.onClick.AddListener(OnClickCancelBtn);
        btnConfirm.onClick.AddListener(OnClickConfirmBtn);

        UIUtil.SetButtonTitle(btnCancel.transform, StaticDataMgr.Instance.GetTextByID("ui_quxiao"));
        UIUtil.SetButtonTitle(btnConfirm.transform, StaticDataMgr.Instance.GetTextByID("ui_queding"));
    }
    public void SetData(string title, string content, TimeEventWrap timeWrap, PromptMsg.PrompDelegate callback, bool autoClose)
    {
        this.callBack = callback;
        this.autoClose = autoClose;
        this.content = content;
        this.timeWrap = timeWrap;

        textTitle.text = title;
        timeWrap.AddUpdateEvent(OnUpdateTime);
    }

    void OnUpdateTime(int time)
    {
        if (time <= 0)
        {
            Close();
        }
        textDesc.text = string.Format(content, UIUtil.Convert_hh_mm_ss(time));
        price = (int)Mathf.Ceil((time / 60.0f) / 10.0f) * 2;
        textCost.text = price.ToString();
    }

    public void Close()
    {
        timeWrap.RemoveUpdateEvent(OnUpdateTime);
        UIMgr.Instance.DestroyUI(this);
    }
    
    void OnClickCancelBtn()
    {
        if (autoClose)
        {
            Close();
        }
        if (callBack != null)
        {
            callBack(PrompButtonClick.Cancle);
        }
    }
    void OnClickConfirmBtn()
    {
        if (autoClose)
        {
            Close();
        }
        if (callBack != null)
        {
            if (GameDataMgr.Instance.PlayerDataAttr.gold < price)
            {
                GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
            }
            else
            {
                callBack(PrompButtonClick.OK);
            }
        }
    }

}
