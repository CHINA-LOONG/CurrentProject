using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
public enum ImMessageType
{
    Msg_Type_Recruit,//招收成员
    Msg_Type_Task,//工会任务
    Num_Msg_Type
}

public class UIIm : UIBase
{
    #region 
    public static string ViewName = "uishowMsg";
    List<PB.HSImMsg> worldChannel = new List<PB.HSImMsg>();//世界頻道列表
    List<PB.HSImMsg> guildChannel = new List<PB.HSImMsg>();//工會頻道列表 
    PB.HSImMsg allMsg;//基础聊天框
    int channel = (int)PB.ImChannel.WORLD;//頻道
    public InputField msgText;
    public Text placeholder;
    public GameObject showLeftChatBox;
    public GameObject showBasicsChat;
    public GameObject basicsChatBox;
    public GameObject globalButton;
    public Image globalBackground;
    public GameObject guildButton;
    public Image guildBackground;
    public GameObject leftChatBox;
    public GameObject basicsChat;
    public GameObject imMsgFather;
    public RectTransform msgBox;
    public Text basicsValue;
    public Text globalText;
    public Text guildText;
    public GameObject msgPos;//位置
    float msgPosY;
    List<GameObject> msgObj = new List<GameObject>();//消息gameobj
    //玩家信息
    public GameObject playerBox;
    public GameObject playerBoxClone;
    public Text playerLevel;
    public Text playerName;
    public GameObject shield;//屏蔽
    public GameObject inviteGuild;//邀请入工会
    public Text guildName;//工会名称
    int guildID = 0;
    int blockID;//屏蔽ID
    ImMessageData imMsgData;
    static UIIm mInst = null;
    bool isSend = true;
    bool isSendRecruit = true;
    bool isRendTask = true;
    private MsgBox.PromptMsg shieldWnd;
    public Transform moduleVec;//对局位置
    public Transform leftCahtMove;//左侧聊天框唤出位置
    Vector3 basiceChatVec;//基础聊天框初始位置
    Vector3 leftCahtVec;//左侧聊天框初始位置  
    [HideInInspector]
    public bool isDrag;
    bool isBasicsChat = true;
    bool isChannel = false;
    public bool showNewMsg;//新消息
    Vector2 msgBoxSizeDelta;
    #endregion
    public static UIIm Instance
    {
        get
        {
            return mInst;
        }
    }
    //------------------------------------------------------------------------------------------------------
    void Awake()
    {
        mDontDestroyWhenSwitchScene = true;
    }
    //------------------------------------------------------------------------------------------------------
	// Use this for initialization
    void Start()
    {        
        mInst = this;
        placeholder.text = StaticDataMgr.Instance.GetTextByID("im_chat_enter");
        globalButton.transform.FindChild("Text").GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("im_chat_global");
        guildButton.transform.FindChild("Text").GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("im_chat_guild");
        shield.transform.FindChild("Text").GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("im_block");
        inviteGuild.transform.FindChild("Text").GetComponent<Text>().text = StaticDataMgr.Instance.GetTextByID("im_invite");
        EventTriggerListener.Get(showLeftChatBox).onClick = MsgOnClick;
        EventTriggerListener.Get(showBasicsChat).onClick = MsgOnClick;
        EventTriggerListener.Get(globalButton).onClick = MsgOnClick;
        EventTriggerListener.Get(guildButton).onClick = MsgOnClick;
        EventTriggerListener.Get(playerBoxClone).onClick = MsgOnClick;
        EventTriggerListener.Get(shield).onClick = ShieldClick;
        EventTriggerListener.Get(inviteGuild).onClick = SelectGuild;
        msgText.onEndEdit.AddListener(delegate { SendClick(msgText); });
        ImMessageData imData;
        for (int i = 0; i < BattleConst.maxMsg; i++)
        {
            GameObject imMessage = ResourceMgr.Instance.LoadAsset("imMessage");
            imMessage.transform.SetParent(imMsgFather.transform, false);
            imMessage.transform.localScale = imMsgFather.transform.localScale;
            msgObj.Add(imMessage);
            imData = imMessage.GetComponent<ImMessageData>();
            EventTriggerListener.Get(imData.mPlayer.gameObject).onClick = PlayerClick;   
            imMessage.SetActive(false);
        }
        basiceChatVec = basicsChatBox.transform.localPosition;
        leftCahtVec = leftChatBox.transform.localPosition;
        msgBoxSizeDelta = msgBox.sizeDelta;
        playerBox.SetActive(false);
        RectTransform initTransform = msgPos.transform as RectTransform;
        msgPosY = -initTransform.anchoredPosition.y;
    }
    //------------------------------------------------------------------------------------------------------
    public void HideIm(bool isHide)
    {
        if (isHide)
        {
             basicsChatBox.SetActive(false);
             leftChatBox.transform.position = leftCahtVec;
             msgBox.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            HideChat();
        }
    }
    //------------------------------------------------------------------------------------------------------
    public void SetLevelVisible(bool visible)
    {
        Vector3 targetPos = leftChatBox.transform.localPosition;
        if (playerBox.activeSelf)
        {
            playerBox.SetActive(false);
            if (shieldWnd != null)
                shieldWnd.Close();
        }
        basicsChatBox.SetActive(visible);
        leftChatBox.transform.localPosition = new Vector3(targetPos.x, leftCahtVec.y, targetPos.z);
    }

