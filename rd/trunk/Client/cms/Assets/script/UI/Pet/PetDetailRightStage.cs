using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PetDetailRightStage : PetDetailRightBase{
   
    public StageAttrPanel curAttrPanel;
    public StageAttrPanel nextAttrPanel;

    public GameObject stageUpView;
    public GameObject stageFullView;

    public Image item1;
    public Image item2;
    public Image monster1Add;
    public Image monster2Add;
    public MonsterIcon monster1Icon;
    public MonsterIcon monster2Icon;

    public PetMaterialLabel item1Label;
    public PetMaterialLabel item2Label;
    public PetMaterialLabel monster1Label;
    public PetMaterialLabel monster2Label;

    public Text demandLabel;
    public Text levelLabel;
    public Text levelValue;
    public Text attrLabel;
    public Text coinLabel;
    public Text stageFull;

    public Button stageButton;

    GameObject m_selectMonsterView;
    List<int> m_monsterList1 = new List<int>();
    List<int> m_monsterList2 = new List<int>();
    GameUnit m_unit;

    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(monster1Add.gameObject).onClick = OpenMonsterSelectUI;
        EventTriggerListener.Get(monster2Add.gameObject).onClick = OpenMonsterSelectUI;
        EventTriggerListener.Get(monster1Icon.gameObject).onClick = OpenMonsterSelectUI;
        EventTriggerListener.Get(monster2Icon.gameObject).onClick = OpenMonsterSelectUI;
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
        m_monsterList1.Clear();
        m_monsterList2.Clear();

        m_unit = param.unit;

        stageFullView.SetActive(true);
        stageUpView.SetActive(true);

        levelLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageDemandLevel);
        demandLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageDemandItem);
        attrLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageAttr);
        stageFull.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageFull);
        stageButton.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStage);

        curAttrPanel.ReloadData(m_unit, m_unit.pbUnit.stage);

        if (m_unit.pbUnit.stage == GameConfig.MaxMonsterStage)
        {
            stageFullView.SetActive(true);
            stageUpView.SetActive(false);
            nextAttrPanel.ReloadData(m_unit, GameConfig.MaxMonsterStage);
            return;
        }
        else
        {
            stageFullView.SetActive(false);
            stageUpView.SetActive(true);
            nextAttrPanel.ReloadData(m_unit, m_unit.pbUnit.stage + 1);
        }

        UnitStageData unitStageData = StaticDataMgr.Instance.getUnitStageData(m_unit.pbUnit.stage + 1);
        levelValue.text = unitStageData.demandLevel.ToString();
        if (UIUtil.CheckIsEnoughLevel(m_unit) == false)
        {
            levelValue.color = ColorConst.text_color_nReq;
            levelValue.GetComponent<Outline>().effectColor = ColorConst.outline_color_nReq;
        }
        else
        {

            levelValue.color = ColorConst.text_color_Req;
            levelValue.GetComponent<Outline>().effectColor = ColorConst.outline_color_Req;
        }

        if ((UIUtil.NeedChangeGrade(m_unit.pbUnit.stage) == true && unitStageData.demandMonsterList.Count != 2) || unitStageData.demandItemList.Count != 2)
        {
            return;
        }

        if (UIUtil.NeedChangeGrade(m_unit.pbUnit.stage) == true)
        {
            monster1Add.gameObject.SetActive(true);
            monster2Add.gameObject.SetActive(true);
            monster1Icon.gameObject.SetActive(true);
            monster2Icon.gameObject.SetActive(true);
            monster1Label.gameObject.SetActive(true);
            monster2Label.gameObject.SetActive(true);

            monster1Icon.Init();
            monster2Icon.Init();
            monster1Icon.SetMonsterStaticId(unitStageData.demandMonsterList[0].itemId);
            monster1Icon.SetStage(unitStageData.demandMonsterList[0].stage);
            monster2Icon.SetMonsterStaticId(unitStageData.demandMonsterList[1].itemId);
            monster2Icon.SetStage(unitStageData.demandMonsterList[1].stage);
        }
        else
        {
            monster1Add.gameObject.SetActive(false);
            monster2Add.gameObject.SetActive(false);
            monster1Icon.gameObject.SetActive(false);
            monster2Icon.gameObject.SetActive(false);
            monster1Label.gameObject.SetActive(false);
            monster2Label.gameObject.SetActive(false);
        }

        UpdateMaterails();
    }

    void OpenMonsterSelectUI(GameObject go)
    {
        UnitStageData unitStageData = StaticDataMgr.Instance.getUnitStageData(m_unit.pbUnit.stage + 1);
        ItemInfo itemInfo  = null;
        if (go == monster1Icon.gameObject || go == monster1Add.gameObject)
        {
            itemInfo = unitStageData.demandMonsterList[0];
        }
        else if (go == monster2Icon.gameObject || go == monster2Add.gameObject)
        {
            itemInfo = unitStageData.demandMonsterList[1];
        }      

        m_selectMonsterView = ResourceMgr.Instance.LoadAsset(PetViewConst.UIPetStageMonsterSelectAssetName);
        m_selectMonsterView.transform.localScale = Vector3.one;
        m_selectMonsterView.transform.localPosition = Vector3.zero;
        m_selectMonsterView.transform.localEulerAngles = Vector3.zero;
        m_selectMonsterView.transform.SetParent(gameObject.transform.parent.parent, false);
        EventTriggerListener.Get(m_selectMonsterView.GetComponent<SelectMonsterPanel>().closeButton.gameObject).onClick = CloseButtonDown;

        if (go == monster1Icon.gameObject || go == monster1Add.gameObject)
        {
            m_selectMonsterView.GetComponent<SelectMonsterPanel>().init(unitStageData.demandMonsterList[0], m_monsterList1, m_unit.pbUnit.guid);
        }
        else if (go == monster2Icon.gameObject || go == monster2Add.gameObject)
        {
            m_selectMonsterView.GetComponent<SelectMonsterPanel>().init(unitStageData.demandMonsterList[1], m_monsterList2, m_unit.pbUnit.guid);
        }
    }

    void CloseButtonDown(GameObject go)
    {
        if (m_selectMonsterView != null)
        {
            ResourceMgr.Instance.DestroyAsset(m_selectMonsterView);
            m_selectMonsterView = null;
        }

        UpdateMaterails();
    }

    void UpdateMaterails()
    {
        UnitStageData unitStageData = StaticDataMgr.Instance.getUnitStageData(m_unit.pbUnit.stage + 1);
        ItemData itemData = null;
        itemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(unitStageData.demandItemList[0].itemId);
        item1Label.ReloadData(itemData == null ? 0 : itemData.count, unitStageData.demandItemList[0].count, 9999);

        itemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(unitStageData.demandItemList[1].itemId);
        item2Label.ReloadData(itemData == null ? 0 : itemData.count, unitStageData.demandItemList[1].count, 9999);

        if (UIUtil.NeedChangeGrade(m_unit.pbUnit.stage) == true)
        {
            monster1Label.ReloadData(m_monsterList1.Count, unitStageData.demandMonsterList[0].count, 9999);
            monster2Label.ReloadData(m_monsterList2.Count, unitStageData.demandMonsterList[1].count, 9999);
        }

        if (UIUtil.NeedChangeGrade(m_unit.pbUnit.stage) == true)
        {
            if (m_monsterList1.Count > 0)
            {
                monster1Add.gameObject.SetActive(false);
                monster1Icon.gameObject.SetActive(true);
            }
            else
            {
                monster1Add.gameObject.SetActive(true);
                monster1Icon.gameObject.SetActive(false);
            }

            if (m_monsterList2.Count > 0)
            {
                monster2Add.gameObject.SetActive(false);
                monster2Icon.gameObject.SetActive(true);
            }
            else
            {
                monster2Add.gameObject.SetActive(true);
                monster2Icon.gameObject.SetActive(false);
            }
        }

        coinLabel.text = unitStageData.demandCoin.ToString();
        ResetMaterialColor(coinLabel, unitStageData.demandCoin, (int)GameDataMgr.Instance.PlayerDataAttr.coin);      

        if (UIUtil.CheckIsEnoughMaterial(m_unit) == true)
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
        foreach (int monsterId in m_monsterList1)
        {
            resquest.consumeMonsterId.Add(monsterId);
        }
        foreach (int monsterId in m_monsterList2)
        {
            resquest.consumeMonsterId.Add(monsterId);
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

        m_unit.pbUnit.stage += 1;
        ReloadData(new PetRightParamBase() { unit = m_unit });

        GameEventMgr.Instance.FireEvent(PetViewConst.ReloadPetStageNotify);
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
            label.color = Color.red;
        }
        else
        {
            label.color = Color.black;
        }
    }
}
