using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICompose : UIBase, TabButtonDelegate
{
    public static string ViewName = "UICompose";

    public Text text_Title;
    public Button btnClose;

    public Text text_Tab1;
    public Text text_Tab2;

    [Serializable]
    public class ComposeView
    {
        public Button btnRemoveAll;

        public Button btnComposeOne;    //合成一次
        public Button btnComposeTen;    //合成十次

        public Text text_Preview;   //合成预览
        public Text text_Tips;      //合成提示
        public Text text_RemoveAll;
        public Text text_ComposeOne;
        public Text text_ComposeTen;

        public Text text_targetName;
        public Transform iconPos;
        [HideInInspector]
        public ItemIcon itemIcon;
        public GameObject gemIcon;

        public List<ComposeField> fields = new List<ComposeField>(); //材料位置

        [HideInInspector]
        public List<string> selectItems = new List<string>();

    }

    [Serializable]
    public class ComposeList
    {

        public Text text_Material;
        public Transform content;

        [HideInInspector]
        public Dictionary<string, ComposeItem> items = new Dictionary<string, ComposeItem>();
        [HideInInspector]
        public List<ComposeItem> itemPool = new List<ComposeItem>();


        public ComposeItem GetElement(ItemDataInfo data, System.Action<ComposeItem> clickBack)
        {
            ComposeItem item = null;
            if (itemPool.Count <= 0)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("ComposeItem");
                if (go!=null)
                {
                    UIUtil.SetParentReset(go.transform, content);
                    item = go.GetComponent<ComposeItem>();
                }
            }
            else
            {
                item = itemPool[itemPool.Count - 1];
                item.gameObject.SetActive(true);
                itemPool.Remove(item);
            }
            item.ReloadData(data, clickBack);
            item.SetDisable(false);
            items.Add(data.itemData.itemId,item);
            item.transform.SetAsLastSibling();
            return item;
        }
        public void RemoveElement(ComposeItem item)
        {
            item.gameObject.SetActive(false);
            itemPool.Add(item);
            if (items.ContainsValue(item))
            {
                items.Remove(item.curData.itemData.itemId);
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

    public ComposeView composeView;
    public ComposeList composeList;
    
    private int tabIndex = -1;
    private int selIndex = 0;
    public enum type
    {
        Gem,
        Materials
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

    public List<ItemDataInfo> infos = new List<ItemDataInfo>();
    [HideInInspector]
    public UIComposeResult uiComposeResult;

    void Start()
    {
        EventTriggerListener.Get(btnClose.gameObject).onClick = OnClickCloseBtn;

        text_Title.text = StaticDataMgr.Instance.GetTextByID("compose_title");
        text_Tab1.text = StaticDataMgr.Instance.GetTextByID("bag_tag_gem");
        text_Tab2.text = StaticDataMgr.Instance.GetTextByID("bag_tag_item");

        composeView.btnRemoveAll.onClick.AddListener(OnClickRemoveAll);
        composeView.btnComposeOne.onClick.AddListener(OnClickComposeOne);
        composeView.btnComposeTen.onClick.AddListener(OnClickComposeTen);
        composeView.text_Preview.text = StaticDataMgr.Instance.GetTextByID("compose_preview");
        composeView.text_RemoveAll.text = StaticDataMgr.Instance.GetTextByID("compose_unload");
        composeView.text_ComposeOne.text = StaticDataMgr.Instance.GetTextByID("compose_btnone");
        composeView.text_ComposeTen.text = StaticDataMgr.Instance.GetTextByID("compose_btnten");

        composeList.text_Material.text = StaticDataMgr.Instance.GetTextByID("compose_chmaterial");

    }

    public override void Init()
    {
        tabIndex = -1;
        selIndex = 0;
        Refresh();
    }
    public override void Clean()
    {
        UIMgr.Instance.DestroyUI(uiComposeResult);
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
        selIndex = index;
        tabIndex = selIndex;
        ReLoadData(tabIndex);
    }

    void ReLoadData(int index)
    {
        #region ReLoadView

        composeView.selectItems.Clear();
        SetTargetIcon(0);

        for (int i = 0; i < composeView.fields.Count; i++)
        {
            composeView.fields[i].Initialize();
        }
        if (Type == type.Gem)
        {
            composeView.text_Tips.text = StaticDataMgr.Instance.GetTextByID("compose_tips1");
        }
        else
        {
            composeView.text_Tips.text = StaticDataMgr.Instance.GetTextByID("compose_tips2");
        }

        #endregion

        #region ReLoadList

        Dictionary<string, ItemData> itemList = GameDataMgr.Instance.PlayerDataAttr.gameItemData.itemList;
        ItemStaticData itemStatic;
        infos.Clear();

        #region 根据选择的页签筛选数据
        if (Type == type.Gem)
        {
            foreach (var item in itemList)
            {
                itemStatic = StaticDataMgr.Instance.GetItemData(item.Value.itemId);
                if (itemStatic == null)
                {
                    Logger.LogError("缺少物品配置:" + item.Value.itemId);
                    continue;
                }
                if (itemStatic.type == (int)PB.toolType.GEMTOOL && itemStatic.grade < BattleConst.maxGemLevel)
                {
                    infos.Add(new ItemDataInfo() { itemData = item.Value, staticData = itemStatic });
                }
            }
        }
        else if (Type == type.Materials)
        {
            foreach (var item in itemList)
            {
                itemStatic = StaticDataMgr.Instance.GetItemData(item.Value.itemId);
                if (itemStatic == null)
                {
                    Logger.LogError("缺少物品配置:" + item.Value.itemId);
                    continue;
                }
                if (itemStatic.type != (int)PB.toolType.FRAGMENTTOOL &&
                    itemStatic.type != (int)PB.toolType.GEMTOOL &&
                    !string.IsNullOrEmpty(itemStatic.targetItem))
                {
                    infos.Add(new ItemDataInfo() { itemData = item.Value, staticData = itemStatic });
                }
            }
        }
        else
        {
            Logger.LogError("合成选择页签错误");
            return;
        }
        #endregion

        infos.Sort(SortItem);
        composeList.RemoveAllElement();
        for (int i = 0; i < infos.Count; i++)
        {
            ComposeItem item = composeList.GetElement(infos[i], OnClickItem);
        }

        #endregion

        SetComposeState(); 
        SetRemoveAllState();
    }
    //点击列表项
    void OnClickItem(ComposeItem item)
    {
        int index = GetFirstField();
        ItemDataInfo dataInfo = item.curData;
        #region 处理材料合成

        if (Type == type.Materials)
        {
            if (index == 0 && CheckIsEmpty())
            {
                ItemInfo info = dataInfo.staticData.TargetItem;
                composeView.selectItems.Clear();
                for (int i = 0; i < dataInfo.staticData.needCount; i++)
                {
                    composeView.selectItems.Add(null);
                }
                for (int i = composeView.selectItems.Count; i < composeView.fields.Count; i++)
                {
                    composeView.fields[i].SetDisable(true);
                }
                SetDisableByItemId(true, dataInfo.itemData.itemId, true);
                ItemData targetData = new ItemData() { itemId = info.itemId, count = 0 };

                composeView.text_Tips.text = string.Format(StaticDataMgr.Instance.GetTextByID("compose_tips3"), dataInfo.staticData.needCount);

                SetTargetIcon(2, 0, targetData);
            }
            composeView.fields[index].SetItemIcon(dataInfo.itemData, OnClickField);
            composeView.selectItems[index] = dataInfo.itemData.itemId;
            item.SelectCount += 1;
            if (CheckIsFull())
            {
                SetDisableByItemId(true, dataInfo.itemData.itemId);
            }
        }
        #endregion
        if (Type==type.Gem)
        {
            if (index == 0 && CheckIsEmpty())
            {
                composeView.selectItems.Clear();
                for (int i = 0; i < composeView.fields.Count; i++)
                {
                    composeView.selectItems.Add(null);
                }
                SetDisableByGrade(true, dataInfo.staticData.grade, true);
                SetTargetIcon(1, dataInfo.staticData.grade + 1, null);
            }
            composeView.fields[index].SetItemIcon(dataInfo.itemData, OnClickField);
            composeView.selectItems[index] = dataInfo.itemData.itemId;
            item.SelectCount += 1;
            if (CheckIsFull())
            {
                SetDisableByGrade(true, dataInfo.staticData.grade);
            }

        }
        SetComposeState();
        SetRemoveAllState();
    }
    //点击合成项
    void OnClickField(ComposeField field)
    {
        if (CheckIsFull())
        {
            if (Type == type.Gem)
            {
                ItemStaticData staticData = StaticDataMgr.Instance.GetItemData(field.curData.itemId);
                SetDisableByGrade(false, staticData.grade);
            }
            else
            {
                SetDisableByItemId(false, field.curData.itemId);
            }
        }
        int index = composeView.fields.IndexOf(field);
        composeList.items[field.curData.itemId].SelectCount -= 1;
        composeView.selectItems[index] = null;
        composeView.fields[index].SetItemIcon(null);
        if (CheckIsEmpty())
        {
            SetTargetIcon(0);
            foreach (var item in composeList.items)
            {
                item.Value.SetDisable(false);
            }
            foreach (var item in composeView.fields)
            {
                item.SetDisable(false);
            }

            if (Type==type.Materials)
            {
                composeView.text_Tips.text = StaticDataMgr.Instance.GetTextByID("compose_tips2");
            }
        }
        SetComposeState();
        SetRemoveAllState();
    }
    //设置合成一次/十次状态
    void SetComposeState()
    {
        if (composeView.selectItems.Count>0&&CheckIsFull())
        {
            composeView.btnComposeOne.interactable = true;
            composeView.btnComposeTen.interactable = true;
        }
        else
        {
            composeView.btnComposeOne.interactable = false;
            composeView.btnComposeTen.interactable = false;
        }
    }
    //设置移除全部状态
    void SetRemoveAllState()
    {
        if (!CheckIsEmpty())
        {
            composeView.btnRemoveAll.gameObject.SetActive(true);
        }
        else
        {
            composeView.btnRemoveAll.gameObject.SetActive(false);
        }
    }

    //获取第一个空的位置
    int GetFirstField()
    {
        for (int i = 0; i < composeView.selectItems.Count; i++)
        {
            if (string.IsNullOrEmpty(composeView.selectItems[i]))
            {
                return i;
            }
        }
        return composeView.selectItems.Count;
    }
    //检测是否已经选择过物体
    bool CheckIsEmpty()
    {
        bool isFirst = true;
        for (int i = 0; i < composeView.selectItems.Count; i++)
        {
            if (!string.IsNullOrEmpty(composeView.selectItems[i]))
            {
                isFirst = false;
                return isFirst;
            }
        }
        return isFirst;
    }
    //检测是否已经添加最后一个物品
    bool CheckIsFull()
    {
        bool isLast = true;
        for (int i = 0; i < composeView.selectItems.Count; i++)
        {
            if (string.IsNullOrEmpty(composeView.selectItems[i]))
            {
                isLast = false;
                return isLast;
            }
        }
        return isLast;
    }

    //设置是否隐藏某等级宝石
    void SetDisableByGrade(bool disable, int grade, bool other = false)
    {
        if (!other)
        {
            foreach (var listItem in composeList.items)
            {
                if (listItem.Value.curData.staticData.grade == grade)
                {
                    listItem.Value.SetDisable(disable);
                }
            }
        }
        else
        {
            foreach (var listItem in composeList.items)
            {
                if (listItem.Value.curData.staticData.grade != grade)
                {
                    listItem.Value.SetDisable(disable);
                }
            }
        }
    }
    //设置是否隐藏某id的物品
    void SetDisableByItemId(bool disable, string itemId, bool other = false)
    {
        if (!other)
        {
            composeList.items[itemId].SetDisable(disable);
        }
        else
        {
            foreach (var listItem in composeList.items)
            {
                if (listItem.Value.curData.itemData.itemId != itemId)
                {
                    listItem.Value.SetDisable(disable);
                }
            }
        }
    }

    void SetTargetIcon(int showTarget, int gemStage = 0, ItemData data = null)
    {
        if (showTarget == 0)
        {
            if (composeView.itemIcon != null)
            {
                composeView.itemIcon.gameObject.SetActive(false);
            }
            composeView.gemIcon.SetActive(false);
            composeView.text_targetName.text = "";
        }
        else if (showTarget == 1)
        {
            composeView.gemIcon.SetActive(true);
            if (composeView.itemIcon != null)
            {
                composeView.itemIcon.gameObject.SetActive(false);
            }
            composeView.text_targetName.text = string.Format(StaticDataMgr.Instance.GetTextByID("compose_gemrandom"), gemStage);
        }
        else
        {
            if (data != null)
            {
                composeView.gemIcon.SetActive(false);
                if (composeView.itemIcon == null)
                {
                    composeView.itemIcon = ItemIcon.CreateItemIcon(data);
                    UIUtil.SetParentReset(composeView.itemIcon.transform, composeView.iconPos);
                }
                else
                {
                    composeView.itemIcon.gameObject.SetActive(true);
                    composeView.itemIcon.RefreshWithItemInfo(data);
                }
                ItemStaticData staticData = StaticDataMgr.Instance.GetItemData(data.itemId);
                composeView.text_targetName.text = StaticDataMgr.Instance.GetTextByID(staticData.name);
            }
        }
    }

    void OnClickRemoveAll()
    {
        Logger.Log("移除所有");
        Refresh();
    }

    void OnClickCloseBtn(GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void OnClickComposeOne()
    {
        Logger.Log("合成了一次");
        SendComposeMessage(false);
    }

    void OnClickComposeTen()
    {
        Logger.Log("合成十次");
        SendComposeMessage(true);
    }

    void SendComposeMessage(bool composeAll)
    {
        if (Type==type.Gem)
        {
            PB.HSGemCompose param = new PB.HSGemCompose();
            #region param.gems<PB.GemSelect>=……

            for (int i = 0; i < composeView.selectItems.Count; i++)
            {
                bool isFind=false;
                for (int j = 0; j < param.gems.Count; j++)
                {
                    if (composeView.selectItems[i] == param.gems[j].gemId)
                    {
                        param.gems[j].count += 1;
                        isFind = true;
                        break;
                    }
                }
                if (!isFind)
                {
                    param.gems.Add(new PB.GemSelect() { gemId = composeView.selectItems[i], count = 1 });
                }
                else
                {
                    continue;
                }
            }
            #endregion
            param.composeAll = composeAll;
            GameApp.Instance.netManager.SendMessage(PB.code.GEM_COMPOSE_C.GetHashCode(), param);
        }
        else
        {
            PB.HSItemCompose param = new PB.HSItemCompose();
            param.itemId = composeView.selectItems[0];
            param.composeAll = composeAll;
            GameApp.Instance.netManager.SendMessage(PB.code.ITEM_COMPOSE_C.GetHashCode(), param);
        }
    }

    public static int SortItem(ItemDataInfo a, ItemDataInfo b)
    {
        return BagDataMgr.SortBagItem(a.itemData, b.itemData);
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

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.GEM_COMPOSE_C.GetHashCode().ToString(), OnGemComposeRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.GEM_COMPOSE_S.GetHashCode().ToString(), OnGemComposeRet);

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_COMPOSE_C.GetHashCode().ToString(), OnItemComposeRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.ITEM_COMPOSE_S.GetHashCode().ToString(), OnItemComposeRet);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnReward);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.GEM_COMPOSE_C.GetHashCode().ToString(), OnGemComposeRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.GEM_COMPOSE_S.GetHashCode().ToString(), OnGemComposeRet);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ITEM_COMPOSE_C.GetHashCode().ToString(), OnItemComposeRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.ITEM_COMPOSE_S.GetHashCode().ToString(), OnItemComposeRet);
    }

    void OnReward(ProtocolMessage msg)
    {
        PB.HSRewardInfo reward = msg.GetProtocolBody<PB.HSRewardInfo>();
        if (reward == null)
            return;
        if (reward.hsCode == PB.code.GEM_COMPOSE_C.GetHashCode() || reward.hsCode == PB.code.ITEM_COMPOSE_C.GetHashCode())
        {
            uiComposeResult = UIMgr.Instance.OpenUI_(UIComposeResult.ViewName) as UIComposeResult;
            uiComposeResult.ReloadData(reward.RewardItems);
        }
        else
        {
            return;
        }
    }

    void OnGemComposeRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("合成物品错误");
            return;
        }
        Refresh();
    }
    void OnItemComposeRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("合成物品错误");
            return;
        }
        Refresh();
    }

}
