using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UIDecompose : UIBase, TabButtonDelegate,IScrollView
{
    public static string ViewName = "UIDecompose";

    public Text text_Title;
    public Button btnClose;

    public Text text_Tab1;
    public Text text_Tab2;
    
    [Serializable]
    public class DecomposeView
    {
        public Button btnDecomposeChoice;    //合成一次
        public Button btnDecompose;    //合成十次

        public bool ShowMenu
        {
            get { return menuObj.activeInHierarchy; }
            set { menuObj.SetActive(value); }
        }
        public GameObject menuObj;
        public Button btnHidemenu;
        public Button btnWhite;
        public Button btnGreen;
        public Button btnBlue;
        public Text text_White;
        public Text text_Green;
        public Text text_Blue;

        public Text text_DecomposeChoice;
        public Text text_Decompose;

        public Text text_Preview;   //分解预览
        public Text text_PreviewTips;
        public Text text_Tips1;
        //public Text text_Tips2;

        public Transform iconPos;
        [HideInInspector]
        public ItemIcon itemIcon;
        [HideInInspector]
        public MonsterIcon monsterIcon;

        [HideInInspector]
        public long firstItem = 0;
        [HideInInspector]
        public List<long> selectItems = new List<long>();
        public List<EquipData> UnloadEquip = new List<EquipData>();

        public Transform content;
        [HideInInspector]
        public changeTypeIcon coinIcon;
        public changeTypeIcon GetElement(PB.changeType type, int count, bool isRemove)
        {
            if (coinIcon!=null)
            {
                if (coinIcon.type != type)
                {
                    Logger.LogError("分解配置设置错误");
                    return null;
                }
                else
                {
                    int coinCount;
                    coinCount = isRemove ? coinIcon.count - count : coinIcon.count + count;
                    if (coinCount <= 0)
                    {
                        RemoveElement(coinIcon);
                        return null;
                    }
                    else
                    {
                        coinIcon.gameObject.SetActive(true);
                        coinIcon.RefreshWithInfo(type, coinCount);
                    }
                }
            }
            else
            {
                coinIcon = changeTypeIcon.CreateIcon(type, count);
                UIUtil.SetParentReset(coinIcon.transform, content);
            }
            coinIcon.transform.SetAsLastSibling();
            return coinIcon;
        }
        public void RemoveElement(changeTypeIcon item)
        {
            if (item!=null)
            {
                item.RefreshWithInfo(item.type, 0);
                item.gameObject.SetActive(false);
            }
        }

        [HideInInspector]
        public Dictionary<string, ItemIcon> items = new Dictionary<string, ItemIcon>();
        [HideInInspector]
        public List<ItemIcon> itemPool = new List<ItemIcon>();

        public ItemIcon GetElement(ItemData data,bool isRemove)
        {
            ItemData tempInfo;
            bool isFind=false;
            foreach (var icon in items)
            {
                tempInfo=icon.Value.ItemInfo;
                if (tempInfo.itemId==data.itemId)
                {
                    isFind = true;
                    break;
                }
            }
            if (isFind)
            {
                tempInfo = items[data.itemId].ItemInfo;
                if (isRemove)
                {
                    if ((tempInfo.count - data.count) == 0)
                    {
                        RemoveElement(items[data.itemId]);
                        return null;
                    }
                    else
                    {
                        tempInfo.count -= data.count;
                    }
                }
                else
                {
                    tempInfo.count += data.count;
                }
                items[data.itemId].RefreshWithItemInfo(new ItemData() { itemId = tempInfo.itemId, count = tempInfo.count });
                return items[data.itemId];
            }
            else
            {
                ItemIcon item = null;
                if (itemPool.Count <= 0)
                {
                    item = ItemIcon.CreateItemIcon(data);
                    UIUtil.SetParentReset(item.transform, content);
                }
                else
                {
                    item = itemPool[itemPool.Count - 1];
                    item.gameObject.SetActive(true);
                    itemPool.Remove(item);
                    item.RefreshWithItemInfo(data);
                }
                item.transform.SetAsLastSibling();
                items.Add(data.itemId, item);
                return item;
            }
        }
        public void RemoveElement(ItemIcon item)
        {
            item.gameObject.SetActive(false);
            itemPool.Add(item);
            if (items.ContainsValue(item))
            {
                items.Remove(item.ItemInfo.itemId);
            }
        }
        public void RemoveAllElement()
        {
            foreach (var item in items)
            {
                item.Value.gameObject.SetActive(false);
                itemPool.Add(item.Value);
            }
            items.Clear();

        }

    }

    [Serializable]
    public class DecomposeList
    {
        public Text text_Material;
        public FixCountScrollView scrollView;
        
        public List<DecomposeItemInfo> itemsInfo = new List<DecomposeItemInfo>();
    }

    public DecomposeView decomposeView;
    public DecomposeList decomposeList;
    
    private int tabIndex = -1;
    private int selIndex = 0;
    public enum type
    {
        Equipment,
        Monsters
    }
    public type Type
    {
        get
        {
            return (type)tabIndex;
        }
    }

    private TabButtonGroup tabGroup;
    public TabButtonGroup TabGroup
    {
        get
        {
            if (tabGroup == null)
            {
                tabGroup = GetComponentInChildren<TabButtonGroup>();
                tabGroup.InitWithDelegate(this);
            }
            return tabGroup;
        }
    }

    public List<EquipData> equipInfos = new List<EquipData>();
    public List<GameUnit> monsterInfos = new List<GameUnit>();
    void Start()
    {
        EventTriggerListener.Get(btnClose.gameObject).onClick = OnClickCloseBtn;
        text_Title.text = StaticDataMgr.Instance.GetTextByID("decompose_title");
        text_Tab1.text = StaticDataMgr.Instance.GetTextByID("decompose_equip");
        text_Tab2.text = StaticDataMgr.Instance.GetTextByID("decompose_monster");

        decomposeView.text_Preview.text = StaticDataMgr.Instance.GetTextByID("decompose_preview");
        decomposeView.text_PreviewTips.text = StaticDataMgr.Instance.GetTextByID("decompose_choise");

        decomposeView.btnDecomposeChoice.onClick.AddListener(OnClickChoiseBtn);
        decomposeView.btnDecompose.onClick.AddListener(OnClickDecomposBtn);
        decomposeView.text_DecomposeChoice.text = StaticDataMgr.Instance.GetTextByID("decompose_choice");
        decomposeView.text_Decompose.text = StaticDataMgr.Instance.GetTextByID("decompose_title");

        decomposeView.btnHidemenu.onClick.AddListener(HideChoiseMenu);
        decomposeView.btnWhite.onClick.AddListener(OnClickWhiteBtn);
        decomposeView.btnGreen.onClick.AddListener(OnClickGreenBtn);
        decomposeView.btnBlue.onClick.AddListener(OnClickBlueBtn);
        UIUtil.SetStageColor(decomposeView.text_White, "decompose_white", 1);
        UIUtil.SetStageColor(decomposeView.text_Green, "decompose_green", 2);
        UIUtil.SetStageColor(decomposeView.text_Blue, "decompose_blue", 3);

        decomposeList.text_Material.text = StaticDataMgr.Instance.GetTextByID("decompose_material");
    }

    public override void Init()
    {
        tabIndex = -1;
        selIndex = 0;
        Refresh();
    }
    public override void Clean()
    {
        decomposeList.scrollView.CleanContent();
    }

    public void Refresh(int select = -1)
    {
        selIndex = (select == -1 ? selIndex : select);
        if (tabIndex!=selIndex)
        {
            TabGroup.OnChangeItem(selIndex);
        }
        else
        {
            ReLoadData(selIndex);
        }
    }

    public void OnTabButtonChanged(int index)
    {
        if (tabIndex==index)
        {
            return;
        }
        tabIndex = index;
        selIndex = index;
        ReLoadData(tabIndex);
    }

    void ReLoadData(int index)
    {
        #region ReLoadView

        decomposeView.selectItems.Clear();
        decomposeView.RemoveElement(decomposeView.coinIcon);
        decomposeView.RemoveAllElement();
        SetSelectIconTips();
        
        #endregion

        #region ReloadList

        if (Type==type.Equipment)
        {
            Dictionary<long, EquipData> allEquip = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.equipList;
            equipInfos.Clear();
            foreach (var item in allEquip)
            {
                if (item.Value.monsterId != BattleConst.invalidMonsterID)
                {
                    continue;
                }
                equipInfos.Add(item.Value);
            }

            equipInfos.Sort(SortEquip);
            decomposeList.itemsInfo.Clear();

            for (int i = 0; i < equipInfos.Count; i++)
            {
                decomposeList.itemsInfo.Add(new DecomposeItemInfo(equipInfos[i]));
            }
        }
        else
        {
            Dictionary<int, GameUnit> allMonster = GameDataMgr.Instance.PlayerDataAttr.allUnitDic;
            monsterInfos.Clear();
            foreach (var item in allMonster)
            {
                if (item.Value.pbUnit.locked || CheckMainMonster(item.Value))
                {
                    continue;
                }
                monsterInfos.Add(item.Value);
            }

            monsterInfos.Sort(SortMonster);
            decomposeList.itemsInfo.Clear();

            for (int i = 0; i < monsterInfos.Count; i++)
            {
                decomposeList.itemsInfo.Add(new DecomposeItemInfo(monsterInfos[i]));
            }
        }
        decomposeList.scrollView.InitContentSize(decomposeList.itemsInfo.Count, this);
        decomposeView.ShowMenu = false;
        #endregion
    }

    void OnClickItem(DecomposeItem item)
    {
        DecomposeItemInfo info = item.CurData;
            if (CheckIsSelect(info.ItemId))
            {
                OnSetDeselect(info);
            }
            else
            {
                OnSetSelect(info);
            }
    }

    void OnSetSelect(DecomposeItemInfo item)
    {
        item.IsSelect = true;
        ReloadDecomposeList(item);
        decomposeView.selectItems.Add(item.ItemId);
        SetSelectIconTips();
    }
    void OnSetSelectStage(int stage)//宠物需要转换品质
    {
        bool isFind = false;
        if (Type == type.Equipment)
        {
            for (int i = 0; i < decomposeList.itemsInfo.Count; i++)
            {
                if (decomposeList.itemsInfo[i].curEquip.stage==stage)
                {
                    isFind = true;
                    OnSetSelect(decomposeList.itemsInfo[i]);
                }
            }
        }
        else
        {
            int quallity, plusQuality;
            for (int i = 0; i < decomposeList.itemsInfo.Count; i++)
            {
                UIUtil.CalculationQuality(decomposeList.itemsInfo[i].curMonster.pbUnit.stage, out quallity, out plusQuality);
                if (quallity==stage)
                {
                    isFind = true;
                    OnSetSelect(decomposeList.itemsInfo[i]);
                }
            }
        }
        if (isFind)
        {
            UIIm.Instance.ShowSystemHints(string.Format(StaticDataMgr.Instance.GetTextByID("compose_record_003"),
                                                        StaticDataMgr.Instance.GetTextByID(stage==1?"decompose_white":
                                                                                                    (stage==2? "decompose_green": 
                                                                                                               "decompose_blue"))), 
                                         (int)PB.ImType.PROMPT);
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("compose_record_004"), (int)PB.ImType.PROMPT);
        }


    }
    void OnSetDeselect(DecomposeItemInfo item)
    {
        item.IsSelect = false;
        ReloadDecomposeList(item, true);
        decomposeView.selectItems.Remove(item.ItemId);
        SetSelectIconTips();
    }
    void OnSetDeselectAll()
    {
        for (int i = 0; i < decomposeList.itemsInfo.Count; i++)
        {
            if (decomposeView.selectItems.Contains(decomposeList.itemsInfo[i].ItemId))
            {
                OnSetDeselect(decomposeList.itemsInfo[i]);
            }
        }
    }

    void ReloadDecomposeList(DecomposeItemInfo item, bool isRemove = false)
    {
        List<ItemInfo> decomposeList = new List<ItemInfo>();
        if (item.type == type.Equipment)
        {
            EquipForgeData forgeData = StaticDataMgr.Instance.GetEquipForgeData(item.curEquip.stage, item.curEquip.level);
            forgeData.GetDecompose(ref decomposeList);
        }
        else
        {
            UnitStageData stageData = StaticDataMgr.Instance.getUnitStageData(item.curMonster.pbUnit.stage);
            decomposeList = stageData.decomposeList;
        }
        for (int i = 0; i < decomposeList.Count; i++)
        {
            if (decomposeList[i].type == (int)PB.itemType.ITEM)
            {
                decomposeView.GetElement(new ItemData() { itemId = decomposeList[i].itemId, count = decomposeList[i].count }, isRemove);
            }
            else if (decomposeList[i].type == (int)PB.itemType.PLAYER_ATTR)
            {
                if (decomposeList[i].itemId.Equals(((int)PB.changeType.CHANGE_COIN).ToString()))
                {
                    decomposeView.GetElement((PB.changeType)int.Parse(decomposeList[i].itemId), decomposeList[i].count,isRemove);
                }
                else
                {
                    Logger.LogError("xiao hao jin bi pei zhi cuo wu !!!!!!!!!!!!!!!!!!");
                }
            }
        }
    }

    //设置选择的图标
    void SetSelectIconTips()
    {
        if (CheckIsEmpty())
        {
            decomposeView.firstItem = -1;
            if (decomposeView.itemIcon != null)
            {
                decomposeView.itemIcon.gameObject.SetActive(false);
            }
            if (decomposeView.monsterIcon != null)
            {
                decomposeView.monsterIcon.gameObject.SetActive(false);
            }

            decomposeView.text_PreviewTips.gameObject.SetActive(true);
            decomposeView.text_Preview.gameObject.SetActive(false);
            if (Type == type.Equipment)
            {
                decomposeView.text_Tips1.text = StaticDataMgr.Instance.GetTextByID("decompose_tips1");
            }
            else
            {
                decomposeView.text_Tips1.text = StaticDataMgr.Instance.GetTextByID("decompose_tips3");
            }
        }
        else
        {
            #region 设置ICON显示列表首项
            if (decomposeView.firstItem != decomposeView.selectItems[0])
            {
                decomposeView.firstItem = decomposeView.selectItems[0];
                if (Type == type.Equipment)
                {
                    EquipData data = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.GetEquip(decomposeView.firstItem);
                    if (decomposeView.itemIcon == null)
                    {
                        decomposeView.itemIcon = ItemIcon.CreateItemIcon(data, false);
                        UIUtil.SetParentReset(decomposeView.itemIcon.transform, decomposeView.iconPos);
                    }
                    else
                    {
                        decomposeView.itemIcon.gameObject.SetActive(true);
                        decomposeView.itemIcon.RefreshWithEquipInfo(data, false);
                    }
                }
                else
                {
                    GameUnit data = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey((int)decomposeView.firstItem);
                    if (decomposeView.monsterIcon == null)
                    {
                        decomposeView.monsterIcon = MonsterIcon.CreateIcon();
                        UIUtil.SetParentReset(decomposeView.monsterIcon.transform, decomposeView.iconPos);
                    }
                    else
                    {
                        decomposeView.monsterIcon.gameObject.SetActive(true);
                        decomposeView.monsterIcon.Init();
                    }
                    decomposeView.monsterIcon.SetId(data.pbUnit.guid.ToString());
                    decomposeView.monsterIcon.SetMonsterStaticId(data.pbUnit.id);
                    decomposeView.monsterIcon.SetStage(data.pbUnit.stage);
                    decomposeView.monsterIcon.SetLevel(data.pbUnit.level);
                    decomposeView.monsterIcon.iconButton.gameObject.SetActive(false);

                }
            }
            #endregion
            decomposeView.text_PreviewTips.gameObject.SetActive(false);
            decomposeView.text_Preview.gameObject.SetActive(true);
            if (Type == type.Equipment)
            {
                decomposeView.text_Tips1.text = StaticDataMgr.Instance.GetTextByID("decompose_tips2");
            }
            else
            {
                decomposeView.text_Tips1.text = StaticDataMgr.Instance.GetTextByID("decompose_tips4");
            }
        }
    }
    //检测是否已经选择过物体
    bool CheckIsEmpty()
    {
        if (decomposeView.selectItems.Count <= 0)
        {
            return true;
        }
        return false;
    }
    //检测是否已经选择过物体
    bool CheckIsOnlyOne()
    {
        if (decomposeView.selectItems.Count == 1)
        {
            return true;
        }
        return false;
    }
    //检测是否已经被选中
    bool CheckIsSelect(long id)
    {
        for (int i = 0; i < decomposeView.selectItems.Count; i++)
        {
            if (decomposeView.selectItems[i]==id)
            {
                return true;
            }
        }
        return false;
    }
    //检测是否是上阵的宠物
    bool CheckMainMonster(GameUnit monster)
    {
        return GameDataMgr.Instance.PlayerDataAttr.mainUnitID.Contains(monster.pbUnit.guid);
    }

    void OnClickCloseBtn(GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void OnClickWhiteBtn()
    {
        Logger.Log("选择白色品质物品");
        OnSetDeselectAll();
        OnSetSelectStage(1);
        HideChoiseMenu();
    }
    void OnClickGreenBtn()
    {
        Logger.Log("选择绿色品质物品");

        OnSetDeselectAll();
        OnSetSelectStage(2);
        HideChoiseMenu();
    }
    void OnClickBlueBtn()
    {
        Logger.Log("选择蓝色品质物品");

        OnSetDeselectAll();
        OnSetSelectStage(3);
        HideChoiseMenu();
    }

    void HideChoiseMenu()
    {
        decomposeView.ShowMenu = false;
    }

    void OnClickChoiseBtn()
    {
        //显示隐藏筛选列表
        decomposeView.ShowMenu = !decomposeView.ShowMenu;
    }
    void OnClickDecomposBtn()
    {
        if (CheckIsEmpty())
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("compose_record_005"), (int)PB.ImType.PROMPT);
        }
        else
        {
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform_Cancel,
                                  StaticDataMgr.Instance.GetTextByID("decompose_sure"),
                                  StaticDataMgr.Instance.GetTextByID("decompose_coin2"),
                                  OnRefreshConformDlgClick);
        }
    }
    void OnRefreshConformDlgClick(MsgBox.PrompButtonClick btnParam)
    {
        if (btnParam == MsgBox.PrompButtonClick.OK)
        {
            if (Type == type.Equipment)
            {
                PB.HSEquipDecompose param = new PB.HSEquipDecompose();
                param.equipId.AddRange(decomposeView.selectItems);
                GameApp.Instance.netManager.SendMessage(PB.code.EQUIP_DECOMPOSE_C.GetHashCode(), param);
            }
            else
            {
                decomposeView.UnloadEquip.Clear();
                
                PB.HSMonsterDecompose param = new PB.HSMonsterDecompose();
                decomposeView.selectItems.ForEach(delegate(long item) 
                                                          {
                                                              param.monsterId.Add((int)item);
                                                              GameUnit unit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey((int)item);
                                                              for (int i = 0; i < unit.equipList.Length; i++)
                                                              {
                                                                  if (unit.equipList[i]!=null)
                                                                  {
                                                                      decomposeView.UnloadEquip.Add(unit.equipList[i]);
                                                                  }
                                                              }
                                                          });
                GameApp.Instance.netManager.SendMessage(PB.code.MONSTER_DECOMPOSE_C.GetHashCode(), param);
            }
        }
    }

    public static int SortEquip(EquipData a, EquipData b)
    {
        if (a.stage < b.stage)
        {
            return -1;
        }
        else if (a.stage > b.stage)
        {
            return 1;
        }
        ItemStaticData staticDataA = StaticDataMgr.Instance.GetItemData(a.equipId);
        ItemStaticData staticDataB = StaticDataMgr.Instance.GetItemData(b.equipId);
        if (staticDataA.part < staticDataB.part)
        {
            return -1;
        }
        else if (staticDataA.part > staticDataB.part)
        {
            return 1;
        }
        return 0;
    }
    public static int SortMonster(GameUnit a,GameUnit b)
    {
        if (a.pbUnit.stage<b.pbUnit.stage)
        {
            return -1;
        }
        else if (a.pbUnit.stage > b.pbUnit.stage)
        {
            return 1;
        }
        return 0;
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
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnReward);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_DECOMPOSE_C.GetHashCode().ToString(), OnEquipDecomposeRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.EQUIP_DECOMPOSE_S.GetHashCode().ToString(), OnEquipDecomposeRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_DECOMPOSE_C.GetHashCode().ToString(), OnMonsterDecomposeRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_DECOMPOSE_S.GetHashCode().ToString(), OnMonsterDecomposeRet);

    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnReward);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_DECOMPOSE_C.GetHashCode().ToString(), OnEquipDecomposeRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.EQUIP_DECOMPOSE_S.GetHashCode().ToString(), OnEquipDecomposeRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_DECOMPOSE_C.GetHashCode().ToString(), OnMonsterDecomposeRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_DECOMPOSE_S.GetHashCode().ToString(), OnMonsterDecomposeRet);
    }

    void OnReward(ProtocolMessage msg)
    {
        PB.HSRewardInfo reward = msg.GetProtocolBody<PB.HSRewardInfo>();
        if (reward == null)
            return;
        
        if (reward.hsCode == PB.code.EQUIP_DECOMPOSE_C.GetHashCode() || reward.hsCode == PB.code.MONSTER_DECOMPOSE_C.GetHashCode())
        {
            string tips = StaticDataMgr.Instance.GetTextByID("compose_record_002");
            string tips1 = "{0}*{1}";

            #region tips=金币*1000    物品A*20……

            PB.RewardItem info;
            ItemStaticData itemData;
            for (int i = 0; i < reward.RewardItems.Count; i++)
            {
                info = reward.RewardItems[i];
                if (info.type == (int)PB.itemType.ITEM)
                {
                    itemData=StaticDataMgr.Instance.GetItemData(info.itemId);
                    if (itemData.type == (int)PB.toolType.GEMTOOL)
                    {
                        continue;
                    }
                    else
                    {
                        tips += string.Format(tips1,
                                              StaticDataMgr.Instance.GetTextByID(itemData.name),
                                              info.count);
                    }
                }
                else if (info.type == (int)PB.itemType.PLAYER_ATTR)
                {
                    if (int.Parse(info.itemId)==(int)PB.changeType.CHANGE_COIN)
                    {
                        tips += string.Format(tips1,
                                              StaticDataMgr.Instance.GetTextByID("decompose_coin1"),
                                              info.count);
                    }
                }
                tips += (i == (reward.RewardItems.Count - 1)) ? "" : ",";
            }
            #endregion

            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("compose_record_001"), (int)PB.ImType.PROMPT);
            UIIm.Instance.ShowSystemHints(tips, (int)PB.ImType.PROMPT);
        }
        else
        {
            return;
        }
    }

    void OnEquipDecomposeRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("分解物品错误");
            return;
        }
        Refresh();
    }

    void OnMonsterDecomposeRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("分解物品错误");
            return;
        }
        for (int i = 0; i < decomposeView.UnloadEquip.Count; i++)
        {
            decomposeView.UnloadEquip[i].monsterId = BattleConst.invalidMonsterID;
        }
        Refresh();
    }

    public void ReloadData(Transform item, int index)
    {
        DecomposeItem material = item.GetComponent<DecomposeItem>();
        material.ReloadData(decomposeList.itemsInfo[index]);
    }

    public Transform CreateData(Transform parent, int index = 0)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("DecomposeItem");
        if (go != null)
        {
            UIUtil.SetParentReset(go.transform, parent);
            DecomposeItem item = go.GetComponent<DecomposeItem>();
            item.Init(OnClickItem);
            return go.transform;
        }
        return null;
    }

    public void CleanData(List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
    }
}
