using UnityEngine;
using Funplus;
using System;
using System.Collections;
using System.Collections.Generic;

public class FunPlus : MonoBehaviour, FunplusSdk.IDelegate,
            FunplusAccount.IDelegate,
            FunplusPayment.IDelegate,
            FunplusFacebook.IDelegate
{
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        FunplusSdk.Instance.SetDelegate(this).Install();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FunplusSdk.IDelegate.OnSdkInstallSuccess(string message)
    {
        Debug.LogFormat("[funsdk] SDK successfully installed, message: {0}.", message);
        FunplusAccount.Instance.SetDelegate(this);
    }

    void FunplusSdk.IDelegate.OnSdkInstallError(FunplusError error)
    {
        Debug.LogErrorFormat("[funsdk] SDK failed to installed, error: {0}.", error.ErrorLocalizedMsg);
    }

    #region FunplusAccount.IDelegate
    void FunplusAccount.IDelegate.OnOpenSession(bool isLoggedIn)
    {
        if (!isLoggedIn)
        {
            FunplusAccount.Instance.Login();
        }
    }

    void FunplusAccount.IDelegate.OnLoginSuccess(FunplusSession session)
    {
        Debug.LogFormat("OnLoginSuccess");
        FunplusPayment.Instance.SetDelegate(this).StartHelper();
        FunplusFacebook.Instance.SetDelegate(this);
        GameEventMgr.Instance.FireEvent<String>(GameEventList.funplusPuid, session.GetFpid());
    }

    void FunplusAccount.IDelegate.OnLoginError(FunplusError error)
    {
        Debug.LogFormat("OnLoginError --> : ");
    }

    void FunplusAccount.IDelegate.OnLogout()
    {
        Debug.LogFormat("OnLogout --> : ");
        GameEventMgr.Instance.FireEvent(GameEventList.LogoutClick);
    }

    void FunplusAccount.IDelegate.OnBindAccountSuccess(FunplusSession session)
    {
    }

    void FunplusAccount.IDelegate.OnBindAccountError(FunplusError error)
    {
    }

    void FunplusAccount.IDelegate.OnCloseUserCenter()
    {
    }

    void FunplusAccount.IDelegate.OnResetPasswordSuccess(string fpid)
    {
    }
    void FunplusAccount.IDelegate.OnResetPasswordError(FunplusError error)
    {
    }

    #endregion // FunplusPayment.IDelegate

    #region FunplusPayment.IDelegate
    void FunplusPayment.IDelegate.OnInitializeSuccess(List<FunplusProduct> products)
    {

        Debug.LogFormat("OnInitializeSuccess --> : " + products.Count);
        for (int i = 0; i < products.Count; i++)
        {
            FunplusProduct product = products[i];
            Debug.Log(product.GetProductId() + " | " + product.GetTitle() + " | " + product.GetFormattedPrice());
        }
    }

    void FunplusPayment.IDelegate.OnInitializeError(FunplusError error)
    {
        Debug.LogFormat("OnInitializeError --> : " + error);
    }

    void FunplusPayment.IDelegate.OnPurchaseSuccess(string productId, string throughCargo)
    {
        UINetRequest.Close();
        Debug.LogFormat("OnPurchaseSuccess --> : " + productId + " | "+ throughCargo);
    }

    void FunplusPayment.IDelegate.OnPurchaseError(FunplusError error)
    {
        UINetRequest.Close();
        Debug.LogFormat("OnPurchaseError --> : " + error);
    }

    #endregion // FunplusPayment.IDelegate

    #region FunplusFacebook.IDelegate
    void FunplusFacebook.IDelegate.OnAskFriendsPermission(bool result)
    {
    }

    void FunplusFacebook.IDelegate.OnAskPublishPermission(bool result)
    {
    }

    void FunplusFacebook.IDelegate.OnGetUserDataSuccess(FunplusSocialUser user)
    {
    }

    void FunplusFacebook.IDelegate.OnGetUserDataError(FunplusError error)
    {
    }

    void FunplusFacebook.IDelegate.OnGetGameFriendsSuccess(List<FunplusFBFriend> friends)
    {
    }

    void FunplusFacebook.IDelegate.OnGetGameFriendsError(FunplusError error)
    {
    }

    void FunplusFacebook.IDelegate.OnGetGameInvitableFriendsSuccess(List<FunplusFBFriend> friends)
    {
    }

    void FunplusFacebook.IDelegate.OnGetGameInvitableFriendsError(FunplusError error)
    {
    }

    void FunplusFacebook.IDelegate.OnSendGameRequestSuccess(string result)
    {
    }

    void FunplusFacebook.IDelegate.OnSendGameRequestError(FunplusError error)
    {
    }

    void FunplusFacebook.IDelegate.OnShareSuccess(string result)
    {
    }

    void FunplusFacebook.IDelegate.OnShareError(FunplusError error)
    {
    }
    #endregion // FunplusFacebook.IDelegate
}
