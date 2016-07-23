using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public interface IMosaicEquipCallBack
{
    void OnSelectReturn(int selIndex);
    void OnMosaicReturn(int selIndex);
}


public class EquipInlayPanel : EquipPanelBase, IMosaicCallBack
{
    public Transform transEquipPos;
    private ItemIcon itemIcon;

    public Text textName;
    public Text text_Zhanli;
    public Text textZhanli;
    public Text text_Desc;

    public Image imgCoin;
    public Text textCoin;
    public Transform materialPos;
    private ItemIcon materialIcon;
    public Text textMaterial;

    public Text text_dont;
    public Text text_Tips;
    public Text text_Open;
    public Button btnOpen;
    public GameObject objOpen;
    public GameObject hidOpen;
    public GameObject dontOpen;

    public IMosaicEquipCallBack ICallBackDelegate { get { return ICallBack; } }


    private EquipProtoData curProto;
    private List<ItemInfo> curDemand = new List<ItemInfo>();

    private EquipData curData;
    private int selIndex = -1;
    private UIEquipInlay.State type;

    private bool isMosaic=false;
    private bool isOpenmax=false;

    public InlayGemItem[] mosaicItems = new InlayGemItem[BattleConst.maxGemCount];

    void Start()
    {
        text_Zhanli.text = StaticDataMgr.Instance.GetTextByID("equip_forge_zhanli");
        text_Desc.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_openmax");
        text_Desc.color = ColorConst.text_color_Req;
        text_Open.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_btnopen");
        text_dont.text = StaticDataMgr.Instance.GetTextByID("equip_gem_NotMent");
        for (int i = 0; i < mosaicItems.Length; i++)
        {
            mosaicItems[i].Index = i;
            mosaicItems[i].mosaicDelegate = this;
        }
        
    }

    //void ReloadData()
    //{
    //    ReloadData(curData, type, selIndex);
    //}

