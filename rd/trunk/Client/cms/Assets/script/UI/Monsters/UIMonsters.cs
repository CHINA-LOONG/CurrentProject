using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UIMonsters : UIBase, 
                          TabButtonDelegate,
                          IScrollView,
                          IOwnedPetItem,
                          ICollectItem
{

    public static string ViewName = "UIMonsters";

    public Text text_Title;
    public Text text_Owned;
    public Text text_Collection;

    public Button btn_Close;

    public FixCountScrollView scrollView_Owend;
    public FixCountScrollView scrollView_Collect;

    public GameObject objFragmentsInfo;
    public Transform iconPos;
    private ItemIcon iconFragment;
    public Text text_Collect;
    public Text textCollect;
    public Text textComFragments;

    //test only
    //void OnBecameVisible()
    //{
    //    enabled = true;
    //}

    //void OnBecameInvisible()
    //{
    //    enabled = false;
    //}



    public enum UIType
    {
        Owned,
        Collection
    }
    public UIType uiType
    {
        get
        {
            return (UIType)tabIndex1st;
        }
        set
        {
            if (tabIndex1st != (int)value)
            {
                tabIndex1st = (int)value;
                selIndex1st = (int)value;
                if (tabIndex1st == (int)UIType.Owned)
                {
                    objFragmentsInfo.SetActive(false);
                }
                else
                {
                    objFragmentsInfo.SetActive(true);
                }
            }

        }
    }
    private int tabIndex1st = -1;
    private int selIndex1st = 0;

    public TabButtonGroup tabGroup1;
    private bool isRegedit1 = false;
    public TabButtonGroup TabGroup1
    {
        get
        {
            if (!isRegedit1)
            {
                isRegedit1 = true;
                tabGroup1.InitWithDelegate(this);
            }
            return tabGroup1;
        }
    }

    public TabButtonGroup tabGroup2;
    private bool isRegedit2 = false;
    public TabButtonGroup TabGroup2
    {
        get
        {
            if (!isRegedit2)
            {
                isRegedit2 = true;
                tabGroup2.InitWithDelegate(this);
            }
            return tabGroup2;
        }
    }

    private int tabIndex2nd = -1;
    private int selIndex2nd = 0;

    public List<GameUnit> OwnedList = new List<GameUnit>();
    public List<CollectUnit> CollectList = new List<CollectUnit>();

    
    private UIPetDetails uiPetDetail;
    public UIPetDetails UIPetDetail
    {
        get { return uiPetDetail; }
    }

    private UIMonsterCompose uiMonsterCompose;
    public UIMonsterCompose UIMonsterCompose
    {
        get { return uiMonsterCompose; }
    }

    private List<GameUnit> mPetList = new List<GameUnit>();

    public override void RefreshOnPreviousUIHide()
    {
        base.RefreshOnPreviousUIHide();
        Refresh();
    }

    public override void Init()
    {
        base.Init();
        tabIndex1st = -1;
        selIndex1st = 0;

        tabIndex2nd = -1;
        selIndex2nd = 0;

        if (GameDataMgr.Instance.PlayerDataAttr.GetPetCount() >= GameConfig.MaxMonsterCount)
        {
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,
                       StaticDataMgr.Instance.GetTextByID("pet_tip_full1"),
                       StaticDataMgr.Instance.GetTextByID("pet_tip_full2"));
        }

        //Refresh();
    }
    public override void Clean()
    {
        UIMgr.Instance.DestroyUI(UIPetDetail);
        UIMgr.Instance.DestroyUI(UIMonsterCompose);

        scrollView_Owend.CleanContent();
        scrollView_Collect.CleanContent();
        base.Clean();
    }
    void ReloadPetLevelNotify(GameUnit gameUnit)
    {
        if (uiType == UIType.Owned)
        {
            //TODO:
            Refresh();
        }
    }
    void ReloadPetStageNotify(GameUnit gameUnit)
    {
        if (uiType == UIType.Owned)
        {
            //TODO:
            Refresh();
        }
    }
    void ReloadPetEquipNotify(EquipData equipData)
    {
        if (uiType == UIType.Owned)
        {
            //TODO:
            Refresh();
        }
    }
    void ReloadEquipForgeNotify(EquipData equipData)
    {
        if (uiType == UIType.Owned)
        {
            //TODO:
            Refresh();
        }
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
        GameEventMgr.Instance.AddListener<GameUnit>(GameEventList.ReloadPetLevelNotify, ReloadPetLevelNotify);
        GameEventMgr.Instance.AddListener<GameUnit>(GameEventList.ReloadPetStageNotify, ReloadPetStageNotify);
        GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadPetEquipNotify, ReloadPetEquipNotify);
        GameEventMgr.Instance.AddListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadEquipForgeNotify);
        GameEventMgr.Instance.AddListener(GameEventList.ReloadPetCollectNotify, OnCollectEventRefresh);
        GameEventMgr.Instance.AddListener(GameEventList.ReloadUseFragmentNotify, OnCollectEventRefresh);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<GameUnit>(GameEventList.ReloadPetLevelNotify, ReloadPetLevelNotify);
        GameEventMgr.Instance.RemoveListener<GameUnit>(GameEventList.ReloadPetStageNotify, ReloadPetStageNotify);
        GameEventMgr.Instance.RemoveListener<EquipData>(GameEventList.ReloadPetEquipNotify, ReloadPetEquipNotify);
        GameEventMgr.Instance.RemoveListener<EquipData>(GameEventList.ReloadEquipForgeNotify, ReloadEquipForgeNotify);
        GameEventMgr.Instance.RemoveListener(GameEventList.ReloadPetCollectNotify, OnCollectEventRefresh);
        GameEventMgr.Instance.RemoveListener(GameEventList.ReloadUseFragmentNotify, OnCollectEventRefresh);
    }
    void OnCollectEventRefresh()
    {
        if (UIType.Collection == uiType)
        {
            Refresh();
        }
    }

    void Start()
    {
        text_Owned.text = StaticDataMgr.Instance.GetTextByID("handbook_owned");
        text_Collection.text = StaticDataMgr.Instance.GetTextByID("handbook_shouji");

        text_Collect.text= StaticDataMgr.Instance.GetTextByID("handbook_ready");

        btn_Close.onClick.AddListener(OnClickCloseBtn);
    }

    public void Refresh(int select1st = -1, int select2nd = -1)
    {
        selIndex1st = (select1st == -1 ? selIndex1st : select1st);
        if (tabIndex1st!=selIndex1st)
        {
            TabGroup1.OnChangeItem(selIndex1st);
        }

        selIndex2nd = (select2nd == -1 ? selIndex2nd : select2nd);
        if (tabIndex2nd != selIndex2nd)
        {
            TabGroup2.OnChangeItem(selIndex2nd);
        }
        else
        {
            ReLoadData(selIndex2nd);
        }
    }
    void ReLoadData(int index)
    {
        if (uiType == UIType.Owned)
        {
            scrollView_Owend.gameObject.SetActive(true);
            scrollView_Collect.gameObject.SetActive(false);
            ReLoadOwnedData(index);
        }
        else if (uiType == UIType.Collection)
        {
            scrollView_Owend.gameObject.SetActive(false);
            scrollView_Collect.gameObject.SetActive(true);

            ItemData comFragment = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(BattleConst.commonFragmentID);
            int curCommon = (comFragment == null) ? 0 : comFragment.count;
            textComFragments.text = curCommon.ToString();
            
            textCollect.text = GameDataMgr.Instance.PlayerDataAttr.GetMonsterCountByCompose(true).ToString();

            ReLoadCollectData(index);
        }
    }

    public void ReSortOwned(int part)
    {
        if (uiType == UIType.Owned)
        {
            for (int i = 0; i < OwnedList.Count; i++)
            {
                if (i != 0 && OwnedList[i].equipList[part - 1] == null)
                {
                    UnitData petData = StaticDataMgr.Instance.GetUnitRowData(OwnedList[i].pbUnit.id);
                    if (GameDataMgr.Instance.PlayerDataAttr.CheckEquipTypePart(petData.equip, part))
                    {
                        GameUnit temp = OwnedList[0];
                        OwnedList[0] = OwnedList[i];
                        OwnedList[i] = temp;
                        break;
                    }
                }
            }
            scrollView_Owend.InitContentSize(OwnedList.Count, this);
        }
        else
        {
            Debug.LogError("当前不在宠物列表页面");
        }
    }
    void ReLoadOwnedData(int index)
    {
        text_Title.text = StaticDataMgr.Instance.GetTextByID("pet_list_title");

        int curType = GetTypeByIndex(index);

        GameDataMgr.Instance.PlayerDataAttr.GetAllPet(ref mPetList);
        OwnedList.Clear();
        if (0 == curType)
        {
            OwnedList.AddRange(mPetList);
        }
        else
        {
            int petCount = mPetList.Count;
            for (int i = 0; i < petCount; i++)
            {
                if (mPetList[i].property == curType)
                {
                    OwnedList.Add(mPetList[i]);
                }
            }
        }
        OwnedList.Sort();

        scrollView_Owend.InitContentSize(OwnedList.Count, this);
    }
    private void CollectIconLoadCallback(GameObject icon, System.EventArgs args)
    {
        iconFragment = icon.GetComponent<ItemIcon>();
        UIUtil.SetParentReset(icon.transform, iconPos);
        iconFragment.HideExceptIcon();
    }

    public void ReSortCollect(bool useCommon)
    {
        if (uiType == UIType.Collection)
        {
            for (int i = 0; i < CollectList.Count; i++)
            {
                if (i != 0 && GameDataMgr.Instance.PlayerDataAttr.CheckMonsterCompose(CollectList[i].unit,useCommon))
                {
                    CollectUnit temp = CollectList[0];
                    CollectList[0] = CollectList[i];
                    CollectList[i] = temp;
                    break;
                }
            }
            scrollView_Collect.InitContentSize(CollectList.Count, this);
        }
        else
        {
            Debug.LogError("当前不在宠物图鉴页面");
        }
    }
    void ReLoadCollectData(int index)
    {
        int curType = GetTypeByIndex(index);

        List<CollectUnit> list = GameDataMgr.Instance.PlayerDataAttr.collectUnit;//SetCollectList(unitList, collect);
        //List<UnitData> unitList = StaticDataMgr.Instance.GetPlayerUnitData();
        //List<string> collect = GameDataMgr.Instance.PlayerDataAttr.petCollect;
        text_Title.text = StaticDataMgr.Instance.GetTextByID("handbook_btn")+string.Format("({0}/{1})",
                                                                                            GameDataMgr.Instance.PlayerDataAttr.petCollect.Count,
                                                                                            list.Count);
        CollectList.Clear();
        if (0 == curType)
        {
            CollectList.AddRange(list);
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].unit.property == curType)
                {
                    CollectList.Add(list[i]);
                }

            }
        }
        scrollView_Collect.InitContentSize(CollectList.Count, this);

        if (iconFragment==null)
        {
            //iconFragment = ItemIcon.CreateItemIcon();
            ItemIcon.CreateItemIconIconAsync(
                new ItemData() { itemId = BattleConst.commonFragmentID, count = 0 },
                true,
                true,
                CollectIconLoadCallback
                );
        }
    }

    int GetTypeByIndex(int index)
    {
        int curType = 0;
        #region 标签与类型匹配

        switch (index)
        {
            case 0:
                curType = 0;
                break;
            case 1:
                curType = SpellConst.propertyFire;
                break;
            case 2:
                curType = SpellConst.propertyWater;
                break;
            case 3:
                curType = SpellConst.propertyWood;
                break;
            case 4:
                curType = SpellConst.propertyGold;
                break;
            case 5:
                curType = SpellConst.propertyEarth;
                break;
            default:
                Logger.LogError("选择图鉴标签出错");
                break;
        }
        #endregion

        return curType;
    }

    void OnClickCloseBtn()
    {
        RequestCloseUi();
    }
    
    public void OnTabButtonChanged(int index, TabButtonGroup tab)
    {
        if (tab==TabGroup1)
        {
            uiType = (UIType)index;
            Refresh(-1, 0);
        }
        else if(tab==TabGroup2)
        {
            if (tabIndex2nd == index)
            {
                return;
            }
            tabIndex2nd = index;
            selIndex2nd = index;
            ReLoadData(tabIndex2nd);
        }
    }

    public Transform IScrollViewCreateItem(FixCountScrollView scrollView, Transform parent)
    {
        if (uiType == UIType.Owned)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("MonsterListItem");
            if (null != go)
            {
                UIUtil.SetParentReset(go.transform, parent);
                MonsterListItem item = go.GetComponent<MonsterListItem>();
                item.iOwnedPetDelegate = this;
                return go.transform;
            }
        }
        else if (uiType == UIType.Collection)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("MonsterCollectItem");
            if (null != go)
            {
                UIUtil.SetParentReset(go.transform, parent);
                MonsterCollectItem item = go.GetComponent<MonsterCollectItem>();
                item.iCollectDelegate = this;
                return go.transform;
            }
        }
        return null;
    }
    public void IScrollViewReloadItem(FixCountScrollView scrollView, Transform item, int index)
    {
        if (uiType == UIType.Owned)
        {
            MonsterListItem owned = item.GetComponent<MonsterListItem>();
            owned.ReloadData(OwnedList[index]);
        }
        else if (uiType == UIType.Collection)
        {
            MonsterCollectItem collect = item.GetComponent<MonsterCollectItem>();
            collect.ReloadData(CollectList[index]);
        }
    }
    public void IScrollViewCleanItem(FixCountScrollView scrollView, List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
    }

    public void OnClickOwnedPet(GameUnit data)
    {
        uiPetDetail = UIMgr.Instance.OpenUI_(UIPetDetails.ViewName) as UIPetDetails;
        UIPetDetail.SetTypeList(data, OwnedList);
    }

    public void OnClickCollectPet(CollectUnit data)
    {
        uiMonsterCompose = UIMgr.Instance.OpenUI_(UIMonsterCompose.ViewName) as UIMonsterCompose;
        UIMonsterCompose.SetTypeList(data, CollectList);
    }
}