    //------------------------------------------------------------------------------------------------------
    void MsgOnClick(GameObject but)//通用button点击事件
    {
        if (but.name == showLeftChatBox.name)//show左侧聊天框
        {
            basicsChatBox.SetActive(false);
            isBasicsChat = false;
            //Tweener imMover = leftChatBox.transform.DOLocalMoveY(leftCahtMove.localPosition.y, 0.5f);
            //imMover.SetUpdate(true);
            leftChatBox.transform.position = leftCahtMove.transform.position;
            //leftChatBox.SetActive(true);
            ShowMessage();
        }
        else if (but.name == showBasicsChat.name)//show基础聊天框
        {
            HideChat();
            ShowMessage();
        }
        else if (but.name == globalButton.name)//世界频道
        {
            if (channel != (int)PB.ImChannel.WORLD)
            {
                isChannel = true;
                channel = (int)PB.ImChannel.WORLD;
                globalBackground.enabled = true;
                guildBackground.enabled = false;
                guildText.GetComponent<Outline>().effectColor = ColorConst.outline_tabColor_normal;
                guildText.color = ColorConst.text_tabColor_normal;
                globalText.GetComponent<Outline>().effectColor = ColorConst.outline_tabColor_select;
                globalText.color = ColorConst.text_tabColor_select;
                msgBox.sizeDelta = msgBoxSizeDelta;
                msgBox.localPosition = new Vector3(0, 0, 0);
                ShowMessage();
            }
        }
        else if (but.name == guildButton.name)//工会频道
        {
            if (GameDataMgr.Instance.SociatyDataMgrAttr.allianceID == 0)
            {
                ShowSystemHints(StaticDataMgr.Instance.GetTextByID("im_record_002"), (int)PB.ImType.PROMPT);
                return;
            }
            if (channel != (int)PB.ImChannel.GUILD)
            {
                isChannel = true;
                channel = (int)PB.ImChannel.GUILD;
                guildBackground.enabled = true;
                globalBackground.enabled = false;
                globalText.GetComponent<Outline>().effectColor = ColorConst.outline_tabColor_normal;
                globalText.color = ColorConst.text_tabColor_normal;
                guildText.GetComponent<Outline>().effectColor = ColorConst.outline_tabColor_select;
                guildText.color = ColorConst.text_tabColor_select;
                msgBox.sizeDelta = msgBoxSizeDelta;
                msgBox.localPosition = new Vector3(0, 0, 0);
                ShowMessage();
            }
        }
        else if (but == playerBoxClone)
        {
            playerBox.SetActive(false);
        }
    }
    //------------------------------------------------------------------------------------------------------
    void PlayerClick(GameObject player)//玩家操作
    {
        player = player.transform.parent.gameObject.transform.parent.gameObject;
        imMsgData = player.GetComponent<ImMessageData>();
        if (GameDataMgr.Instance.PlayerDataAttr.playerId != imMsgData.speakerID && imMsgData.speakerID != 0)
        {
            PB.HSImPlayerGet param = new PB.HSImPlayerGet() { playerId = imMsgData.speakerID };
            GameApp.Instance.netManager.SendMessage(PB.code.IM_PLAYER_GET_C.GetHashCode(), param, false);
        }
    }
    //------------------------------------------------------------------------------------------------------
    Transform GetRootObject(Transform childObject)
    {
        return childObject.transform.parent.transform.parent;
    }
    //------------------------------------------------------------------------------------------------------
    void HyperlinkJumpRecruit(GameObject btn)
    {
        if (GameMain.Instance.IsCurModule<BattleModule>())
            ShowSystemHints(StaticDataMgr.Instance.GetTextByID("im_record_006"), (int)PB.ImType.PROMPT);
        else
            GameDataMgr.Instance.SociatyDataMgrAttr.OpenSociaty(GetRootObject(btn.transform.parent).GetComponent<ImMessageData>().guildID);
    }
    //------------------------------------------------------------------------------------------------------
    void HyperlinkJumpTask(GameObject btn)
    {
        if (GameMain.Instance.IsCurModule<BattleModule>())
            ShowSystemHints(StaticDataMgr.Instance.GetTextByID("im_record_006"), (int)PB.ImType.PROMPT);
        else
        {
            if (GetRootObject(btn.transform.parent).GetComponent<ImMessageData>().taskID != null)
            {
                GameDataMgr.Instance.SociatyDataMgrAttr.OpenSociatyTaskWithTeam(SociatyTaskContenType.OtherTeam, GetRootObject(btn.transform.parent).GetComponent<ImMessageData>().taskID);
            } 
        }
    }
    //------------------------------------------------------------------------------------------------------
    void SelectGuild(GameObject btn)
    {
        if (GameMain.Instance.IsCurModule<BattleModule>())
            ShowSystemHints(StaticDataMgr.Instance.GetTextByID("im_record_006"), (int)PB.ImType.PROMPT);
        else
        {
            if (guildID != 0)
            {
                GameDataMgr.Instance.SociatyDataMgrAttr.OpenSociaty(guildID.ToString());
            }
            else
            {
                ShowSystemHints(StaticDataMgr.Instance.GetTextByID("im_record_005"), (int)PB.ImType.PROMPT);
            }
        }        
    }
    //------------------------------------------------------------------------------------------------------
    void OnMsgReturn(ProtocolMessage msg)//返回服务器消息数据
    {
        PB.HSImPush result = msg.GetProtocolBody<PB.HSImPush>();
        List<PB.HSImMsg> messageData = result.imMsg;
        foreach (var item in messageData)
        {            
            //系统公告
            if (item.type == (int)PB.ImType.NOTICE)
            {
                uiHintMsg.Instance.NoticeAdd(item.origText);
                worldChannel.Add(item);
                if (GameDataMgr.Instance.SociatyDataMgrAttr.allianceID > 0)
                {
                    guildChannel.Add(item);
                }
                //所有消息
                allMsg = item;
                continue;
            }
            ShowSystemHints(item.origText, item.type);
            //频道相关
            if (item.channel == (int)PB.ImChannel.WORLD && item.type != (int)PB.ImType.LANTERN && item.type != (int)PB.ImType.PROMPT)
            {
                worldChannel.Add(item);
                allMsg = item;
            }
            if (item.channel == (int)PB.ImChannel.GUILD)
            {
                guildChannel.Add(item);
                allMsg = item;
            }
        }
        ShowMessage();
    }
    //------------------------------------------------------------------------------------------------------
    void ShowMessage()//显示消息
    {
        if (channel == (int)PB.ImChannel.WORLD)
        {
            ReadMsg(ref worldChannel, StaticDataMgr.Instance.GetTextByID("im_chat_global"), ColorConst.globalColor);
        }
        else if (channel == (int)PB.ImChannel.GUILD)
        {
            ReadMsg(ref guildChannel, StaticDataMgr.Instance.GetTextByID("im_chat_guild"), ColorConst.guildColor);
        }
        if (isBasicsChat && allMsg != null)//basicsChat.activeSelf && worldChannel.Count > 0)
        {
            //if (!basicsChat.activeSelf)
            {
                basicsChat.SetActive(true);
                basicsChat.transform.DORotate(new Vector3(0, 0, 0), 0.8f);
            }
            string msg = null;
            if (allMsg.channel == (int)PB.ImChannel.WORLD)
            {
                if (allMsg.type == (int)PB.ImType.NOTICE)
                    msg = "<color=" + ColorConst.colorTo_Hstr(ColorConst.systemColor) + ">[" + StaticDataMgr.Instance.GetTextByID("im_chat_system") + "] " +
                        allMsg.origText + "</color>";
                else
                    msg = "<color=" + ColorConst.colorTo_Hstr(ColorConst.globalColor) + ">[" + StaticDataMgr.Instance.GetTextByID("im_chat_global") + "] </color>" +
                        allMsg.senderName + ": <color=" + ColorConst.colorTo_Hstr(ColorConst.globalColor) + ">" + allMsg.origText + "</color>";
            }
            else
            {
                msg = "<color=" + ColorConst.colorTo_Hstr(ColorConst.guildColor) + ">[" + StaticDataMgr.Instance.GetTextByID("im_chat_guild") + "] </color>" +
                       allMsg.senderName + ": <color=" + ColorConst.colorTo_Hstr(ColorConst.guildColor) + ">" + allMsg.origText + "</color>";
            }
            basicsValue.text = msg;
            StartCoroutine(Func());
            allMsg = null;
        }
    }
    //------------------------------------------------------------------------------------------------------
    IEnumerator Func()
    {
        yield return new WaitForSeconds(3f);
        //if (allMsg == null)
        //{
            basicsChat.transform.DORotate(new Vector3(-90, 0, 0), 0.8f);
        //}
    }
    //------------------------------------------------------------------------------------------------------
    void ReadMsg(ref List<PB.HSImMsg> msgList, string channelName, Color msgColor)//读取赋值消息
    {
        float msgHeight;//一条消息的高
        float newMsgPos;//新的消息的位置
        int overCount = msgList.Count - BattleConst.maxMsg;
        for (int j = 0; j < overCount; ++j)
        {
            if (msgList[0].expansion != null)
            {
                ImMessageData curMsgData = msgObj[0].gameObject.GetComponent<ImMessageData>();
                if (curMsgData != null && curMsgData.mScrollmContentClick != null)
                {
                    Destroy(curMsgData.mScrollmContentClick);
                    curMsgData.mScrollmContentClick = null;
                }
            }
            msgList.RemoveAt(0);
        }
        int i = 0;
        for (; i < msgList.Count; i++)
        {
            msgObj[i].transform.localPosition = msgPos.transform.localPosition;
            imMsgData = msgObj[i].gameObject.GetComponent<ImMessageData>();
            if (i != 0)
            {
                GameObject msgText = Util.FindChildByName(msgObj[i - 1], "msgText");
                if (msgText != null)
                {
                    RectTransform msgTrans = msgText.transform as RectTransform;
                    msgHeight = msgTrans.sizeDelta.y - msgTrans.localPosition.y;
                    newMsgPos = msgObj[i - 1].transform.localPosition.y - msgHeight;
                    msgObj[i].transform.localPosition = new Vector3(msgPos.transform.localPosition.x, newMsgPos, 0);
                }
            }
            Color textColor;
            msgObj[i].SetActive(true);
            if (msgList[i].type == (int)PB.ImType.NOTICE)
            {
                textColor = ColorConst.systemColor;
                imMsgData.mChannel.text = "[" + StaticDataMgr.Instance.GetTextByID("im_chat_system") + "]";
                imMsgData.mContent.supportRichText = true;
                imMsgData.mSpeaker.text = msgList[i].origText;
                imMsgData.mContent.text = null;
                imMsgData.mSpeaker.color = textColor;
                imMsgData.mPlayer.gameObject.SetActive(false);
            }
            else
            {
                imMsgData.mPlayer.gameObject.SetActive(true);
                imMsgData.mSpeaker.text = msgList[i].senderName + ":";
                imMsgData.mChannel.text = "[" + channelName + "]";
                imMsgData.mContent.supportRichText = false;
                imMsgData.mContent.text = msgList[i].origText;
                textColor = msgColor;
                imMsgData.mSpeaker.color = ColorConst.nameColor;
                imMsgData.mPlayer.rectTransform.sizeDelta = new Vector2(imMsgData.mSpeaker.preferredWidth,
                        imMsgData.mPlayer.rectTransform.sizeDelta.y);
            }
            imMsgData.playerName = msgList[i].senderName;
            imMsgData.mContent.color = textColor;
            imMsgData.speakerID = msgList[i].senderId;
            imMsgData.mChannel.color = textColor;
            if (msgList[i].expansion != null)
            {
                string[] msgData = msgList[i].expansion.Split('^');
                if (ImMessageType.Msg_Type_Recruit.ToString() == msgData[0])
                {
                    ScrollViewEventListener.Get(imMsgData.mContent.gameObject).onClick = HyperlinkJumpRecruit;
                    imMsgData.guildID = msgData[1];
                }
                else if (ImMessageType.Msg_Type_Task.ToString() == msgData[0])
                {
                    ScrollViewEventListener.Get(imMsgData.mContent.gameObject).onClick = HyperlinkJumpTask;
                    imMsgData.mContent.color = ColorConst.guildTaskColor;
                    imMsgData.taskID = msgData[1];
                }
                imMsgData.mScrollmContentClick = imMsgData.mContent.gameObject.GetComponent<ScrollViewEventListener>();
            }
        }
        for (; i < msgObj.Count; ++i)
        {
            msgObj[i].SetActive(false);
        }
        if (isChannel)
        {
            StartCoroutine(RenovateMsg());
            isChannel = !isChannel;
        }
        else
        {
            if (!isDrag)
            {
                StartCoroutine(RenovateMsg(false));
            }
        }
    }
    //------------------------------------------------------------------------------------------------------
    public void ShowSystemHints(string hintsValue,int hintsType)//系统提示/系统公告/走马灯
    {
        //走马灯
        if (hintsType == (int)PB.ImType.LANTERN)
        {
            if (!GameMain.Instance.IsCurModule<BattleModule>())
            {
                uiHintMsg.Instance.LanternAdd(hintsValue);
            }
        }
        //系统公告
        if (hintsType == (int)PB.ImType.NOTICE)
        {
            uiHintMsg.Instance.NoticeAdd(hintsValue);
        }
        //系统提示
        if (hintsType == (int)PB.ImType.PROMPT)
        {
            uiHintMsg.Instance.HintShow(hintsValue);
        }
    }
    //------------------------------------------------------------------------------------------------------
    IEnumerator RenovateMsg(bool Renovate = true)
    {
        yield return new WaitForEndOfFrame();
        if (Renovate)
            ShowMessage();
        else
            showNewMsg = true;
    }
    //------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (showNewMsg)
        {
            float sizeMsg = msgPosY;
            GameObject msgText;
            List<PB.HSImMsg> msgChannelList;
            int count = 0;
            if (channel == (int)PB.ImChannel.GUILD)
            {
                count = guildChannel.Count;
                msgChannelList = guildChannel;
            }
            else
            {
                count = worldChannel.Count;
                msgChannelList = worldChannel;
            }
            for (int i = 0; i < count; i++)
            {
                msgText = Util.FindChildByName(msgObj[i], "msgText");
                RectTransform msgTrans = msgText.transform as RectTransform;
                if (msgTrans.sizeDelta.y != 0)
                {
                    if (i == count - 1)
                    {
                        showNewMsg = false;
                    }
                    if (msgChannelList[i].type == (int)PB.ImType.NOTICE)
                    {
                        sizeMsg += (0 - msgTrans.localPosition.y);
                    }
                    else
                    {
                        sizeMsg += (msgTrans.sizeDelta.y - msgTrans.localPosition.y);
                    }
                    if (sizeMsg > msgBox.sizeDelta.y)
                    {
                        msgBox.sizeDelta = new Vector2(msgBox.sizeDelta.x, sizeMsg);
                    }
                }
            }
        }
    }
    //------------------------------------------------------------------------------------------------------  
    public void UpdateIMPos(bool isInBattle)//对局
    {
        UIMgr.Instance.ChangeRoot(this, isInBattle);
        if (isInBattle)
        {
            HideChat();
            basicsChatBox.transform.localPosition = moduleVec.localPosition;
        }
        else
            basicsChatBox.transform.localPosition = basiceChatVec;
    }
    //------------------------------------------------------------------------------------------------------
    public void HideChat()//隐藏大聊天框
    {
        if (!basicsChatBox.activeSelf)
        {
            basicsChatBox.SetActive(true);
            isBasicsChat = true;
            //Tweener imMover = leftChatBox.transform.DOLocalMoveY(leftCahtVec.y, 0.5f);
            //imMover.SetUpdate(true);
            leftChatBox.transform.position = leftCahtVec;
            msgBox.localPosition = new Vector3(0, 0, 0);
        }
    }
    //------------------------------------------------------------------------------------------------------
    void SendInterval() //发言间隔
    {
        isSend = true;       
    }
    void SendRecruitInterval() //招收成员间隔
    {
        isSendRecruit = true;
    }
    void SendTaskInterval() //工会任务招募间隔
    {
        isRendTask = true;
    }
    //------------------------------------------------------------------------------------------------------
    void ShieldClick(GameObject btn)
    {
        if (btn.name == shield.name)
        {
            shieldWnd = MsgBox.PromptMsg.Open(
            MsgBox.MsgBoxType.Conform_Cancel,StaticDataMgr.Instance.GetTextByID("im_block_chat"),
            StaticDataMgr.Instance.GetTextByID("im_block_cancel"),
            SendShield,
            false
            );
            shieldWnd.transform.SetParent(transform);
        }
    }
    //------------------------------------------------------------------------------------------------------
    void SendClick(InputField input)//发送协议
    {
        if (input.text.Length == 0)
            return;
        OnSendMsg(input.text);
    }
    //------------------------------------------------------------------------------------------------------
    void OnPlayerGetRet(ProtocolMessage msg)//返回玩家信息
    {
        PB.HSImPlayer result = msg.GetProtocolBody<PB.HSImPlayerGetRet>().imPlayer;
        playerBox.SetActive(true);
        playerName.text = result.nickname;
        playerLevel.text = result.level.ToString();
        blockID = (int)result.playerId;
        guildID = result.guildId;
        if (result.guildId != 0)
            guildName.text = StaticDataMgr.Instance.GetTextByID("im_chat_guild") + ": " + result.guildName;
        else
            guildName.text = StaticDataMgr.Instance.GetTextByID("im_noGuild");
    }
    //------------------------------------------------------------------------------------------------------
    public bool OnSendMsg(string message, ImMessageType msgType = ImMessageType.Num_Msg_Type, string guildID = null)//发送消息
    {
        PB.HSImChatSend param = null;
        if (msgType == ImMessageType.Num_Msg_Type)
        {
            if (isSend)
            {
                isSend = !isSend;
                Invoke("SendInterval", 5.0f);
                param = new PB.HSImChatSend()
                {
                    channel = channel,
                    text = message
                };
            }
            else
            {
                uiHintMsg.Instance.HintShow(StaticDataMgr.Instance.GetTextByID("im_record_001"));
                return false;
            }
        }
        else if (msgType == ImMessageType.Msg_Type_Recruit && guildID != null)
        {
            if (isSendRecruit)
            {
                isSendRecruit = !isSendRecruit;
                Invoke("SendRecruitInterval", 10f);
                param = new PB.HSImChatSend()
                {
                    channel = (int)PB.ImChannel.WORLD,
                    text = message,
                    expansion = msgType.ToString() + "^" + guildID
                };
            }
            else
            {
                uiHintMsg.Instance.HintShow(StaticDataMgr.Instance.GetTextByID("im_record_001"));
                return false;
            }
        }
        else if (msgType == ImMessageType.Msg_Type_Task && guildID != null)
        {
            if (isRendTask)
            {
                isRendTask = !isRendTask;
                Invoke("SendTaskInterval", 10f);
                param = new PB.HSImChatSend()
                {
                    channel = (int)PB.ImChannel.GUILD,
                    text = message,
                    expansion = msgType.ToString() + "^" + guildID
                };
            }
            else
            {
                uiHintMsg.Instance.HintShow(StaticDataMgr.Instance.GetTextByID("im_record_001"));
                return false;
            }
        }
        GameApp.Instance.netManager.SendMessage(PB.code.IM_CHAT_SEND_C.GetHashCode(), param, false);
        msgText.text = "";
        return true;
    }
    //------------------------------------------------------------------------------------------------------
    public void SendShield(MsgBox.PrompButtonClick state)//屏蔽禁言
    {
        if (state == MsgBox.PrompButtonClick.OK)
        {
            PB.HSSettingBlock param = new PB.HSSettingBlock()
            {
                playerId = blockID,
                isBlock = true
            };
            GameApp.Instance.netManager.SendMessage(PB.code.SETTING_BLOCK_C.GetHashCode(), param,false); 
        }
        else if (state == MsgBox.PrompButtonClick.Cancle)
        {            
            shieldWnd.Close();
        }
    }
    //------------------------------------------------------------------------------------------------------
    public void OnBlockRet(ProtocolMessage msg)//返回屏蔽
    {
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode errorCode = msg.GetProtocolBody<PB.HSErrorCode>();
            if (errorCode != null)
            {
                switch (errorCode.errCode)
                {
                    case (int)PB.settingError.SETTING_BLOCK_FULL:
                        uiHintMsg.Instance.HintShow(StaticDataMgr.Instance.GetTextByID("im_record_004"));
                        shieldWnd.Close();
                        break;
                }
            }
        }
        else
        {
            PB.HSSettingBlockRet result = msg.GetProtocolBody<PB.HSSettingBlockRet>();
            if (result.isBlock)
            {
                shieldWnd.Close();
                playerBox.SetActive(false);
                uiHintMsg.Instance.HintShow(StaticDataMgr.Instance.GetTextByID("im_record_003"));
                GameDataMgr.Instance.PlayerDataAttr.AddBlockPlayer(result.playerId);
            }
            else
            {
                GameDataMgr.Instance.PlayerDataAttr.RemoveBlockPlayer(result.playerId);
            }
        }
    }
    //------------------------------------------------------------------------------------------------------
    void OnEnable()
    {
        BindListener();
    }
    //------------------------------------------------------------------------------------------------------
    void OnDisable()
    {
        UnBindListener();
    }
    //------------------------------------------------------------------------------------------------------
    void BindListener()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.IM_PUSH_S.GetHashCode().ToString(), OnMsgReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.IM_PLAYER_GET_S.GetHashCode().ToString(), OnPlayerGetRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SETTING_BLOCK_S.GetHashCode().ToString(), OnBlockRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SETTING_BLOCK_C.GetHashCode().ToString(), OnBlockRet);
    }
    //------------------------------------------------------------------------------------------------------
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.IM_PUSH_S.GetHashCode().ToString(), OnMsgReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.IM_PLAYER_GET_S.GetHashCode().ToString(), OnPlayerGetRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SETTING_BLOCK_S.GetHashCode().ToString(), OnBlockRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SETTING_BLOCK_C.GetHashCode().ToString(), OnBlockRet);
    }
    //------------------------------------------------------------------------------------------------------
}
