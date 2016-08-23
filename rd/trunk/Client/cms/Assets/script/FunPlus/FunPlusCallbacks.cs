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
            MonoBehaviour.print("{Funplus SDK install success --> Message: " + message);
            
        }

        void FunplusSdk.IDelegate.OnSdkInstallError(FunplusError error)
        {
            MonoBehaviour.print("Funplus SDK install error --> Error: " + error.GetErrorMsg());
        }


        void FunplusAccount.IDelegate.OnLoginSuccess(FunplusSession session)
        {
            MonoBehaviour.print("OnLoginSuccess --> Success:" + session.GetFpid()+ session.GetSessionKey());
            GameEventMgr.Instance.FireEvent<String>(GameEventList.funplusPuid, session.GetFpid());
        }

        void FunplusAccount.IDelegate.OnLoginError(FunplusError error)
        {
            MonoBehaviour.print("OnLoginError --> Error: " + error.GetErrorMsg());
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
			MonoBehaviour.print("OnLogout --> : " );

        }

        void FunplusAccount.IDelegate.OnBindAccountSuccess(FunplusSession session)
        { 
			MonoBehaviour.print("OnBindAccountSuccess --> : " + session.GetFpid() + session.GetSessionKey());
        }

        void FunplusAccount.IDelegate.OnBindAccountError(FunplusError error)
        {
			MonoBehaviour.print("OnBindAccountError --> Error: " + error.GetErrorMsg());

        }

        void FunplusAccount.IDelegate.OnCloseUserCenter()
        {

        }
}
