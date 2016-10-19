using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UIMonsterCompose : UIBase,TabButtonDelegate
{
    public static string ViewName = "UIMonsterCompose";

    public Text textName;
    public Text textType;
    public Image imgProIcon;

    public Text textTips;
    public Slider proFragments;
    public Text text_Fragments;
    public Text textFragments;
    public Toggle tglUseCommon;
    public Transform iconPos;
    private ItemIcon iconCommon;
    public Text textCommon;
    public Button btnCompose;

    public Button btnPrevious;
    public Button btnNext;
    public Button btnClose;
    
    public ImageView imageView;

    public Transform tipsContent;
    //public Text text_GetBy;
    private FoundItem[] foundItems = new FoundItem[3];
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
    public Text text_Tab1;
    public Text text_Tab2;
    private int selIndex = -1;

    [Serializable]
    public class AttributePanel
    {
        public Text text_Title;
        public Text text_STA;
        public Text textSTA;
        public Text text_STR;
        public Text textSTR;
        public Text text_INT;
        public Text textINT;
        public Text text_DEF;
        public Text textDEF;
        public Text text_SPD;
        public Text textSPD;
    }
    public AttributePanel attrPanel;

    public Transform spellParent;
    private SpellIcon[] spellIcons = new SpellIcon[5];
    public Transform spellTipsParent;
    private SkilTips skilTips;

    private List<CollectUnit> curUnitList;
    private int curUnitIndex;
    public CollectUnit CurData
    {
        get { return curUnitList[curUnitIndex]; }
    }
    private string monsterId;
    private UnitBaseData baseData = null;
    private BattleObject unitModel;

    private ItemData curFragment;
    private int curCount;
    private ItemData comFragment;
    private int comCount;

    public override void Disable()
    {
        imageView.CleanImageView();//关闭时释放模型资源
    }

    void Start()
    {
        btnCompose.onClick.AddListener(ClickComposeBtn);
        btnPrevious.onClick.AddListener(ClickPreviousBtn);
        btnNext.onClick.AddListener(ClickNextBtn);
        btnClose.onClick.AddListener(ClickCloseBtn);

        btnCompose.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("handbook_zhaohuan");

        attrPanel.text_Title.text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage_attr");
        attrPanel.text_STA.text = StaticDataMgr.Instance.GetTextByID("common_attr_health");
        attrPanel.text_STR.text = StaticDataMgr.Instance.GetTextByID("common_attr_strenth");
        attrPanel.text_INT.text = StaticDataMgr.Instance.GetTextByID("common_attr_intelligence");
        attrPanel.text_DEF.text = StaticDataMgr.Instance.GetTextByID("common_attr_defence");
        attrPanel.text_SPD.text = StaticDataMgr.Instance.GetTextByID("common_attr_speed");

        textName.color = ColorConst.GetStageTextColor(1);
        textName.GetComponent<Outline>().effectColor = ColorConst.GetStageOutLineColor(1);

        //text_GetBy.text = StaticDataMgr.Instance.GetTextByID("handbook_huodeway");
        text_Tab1.text = StaticDataMgr.Instance.GetTextByID("pet_list_title");
        text_Tab2.text = StaticDataMgr.Instance.GetTextByID("item_type_chip");
    }

    public void SetTypeList(CollectUnit unit, List<CollectUnit> unitList)
    {
        curUnitList = unitList;
        curUnitIndex = unitList.IndexOf(unit);
        Refresh();
    }

    void Refresh()
    {
        tglUseCommon.isOn = false;
        textName.text = CurData.unit.NickNameAttr;
        textType.text = StaticDataMgr.Instance.GetTextByID("monstertype" + CurData.unit.type);
        imgProIcon.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>("property_" + CurData.unit.property);

        if (!string.Equals(monsterId, CurData.unit.id))
        {
            unitModel = imageView.ReloadData(CurData.unit.id);
            monsterId = CurData.unit.id;
        }
        if (null == baseData)
        {
            baseData = StaticDataMgr.Instance.GetUnitBaseRowData(1);
        }
        attrPanel.textSTA.text = string.Format("{0:F0}", baseData.health * CurData.unit.healthModifyRate);
        attrPanel.textSTR.text = string.Format("{0:F0}", baseData.strength * CurData.unit.strengthModifyRate);
        attrPanel.textINT.text = string.Format("{0:F0}", baseData.intelligence * CurData.unit.intelligenceModifyRate);
        attrPanel.textDEF.text = string.Format("{0:F0}", baseData.defense * CurData.unit.defenseModifyRate);
        attrPanel.textSPD.text = string.Format("{0:F0}", baseData.speed * CurData.unit.speedModifyRate);


        UnitRarityData rarity = StaticDataMgr.Instance.GetUnitRarityData(CurData.unit.rarity);
        textTips.text = string.Format(StaticDataMgr.Instance.GetTextByID("handbook_tips"), rarity.commonRatio);
        curFragment = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(CurData.unit.fragmentId);
        curCount = (curFragment == null ? 0 : curFragment.count);
        textFragments.text = string.Format("{0}/{1}", curCount, CurData.unit.fragmentCount);
        proFragments.value = curCount / (float)CurData.unit.fragmentCount;

        if (iconCommon==null)
        {
            iconCommon = ItemIcon.CreateItemIcon(new ItemData() { itemId = BattleConst.commonFragmentID, count = 0 });
            UIUtil.SetParentReset(iconCommon.transform, iconPos);
            iconCommon.HideExceptIcon();
        }
        comFragment = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(BattleConst.commonFragmentID);
        comCount = (comFragment == null ? 0 : comFragment.count);
        textCommon.text = comCount.ToString();

        RefreshSpell(CurData.unit.SpellList);

        if (selIndex != 0)
        {
            TabGroup.OnChangeItem(0);
        }
        else
        {
            RefreshFoundTips(selIndex);
        }
    }
    #region GetByTips
    public void OnTabButtonChanged(int index, TabButtonGroup tab)
    {
        selIndex = index;
        RefreshFoundTips(index);
    }
    void RefreshFoundTips(int index)
    {
        List<List<string>> foundList;
        if (index==0)
        {
            foundList = CurData.unit.FoundList;
        }
        else
        {
            foundList = StaticDataMgr.Instance.GetItemData(CurData.unit.fragmentId).FoundList;
        }
        if (foundList != null&&foundList.Count>foundItems.Length)
        {
            Logger.LogError("配置获取途径有错误");
        }
        for (int i = 0; i < foundItems.Length; i++)
        {
            FoundItem fountItem = foundItems[i];
            if (foundList == null||i>=foundList.Count)
            {
                if (fountItem != null)
                {
                    fountItem.gameObject.SetActive(false);
                }
                continue;
            }
            else
            {
                if (fountItem != null)
                {
                    fountItem.gameObject.SetActive(true);
                    fountItem.ReLoadData(foundList[i]);
                }
                else
                {
                    foundItems[i] = FoundItem.CreateItem(foundList[i]);
                    UIUtil.SetParentReset(foundItems[i].transform, tipsContent);
                }
            }
        }
    }

    #endregion

    #region spell
    void RefreshSpell(List<SpellProtoType> spellList)
    {
        int spellCount = 0; //最大显示5个
        for (int i = 0; i < spellList.Count; i++)
        {
            if (null == spellList[i])
            {
                continue;
            }
            if (string.IsNullOrEmpty(spellList[i].tips))
            {
                continue;
            }
            if (null == spellIcons[spellCount])
            {
                spellIcons[spellCount] = SpellIcon.CreateWith(spellParent);
                EventTriggerListener.Get(spellIcons[spellCount].iconButton.gameObject).onEnter = OnPointerEnter;
                EventTriggerListener.Get(spellIcons[spellCount].iconButton.gameObject).onExit = OnPointerExit;
            }
            else
            {
                spellIcons[spellCount].gameObject.SetActive(true);
            }
            spellIcons[spellCount].SetData(1, spellList[i].textId);
            spellCount++;
            if (spellCount >= spellIcons.Length)
            {
                break;
            }
        }
        for (int i = spellCount; i < spellIcons.Length; i++)
        {
            if (null != spellIcons[i])
            {
                spellIcons[i].gameObject.SetActive(false);
            }
        }
    }
    #endregion
    public void OnPointerEnter(GameObject go)
    {
        //Logger.LogError ("--------" + test++);
        RectTransform iconTrans;
        RectTransform tipsTrans;
        if (skilTips == null)
        {
            GameObject objTips = ResourceMgr.Instance.LoadAsset("SkilTips") as GameObject;

            tipsTrans = objTips.transform as RectTransform;
            tipsTrans.anchorMin = new Vector2(0, 1);
            tipsTrans.anchorMax = new Vector2(0, 1);
            tipsTrans.pivot = new Vector2(1, 0.5f);
            UIUtil.SetParentReset(objTips.transform, spellTipsParent);

            skilTips = objTips.GetComponent<SkilTips>();
        }
        else
        {
            skilTips.gameObject.SetActive(true);
        }

        SpellIcon icon = go.GetComponentInParent<SpellIcon>();
        icon.SetMask(true);
        skilTips.SetSpellId(icon.spellId, icon.level);

        iconTrans = icon.transform as RectTransform;
        tipsTrans = skilTips.transform as RectTransform;
        
        Vector2 iconPos = iconTrans.anchoredPosition;
        Vector2 tipsPos = tipsTrans.anchoredPosition;
        tipsPos.y = iconPos.y;

        tipsTrans.anchoredPosition = tipsPos;
    }

    public void OnPointerExit(GameObject go)
    {
        SpellIcon icon = go.GetComponentInParent<SpellIcon>();
        icon.SetMask(false);
        skilTips.gameObject.SetActive(false);
    }


    void ClickPreviousBtn()
    {
        if (curUnitList.Count == 1)
        {
            return;
        }
        curUnitIndex = (curUnitIndex - 1 + curUnitList.Count) % curUnitList.Count;
        Refresh();
    }
    void ClickNextBtn()
    {
        if (curUnitList.Count == 1)
        {
            return;
        }
        curUnitIndex = (curUnitIndex + 1) % curUnitList.Count;
        Refresh();
    }
    void ClickComposeBtn()
    {
        bool useCommon = tglUseCommon.isOn;
        //ItemData itemData = null;
        //if (true)
        //{
        //    itemData = new ItemData() { itemId = CurData.unit.fragmentId, count = CurData.unit.fragmentCount };
        //    PromptComposeMST prompt = PromptComposeMST.Open(StaticDataMgr.Instance.GetTextByID("handbook_tips1"),
        //                                                    ""/*StaticDataMgr.Instance.GetTextByID("handbook_tips")*/, itemData, 0, OnConfirmCompose, OnCancelCompose);

        //    return;
        //}
        if (!useCommon)
        {
            if (curCount >= CurData.unit.fragmentCount)
            {
                PromptComposeMST prompt = PromptComposeMST.Open(StaticDataMgr.Instance.GetTextByID("handbook_tips1"),
                                                                ""/*StaticDataMgr.Instance.GetTextByID("handbook_tips")*/, CurData.unit.fragmentId, CurData.unit.fragmentCount, 0, OnConfirmCompose, OnCancelCompose);
            }
            else
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("handbook_record_001"), (int)PB.ImType.PROMPT);
            }
        }
        else
        {
            UnitRarityData rarity = StaticDataMgr.Instance.GetUnitRarityData(CurData.unit.rarity);
            int needComCount = (int)(rarity.commonRatio * CurData.unit.fragmentCount);
            int needCurCount = CurData.unit.fragmentCount - needComCount;
            if (curCount >= needCurCount && (curCount +comCount)>=CurData.unit.fragmentCount)
            {
                if (comCount >= needComCount)
                {
                    PromptComposeMST prompt = PromptComposeMST.Open(StaticDataMgr.Instance.GetTextByID("handbook_tips1"),
                                                                    string.Format(StaticDataMgr.Instance.GetTextByID("handbook_tips"), 
                                                                    rarity.commonRatio), 
                                                                    CurData.unit.fragmentId, 
                                                                    needCurCount, 
                                                                    needComCount, 
                                                                    OnConfirmCompose, 
                                                                    OnCancelCompose);
                }
                else
                {
                    PromptComposeMST prompt = PromptComposeMST.Open(StaticDataMgr.Instance.GetTextByID("handbook_tips1"),
                                                                    string.Format(StaticDataMgr.Instance.GetTextByID("handbook_tips"), 
                                                                    rarity.commonRatio),
                                                                    CurData.unit.fragmentId,
                                                                    CurData.unit.fragmentCount - comCount,
                                                                    comCount, 
                                                                    OnConfirmCompose, 
                                                                    OnCancelCompose);
                }
            }
            else
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("handbook_record_001"), (int)PB.ImType.PROMPT);
            }
        }
    }
    void OnConfirmCompose()
    {
        PB.HSMonsterCompose param = new PB.HSMonsterCompose();
        param.cfgId = CurData.unit.id;
        param.useCommon = tglUseCommon.isOn;
        GameApp.Instance.netManager.SendMessage(PB.code.MONSTER_COMPOSE_C.GetHashCode(), param);
    }
    void OnCancelCompose()
    {

    }
    
    void ClickCloseBtn()
    {
        UIMgr.Instance.CloseUI_(this);
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

        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_COMPOSE_C.GetHashCode().ToString(), OnMonsterComposeRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_COMPOSE_S.GetHashCode().ToString(), OnMonsterComposeRet);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnReward);

        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_COMPOSE_C.GetHashCode().ToString(), OnMonsterComposeRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_COMPOSE_S.GetHashCode().ToString(), OnMonsterComposeRet);

    }

    void OnReward(ProtocolMessage msg)
    {
        PB.HSRewardInfo reward = msg.GetProtocolBody<PB.HSRewardInfo>();
        if (reward == null)
            return;
        if (reward.hsCode == PB.code.MONSTER_COMPOSE_C.GetHashCode() ||
            reward.hsCode == PB.code.MONSTER_COMPOSE_S.GetHashCode())
        {
            unitModel.TriggerEvent("dazhaoxuanyao_fashu", Time.time, null);
        }
        else
        {
            return;
        }

    }
    void OnMonsterComposeRet(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType()==(int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("合成宠物出现错误");
            return;
        }
        GameEventMgr.Instance.FireEvent(GameEventList.ReloadUseFragmentNotify);
        Refresh();
    }

}