    public override void ReloadData(EquipData data, UIEquipInlay.State type, int select = -1)
    {
        curData = data; 
        this.type = type;
        this.selIndex = (select == -1 ? selIndex : select);
        hidOpen.SetActive(type == UIEquipInlay.State.PetUI);

        ItemStaticData itemData=StaticDataMgr.Instance.GetItemData(curData.equipId);
        UIUtil.SetStageColor(textName, itemData.name, curData.stage, curData.level);

        if (itemIcon==null)
        {
            itemIcon = ItemIcon.CreateItemIcon(curData);
            UIUtil.SetParentReset(itemIcon.transform, transEquipPos);
        }
        else
        {
            itemIcon.RefreshWithEquipInfo(curData);
        }

        if (curData.stage < BattleConst.minGemStage)
        {
            objOpen.SetActive(false);
            dontOpen.SetActive(true);
            return;
        }
        else
        {
            objOpen.SetActive(true);
            dontOpen.SetActive(false);
        }

        int canOpenCount=curData.stage - (BattleConst.minGemStage - 1);
        isMosaic = false;
        isOpenmax = (curData.gemList.Count >= canOpenCount);
        text_Desc.gameObject.SetActive(isOpenmax);
        text_Tips.text = string.Format(StaticDataMgr.Instance.GetTextByID("equip_inlay_tips"), canOpenCount);
        #region 设置宝石插槽的状态
        for (int i = 0; i < mosaicItems.Length; i++)
        {
            if (canOpenCount <= i)
            {
                mosaicItems[i].gameObject.SetActive(false);
                continue;
            }
            if (curData.gemList.Count>i)
            {
                mosaicItems[i].gameObject.SetActive(true);
                mosaicItems[i].Reload(curData.gemList[i]);
                if (!curData.gemList[i].gemId.Equals(BattleConst.invalidGemID))
                {
                    isMosaic = true;
                }
            }
            else
            {
                if (type == UIEquipInlay.State.Setting)
                {
                    mosaicItems[i].gameObject.SetActive(false);
                    continue;
                }
                else
                {
                    mosaicItems[i].Reload(null);
                }
            }
            if (ParentNode.uiType == UIEquipInlay.State.Setting)
            {
                mosaicItems[i].SetSelectState(i == selIndex);
            }
        }
        
        #endregion
        #region 消耗材料解析计算
        curProto = StaticDataMgr.Instance.GetEquipProtoData(curData.equipId, curData.stage);
        curDemand.Clear();
        curProto.GetPunchDemand(ref curDemand);
        if (UIUtil.CheckIsEnoughMaterial(curDemand))
        {
            btnOpen.interactable = true;
            EventTriggerListener.Get(btnOpen.gameObject).onClick = OnClickOpen;
        }
        else
        {
            btnOpen.interactable = false;
            EventTriggerListener.Get(btnOpen.gameObject).onClick = null;
        }


        for (int i = 0; i < curDemand.Count; i++)
        {
            if (curDemand[i].type == (int)PB.itemType.ITEM)
            {
                ItemData mineItem = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(curDemand[i].itemId);
                if (mineItem == null)
                {
                    mineItem = new ItemData() { itemId = curDemand[i].itemId, count = 0 };
                }
                if (mineItem.count < curDemand[i].count)
                {
                    textMaterial.color = ColorConst.text_color_nReq;
                }
                else
                {
                    textMaterial.color = ColorConst.text_color_Req;
                }
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
                textMaterial.text = mineItem.count + "/" + curDemand[i].count;
            }
            else if (curDemand[i].type == (int)PB.itemType.PLAYER_ATTR)
            {
                if (curDemand[i].itemId.Equals(((int)PB.playerAttr.COIN).ToString()))
                {
                    if (GameDataMgr.Instance.PlayerDataAttr.coin < curDemand[i].count)
                    {
                        textCoin.color = ColorConst.text_color_nReq;
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

    void OnPrompButtonClick(MsgBox.PrompButtonClick state)
    {
        if (state == MsgBox.PrompButtonClick.OK)
        {
            OnSendToOpen(curData);
        }
    }


    void OnClickOpen(GameObject go)
    {
        if (isMosaic||isOpenmax)
        {
            Logger.Log("已经最大数量了");
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel, StaticDataMgr.Instance.GetTextByID("equip_random_open") + "\n" + StaticDataMgr.Instance.GetTextByID("equip_return_gem"), OnPrompButtonClick);
            return;
        }
        else
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
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_PUNCH_C.GetHashCode().ToString(), OnEquipPunchRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_PUNCH_S.GetHashCode().ToString(), OnEquipPunchRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_GEM_C.GetHashCode().ToString(), OnEquipMosaicRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_GEM_S.GetHashCode().ToString(), OnEquipMosaicRet);

        //GameEventMgr.Instance.AddListener(GameEventList.ReloadEquipGemNotify, ReloadData);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_PUNCH_C.GetHashCode().ToString(), OnEquipPunchRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_PUNCH_S.GetHashCode().ToString(), OnEquipPunchRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_GEM_C.GetHashCode().ToString(), OnEquipMosaicRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_GEM_S.GetHashCode().ToString(), OnEquipMosaicRet);
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
        #endregion

        ICallBackDelegate.OnMosaicReturn(selIndex);

    }
    //卸载返回
    void OnEquipMosaicRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSEquipGemRet result = msg.GetProtocolBody<PB.HSEquipGemRet>();

        #region 同步服务器宝石
        curData.gemList.Clear();
        foreach (PB.GemPunch element in result.gemItems)
        {
            GemInfo gemInfo = new GemInfo(element);
            curData.gemList.Add(gemInfo);
        }
        #endregion

        ICallBackDelegate.OnMosaicReturn(selIndex);
    }

    //选中位置
    public void OnSelectItem(int selIndex)
    {
        if (ParentNode.uiType == UIEquipInlay.State.Setting)
        {
            if (this.selIndex == selIndex)
            {
                return;
            }
            for (int i = 0; i < mosaicItems.Length; i++)
            {
                mosaicItems[i].SetSelectState(i == selIndex);
            }
        }
        this.selIndex = selIndex;
        if (ICallBackDelegate!=null)
        {
            ICallBackDelegate.OnSelectReturn(selIndex);
        }
    }
    //卸载宝石
    public void OnClickMosaic(int selIndex)
    {
        PB.HSEquipGem param = new PB.HSEquipGem();
        param.id = curData.id;
        param.type = curData.gemList[selIndex].type;
        param.oldGem = curData.gemList[selIndex].gemId;
        GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_GEM_C.GetHashCode(), param);
    }
}
