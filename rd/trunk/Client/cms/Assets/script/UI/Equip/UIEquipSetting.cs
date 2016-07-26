using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIEquipSetting : UIBase, IGemListCallBack, IEquipInlayCallBack
{
    public static string ViewName = "UIEquipSetting";

    public Transform transLeftPos;
    public Transform transRightPos;

    public Button btnClose;

    private UIEquipInlay uiEquipInlay=null;
    private UIGemList uiGemList=null;

    private EquipData curEquip;
    private int curTabIndex;
    private int curSelIndex;

    void Start()
    {
        EventTriggerListener.Get(btnClose.gameObject).onClick = OnClickClose;
    }

    public void Refresh(EquipData data,int tabIndex,int selIndex)
    {
        curEquip = data;
        curTabIndex = tabIndex;
        curSelIndex = (selIndex == -1 ? curSelIndex : selIndex);

        if (uiEquipInlay==null)
        {
            uiEquipInlay = UIEquipInlay.CreateEquipInlay();
            uiEquipInlay.ICallBackDelegate = this;
            UIUtil.SetParentReset(uiEquipInlay.transform, transLeftPos);
        }
        else
        {
            uiEquipInlay.gameObject.SetActive(true);
        }
        uiEquipInlay.Refresh(curEquip, curTabIndex, curSelIndex,UIEquipInlay.State.Setting);

        GemInfo gemInfo = curEquip.gemList[selIndex];

        if (uiGemList==null)
        {
            uiGemList = UIGemList.CreateEquipInlay();
            uiGemList.ICallBackDeletage = this;
            UIUtil.SetParentReset(uiGemList.transform, transRightPos);
        }
        else
        {
            uiGemList.gameObject.SetActive(true);
        }
        uiGemList.Refresh(gemInfo.type);
    }

    void OnClickClose(GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }

    //镶嵌宝石返回
    void OnEquipMosaicRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg == null || msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }
        PB.HSEquipGemRet result = msg.GetProtocolBody<PB.HSEquipGemRet>();

        #region 同步服务器宝石
        curEquip.gemList.Clear();
        foreach (PB.GemPunch element in result.gemItems)
        {
            GemInfo gemInfo = new GemInfo(element);
            curEquip.gemList.Add(gemInfo);
        }
        #endregion

        uiEquipInlay.Refresh(curEquip, curTabIndex, curSelIndex,UIEquipInlay.State.Setting);
        uiGemList.Refresh(curEquip.gemList[curSelIndex].type);
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
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_GEM_C.GetHashCode().ToString(), OnEquipMosaicRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_GEM_S.GetHashCode().ToString(), OnEquipMosaicRet);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_GEM_C.GetHashCode().ToString(), OnEquipMosaicRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_GEM_S.GetHashCode().ToString(), OnEquipMosaicRet);
    }

    public void OnMosaicReturn(ItemData data)
    {
        PB.HSEquipGem param = new PB.HSEquipGem();
        param.id = curEquip.id;
        param.slot = curSelIndex;
        param.type = curEquip.gemList[curSelIndex].type;
        param.newGem = data.itemId;
        if (!curEquip.gemList[curSelIndex].gemId.Equals(BattleConst.invalidGemID))
        {
            param.oldGem = curEquip.gemList[curSelIndex].gemId;
        }
        GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_GEM_C.GetHashCode(), param);
    }

    public void OnSelectReturn(int tabIndex, int selIndex)
    {
        Refresh(curEquip, tabIndex, selIndex);
    }

    public void OnMosaicReturn(int tabIndex, int selIndex)
    {
        Refresh(curEquip, tabIndex, selIndex);
    }
}
