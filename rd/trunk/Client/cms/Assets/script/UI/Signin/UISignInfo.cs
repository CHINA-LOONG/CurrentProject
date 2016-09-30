//================================
//  Create by xuelong.
//  Mail:[xuelong@way4games.com]
//================================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UISignInfo : UIBase
{
    public static string ViewName = "UISignInfo";

    public static UISignInfo Open(PB.RewardItem reward)
    {
        UISignInfo mInfo = UIMgr.Instance.OpenUI_(UISignInfo.ViewName) as UISignInfo;
        mInfo.ShowWithData(reward);
        return mInfo;
    }

    public Text textTitle;
    public Transform rewardParent;
    private GameObject rewardItem;
    public Button btnConfirm;

    private Animator animator;
    public Animator Animator
    {
        get
        {
            if (animator==null)
            {
                animator = GetComponent<Animator>();
            }
            return animator;
        }
    }
    

    //public System.Action StartEvent;
    //public System.Action EndEvent;

    // Use this for initialization
    void Start()
    {
        textTitle.text = StaticDataMgr.Instance.GetTextByID("monthlyevent_reward_001");
        UIUtil.SetButtonTitle(btnConfirm.transform, StaticDataMgr.Instance.GetTextByID("ui_queding"));
        btnConfirm.onClick.AddListener(OnClickConfirmBtn);
    }

    void ShowWithData(PB.RewardItem rewardData)
    {
        //this.StartEvent = StartEvent;
        //if (this.StartEvent!=null)
        //{
        //    this.StartEvent();
        //}
        //this.EndEvent = EndEvent;
        //TODO:

        PB.RewardItem info= rewardData;
        if (rewardItem != null)
        {
            ResourceMgr.Instance.DestroyAsset(rewardItem);
        }
        if (info.type == (int)PB.itemType.ITEM|| info.type == (int)PB.itemType.PLAYER_ATTR)
        {
            ItemIcon icon = ItemIcon.CreateItemIcon(RewardItemData.GetItemData(info), true, false);
            UIUtil.SetParentReset(icon.transform, rewardParent);
            rewardItem = icon.gameObject;
        }
    }

    void OnClickConfirmBtn()
    {
        Logger.Log("click confirmbtn down");
        UIMgr.Instance.CloseUI_(this);
        //if (this.EndEvent!=null)
        //{
        //    this.EndEvent();
        //}
    }
}
