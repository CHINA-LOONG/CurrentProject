using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
public class UIIm : UIBase
{
    public static string ViewName = "uishowMsg";
    List<PB.HSImMsg> worldChannel = new List<PB.HSImMsg>();//世界頻道列表
    List<PB.HSImMsg> gangChannel = new List<PB.HSImMsg>();//工會頻道列表 
    PB.HSImMsg allMsg;//基础聊天框
    List<GameObject> hintMsg = new List<GameObject>();//系统提示
    int Channel = (int)PB.ImChannel.WORLD;//頻道
    public InputField msgText; 
    public GameObject showLeftChatBox;
    public GameObject showBasicsChat;
    public GameObject globalButton;
    public Image globalBackground;
    public GameObject guildButton;
    public Image guildBackground;
    public GameObject leftChatBox;
    public GameObject basicsChat;
    public GameObject imMsgFather;
    public RectTransform msgBox;
    public GameObject sendButton;
    public Text basicsValue;
    public GameObject msgPos;//位置
    List<GameObject> msgObj = new List<GameObject>();//消息gameobj
    bool showNewMsg;//新消息
    //玩家信息
    public GameObject playerBox;
    public GameObject playerBoxClone;
    public Text playerName;
    public GameObject shield;//屏蔽
    int blockID;//屏蔽ID
    public GameObject noticeUI;//系统公告
    Lantern noticeMove;
    public GameObject lanternUI;//走马灯
    Lantern lanternMove;//走马灯文本
    ImMessageData imMsgData;
    static UIIm mInst = null;
    public static UIIm Instance
    {
        get
        {
            return mInst;
        }
    }
    //------------------------------------------------------------------------------------------------------
	// Use this for initialization
    void Start()
    {
        mInst = this;
        EventTriggerListener.Get(showLeftChatBox).onClick = MsgOnClick;
        EventTriggerListener.Get(showBasicsChat).onClick = MsgOnClick;
        EventTriggerListener.Get(globalButton).onClick = MsgOnClick;
        EventTriggerListener.Get(guildButton).onClick = MsgOnClick;
        EventTriggerListener.Get(sendButton).onClick = SendClick;
        EventTriggerListener.Get(playerBoxClone).onClick = MsgOnClick;
        EventTriggerListener.Get(shield).onClick = SendClick;
        GameObject player;
        for (int i = 0; i < BattleConst.maxMsg; i++)
        {
            GameObject imMessage = ResourceMgr.Instance.LoadAsset("imMessage");
            imMessage.transform.parent = imMsgFather.transform;
            imMessage.transform.localScale = imMsgFather.transform.localScale;
            msgObj.Add(imMessage);
            player = imMessage.transform.FindChild("channelText").gameObject;
            player = player.transform.FindChild("nameText").gameObject;
            EventTriggerListener.Get(player).onClick = PlayerClick;
            imMessage.SetActive(false);
        }
        noticeMove = noticeUI.GetComponent<Lantern>();
        lanternMove = lanternUI.GetComponent<Lantern>();
        leftChatBox.SetActive(false);
        playerBox.SetActive(false);
        noticeUI.SetActive(false);
        lanternUI.SetActive(false);
    }
    //------------------------------------------------------------------------------------------------------
    void MsgOnClick(GameObject but)//通用button点击事件
    {
        if (but.name == showLeftChatBox.name)//show左侧聊天框
        {
            basicsChat.SetActive(false);
            leftChatBox.SetActive(true);
        }
        else if (but.name == showBasicsChat.name)//show基础聊天框
        {
            leftChatBox.SetActive(false);
            basicsChat.SetActive(true);
            ShowMessage();
        }
        else if (but.name == globalButton.name)//世界频道
        {
            Channel = (int)PB.ImChannel.WORLD;
            globalBackground.enabled = true;
            guildBackground.enabled = false;
            ShowMessage();
        }
        else if (but.name == guildButton.name)//工会频道
        {
            Channel = (int)PB.ImChannel.GUILD;
            guildBackground.enabled = true;
            globalBackground.enabled = false;
            ShowMessage();
        }
        else if (but ==playerBoxClone)
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
            playerBox.SetActive(true);
            playerName.text = imMsgData.playerName;
            blockID = imMsgData.speakerID;
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
                noticeMove.AddMsg(item.origText);
                worldChannel.Add(item);
                noticeMove.moveTypeEnd = false;
                //所有消息
                allMsg = item;
                continue;
            }
            ShowSystemHints(item.origText,item.type);
            //频道相关
            if (item.channel == (int)PB.ImChannel.WORLD && item.type != (int)PB.ImType.LANTERN && item.type != (int)PB.ImType.PROMPT)
            {
                worldChannel.Add(item);
                allMsg = item;
            }
            if (item.channel == (int)PB.ImChannel.GUILD)
            {
                gangChannel.Add(item);
                allMsg = item;
            }
        }
        ShowMessage();
    }
    //------------------------------------------------------------------------------------------------------
    void ShowMessage()//显示消息
    {
        if (leftChatBox.activeSelf)
        {
            if (Channel == (int)PB.ImChannel.WORLD)
            {
                ReadMsg(ref worldChannel, "Global", ColorConst.globalColor);
            }
            else if (Channel == (int)PB.ImChannel.GUILD)
            {
                ReadMsg(ref gangChannel, "Guild", ColorConst.guildColor);
            }
        }
        else if(worldChannel.Count > 0)
        {
            string msg = null;
            if (allMsg.channel == (int)PB.ImChannel.WORLD)
            {
                if (allMsg.type == (int)PB.ImType.NOTICE)
                    msg = "<color=" + ColorConst.colorTo_Hstr(ColorConst.systemColor) + ">[System] " + allMsg.origText + "</color>";
                else
                    msg = "<color=" + ColorConst.colorTo_Hstr(ColorConst.globalColor) + ">[Global] </color>" +
                        allMsg.senderName + ": <color=" + ColorConst.colorTo_Hstr(ColorConst.globalColor) + ">" + allMsg.origText + "</color>";
            }
            else
            {
                msg = "<color=" + ColorConst.colorTo_Hstr(ColorConst.guildColor) + ">[Guild] </color>" +
                       allMsg.senderName + ": <color=" + ColorConst.colorTo_Hstr(ColorConst.guildColor) + ">" + allMsg.origText + "</color>";
            }
            basicsValue.text = msg;
        }
    }
    //------------------------------------------------------------------------------------------------------#7986FE0100FF
    void ReadMsg(ref List<PB.HSImMsg> msgList, string channelName, Color msgColor)//读取赋值消息
    {
        float msgHeight;//一条消息的高
        float newMsgPos;//新的消息的位置
        int overCount = msgList.Count - BattleConst.maxMsg;
        for (int j = 0; j < overCount; ++j)
        {
            msgList.RemoveAt(0);
        }
        int i = 0;
        for (; i < msgList.Count; i++)
        {
            msgObj[i].transform.localPosition = msgPos.transform.localPosition;
            imMsgData = msgObj[i].gameObject.GetComponent<ImMessageData>();     
            if (i != 0)
            {
                msgHeight = msgObj[i - 1].transform.FindChild("channelText").transform.FindChild("nameText").
                    transform.FindChild("msgText").GetComponent<Text>().rectTransform.sizeDelta.y;
                msgHeight = imMsgData.mChannel.rectTransform.sizeDelta.y + msgHeight;
                newMsgPos = msgObj[i - 1].transform.localPosition.y - msgHeight;
                msgObj[i].transform.localPosition = new Vector3(msgPos.transform.localPosition.x, newMsgPos, 0);                   
            }
            Color textColor;
            msgObj[i].SetActive(true);
            if (msgList[i].type == (int)PB.ImType.NOTICE)
            {
                textColor = ColorConst.systemColor;
                imMsgData.mChannel.text = "[" + "System" + "]";
            }
            else
            {
                imMsgData.mSpeaker.text = msgList[i].senderName + ":";
                imMsgData.mChannel.text = "[" + channelName + "]";
                textColor = msgColor;
            }
            imMsgData.playerName = msgList[i].senderName;
            imMsgData.mContent.text = msgList[i].origText;
            imMsgData.mContent.color = textColor;
            imMsgData.speakerID = msgList[i].senderId;             
            imMsgData.mChannel.color = textColor;
            showNewMsg = true;
        }
        for (; i < msgObj.Count; ++i)
        {
            msgObj[i].SetActive(false);
        }
    }
    //------------------------------------------------------------------------------------------------------
    public void ShowSystemHints(string hintsValue,int hintsType)//
    {
        //走马灯
        if (hintsType == (int)PB.ImType.LANTERN)
        {
            lanternMove.AddMsg(hintsValue);
            lanternMove.moveTypeEnd = true;
        }
        //系统公告
        if (hintsType == (int)PB.ImType.NOTICE)
        {
            noticeMove.AddMsg(hintsValue);
            noticeMove.moveTypeEnd = false;
        }
        //系统提示
        if (hintsType == (int)PB.ImType.PROMPT)
        {
            HintShow(hintsValue);
        }
    }
    //------------------------------------------------------------------------------------------------------
    public void HintShow(string hintText)//系统提示
    {
        GameObject hintBox = ResourceMgr.Instance.LoadAsset("hintMessage");
        hintBox.transform.parent = gameObject.transform;
        hintBox.transform.localScale = gameObject.transform.localScale;
        Hint hintComponent = hintBox.GetComponent<Hint>();
        hintComponent.ownedList = hintMsg;
        GameObject hint = hintBox.transform.FindChild("Image").gameObject;
        hint.SetActive(false);
        hint.transform.FindChild("Text").GetComponent<Text>().text = hintText;
        hintComponent.showTime = Time.time;
        hintComponent.SetFadeTime(hintComponent.stopTime + Time.time);
        if (hintMsg.Count == 1)
        {
            Hint lastHit = hintMsg[hintMsg.Count - 1].GetComponent<Hint>();
            lastHit.SetFadeTime(Time.time);
            hintComponent.SetFadeTime(hintComponent.stopTime + lastHit.endTime);
            hintComponent.showTime = lastHit.endTime;
        }
        else if (hintMsg.Count > 1)
        {
            Hint lastHit = hintMsg[hintMsg.Count - 1].GetComponent<Hint>();
            lastHit.SetFadeTime(lastHit.fadeTime - lastHit.stopTime);
            //lastHit.SetFadeTime(Time.time);
            hintComponent.SetFadeTime(hintComponent.stopTime + lastHit.endTime);
            hintComponent.showTime = lastHit.endTime;
        }
        hintMsg.Add(hintBox);
    }
    //------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (showNewMsg)
        {
            float sizeMsg = 0; Text channelText; Text msgText;
            for (int i = 0; i < worldChannel.Count; i++)
            {
                channelText = msgObj[i].transform.FindChild("channelText").GetComponent<Text>();
                msgText = channelText.transform.FindChild("nameText").transform.FindChild("msgText").GetComponent<Text>();
                if (msgText.rectTransform.sizeDelta.y != 0)
                {
                    sizeMsg += (channelText.rectTransform.sizeDelta.y + msgText.rectTransform.sizeDelta.y);
                    if (sizeMsg > msgBox.sizeDelta.y)
                    {
                        msgBox.sizeDelta = new Vector2(msgBox.sizeDelta.x, sizeMsg);
                        //if (msgBox.transform.position.y != 0)
                        //{
                        //    msgBox.transform.localPosition = new Vector3(0, 0, 0);
                        //}                        
                        showNewMsg = false;
                    }
                }
            }
        }
        lanternMove.UpdateLantern();
        noticeMove.UpdateLantern();
    }
    //------------------------------------------------------------------------------------------------------   
    void SendInterval() //发言间隔
    {
        sendButton.GetComponent<Button>().enabled = true;
    }
    //------------------------------------------------------------------------------------------------------
    void SendClick(GameObject but)//发送协议
    {
        if (but.name == sendButton.name)
        {
            if (msgText.text == string.Empty || msgText.text == null)
                return;
            else
            {
                OnSendMsg(msgText.text);
                Invoke("SendInterval", 2.0f);
            }
        }
        else if (but.name == shield.name)
        {
            OnShield();
        }
    }
    //------------------------------------------------------------------------------------------------------
    public void OnSendMsg(string message)//发送消息
    {
        PB.HSImChatSend param = new PB.HSImChatSend()
        {
            channel = Channel,
            text = message
        };
        GameApp.Instance.netManager.SendMessage(PB.code.IM_CHAT_SEND_C.GetHashCode(), param, false);
        msgText.text = "";
        sendButton.GetComponent<Button>().enabled = false;
    }
    //------------------------------------------------------------------------------------------------------
    public void OnShield()//屏蔽禁言
    {
        PB.HSSettingBlock param = new PB.HSSettingBlock()
        {
            playerId = blockID,
            isBlock = true
        };
        GameApp.Instance.netManager.SendMessage(PB.code.SETTING_BLOCK_C.GetHashCode(), param);
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
    }
    //------------------------------------------------------------------------------------------------------
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.IM_PUSH_S.GetHashCode().ToString(), OnMsgReturn);
    }
    //------------------------------------------------------------------------------------------------------
}
