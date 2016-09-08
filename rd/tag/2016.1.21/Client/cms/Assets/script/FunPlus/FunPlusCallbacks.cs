using HSMiniJSON;
using Funplus;
using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class FunPlusCallbacks : FunplusSdk.IDelegate, FunplusAccount.IDelegate {

        void FunplusSdk.IDelegate.OnSdkInstallSuccess(string message)
        {
            //Logger.Log("{Funplus SDK install success --> Message: " + message);
            Logger.Log("{Funplus SDK install success --> Message: " + message);
            
        }

        void FunplusSdk.IDelegate.OnSdkInstallError(FunplusError error)
        {
            Logger.Log("Funplus SDK install error --> Error: " + error.GetErrorMsg());
        }


        void FunplusAccount.IDelegate.OnLoginSuccess(FunplusSession session)
        {
            Logger.Log("OnLoginSuccess --> Success:" + session.GetFpid()+ session.GetSessionKey());
            GameEventMgr.Instance.FireEvent<String>(GameEventList.funplusPuid, session.GetFpid());
        }

        void FunplusAccount.IDelegate.OnLoginError(FunplusError error)
        {
            Logger.Log("OnLoginError --> Error: " + error.GetErrorMsg());
        }

        void FunplusAccount.IDelegate.OnOpenSession(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                // Player has logged in automatically, do nothing.
            }
            else
            {
                FunplusAccount.GetInstance().Login();
                //FunplusAccount.GetInstance().ShowUserCenter();
            }
        }

        void FunplusAccount.IDelegate.OnLogout()
        { 
			Logger.Log("OnLogout --> : " );
            GameEventMgr.Instance.FireEvent(GameEventList.LogoutClick);
        }

        void FunplusAccount.IDelegate.OnBindAccountSuccess(FunplusSession session)
        { 
			Logger.Log("OnBindAccountSuccess --> : " + session.GetFpid() + session.GetSessionKey());
        }

        void FunplusAccount.IDelegate.OnBindAccountError(FunplusError error)
        {
			Logger.Log("OnBindAccountError --> Error: " + error.GetErrorMsg());

        }

        void FunplusAccount.IDelegate.OnCloseUserCenter()
        {

        }
}
