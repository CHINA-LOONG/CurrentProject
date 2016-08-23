using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class EquipEmbed : EquipInfoBase,
                          IGemSlotItem
{
    public Text text_Gems;
    public Text text_Tips1;     //开孔已满
    public Text text_Tips2;     //开孔数量
    public Text textNotSlot;     //不能开孔

    public GameObject objSocket;

    public Transform GemsPos;

    public Image imgCoin;
    public Text textCoin;
    public Transform materialPos;
    private ItemIcon materialIcon;
    public Text textMaterial;

    public Button btnSocket;
    public Text textSocket;

    private GemSlotItem[] mosaicItems = new GemSlotItem[BattleConst.maxGemCount];
    public EquipData curData;

    private EquipForgeData curForge;
    private List<ItemInfo> curDemand = new List<ItemInfo>();

    private bool isMosaic = false;
    private bool isOpenmax = false;
    private bool enoughCoin;
    private bool enoughItem;
    void Start()
    {
        text_Gems.text = StaticDataMgr.Instance.GetTextByID("equip_smalltitle");
        textSocket.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_btnopen");
        text_Tips1.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_openmax");
        textNotSlot.text = StaticDataMgr.Instance.GetTextByID("equip_gem_NotMent");

        btnSocket.onClick.AddListener(OnClickSocketBtn);
    }
    void RefreshNotify(EquipData equipData)
    {
        if (curData==equipData)
        {
            ReloadData(curData);
        }
    }

    public override void ReloadData(EquipData data)
    {
        curData = data;

        #region 设置装备宝石列表

        int canOpenCount = curData.stage - (BattleConst.minGemStage - 1);

        isMosaic = false;
        isOpenmax = (curData.gemList.Count >= canOpenCount);
        bool canSocket = (canOpenCount > 0);
        text_Tips1.gameObject.SetActive(isOpenmax && canSocket);
        textNotSlot.gameObject.SetActive(!canSocket);
        objSocket.gameObject.SetActive(canSocket);
        //下面的要集中处理与textNotSlot的关系
        text_Tips2.text = string.Format(StaticDataMgr.Instance.GetTextByID("equip_inlay_tips"), canOpenCount);
        Action<int> SetGemSlot = (index) =>
        {
            if (mosaicItems[index] != null)
            {
                mosaicItems[index].gameObject.SetActive(true);
            }
            else
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("GemSlotItem");
                UIUtil.SetParentReset(go.transform, GemsPos);
                mosaicItems[index] = go.GetComponent<GemSlotItem>();
                mosaicItems[index].IGemSlotItemDelegate = this;
            }
        };
        for (int i = 0; i < mosaicItems.Length; i++)
        {
            if (canOpenCount <= i)
            {
                if (mosaicItems[i] != null)
                {
                    mosaicItems[i].gameObject.SetActive(false);
                }
                continue;
            }
            SetGemSlot(i);
            if (curData.gemList.Count > i)
            {
                mosaicItems[i].ReloadData(curData.gemList[i],true);
                if (!curData.gemList[i].gemId.Equals(BattleConst.invalidGemID))
                {
                    isMosaic = true;
                }
            }
            else
            {
                mosaicItems[i].ReloadData(null);
            }
        }
        #endregion

        #region 消耗材料解析计算
        
        curForge = StaticDataMgr.Instance.GetEquipForgeData(curData.stage, curData.level);
        curDemand.Clear();
        curForge.GetPunchDemand(ref curDemand);

        btnSocket.interactable = UIUtil.CheckIsEnoughMaterial(curDemand);

        enoughCoin = true;
        enoughItem = true;
        for (int i = 0; i < curDemand.Count; i++)
        {
            if (curDemand[i].type==(int)PB.itemType.ITEM)
            {
                ItemData mineItem = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(curDemand[i].itemId);
                if (mineItem == null)
                {
                    mineItem = new ItemData() { itemId = curDemand[i].itemId, count = 0 };
                }
                mineItem.count = Mathf.Clamp(mineItem.count, 0, 9999);
                ItemData material = new ItemData() { itemId = curDemand[i].itemId, count = 0 };
                if (materialIcon == null)
                {
                    materialIcon = ItemIcon.CreateItemIcon(material);
                    UIUtil.SetParentReset(materialIcon.transform, materialPos);
                }
                else
                {
                    materialIcon.RefreshWithItemInfo(material);
                }
                Color color;
                if (mineItem.count < curDemand[i].count)
                {
                    color = ColorConst.text_color_nReq;
                    enoughItem = false;
                }
                else
                {
                    color = ColorConst.text_color_Req;
                }
                textMaterial.text = "<color=" + ColorConst.colorTo_Hstr(color) + ">" + mineItem.count + "</color>/" + curDemand[i].count;
            }
            else if (curDemand[i].type == (int)PB.itemType.PLAYER_ATTR)
            {
                if (curDemand[i].itemId.Equals(((int)PB.changeType.CHANGE_COIN).ToString()))
                {
                    if (GameDataMgr.Instance.PlayerDataAttr.coin < curDemand[i].count)
                    {
                        textCoin.color = ColorConst.text_color_nReq;
                        enoughCoin = false;
                    }
                    else
                    {
                        textCoin.color = ColorConst.text_color_Req;
                    }
                    imgCoin.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(BattleConst.icon_jinbi);
                    textCoin.text = curDemand[i].count.ToString();
                }
                else
                {
                    Logger.LogError("qing shi yong jin bi!!");
                }
            }
            else
            {
                Logger.LogError("配置出现错误");
            }
        }
        #endregion
    }

    void OnClickSocketBtn()
    {
        if (!enoughItem||!enoughCoin)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("monster_record_004"), (int)PB.ImType.PROMPT);
            return;
        }

        if (isMosaic || isOpenmax)
        {
            Logger.Log("已经最大数量了");
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel,
                                StaticDataMgr.Instance.GetTextByID("equip_random_open"),
                                StaticDataMgr.Instance.GetTextByID("equip_return_gem"),
                                OnPrompCallBack);
            return;
        }
        else
        {
            OnSendToOpen(curData);
        }
    }
    void OnPrompCallBack(MsgBox.PrompButtonClick state)
    {
        if (state == MsgBox.PrompButtonClick.OK)
        {
            OnSendToOpen(curData);
        }
    }

    void OnSendToOpen(EquipData equip)
    {
        PB.HSEquipPunch param = new PB.HSEquipPunch();
        param.id = equip.id;
        GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_PUNCH_C.GetHashCode(), param);
    }

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
        GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadEquipSocketNotify, RefreshNotify);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_PUNCH_C.GetHashCode().ToString(), OnEquipPunchRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_PUNCH_S.GetHashCode().ToString(), OnEquipPunchRet);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<EquipData>(GameEventList.ReloadEquipSocketNotify, RefreshNotify);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_PUNCH_C.GetHashCode().ToString(), OnEquipPunchRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_PUNCH_S.GetHashCode().ToString(), OnEquipPunchRet);
    }

    //开孔返回
    void OnEquipPunchRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSEquipPunchRet result = msg.GetProtocolBody<PB.HSEquipPunchRet>();

        #region 同步服务器宝石
        curData.gemList.Clear();
        foreach (PB.GemPunch element in result.gemItems)
        {
            GemInfo gemInfo = new GemInfo(element);
            curData.gemList.Add(gemInfo);
        }
        curData.RefreshGemAttr();
        #endregion

        GameEventMgr.Instance.FireEvent<EquipData>(GameEventList.ReloadEquipSocketNotify, curData);
    }
    
    public void OnClickGemSlot(GemInfo info)
    {
        UISelectGemList uiSelectGemList = UIMgr.Instance.OpenUI_(UISelectGemList.ViewName) as UISelectGemList;
        uiSelectGemList.ReloadData(curData, info);
    }
}
