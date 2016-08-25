using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class PetDetailsAdvance : PetDetailsRight
{
    public static string ViewName = "PetDetailsAdvance";

    public Text text_Title;
    public Text text_BaseAttr;
    
    public Transform BasePos;
    private ShowAttributesItem[] AttrList = new ShowAttributesItem[5];

    public GameObject objAdvance;
    public Text textAdvanceFull;

    public Text text_LevelText;
    public Text textLevelValue;

    public GameObject[] ObjLine = new GameObject[2];

    public Transform iconPos;
    private MonsterIcon targetIcon;

    public Transform itemPos;
    private ItemIcon itemIcon;
    public Text textItem;

    public List<NeedMonsterElement> monsterFields;
    private SelectMonsterPanel selectMonsterPanel;
    
    public Image imgCoin;
    public Text textCoin;
    public Button btnAdvance;
    public Text text_Advance;

    List<List<int>> monsterSelect = new List<List<int>>();

    private GameUnit curData;
    private UnitStageData unitStageData;

    private bool maxStage;

    private bool enoughLevel;
    private bool enoughCoin;
    private bool enoughItem;
    private bool enoughMonster;

    public EquipForgeEffect advanceEffect;
    private bool isPlayingFX=false;
       
    void Start()
    {
        text_Title.text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage");
        text_LevelText.text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage_demand_level");
        text_Advance.text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage");
        textAdvanceFull.text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage_full");
        btnAdvance.onClick.AddListener(OnClickAdvanceBtn);

        for (int i = 0; i < monsterFields.Count; i++)
        {
            EventTriggerListener.Get(monsterFields[i].button.gameObject).onClick = OpenMonsterSelectUI;
        }

    }

    public void ReloadData(GameUnit unit)
    {
        curData = unit;

        int currentHealth, currentStrength, currentInteligence, currentDefence, currentSpeed;
        int changeHealth, changeStrength, changeInteligence, changeDefence, changeSpeed;
        UIUtil.GetAttrValue(curData, curData.pbUnit.stage, out currentHealth, out currentStrength, out currentInteligence, out currentDefence, out currentSpeed);

        maxStage = curData.pbUnit.stage >= GameConfig.MaxMonsterStage;
        enoughLevel = UIUtil.CheckIsEnoughLevel(curData);
        #region 显示属性
        if (maxStage)//检测是否达到最大等级
        {
            changeHealth = 0;
            changeStrength = 0;
            changeInteligence = 0;
            changeDefence = 0;
            changeSpeed = 0;
        }
        else
        {
            UIUtil.GetAttrValue(curData, curData.pbUnit.stage + 1, out changeHealth, out changeStrength, out changeInteligence, out changeDefence, out changeSpeed);
            changeHealth -= currentHealth;
            changeStrength -= currentStrength;
            changeInteligence -= currentInteligence;
            changeDefence -= currentDefence;
            changeSpeed -= currentSpeed;
        }
        for (int i = 0; i < AttrList.Length; i++)
        {
            if (AttrList[i] != null)
            {
                AttrList[i].gameObject.SetActive(false);
            }
        }
        Action<int, string, float, float> action = (index, name, value, change) =>
        {
            if (AttrList[index] == null)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("ShowAttributesItem");
                UIUtil.SetParentReset(go.transform, BasePos);
                AttrList[index] = go.GetComponent<ShowAttributesItem>();
            }
            else
            {
                AttrList[index].gameObject.SetActive(true);
            }
            AttrList[index].SetValue(name, (int)value, (int)change);
        };
        action(0, StaticDataMgr.Instance.GetTextByID("common_attr_health"), currentHealth, changeHealth);
        action(1, StaticDataMgr.Instance.GetTextByID("common_attr_strenth"), currentStrength, changeStrength);
        action(2, StaticDataMgr.Instance.GetTextByID("common_attr_intelligence"), currentInteligence, changeInteligence);
        action(3, StaticDataMgr.Instance.GetTextByID("common_attr_defence"), currentDefence, changeDefence);
        action(4, StaticDataMgr.Instance.GetTextByID("common_attr_speed"), currentSpeed, changeSpeed);
        #endregion

        #region 设置是否满级状态
        if (maxStage)//检测是否达到最大等级
        {
            textAdvanceFull.gameObject.SetActive(true);
            objAdvance.SetActive(false);
            return;
        }
        else
        {
            textAdvanceFull.gameObject.SetActive(false);
            objAdvance.SetActive(true);
        }

        #endregion

        #region 设置升级材料

        unitStageData = StaticDataMgr.Instance.getUnitStageData(curData.pbUnit.stage + 1);
        if ((UIUtil.NeedChangeGrade(curData.pbUnit.stage) == true && unitStageData.demandMonsterList.Count < 1) || unitStageData.demandItemList.Count != 1)
        {
            Logger.LogError("宠物升级进阶材料配置错误");
            return;
        }

        #region 宠物需求等级设置
        enoughLevel = UIUtil.CheckIsEnoughLevel(curData);
        textLevelValue.text = unitStageData.demandLevel.ToString();//显示升级的等级需求
        textLevelValue.color = enoughLevel ? ColorConst.system_color_white : ColorConst.text_color_nReq;
        #endregion

        #region 进阶目标

        if (targetIcon==null)
        {
            targetIcon = MonsterIcon.CreateIcon();
            UIUtil.SetParentReset(targetIcon.transform, iconPos);
        }
        else
        {
            targetIcon.Init();
        }
        targetIcon.SetId(curData.pbUnit.guid.ToString());
        targetIcon.SetMonsterStaticId(curData.pbUnit.id);
        targetIcon.SetStage(curData.pbUnit.stage + 1);
        targetIcon.iconButton.gameObject.SetActive(false);
        #endregion

        #region 设置需求的物品
        ItemData itemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(unitStageData.demandItemList[0].itemId);
        ItemData tempData = new ItemData() { itemId = unitStageData.demandItemList[0].itemId, count = 0 };
        if (itemIcon==null)
        {
            itemIcon = ItemIcon.CreateItemIcon(tempData);
            UIUtil.SetParentReset(itemIcon.transform, itemPos);
        }
        else
        {
            itemIcon.RefreshWithItemInfo(tempData);
        }
        int current = Mathf.Clamp((itemData == null ? 0 : itemData.count), 0, 9999);
        int need = unitStageData.demandItemList[0].count;
        enoughItem = current > need;
        Color color;
        if (enoughItem)
        {
            color = ColorConst.system_color_white;
        }
        else
        {
            color = ColorConst.text_color_nReq;
        }
        textItem.text = "<color=" + ColorConst.colorTo_Hstr(color) + ">" + current + "</color>/" + need;

        #endregion

        #region 设置需求的金币

        enoughCoin = (GameDataMgr.Instance.PlayerDataAttr.coin >= unitStageData.demandCoin);
        textCoin.text = unitStageData.demandCoin.ToString();
        if (enoughCoin)
        {
            textCoin.color = ColorConst.text_color_Req;
        }
        else
        {
            textCoin.color = ColorConst.text_color_nReq;
        }
        #endregion

        #region 设置需求的宠物

        monsterFields.ForEach(delegate (NeedMonsterElement item) { item.gameObject.SetActive(false); });
        ItemInfo itemInfo;
        monsterSelect.Clear();
        for (int i = 0; i < unitStageData.demandMonsterList.Count; i++)
        {
            itemInfo = unitStageData.demandMonsterList[i];
            monsterSelect.Add(new List<int>());
            monsterFields[i].gameObject.SetActive(true);
            monsterFields[i].Refresh((itemInfo.itemId.Equals(BattleConst.stageSelfId) ? curData.pbUnit.id : itemInfo.itemId),
                                      itemInfo.stage/*,
                                      itemInfo.count,
                                      monsterSelect[i].Count*/);
        }
        UpdateMaterails();
        #endregion

        #region 设置显示需求背景

        for (int i = 0; i < ObjLine.Length; i++)
        {
            ObjLine[i].SetActive(i == (unitStageData.demandMonsterList.Count));
        }

        #endregion
        #endregion
    }

    void UpdateMaterails()
    {
        enoughMonster = true;
        for (int i = 0; i < unitStageData.demandMonsterList.Count; i++)
        {
            monsterFields[i].UpdateCount(unitStageData.demandMonsterList[i].count, monsterSelect[i].Count);
            if (monsterSelect[i].Count<unitStageData.demandMonsterList[i].count)
            {
                enoughMonster = false;
            }
        }
    }

    void OpenMonsterSelectUI(GameObject go)
    {
        ItemInfo itemInfo = null;

        selectMonsterPanel = UIMgr.Instance.OpenUI_(SelectMonsterPanel.ViewName, false) as SelectMonsterPanel;
        selectMonsterPanel.closeButton.onClick.AddListener(OnClickSelectPanelBtn);
        for (int i = 0; i < unitStageData.demandMonsterList.Count; i++)
        {
            if (go == monsterFields[i].button.gameObject)
            {
                itemInfo = unitStageData.demandMonsterList[i];
                selectMonsterPanel.init(unitStageData.demandMonsterList[i], monsterSelect, monsterSelect[i], curData);
                break;
            }
        }
    }
    void OnClickSelectPanelBtn()
    {
        if (selectMonsterPanel != null)
        {
            UIMgr.Instance.DestroyUI(selectMonsterPanel);
            selectMonsterPanel = null;
        }
        UpdateMaterails();
    }

    void OnClickAdvanceBtn()
    {
        if (isPlayingFX)
        {
            return;
        }
        if (!enoughLevel)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("monster_record_011"), (int)PB.ImType.PROMPT);
            return;
        }
        if (!enoughItem||!enoughMonster)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("monster_record_004"), (int)PB.ImType.PROMPT);
            return;
        }
        if(!enoughCoin)
        {
            GameDataMgr.Instance.ShopDataMgrAttr.JinbiNoEnough();
            return;
        }
        PB.HSMonsterStageUp resquest = new PB.HSMonsterStageUp();
        resquest.monsterId = curData.pbUnit.guid;
        for (int i = 0; i < monsterSelect.Count; i++)
        {
            for (int j = 0; j < monsterSelect[i].Count; j++)
            {
                resquest.consumeMonsterId.Add(monsterSelect[i][j]);
            }
        }
        GameApp.Instance.netManager.SendMessage(ProtocolMessage.Create(PB.code.MONSTER_STAGE_UP_C.GetHashCode(), resquest));
    }

    void OnCoinChangedRefresh(long coin)
    {
        ReloadData(curData);
    }

    void OnEnable()
    {
        BindListerner();
    }
    void OnDisable()
    {
        UnBindListerner();
    }
    void OnDestroy()
    {
        if (isPlayingFX)
        {
            EffectEnd();
        }
    }
    void EffectEnd()
    {
        isPlayingFX = false;
        GameEventMgr.Instance.FireEvent<GameUnit>(GameEventList.ReloadPetStageNotify, curData);
    }

    void BindListerner()
    {
        GameEventMgr.Instance.AddListener<long>(GameEventList.CoinChanged, OnCoinChangedRefresh);
        GameEventMgr.Instance.AddListener<GameUnit>(GameEventList.ReloadPetStageNotify, ReloadData);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_STAGE_UP_C.GetHashCode().ToString(), OnStageUpResponse);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_STAGE_UP_S.GetHashCode().ToString(), OnStageUpResponse);
    }
    void UnBindListerner()
    {
        GameEventMgr.Instance.RemoveListener<long>(GameEventList.CoinChanged, OnCoinChangedRefresh);
        GameEventMgr.Instance.RemoveListener<GameUnit>(GameEventList.ReloadPetStageNotify, ReloadData);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_STAGE_UP_C.GetHashCode().ToString(), OnStageUpResponse);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_STAGE_UP_S.GetHashCode().ToString(), OnStageUpResponse);
    }

    void OnStageUpResponse(ProtocolMessage msg)
    {
        UINetRequest.Close();

        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("重聚进阶错误");
            return;
        }
        if (unitStageData.demandMonsterList.Count <= 0)
        {
            isPlayingFX = true;
            advanceEffect.Play(1, EffectEnd);
        }
        else
        {
            isPlayingFX = true;
            advanceEffect.Play(2, EffectEnd);
        }

        curData.SetStage(curData.pbUnit.stage + 1);
    }
}
