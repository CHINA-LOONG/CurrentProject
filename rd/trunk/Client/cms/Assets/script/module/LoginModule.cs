using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using System;
using System.IO;

public class LoginModule : ModuleBase 
{
	void Start()
	{
		
	}
	void BindListener()
	{
        GameEventMgr.Instance.AddListener<int>(NetEventList.NetConnectFinished, OnNetConnectFinished);
		GameEventMgr.Instance.AddListener (GameEventList.LoginClick, OnLoginClick);
        GameEventMgr.Instance.AddListener<Hashtable>(GameEventList.ServerClick, OnServerClick);
		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.LOGIN_S.GetHashCode ().ToString(), OnNetLoginFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.LOGIN_C.GetHashCode().ToString(), OnNetLoginFinished);
	}
	
	void UnBindListener()
	{
        GameEventMgr.Instance.RemoveListener<int>(NetEventList.NetConnectFinished, OnNetConnectFinished);
		GameEventMgr.Instance.RemoveListener (GameEventList.LoginClick, OnLoginClick);
        GameEventMgr.Instance.RemoveListener<Hashtable>(GameEventList.ServerClick, OnServerClick);	
		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.LOGIN_S.GetHashCode ().ToString (), OnNetLoginFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.LOGIN_C.GetHashCode().ToString(), OnNetLoginFinished);
	}

	void OnLoginClick()
	{
        StartCoroutine(GetGameServer());
	}

    void OnServerClick(Hashtable serverInfo)
    {
        GameApp.Instance.netManager.GameServerAdd = serverInfo["hostIp"].ToString();
        GameApp.Instance.netManager.GameServerPort = int.Parse(serverInfo["port"].ToString());

        GameApp.Instance.netManager.SendConnect();
        UIMgr.Instance.DestroyUI(UIMgr.Instance.GetUI(UISelectServer.ViewName));
    }

    IEnumerator GetGameServer()
    {
        //GameApp.Instance.netManager.SendConnect();

        UINetRequest.Open();
        HTTPRequest centerRequest = new HTTPRequest(new Uri(Const.CollectorUrl), HTTPMethods.Post);
        centerRequest.AddField("game", Const.AppName);
        centerRequest.AddField("platform", Const.platform);
        centerRequest.AddField("channel", Const.channel);
        centerRequest.Send();
        yield return StartCoroutine(centerRequest);
        if (centerRequest.Response == null || !centerRequest.Response.IsSuccess)
        {
            Debug.Log("连接中心服务器失败");
            yield break;
        }

        //账号服务器返回值
        string accountServerAddress = centerRequest.Response.DataAsText;
        int port = 0;
        string ip = null;

        Hashtable ht = MiniJsonExtensions.hashtableFromJson(accountServerAddress);
        if (null == ht)
        {
           Debug.Log("账号服务器分配失败");
           yield break;
        }
        else
        {
           port = int.Parse(ht["httpPort"].ToString());
           ip = ht["hostIp"].ToString();
        }

        Debug.Log("连接中心服务器成功");

        string path = "http://" + ip + ":" + port + "" + "/fetch_gameServer";
        HTTPRequest accountRequest = new HTTPRequest(new Uri(path), HTTPMethods.Post);
        accountRequest.AddField("game", Const.AppName);
        accountRequest.AddField("platform", Const.platform);
        accountRequest.AddField("channel", Const.channel);
        if (string.IsNullOrEmpty(PlayerPrefs.GetString ("testGuid")) == false)
        {
            accountRequest.AddField("puid", PlayerPrefs.GetString("testGuid"));
        }

        accountRequest.Send();
        yield return StartCoroutine(accountRequest);
        if (accountRequest.Response == null || !accountRequest.Response.IsSuccess)
        {
            Debug.Log("连接账号服务器失败");
            yield break;
        }

        string gameServerAddress = accountRequest.Response.DataAsText;

        ArrayList serverList = MiniJsonExtensions.arrayListFromJson(gameServerAddress);
        if (null == serverList)
        {
            Debug.Log("游戏服务器获取失败");
            yield break;
        }

        Debug.Log("连接账号服务器成功");
        UISelectServer uiSelectServer = UIMgr.Instance.OpenUI_(UISelectServer.ViewName) as UISelectServer;
        if (uiSelectServer != null)
        {
            uiSelectServer.ResetServerList(serverList);
        }
        UINetRequest.Close();
    }

	void OnNetLogin(object param)
	{
		GameMain.Instance.ChangeModule<BuildModule>();
	}

    void OnNetConnectFinished(int state)
    {
        if (1 == state)
        {
            Debug.LogWarning("OK for net");

            if (string.IsNullOrEmpty(PlayerPrefs.GetString("testGuid")) == false)
            {
                PB.HSLogin hsLogin = new PB.HSLogin();
                hsLogin.puid = PlayerPrefs.GetString("testGuid");
                hsLogin.token = GameDataMgr.Instance.UserDataAttr.token;
                GameApp.Instance.netManager.SendMessage(PB.code.LOGIN_C.GetHashCode(), hsLogin);
            }
            else
            {
                UINetRequest.Close();
                UIMgr.Instance.DestroyUI(UIMgr.Instance.GetUI(UILogin.ViewName));
                GameMain.Instance.ChangeModule<CreatePlayerModule>();
            }
        }
        else
        {
            Debug.LogError("Error for Net");
        }
    }

	void OnNetLoginFinished(ProtocolMessage  msg)
    {
        UINetRequest.Close();

        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {

            return;
        }

        BuildModule.needSyncInfo = true;
		PB.HSLoginRet loginS = msg.GetProtocolBody<PB.HSLoginRet> ();
		UIMgr.Instance.DestroyUI(UIMgr.Instance.GetUI(UILogin.ViewName));
		if (loginS.playerId > 0) 
		{
			GameDataMgr.Instance.PlayerDataAttr.playerId = loginS.playerId;
			GameMain.Instance.ChangeModule<BuildModule>();
		}
		else
		{
			GameMain.Instance.ChangeModule<CreatePlayerModule>();
        }
	}

	public override void OnInit(object param)
	{
		UIMgr.Instance.OpenUI_(UILogin.ViewName);
	}
	
	public override void OnEnter(object param)
	{
		BindListener();
	}
	
	public override void OnExecute()
	{
		
	}
	
	public override void OnExit()
	{
		UnBindListener();
	}	
}
