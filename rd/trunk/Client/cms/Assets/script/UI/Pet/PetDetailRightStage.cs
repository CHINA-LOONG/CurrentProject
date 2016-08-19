using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PetDetailRightStage : PetDetailRightBase{
   
    public StageAttrPanel curAttrPanel;
    public StageAttrPanel nextAttrPanel;

    public GameObject stageUpView;
    public GameObject stageFullView;

    public ItemIcon itemIcon;
    public Text itemCount;

    public List<NeedMonsterElement> monsterFields;

    public Text demandLabel;
    public Text levelLabel;
    public Text levelValue;
    public Text attrLabel;
    public Text coinLabel;
    public Text stageFull;

    public Button stageButton;

    private SelectMonsterPanel selectMonsterPanel;
    List<List<int>> monsterSelect = new List<List<int>>();
    List<int> m_monsterList1 = new List<int>();
    List<int> m_monsterList2 = new List<int>();
    GameUnit m_unit;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < monsterFields.Count; i++)
        {
            EventTriggerListener.Get(monsterFields[i].button.gameObject).onClick = OpenMonsterSelectUI;
        }
    }

    void OnEnable()
    {
        BindListerner();
    }

    void OnDisable()
    {
        UnBindListerner();
    }

    void BindListerner()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_STAGE_UP_C.GetHashCode().ToString(), OnStageUpResponse);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_STAGE_UP_S.GetHashCode().ToString(), OnStageUpResponse);
    }

    void UnBindListerner()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_STAGE_UP_C.GetHashCode().ToString(), OnStageUpResponse);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_STAGE_UP_S.GetHashCode().ToString(), OnStageUpResponse);
    }

    public override void ReloadData(PetRightParamBase param)
    {
        monsterSelect.Clear();
        //获取宠物信息
        m_unit = param.unit;
        //显示满级和升级界面   …………应该没啥用了
        stageFullView.SetActive(true);
        stageUpView.SetActive(true);

        levelLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageDemandLevel);
        demandLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageDemandItem);
        attrLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageAttr);
        stageFull.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageFull);
        stageButton.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStage);

        curAttrPanel.ReloadData(m_unit, m_unit.pbUnit.stage);//更新当前宠物的属性和图标

        if (m_unit.pbUnit.stage == GameConfig.MaxMonsterStage)//检测是否达到最大等级
        {
            stageFullView.SetActive(true);          //显示满级的文字提示
            stageUpView.SetActive(false);           //隐藏升级界面
            nextAttrPanel.ReloadData(m_unit, GameConfig.MaxMonsterStage);//更新下一级的宠物提示（满级是当前级）
            return;
        }
        else
        {
            stageFullView.SetActive(false);
            stageUpView.SetActive(true);
            nextAttrPanel.ReloadData(m_unit, m_unit.pbUnit.stage + 1);//显示下一级的宠物提示
        }
        UnitStageData unitStageData = StaticDataMgr.Instance.getUnitStageData(m_unit.pbUnit.stage + 1);
        levelValue.text = unitStageData.demandLevel.ToString();//显示升级的等级需求
        if (UIUtil.CheckIsEnoughLevel(m_unit) == false)//检测宠物是否达到升级下一级的需求
        {
            levelValue.color = ColorConst.text_color_nReq;
        }
        else
        {

            levelValue.color = ColorConst.text_color_Req;
        }
        //modify： xuelong 2015-9-24 10:56:12  
        //TODO：
        if ((UIUtil.NeedChangeGrade(m_unit.pbUnit.stage) == true && unitStageData.demandMonsterList.Count < 1) || unitStageData.demandItemList.Count != 1)
        {
            Logger.LogError("宠物升级进阶材料配置错误");
            return;
        }
        #region 设置需要的物品材料

        ItemData itemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(unitStageData.demandItemList[0].itemId);
        itemIcon.RefreshWithItemInfo(new ItemData() { itemId = unitStageData.demandItemList[0].itemId, count = 0 });
        int current = Mathf.Clamp((itemData == null ? 0 : itemData.count), 0, 9999);
        int need = unitStageData.demandItemList[0].count;
        Color color;
        if (need > current)
        {
            color = ColorConst.text_color_nReq;
        }
        else
        {
            color = ColorConst.text_color_Req;
        }

        itemCount.text = "<color=" + ColorConst.colorTo_Hstr(color) + ">" + current + "</color>/" + need;

        #endregion

        #region 设置需求的金币
        coinLabel.text = unitStageData.demandCoin.ToString();
        ResetMaterialColor(coinLabel, unitStageData.demandCoin, (int)GameDataMgr.Instance.PlayerDataAttr.coin); 
        #endregion
        
        monsterFields.ForEach(delegate(NeedMonsterElement item) { item.gameObject.SetActive(false); });
        ItemInfo itemInfo;
        for (int i = 0; i < unitStageData.demandMonsterList.Count; i++)
        {
            itemInfo = unitStageData.demandMonsterList[i];
            monsterSelect.Add(new List<int>());
            monsterFields[i].gameObject.SetActive(true);
            monsterFields[i].Refresh((itemInfo.itemId.Equals(BattleConst.stageSelfId) ? m_unit.pbUnit.id : itemInfo.itemId),
                                   itemInfo.stage,
                                   itemInfo.count,
                                   monsterSelect[i].Count);
        }

        UpdateMaterails();
    }

    void OpenMonsterSelectUI(GameObject go)
    {
        UnitStageData unitStageData = StaticDataMgr.Instance.getUnitStageData(m_unit.pbUnit.stage + 1);
        ItemInfo itemInfo  = null;

        selectMonsterPanel = UIMgr.Instance.OpenUI_(SelectMonsterPanel.ViewName, false) as SelectMonsterPanel;
        EventTriggerListener.Get(selectMonsterPanel.closeButton.gameObject).onClick = CloseButtonDown;
        
        for (int i = 0; i < unitStageData.demandMonsterList.Count; i++)
        {
            if (go==monsterFields[i].button.gameObject)
            {
                itemInfo = unitStageData.demandMonsterList[i];
                selectMonsterPanel.init(unitStageData.demandMonsterList[i], monsterSelect, monsterSelect[i], m_unit/*.pbUnit.guid*/);
                break;
            }
        }
    }

    void CloseButtonDown(GameObject go)
    {
        if (selectMonsterPanel != null)
        {
            UIMgr.Instance.DestroyUI(selectMonsterPanel);
            selectMonsterPanel = null;
        }

        UpdateMaterails();
    }

    void UpdateMaterails()
    {
        UnitStageData unitStageData = StaticDataMgr.Instance.getUnitStageData(m_unit.pbUnit.stage + 1);
        for (int i = 0; i < unitStageData.demandMonsterList.Count; i++)
        {
            monsterFields[i].UpdateCount(unitStageData.demandMonsterList[i].count, monsterSelect[i].Count);
        }
        if (UIUtil.CheckIsEnoughMaterial(m_unit))
        {
            stageButton.interactable = true;
            EventTriggerListener.Get(stageButton.gameObject).onClick = StageUpButtonDown;
        }
        else
        {
            stageButton.interactable = false;
            EventTriggerListener.Get(stageButton.gameObject).onClick = null;
        }
    }

    void StageUpButtonDown(GameObject go)
    {
        PB.HSMonsterStageUp resquest = new PB.HSMonsterStageUp();
        resquest.monsterId = m_unit.pbUnit.guid;
        for (int i = 0; i < monsterSelect.Count; i++)
        {
            for (int j = 0; j < monsterSelect[i].Count; j++)
            {
                resquest.consumeMonsterId.Add(monsterSelect[i][j]);
            }
        }
        GameApp.Instance.netManager.SendMessage(ProtocolMessage.Create(PB.code.MONSTER_STAGE_UP_C.GetHashCode(), resquest));
    }

    void OnStageUpResponse(ProtocolMessage msg)
    {
        UINetRequest.Close();

        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            return;
        }

        //m_unit.pbUnit.stage += 1;
        m_unit.SetStage(m_unit.pbUnit.stage + 1);
        ReloadData(new PetRightParamBase() { unit = m_unit });

        GameEventMgr.Instance.FireEvent<GameUnit>(GameEventList.ReloadPetStageNotify,m_unit);
    }

    string ShowFormatMaterailCount(int currentCount, int needCount, int currentMaxCount)
    {
        currentCount = currentCount > currentMaxCount ? currentMaxCount : currentCount;
        return string.Format("{0}/{1}", currentCount, needCount);
    }

    void ResetMaterialColor(Text label, int targetCount, int currentCount)
    {
        if (currentCount < targetCount)
        {
            label.color = ColorConst.text_color_nReq;
        }
        else
        {
            label.color = ColorConst.text_color_Req;
        }
    }
}
