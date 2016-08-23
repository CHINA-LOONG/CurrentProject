using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UISelectGemList : UIBase,
                               IScrollView,
                               IGemListItem
{
    public static string ViewName = "UISelectGemList";

    public Text textTitle;
    public Text textNotfound;
    public Button btnClose;
    public FixCountScrollView scrollView;

    private EquipData curData;
    private GemInfo curInfo;
    private bool isEmbed;
    private int slotIndex;

    private List<GemListItemInfo> infos = new List<GemListItemInfo>();

    void Start()
    {
        textTitle.text = StaticDataMgr.Instance.GetTextByID("gem_title");
        textNotfound.text = StaticDataMgr.Instance.GetTextByID("list_empty");
        btnClose.onClick.AddListener(OnClickCloseBtn);
    }

    public void ReloadData(EquipData data,GemInfo info)
    {
        curData = data;
        curInfo = info;
        if (info==null)
        {
            Logger.Log("为开孔时不可点击,出现bug");
            return;
        }
        slotIndex = curData.gemList.IndexOf(curInfo);
        isEmbed = !curInfo.gemId.Equals(BattleConst.invalidGemID);

        Dictionary<string, ItemData> itemList = GameDataMgr.Instance.PlayerDataAttr.gameItemData.itemList;
        infos.Clear();
        ItemStaticData itemStatic;
        if (isEmbed)
        {
            itemStatic = StaticDataMgr.Instance.GetItemData(curInfo.gemId);
            infos.Add(
                new GemListItemInfo()
                {
                    itemData = new ItemData()
                    {
                        itemId = curInfo.gemId,
                        count = 1
                    },
                    staticData = itemStatic,
                    type = GemListItemInfo.Type.Remove
                });
        }
        GemListItemInfo.Type tempType = isEmbed ? GemListItemInfo.Type.Change : GemListItemInfo.Type.Embed;
        foreach (var item in itemList)
        {
            itemStatic = StaticDataMgr.Instance.GetItemData(item.Value.itemId);
            if (itemStatic == null)
            {
                Logger.LogError("缺少物品配置:" + item.Value.itemId);
                continue;
            }
            if (itemStatic.type == 3 && itemStatic.gemType == info.type)
            {
                infos.Add(
                    new GemListItemInfo()
                    {
                        itemData = item.Value,
                        staticData = itemStatic,
                        type = tempType
                    });
            }
        }
        textNotfound.gameObject.SetActive(infos.Count <= 0);

        scrollView.InitContentSize(infos.Count, this);
    }
    
    public void OnClickCloseBtn()
    {
        UIMgr.Instance.DestroyUI(this);
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
        curData.RefreshGemAttr();
        #endregion

        GameEventMgr.Instance.FireEvent<EquipData>(GameEventList.ReloadEquipEmbedNotify, curData);
        
        UIMgr.Instance.DestroyUI(this);
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
    
    #region ScrollView
    public void ReloadData(Transform item, int index)
    {
        GemListItem gem = item.GetComponent<GemListItem>();
        gem.OnReload(infos[index]);
    }

    public Transform CreateData(Transform parent, int index = 0)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("GemListItem");
        if (go != null)
        {
            UIUtil.SetParentReset(go.transform, parent);
            GemListItem item = go.GetComponent<GemListItem>();
            item.IGemListItemDelegate = this;
            return go.transform;
        }
        return null;
    }

    public void CleanData(List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
        itemList.Clear();
    }

    #endregion

    #region IGemListItem

    public void OnRemoveReturn(ItemData data)
    {
        if (!string.Equals(data.itemId, curData.gemList[slotIndex].gemId))
        {
            Logger.LogError("卸载宝石出错");
            return;
        }
        PB.HSEquipGem param = new PB.HSEquipGem();
        param.id = curData.id;
        param.slot = slotIndex;
        param.type = curData.gemList[slotIndex].type;
        param.oldGem = curData.gemList[slotIndex].gemId;
        GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_GEM_C.GetHashCode(), param);
    }

    public void OnEmbedReturn(ItemData data)
    {
        if (!curData.gemList[slotIndex].gemId.Equals(BattleConst.invalidGemID))
        {
            Logger.LogError("镶嵌宝石错误");
            return;
        }
        PB.HSEquipGem param = new PB.HSEquipGem();
        param.id = curData.id;
        param.slot = slotIndex;
        param.type = curInfo.type;
        param.newGem = data.itemId;
        GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_GEM_C.GetHashCode(), param);
    }

    public void OnChangeReturn(ItemData data)
    {
        if (curData.gemList[slotIndex].gemId.Equals(BattleConst.invalidGemID))
        {
            Logger.LogError("镶嵌宝石错误");
            return;
        }
        PB.HSEquipGem param = new PB.HSEquipGem();
        param.id = curData.id;
        param.slot = slotIndex;
        param.type = curInfo.type;
        param.newGem = data.itemId;
        param.oldGem = curData.gemList[slotIndex].gemId;
        GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_GEM_C.GetHashCode(), param);
    }
    #endregion
}
