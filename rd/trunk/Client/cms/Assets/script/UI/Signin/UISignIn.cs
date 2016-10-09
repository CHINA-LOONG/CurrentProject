//================================
//  Create by xuelong.
//  Mail:[xuelong@way4games.com]
//================================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UISignIn : UIBase,
                        IScrollView
{
    public static string ViewName = "UISignIn";

    public static UISignIn Open()
    {
        UISignIn uiSignIn = UIMgr.Instance.OpenUI_(UISignIn.ViewName, false) as UISignIn;
        uiSignIn.ReloadData();
        if (!SigninDataMgr.Instance.isPopup)
        {
            SigninDataMgr.Instance.isPopup = true;
            GameEventMgr.Instance.FireEvent(GameEventList.SignInPopupChange);
        }
        return uiSignIn;
    }

    public Text text_Title;

    public Button btnClose;
    public Button btnRetroactive;

    public Text textRetroactive;
    public Text textPrice;

    public Text textMsg1;
    public Text textMsg2;

    public FixCountScrollView scrollView;
    public ScrollRect scrollRect
    {
        get { return scrollView.GetComponent<ScrollRect>(); }
    }

    List<SignInItemInfo> dataList = new List<SignInItemInfo>();

    private UISignInfo uiSignInfo;
    private UIGainPet mGainPetUI;
    // Use this for initialization

    private int priceRetroactive;
    public Animator Animator
    {
        get
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            return animator;
        }
    }
    private Animator animator;
    void Start()
    {
        text_Title.text = StaticDataMgr.Instance.GetTextByID("monthlyevent_title");
        btnClose.onClick.AddListener(OnClickCloseBtn);
        btnRetroactive.onClick.AddListener(OnClickRetroactiveBtn);
    }

    void ReloadData()
    {
        List<string> rewardList = SigninDataMgr.Instance.SigninList;
        
        dataList.Clear();
        for (int i = 0; i < rewardList.Count; i++)
        {
            RewardData reward = StaticDataMgr.Instance.GetRewardData(rewardList[i]);

            if (reward == null ||
                reward.itemList.Count != 1 ||
                (reward.itemList[0].protocolData.type != (int)PB.itemType.ITEM && reward.itemList[0].protocolData.type != (int)PB.itemType.PLAYER_ATTR && reward.itemList[0].protocolData.type != (int)PB.itemType.MONSTER))
            {
                Logger.Log("签到奖励配置错误奖励没有配置：i=" + i);
                continue;
            }
            SignInItemInfo data = new SignInItemInfo();
            dataList.Add(data);
            data.protocolData = reward.itemList[0].protocolData;
            if (i < SigninDataMgr.Instance.signinTimesMonthly)
            {
                data.Type = SignInType.YiQianDao;
            }
            else if (!SigninDataMgr.Instance.isSigninDaily && i == SigninDataMgr.Instance.signinTimesMonthly)
            {
                data.Type = SignInType.KeQianDao;
            }
            else if (i < SigninDataMgr.Instance.curDay)
            {
                data.Type = SignInType.KeBuQian;
            }
            else
            {
                data.Type = SignInType.WeiQianDao;
            }
        }
        scrollView.InitContentSize(dataList.Count, this);
        if (SigninDataMgr.Instance.signinTimesMonthly < 6)
        {
            scrollRect.verticalNormalizedPosition = 1.0f;
        }
        else
        {
            scrollRect.verticalNormalizedPosition = 0.0f;
        }
        textMsg1.text = string.Format(StaticDataMgr.Instance.GetTextByID("monthlyevent_count_001"), SigninDataMgr.Instance.signinTimesMonthly, rewardList.Count);
        if (SigninDataMgr.Instance.canSigninFillTimes > 0)
        {
            textMsg2.gameObject.SetActive(true);
            textMsg2.text = string.Format(StaticDataMgr.Instance.GetTextByID("monthlyevent_count_002"), SigninDataMgr.Instance.canSigninFillTimes);
            btnRetroactive.gameObject.SetActive(true);
            UpdateRetroactivePrice();
        }
        else
        {
            textMsg2.gameObject.SetActive(false);
            btnRetroactive.gameObject.SetActive(false);
        }
    }

    //更新签到次数
    //void UpdateSigninTimes()
    //{
    //    textMsg1.text = string.Format(StaticDataMgr.Instance.GetTextByID("monthlyevent_count_001"), SigninDataMgr.Instance.signinTimesMonthly, rewardList.Count);
    //    textMsg2.text = string.Format(StaticDataMgr.Instance.GetTextByID("monthlyevent_count_002"), SigninDataMgr.Instance.canSigninFillTimes);
    //}
    //更新补签价格
    void UpdateRetroactivePrice()
    {
        SigninFillPriceData priceCfg = StaticDataMgr.Instance.SigninFillPriceData;
        int mutiple = (int)Mathf.Pow(2, SigninDataMgr.Instance.signinFillTimesMonthly / priceCfg.doubleTimes);
        int price = mutiple * (priceCfg.consume + SigninDataMgr.Instance.signinFillTimesMonthly * priceCfg.consumeAdd);
        priceRetroactive = Math.Min(price, priceCfg.ceiling);
        textPrice.text = priceRetroactive.ToString();
    }
    //签到
    void OnClickSigninAction()
    {
        if (SigninDataMgr.Instance.isSigninDaily)
        {
            return;
        }
        PB.HSSignin param = new PB.HSSignin();
        param.month = SigninDataMgr.Instance.curMonth;
        GameApp.Instance.netManager.SendMessage(PB.code.SIGNIN_C.GetHashCode(), param);
    }
    //补签
    void OnClickRetroactiveBtn()
    {
        if (!SigninDataMgr.Instance.isSigninDaily)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("Sign_buqian_001"), (int)PB.ImType.PROMPT);
            return;
        }
        else if (GameDataMgr.Instance.PlayerDataAttr.gold<priceRetroactive)
        {
            //钻石不足
            GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
            return;
        }
        PB.HSSigninFill param = new PB.HSSigninFill();
        param.month = SigninDataMgr.Instance.curMonth;
        GameApp.Instance.netManager.SendMessage(PB.code.SIGNIN_FILL_C.GetHashCode(), param);
    }

    void OnClickCloseBtn()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    #region UIBase

    public override void Init()
    {
        if (Animator != null)
        {
            Animator.SetTrigger("enter");
        }
    }
    public override void Clean()
    {
        UIMgr.Instance.DestroyUI(uiSignInfo);
        UIMgr.Instance.DestroyUI(mGainPetUI);
    }
    #endregion

    #region IScrollView
    public void IScrollViewReloadItem(FixCountScrollView scrollView, Transform item, int index)
    {
        SignInItem signInItem = item.GetComponent<SignInItem>();
        signInItem.ReloadData(dataList[index]);
    }

    public Transform IScrollViewCreateItem(FixCountScrollView scrollView, Transform parent)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("SignInItem");
        if (go != null)
        {
            UIUtil.SetParentReset(go.transform, parent);
            SignInItem item = go.GetComponent<SignInItem>();
            item.InitItem(OnClickSigninAction);
            return go.transform;
        }
        return null;
    }

    public void IScrollViewCleanItem(FixCountScrollView scrollView, List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
    }

    #endregion
    
    #region BindListener
    void OnEnable()
    {
        BindListener();
    }
    void OnDisable()
    {
        UnBindListener();
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SIGNIN_C.GetHashCode().ToString(), OnSignInReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SIGNIN_FILL_C.GetHashCode().ToString(), OnSignInReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SIGNIN_S.GetHashCode().ToString(), OnSignInReturn);

        GameEventMgr.Instance.AddListener(GameEventList.SignInChange, ReloadData);
        GameEventMgr.Instance.AddListener(GameEventList.SignInDataChange, OnClickCloseBtn);
        //GameEventMgr.Instance.AddListener(GameEventList.si)
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SIGNIN_C.GetHashCode().ToString(), OnSignInReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SIGNIN_FILL_C.GetHashCode().ToString(), OnSignInReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SIGNIN_S.GetHashCode().ToString(), OnSignInReturn);

        GameEventMgr.Instance.RemoveListener(GameEventList.SignInChange, ReloadData);
        GameEventMgr.Instance.RemoveListener(GameEventList.SignInDataChange, OnClickCloseBtn);
    }

    void OnSignInReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
            if (error.errCode == (int)PB.PlayerError.GOLD_NOT_ENOUGH)
            {
                GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
            }
            Logger.LogError("签到失败");
            return;
        }
        PB.HSSigninRet result = msg.GetProtocolBody<PB.HSSigninRet>();
        if ((result.signinTimes - 1) == SigninDataMgr.Instance.signinTimesMonthly)
        {
            if (!SigninDataMgr.Instance.isSigninDaily)//签到成功
            {
                SigninDataMgr.Instance.isSigninDaily = true;
            }
            else//补签成功
            {
                SigninDataMgr.Instance.signinFillTimesMonthly += 1;
            }
            SigninDataMgr.Instance.signinTimesMonthly = result.signinTimes;
            GameEventMgr.Instance.FireEvent(GameEventList.SignInChange);

            //TODO:弹出窗口
            if (result.reward.type == (int)PB.itemType.ITEM || result.reward.type == (int)PB.itemType.PLAYER_ATTR)
            {
                uiSignInfo = UISignInfo.Open(result.reward);
            }
            else if (result.reward.type == (int)PB.itemType.MONSTER)
            {
                mGainPetUI = UIMgr.Instance.OpenUI_(UIGainPet.ViewName) as UIGainPet;
                mGainPetUI.ShowGainPet(result.reward.itemId);
                mGainPetUI.SetConfirmCallback(OnClickGainPetUIBtn);
            }
            else
            {
                Logger.LogError("奖励类型与客户端不一致");
            }
        }
        else
        {
            Logger.LogError("签到次数异常");
        }
    }
    #endregion

    void OnClickGainPetUIBtn(GameObject go)
    {
        UIMgr.Instance.DestroyUI(mGainPetUI);
    }
}
