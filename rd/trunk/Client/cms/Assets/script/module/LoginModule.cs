using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using System;
using System.IO;

public class LoginModule : ModuleBase 
{
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
        //HTTPRequest centerRequest = new HTTPRequest(new Uri(Const.CollectorUrl), HTTPMethods.Post);
        //centerRequest.AddField("game", Const.AppName);
        //centerRequest.AddField("platform", Const.platform);
        //centerRequest.AddField("channel", Const.channel);
        //centerRequest.Send();
        //yield return StartCoroutine(centerRequest);
        //if (centerRequest.Response == null || !centerRequest.Response.IsSuccess)
        //{
        //    Logger.Log("连接中心服务器失败");
        //    yield break;
        //}

        ////账号服务器返回值
        //string accountServerAddress = centerRequest.Response.DataAsText;
        //int port = 0;
        //string ip = null;

        //Hashtable ht = MiniJsonExtensions.hashtableFromJson(accountServerAddress);
        //if (null == ht)
        //{
        //   Logger.Log("账号服务器分配失败");
        //   yield break;
        //}
        //else
        //{
        //   port = int.Parse(ht["httpPort"].ToString());
        //   ip = ht["hostIp"].ToString();
        //}

        //Logger.Log("连接中心服务器成功");

        string path = Const.ServerUrl;//"http://" + "192.168.199.177:9101 " + "/fetch_gameServer";123.59.45.55
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
            Logger.Log("连接账号服务器失败");
            yield break;
        }
        string gameServerAddress = accountRequest.Response.DataAsText;
        Dictionary<string, List<UIServerData>> serverDictionary = new Dictionary<string, List<UIServerData>>();
        Hashtable jsonObjects = MiniJSON.jsonDecode(gameServerAddress) as Hashtable;
        if (null == jsonObjects)
        {
            Logger.Log("游戏服务器获取失败");
            yield break;
        }

        Hashtable serverHashtable = jsonObjects["servers"] as Hashtable;
        Hashtable roles = jsonObjects["roles"] as Hashtable;

        ArrayList serverHashtableList = new ArrayList(serverHashtable.Values);
        serverHashtableList.Reverse();
        for (int i = 0; i < serverHashtableList.Count; i++)
        {
            List<UIServerData> serverList = new List<UIServerData>();
            Hashtable areaInfo = serverHashtableList[i] as Hashtable;
            ArrayList servers = areaInfo["serverList"] as ArrayList;
            foreach (Hashtable item1 in servers)
            {
                UIServerData uiServerData = new UIServerData();
                uiServerData.serverName = item1["serverName"].ToString();
                uiServerData.serverIndex = int.Parse(item1["serverIndex"].ToString());
                uiServerData.hostIp = item1["hostIp"].ToString();
                uiServerData.port = int.Parse(item1["port"].ToString());
                foreach (UIServerType serverType in Enum.GetValues(typeof(UIServerType)))
                {
                    if ((int)serverType == int.Parse(item1["state"].ToString()))
                    {
                        uiServerData.serverType = serverType;
                    }
                }
                if(roles != null) 
                {
                    foreach( var index in roles.Keys)
                    {
                        if (uiServerData.serverIndex == int.Parse(index.ToString()))
                        {
                            Hashtable playerInfo = roles[index] as Hashtable;
                            uiServerData.nickName = playerInfo["nickname"].ToString();                            
                            uiServerData.level = int.Parse(playerInfo["level"].ToString());
                            break;
                        }
                    }
                }
                serverList.Add(uiServerData);
            }
            serverDictionary.Add(areaInfo["name"].ToString(), serverList);
        }
        Logger.Log("连接账号服务器成功");
        UIServer uiSelectServer = UIMgr.Instance.OpenUI_(UIServer.ViewName) as UIServer;
        if (uiSelectServer != null)
        {
            uiSelectServer.SetCurrServer(serverDictionary);
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
            Logger.LogWarning("OK for net");
            
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("testGuid")) == false)
            {
                GameDataMgr.Instance.UserDataAttr.guid = PlayerPrefs.GetString("testGuid");
                PB.HSLogin hsLogin = new PB.HSLogin();
                hsLogin.puid = PlayerPrefs.GetString("testGuid");
                hsLogin.token = GameDataMgr.Instance.UserDataAttr.token;
                GameApp.Instance.netManager.SendMessage(PB.code.LOGIN_C.GetHashCode(), hsLogin);
            }
            else
            {
                Logger.Log("username cant't be empty");
            }
        }
        else
        {
            Logger.LogError("Error for Net");
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

		GameDataMgr.Instance.PlayerDataAttr.playerId = loginS.playerId;
		GameMain.Instance.ChangeModule<BuildModule>();
	}

	public override void OnInit(object param)
	{
		UIMgr.Instance.OpenUI_(UILogin.ViewName);
	}
	
	public override void OnEnter(object param)
	{
		BindListener();
        AudioSystemMgr.Instance.PlayMusicByName("Loginmusic");
	}
	
	public override void OnExecute()
	{
		
	}
	
	public override void OnExit()
	{
		UnBindListener();
	}	
}
