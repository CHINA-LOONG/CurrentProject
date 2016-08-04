using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PromptComposeMST : UIBase
{

    public static string ViewName = "PromptComposeMST";

    public Text textTitle;
    public Text textMsg;
    public Button btnConfirm;
    public Button btnCancel;
    //碎片
    public Transform iconCurPos;
    public Text textFragment;
    private ItemIcon iconFragment;
    //万能碎片
    public GameObject commonFragment;
    public Transform iconComPos;
    private ItemIcon iconCommon;
    public Text textCommonFragment;

    private System.Action ConfirmEvent;
    private System.Action CancelEvent;

    public static PromptComposeMST Open(string title, 
                                        string msg,
                                        string currentItem,
                                        int currentCount,
                                        int commonCount, 
                                        System.Action clickConfirm, 
                                        System.Action clickCancel)
    {
        PromptComposeMST prompt = UIMgr.Instance.OpenUI_(PromptComposeMST.ViewName, false) as PromptComposeMST;
        prompt.SetData(title, msg, currentItem, currentCount, commonCount, clickConfirm, clickCancel);
        return null;
    }

    public void SetData(string title,
                        string msg,
                        string currentItem,
                        int currentCount,
                        int commonCount,
                        System.Action clickConfirm,
                        System.Action clickCancel)
    {
        ConfirmEvent = clickConfirm;
        CancelEvent = clickCancel;

        textTitle.text = title;
        textTitle.alignment = (textTitle.preferredWidth > ((textTitle.transform as RectTransform).rect.width) ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter);

        textMsg.text = msg;
        textMsg.alignment = (textMsg.preferredWidth > ((textMsg.transform as RectTransform).rect.width) ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter);

        ItemData itemInfo = new ItemData() { itemId = currentItem, count = 0 };
        if (null == iconFragment)
        {
            iconFragment = ItemIcon.CreateItemIcon(itemInfo, false);
            UIUtil.SetParentReset(iconFragment.transform, iconCurPos);
        }
        else
        {
            iconFragment.RefreshWithItemInfo(itemInfo, false);
        }
        textFragment.text = string.Format("*{0}", currentCount);
        
        commonFragment.SetActive(commonCount > 0);
        if (commonCount > 0)
        {
            if (iconCommon == null)
            {
                iconCommon = ItemIcon.CreateItemIcon(new ItemData() { itemId = BattleConst.commonFragmentID, count = 0 });
                UIUtil.SetParentReset(iconCommon.transform, iconComPos);
                iconCommon.HideExceptIcon();
            }
            textCommonFragment.text = string.Format("*{0}", commonCount);
        }
    }

    void Start()
    {
        btnConfirm.onClick.AddListener(ClickConfirmBtn);
        btnCancel.onClick.AddListener(ClickCancelBtn);
    }

    void ClickConfirmBtn()
    {
        if (null!=ConfirmEvent)
        {
            ConfirmEvent();
        }
        UIMgr.Instance.DestroyUI(this);
    }
    void ClickCancelBtn()
    {
        if (null != CancelEvent)
        {
            CancelEvent();
        }
        UIMgr.Instance.DestroyUI(this);
    }
}
