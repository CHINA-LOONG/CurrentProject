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

    public Text text_Material;
    public Image imgCoin;
    public Text textCoin;
    public Image imgMaterial;
    public Text textMaterial;

    public Text text_Tips;
    public Text text_Open;
    public Button btnOpen;

    public IMosaicEquipCallBack ICallBackDelegate { get { return ICallBack; } }


    private EquipProtoData curProto;
    private List<ItemInfo> curDemand = new List<ItemInfo>();

    private EquipData curData;
    private int selIndex = -1;

    private bool isMosaic=false;
    private bool isOpenmax=false;

    public InlayGemItem[] mosaicItems = new InlayGemItem[4];

    void Start()
    {
        text_Zhanli.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_zhanli");
        text_Desc.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_openmax");
        text_Material.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_xiaohao");
        text_Open.text = StaticDataMgr.Instance.GetTextByID("equip_inlay_btnopen");

        EventTriggerListener.Get(btnOpen.gameObject).onClick = OnClickOpen;

        for (int i = 0; i < mosaicItems.Length; i++)
        {
            mosaicItems[i].Index = i;
            mosaicItems[i].mosaicDelegate = this;
        }
        
    }

    public override void ReloadData(EquipData data, int index = -1)
    {
        curData = data;
        this.selIndex = (index==-1?selIndex:index);

        if (itemIcon==null)
        {
            itemIcon = ItemIcon.CreateItemIcon(curData);
            UIUtil.SetParentReset(itemIcon.transform, transEquipPos);
        }
        else
        {
            itemIcon.RefreshWithEquipInfo(curData);
        }

        isMosaic = false;
        isOpenmax = true;
        for (int i = 0; i < mosaicItems.Length; i++)
        {
            mosaicItems[i].gameObject.SetActive((curData.stage - 3) >= i);
            if (curData.gemList.Count>i)
            {
                mosaicItems[i].Reload(curData.gemList[i]);
                if (!curData.gemList[i].gemId.Equals(BattleConst.invalidGemID))
                {
                    isMosaic = true;
                }
            }
            else
            {
                isOpenmax = false;
                mosaicItems[i].Reload(null);
            }
            if (ParentNode.uiType == UIEquipInlay.UIType.Left)
            {
                mosaicItems[i].SetSelectState(i == selIndex);
            }
        }

        if (curData.stage<3)
        {
            Logger.Log("不能打孔");
        }
        else
        {
            text_Tips.text = string.Format(StaticDataMgr.Instance.GetTextByID("equip_inlay_tips"), curData.stage - 2);
        }
        #region 消耗材料解析计算
        curProto = StaticDataMgr.Instance.GetEquipProtoData(curData.equipId, curData.stage);
        curProto.GetPunchDemand(ref curDemand);
        for (int i = 0; i < curDemand.Count; i++)
        {
            if (curDemand[i].type == (int)PB.itemType.ITEM)
            {
                ItemData mineItem = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(curDemand[i].itemId);
                if (mineItem==null)
                {
                    mineItem = new ItemData() { itemId = curDemand[i].itemId, count = 0 };
                }
                if (mineItem.count < curDemand[i].count)
                {
                    textMaterial.color = Color.red;
                }
                else
                {
                    textMaterial.color = Color.white;
                }
                //TODO:set icon
                imgMaterial.sprite = null;
                textMaterial.text = mineItem.count + "/" + curDemand[i].count;
            }
            else if (curDemand[i].type == (int)PB.itemType.PLAYER_ATTR)
            {
                if (curDemand[i].itemId.Equals(((int)PB.playerAttr.COIN).ToString()))
                {
                    if (GameDataMgr.Instance.PlayerDataAttr.coin < curDemand[i].count)
                    {
                        textCoin.color = Color.red;
                    }
                    else
                    {
                        textCoin.color = Color.white;
                    }
                    //TODO:set icon
                    imgCoin.sprite = null;
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

    void OnClickOpen(GameObject go)
    {
        if (isMosaic||isOpenmax)
        {
            Logger.Log("已经最大数量了");
            OnSendToOpen(curData);
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
        if (ParentNode.uiType == UIEquipInlay.UIType.Left)
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
