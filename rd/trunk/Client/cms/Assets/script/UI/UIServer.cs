using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
public class UIServer : UIBase {
    public static string ViewName = "UIServer";
    public GameObject login;
    public GameObject selectServerBtn;
    public GameObject LogServerPanel;
    public GameObject serverListPanel;
    public GameObject recommendServer;
    public GameObject Close;
    public Animator logAnimator;
    public Animator ServerAnimator;
    public Transform IntervalItemBox;
    public Transform serverItemBox;
    public Transform serverPlayerItemBox;
    public Text lastServer;//Recommend / Last
    public Text lastServerName;
    public Text lasrServerType;
    public Text currServerName;
    public Text serverStageText;
    public Text currLogServerText;
    public Text changeBtnText;
    public Text selectServerText;
    int interval = 0;
    bool isCreateRecommend = false;
    bool isOut = false;
    UIServerData recordCurrServer = null;
    List<GameObject> uiServerItem = new List<GameObject>();
    List<GameObject> uiServerIntervalItem = new List<GameObject>();
    List<UIServerData> serverPlayerInfo = new List<UIServerData>();
    Dictionary<string, List<UIServerData>> uiServerData;
    //------------------------------------------------------------------------------------------------------
    void AnimatorInterval()
    {
        if (isOut)
        {
            serverListPanel.SetActive(true);
            LogServerPanel.SetActive(false);
        }
        else
        {
            serverListPanel.SetActive(false);
            LogServerPanel.SetActive(true);
        }
    }
    void OpenServerList(GameObject btn)//打开服务器列表
    {
        logAnimator.SetBool("isOut", true);
        logAnimator.SetBool("isIn", true);
        isOut = true;
        Invoke("AnimatorInterval", 0.05f);
        if (interval != 0)
            return;
        if (serverPlayerInfo.Count > 0)//有没有推荐服务器
        {
            CreateIntervalItem("login_server_001", null, 1);
        }
        foreach (var item in uiServerData) 
        {
            interval++;
            CreateIntervalItem(item.Key, item.Key);
        }
    }
    //------------------------------------------------------------------------------------------------------
    void CloseServerList(GameObject btn)
    {
        ServerAnimator.SetBool("isOut", true);
        ServerAnimator.SetBool("isIn", false);
        isOut = false;
        Invoke("AnimatorInterval",0.05f);
    }
    //------------------------------------------------------------------------------------------------------
    void SetServerItem(List<UIServerData> serverData)//设置服务器列表
    {
        GameSetActive(false);
        int i = 0;
        ServerItemData serverItemData = null;
        for (; i < serverData.Count; i++)
        {
            serverItemData = uiServerItem[i].GetComponent<ServerItemData>();
            serverItemData.sServerName.text = serverData[i].serverIndex + " - " + serverData[i].serverName;
            serverItemData.sServerType.text = serverData[i].serverType.ToString();
            SetTextColor(serverItemData.sServerType);
            serverItemData.sHostIp = serverData[i].hostIp;
            serverItemData.sPort = serverData[i].port;
            uiServerItem[i].SetActive(true);
        }
        for (; i < uiServerItem.Count; i++)
            uiServerItem[i].SetActive(false);
    }
    //------------------------------------------------------------------------------------------------------
    void CreateIntervalItem(string stageName, string serverInterval, int serverStageType = 0)//创建服务器区
    {
        ServerStageItemData stageItemData = null;
        GameObject stageItem = ResourceMgr.Instance.LoadAsset("ServerStageItem");
        stageItem.transform.SetParent(IntervalItemBox);
        stageItem.transform.localScale = IntervalItemBox.localScale;
        ScrollViewEventListener.Get(stageItem).onClick = StageIntervalItemClick;
        stageItemData = stageItem.GetComponent<ServerStageItemData>();
        if (serverStageType == 1)
        {
            stageName = StaticDataMgr.Instance.GetTextByID(stageName);
        }
        stageItemData.stageName.text = stageName; 
        stageItemData.serverStageType = serverStageType;
        stageItemData.interval = serverInterval;
        stageItemData.selectBox.SetActive(false);
        uiServerIntervalItem.Add(stageItem);
        if (uiServerIntervalItem.Count == uiServerData.Count)
        {
            ServerStageItemData sSItemData = uiServerIntervalItem[uiServerIntervalItem.Count - 1].GetComponent<ServerStageItemData>();
            sSItemData.selectBox.SetActive(true);
            serverStageText.text = sSItemData.stageName.text;
            SetServerItem(uiServerData[serverInterval]);
        }
       
    }
    //------------------------------------------------------------------------------------------------------
    void StageIntervalItemClick(GameObject btn)//服务器区点击事件
    {
        ServerStageItemData stageItemData = null;
        for (int i = 0; i < uiServerIntervalItem.Count; i++)
        {
            if (uiServerIntervalItem[i] == btn)
            {
                stageItemData = uiServerIntervalItem[i].GetComponent<ServerStageItemData>();
                stageItemData.selectBox.SetActive(true);
                serverStageText.text = stageItemData.stageName.text;
                if (stageItemData.serverStageType == 1)//1=推荐服务器
                    RecommendServer(serverPlayerInfo);
                else
                    SetServerItem(uiServerData[stageItemData.interval]);
            }
            else
            {
                stageItemData = uiServerIntervalItem[i].GetComponent<ServerStageItemData>();
                stageItemData.selectBox.SetActive(false);
            }
        }
    }
    //------------------------------------------------------------------------------------------------------
    void GameSetActive(bool isShowRecommendServer)
    {
        if (isShowRecommendServer)
        {
            serverStageText.gameObject.SetActive(false);
            serverItemBox.gameObject.SetActive(false);
            recommendServer.gameObject.SetActive(true);
        }
        else
        {
            serverStageText.gameObject.SetActive(true);
            serverItemBox.gameObject.SetActive(true);
            recommendServer.gameObject.SetActive(false);
        }
    }
    //------------------------------------------------------------------------------------------------------
    void RecommendServer(List<UIServerData> serverData)//创建推荐服务器
    {
        GameSetActive(true);
        if (isCreateRecommend)
            return;
        GameObject serverPlayerItem;
        ServerItemData serverItemData;
        for (int i = 0; i < serverData.Count; i++)
        {
            if (serverData[i].nickName != null)
            {
                serverPlayerItem = ResourceMgr.Instance.LoadAsset("ServerInfo");
                serverPlayerItem.transform.SetParent(serverPlayerItemBox);
                serverPlayerItem.transform.localScale = serverPlayerItemBox.localScale;
                ScrollViewEventListener.Get(serverPlayerItem).onClick = ServerItemClick;
                serverItemData = serverPlayerItem.GetComponent<ServerItemData>();
                serverItemData.sServerName.text = serverData[i].serverIndex + " - " + serverData[i].serverName;
                serverItemData.sPlayerName.text = serverData[i].nickName;
                serverItemData.sPlayerLv.text = "lv:" + serverData[i].level;
                serverItemData.lv = serverData[i].level;
                serverItemData.sServerType.text = serverData[i].serverType.ToString();
                SetTextColor(serverItemData.sServerType);
                serverItemData.sHostIp = serverData[i].hostIp;
                serverItemData.sPort = serverData[i].port;
            }
        }
        isCreateRecommend = true;
    }
    //------------------------------------------------------------------------------------------------------
    int ServerSort(UIServerData a, UIServerData b)//排序
    {
        if (a.level > b.level)
        {
            return 1;
        }
        else if (a.level<b.level)
        {
            return -1;
        }
        return 0;
    }
    //------------------------------------------------------------------------------------------------------
    public void SetCurrServer(Dictionary<string, List<UIServerData>> serverData, string serverName = null)//设置当前选择的服务器
    {
        if (serverName != null)
        {
             currServerName.text = serverName;
             return;
        }
        uiServerData = serverData;
        foreach (var item in uiServerData)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                if (item.Value[i].nickName != null)
                {
                    serverPlayerInfo.Add(item.Value[i]);
                }                
            }
            serverName = item.Value[item.Value.Count - 1].serverIndex + " - " + item.Value[item.Value.Count - 1].serverName;
            lasrServerType.text = item.Value[item.Value.Count - 1].serverType.ToString();
        }
        serverPlayerInfo.Sort(ServerSort);//按等级最高排序
        if (PlayerPrefs.GetString("_serverName") != string.Empty)
        { 
            serverName = PlayerPrefs.GetString("_serverName");
            lastServer.text = StaticDataMgr.Instance.GetTextByID("login_server_002");
            lasrServerType.text = PlayerPrefs.GetString("_serverType");
        }
        else if (serverPlayerInfo.Count > 0)
        {
            serverName = serverPlayerInfo[0].serverIndex + " - " + serverPlayerInfo[0].serverName;
            lasrServerType.text = serverPlayerInfo[0].serverType.ToString();
        }
        SetTextColor(lasrServerType);
        currServerName.text = serverName;
        lastServerName.text = serverName;
    }
    //------------------------------------------------------------------------------------------------------
    void SetTextColor(Text text)//设置字体颜色
    {
        if (text.text ==UIServerType.New.ToString())
            text.color = ColorConst.server_color_new;
        else if (text.text ==UIServerType.Full.ToString())
            text.color = ColorConst.server_color_full;
        else if (text.text == UIServerType.Hot.ToString())
            text.color = ColorConst.server_color_hot;
        else if (text.text == UIServerType.Maintain.ToString())
            text.color = ColorConst.server_color_Maintain;
    }
    //------------------------------------------------------------------------------------------------------
    void ServerItemClick(GameObject btn)//服务器Item点击事件
    {
        ServerItemData serverItemData = btn.GetComponent<ServerItemData>();
        if (serverItemData.sServerType.text == UIServerType.Maintain.ToString() ||
            serverItemData.sServerType.text == UIServerType.SERVER_TYPE_UNKNOW.ToString())
        {
            MsgBox.PromptMsg.Open(
            MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("login_weihu_001"));
            return;
        }
        recordCurrServer = new UIServerData();
        recordCurrServer.serverType = GetServerType(serverItemData.sServerType.text);
        recordCurrServer.serverName = serverItemData.sServerName.text;
        if (serverItemData.sPlayerName != null)
        {
            recordCurrServer.nickName = serverItemData.sPlayerName.text;
            recordCurrServer.level = serverItemData.lv;
        }
        recordCurrServer.hostIp = serverItemData.sHostIp;
        recordCurrServer.port = serverItemData.sPort;
        SetCurrServer(null, serverItemData.sServerName.text);
        CloseServerList(null);
    }
    //------------------------------------------------------------------------------------------------------
    public UIServerType GetServerType(string Type)
    {
        foreach (UIServerType serverType in Enum.GetValues(typeof(UIServerType)))
        {
            if (Type == serverType.ToString())
            {
                return serverType;
            }
        }
        return UIServerType.SERVER_TYPE_UNKNOW;
    }
    //------------------------------------------------------------------------------------------------------
    void LogServer(GameObject btn)//进入游戏
    {
        Hashtable CurrServerHashtable = new Hashtable();
        if (recordCurrServer == null && PlayerPrefs.GetString("_serverName") == string.Empty)
        {
            Debug.LogError("请选择服务器");
            return;
        }
        else if (recordCurrServer != null )
        {            
            PlayerPrefs.SetString("_serverName", recordCurrServer.serverName);
            PlayerPrefs.SetString("_serverType", recordCurrServer.serverType.ToString());
            PlayerPrefs.SetString("_hostIp", recordCurrServer.hostIp);
            PlayerPrefs.SetInt("_port", recordCurrServer.port);
            if (recordCurrServer.serverName != string.Empty)
            {
                PlayerPrefs.SetString("_nickName", recordCurrServer.nickName);
                PlayerPrefs.SetString("_level", recordCurrServer.level.ToString());
            }
        }
        else if(PlayerPrefs.GetString("_serverName") != string.Empty)
        {
            recordCurrServer = new UIServerData();
            recordCurrServer.hostIp = PlayerPrefs.GetString("_hostIp");
            recordCurrServer.serverName = PlayerPrefs.GetString("_serverName");
            recordCurrServer.serverType = GetServerType(PlayerPrefs.GetString("_serverType"));
            recordCurrServer.port = PlayerPrefs.GetInt("_port");
            if (PlayerPrefs.GetString("_nickName") != string.Empty)
            {
                recordCurrServer.nickName = PlayerPrefs.GetString("_nickName");
                recordCurrServer.level = int.Parse(PlayerPrefs.GetString("_level"));
            }
        }
        if (recordCurrServer.serverType == UIServerType.Maintain ||
            recordCurrServer.serverType == UIServerType.SERVER_TYPE_UNKNOW)
        {
            MsgBox.PromptMsg.Open(
            MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("login_weihu_001"));
            return;
        }
        CurrServerHashtable.Add("hostIp", recordCurrServer.hostIp);
        CurrServerHashtable.Add("serverName", recordCurrServer.serverName);
        CurrServerHashtable.Add("port", recordCurrServer.port);
        if (recordCurrServer.nickName != string.Empty)
        {
            Hashtable role = new Hashtable();
            role.Add("nickName", recordCurrServer.nickName);
            role.Add("level", recordCurrServer.level);
            CurrServerHashtable.Add("role", role);
        }
        GameEventMgr.Instance.FireEvent<Hashtable>(GameEventList.ServerClick, CurrServerHashtable);
        UIMgr.Instance.CloseUI_(UIServer.ViewName);
    }
    //------------------------------------------------------------------------------------------------------
	void Start () {
        EventTriggerListener.Get(selectServerBtn).onClick = OpenServerList;
        EventTriggerListener.Get(login).onClick = LogServer;
        EventTriggerListener.Get(Close).onClick = CloseServerList;
        GameObject serverItem;
        for (int i = 0; i < 10; i++)//一个分区最多10个服务器
        {
            serverItem = ResourceMgr.Instance.LoadAsset("ServerItem");
            serverItem.transform.SetParent(serverItemBox);
            serverItem.transform.localScale = serverItemBox.localScale;
            EventTriggerListener.Get(serverItem).onClick = ServerItemClick;
            uiServerItem.Add(serverItem);
            uiServerItem[i].SetActive(false);
        }
        lastServer.text = StaticDataMgr.Instance.GetTextByID("login_server_001");
        currLogServerText.text = StaticDataMgr.Instance.GetTextByID("login_denglu_001");
        changeBtnText.text = StaticDataMgr.Instance.GetTextByID("login_denglu_002");
        selectServerText.text = StaticDataMgr.Instance.GetTextByID("login_server_003");
	}
    //------------------------------------------------------------------------------------------------------
}
