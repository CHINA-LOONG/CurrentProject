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
    /// <summary>
    /// login http服务器，目的是获得游戏服务器信息
    /// </summary>
	void OnLoginClick()
	{
        StartCoroutine(GetGameServer());
	}

    void OnServerClick(Hashtable serverInfo)
    {
        GameApp.Instance.netManager.GameServerAdd = serverInfo["hostIp"].ToString();
        GameApp.Instance.netManager.GameServerPort = int.Parse(serverInfo["port"].ToString());

        GameApp.Instance.netManager.SendConnect();
       // UIMgr.Instance.DestroyUI(UIMgr.Instance.GetUI(UISelectServer.ViewName));
    }

    IEnumerator GetGameServer()
    {
        UINetRequest.Open();

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
            OnGetGameServerFaild();
            yield break;
        }
        string gameServerAddress = accountRequest.Response.DataAsText;
        Dictionary<string, List<UIServerData>> serverDictionary = new Dictionary<string, List<UIServerData>>();
        Hashtable jsonObjects = MiniJSON.jsonDecode(gameServerAddress) as Hashtable;
        if (null == jsonObjects)
        {
            Logger.Log("游戏服务器获取失败");
            OnGetGameServerFaild();
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
    void OnGetGameServerFaild()
    {
        UINetRequest.Close();
        MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("retry_yichang"));
    }


    /// <summary>
    /// 与GameServer socket连接回调
    /// </summary>
    /// <param name="state"></param>
    void OnNetConnectFinished(int state)
    {
        if (1 == state)
        {            
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
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("retry_yichang"));
            Logger.LogError("Error for Net to socket connect!");
        }
    }
    /// <summary>
    /// socket 连接成功后 的login 回调
    /// </summary>
    /// <param name="msg"></param>
	void OnNetLoginFinished(ProtocolMessage  msg)
    {
        UINetRequest.Close();

        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("retry_yichang"));
            return;
        }

        BuildModule.needSyncInfo = true;
		PB.HSLoginRet loginS = msg.GetProtocolBody<PB.HSLoginRet> ();
		UIMgr.Instance.DestroyUI(UIMgr.Instance.GetUI(UILogin.ViewName));

        if (loginS.playerId > 0 && !string.IsNullOrEmpty(loginS.nickname))
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
