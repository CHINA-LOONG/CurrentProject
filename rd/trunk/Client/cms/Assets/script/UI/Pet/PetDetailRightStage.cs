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
    public Image monster1Icon;
    public Image monster2Icon;

    public Text item1Label;
    public Text item2Label;
    public Text monster1Label;
    public Text monster2Label;

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
        EventTriggerListener.Get(monster1Icon.gameObject).onClick = OpenMonsterSelectUI;
        EventTriggerListener.Get(monster2Icon.gameObject).onClick = OpenMonsterSelectUI;
        BindListerner();
    }

    void OnDestroy()
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
    // Update is called once per frame
    void Update()
    {

    }

    override public void ReloadData(GameUnit unit)
    {
        m_monsterList1.Clear();
        m_monsterList2.Clear();
        m_unit = unit;

        stageFullView.SetActive(true);
        stageUpView.SetActive(true);

        levelLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageDemandLevel);
        demandLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageDemandItem);
        attrLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageAttr);
        stageFull.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageFull);
        stageButton.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStage);

        curAttrPanel.ReloadData(unit, unit.pbUnit.stage);

        if (unit.pbUnit.stage == GameConfig.MaxMonsterStage)
        {
            stageFullView.SetActive(true);
            stageUpView.SetActive(false);
            nextAttrPanel.ReloadData(unit, GameConfig.MaxMonsterStage);
            return;
        }
        else
        {
            stageFullView.SetActive(false);
            stageUpView.SetActive(true);
            nextAttrPanel.ReloadData(unit, unit.pbUnit.stage + 1);
        }

        UnitStageData unitStageData = StaticDataMgr.Instance.getUnitStageData(unit.pbUnit.stage + 1);
        levelValue.text = unitStageData.demandLevel.ToString();
        if (UIUtil.CheckIsEnoughLevel(m_unit) == false)
        {
            levelValue.color = Color.red;
        }
        else
        {
            levelValue.color = Color.black;
        }

        if ((UIUtil.NeedChangeGrade(unit.pbUnit.stage) == true && unitStageData.demandMonsterList.Count != 2) || unitStageData.demandItemList.Count != 2)
        {
            return;
        }

        if (UIUtil.NeedChangeGrade(unit.pbUnit.stage) == true)
        {
            monster1Icon.gameObject.SetActive(true);
            monster2Icon.gameObject.SetActive(true);
            monster1Label.gameObject.SetActive(true);
            monster2Label.gameObject.SetActive(true);
        }
        else
        {
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
        if (go == monster1Icon.gameObject)
        {
            itemInfo = unitStageData.demandMonsterList[0];
        }
        else if (go == monster2Icon.gameObject)
        {
            itemInfo = unitStageData.demandMonsterList[1];
        }
         
        List<GameUnit> material = GameDataMgr.Instance.PlayerDataAttr.GetAllPet(itemInfo.itemId, itemInfo.stage);
        material.Remove(m_unit);
        if (material.Count == 0)
        {
            MsgBox.PromptMsg.Open("提示","当前无此宠物","确定");
            return;
        }

        m_selectMonsterView = ResourceMgr.Instance.LoadAsset(PetViewConst.UIPetStageMonsterSelectAssetName);
        m_selectMonsterView.transform.localScale = Vector3.one;
        m_selectMonsterView.transform.localPosition = Vector3.zero;
        m_selectMonsterView.transform.localEulerAngles = Vector3.zero;
        m_selectMonsterView.transform.SetParent(gameObject.transform.parent.parent, false);
        EventTriggerListener.Get(m_selectMonsterView.GetComponent<SelectMonsterPanel>().closeButton.gameObject).onClick = CloseButtonDown;
        
        if (go == monster1Icon.gameObject)
        {
            m_selectMonsterView.GetComponent<SelectMonsterPanel>().init(unitStageData.demandMonsterList[0], m_monsterList1, m_unit.pbUnit.guid);
        }
        else if (go == monster2Icon.gameObject)
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
        item1Label.text = ShowFormatMaterailCount(itemData == null ? 0 : itemData.count, unitStageData.demandItemList[0].count, 999);
        if (itemData == null || itemData.count < unitStageData.demandItemList[0].count)
        {
            item1Label.color = Color.red;
        }
        else
        {
            item1Label.color = Color.black;
        }

        itemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(unitStageData.demandItemList[1].itemId);
        item2Label.text = ShowFormatMaterailCount(itemData == null ? 0 : itemData.count, unitStageData.demandItemList[1].count, 999);
        if (itemData == null || itemData.count < unitStageData.demandItemList[1].count)
        {
            item2Label.color = Color.red;
        }
        else
        {
            item2Label.color = Color.black;
        }

        if (UIUtil.NeedChangeGrade(m_unit.pbUnit.stage) == true)
        {
            monster1Label.text = string.Format("{0}/{1}", m_monsterList1.Count, unitStageData.demandMonsterList[0].count);
            monster2Label.text = string.Format("{0}/{1}", m_monsterList2.Count, unitStageData.demandMonsterList[1].count);
        }

        coinLabel.text = unitStageData.demandCoin.ToString();
        if (GameDataMgr.Instance.PlayerDataAttr.coin >= unitStageData.demandCoin)
        {
            coinLabel.color = Color.black;
        }
        else
        {
            coinLabel.color = Color.red;
        }

        if (UIUtil.CheckIsEnoughMaterial(m_unit, false) == true && UIUtil.CheckIsEnoughLevel(m_unit) == true)
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
        ReloadData(m_unit);

        GameEventMgr.Instance.FireEvent(PetViewConst.ReloadPetStageNotify);
    }

    string ShowFormatMaterailCount(int currentCount, int needCount, int currentMaxCount)
    {
        currentCount = currentCount > currentMaxCount ? currentMaxCount : currentCount;
        if (currentCount< 10)
        {
            if (needCount < 10)
                return string.Format("{0}/{1}",currentCount,needCount);
            else if (needCount < 100)
                return string.Format(" {0}/{1}", currentCount, needCount);
            else
                return string.Format("  {0}/{1}", currentCount, needCount);
        }
        else if (currentCount < 100)
        {
            if (needCount < 10)
                return string.Format("{0}/{1} ", currentCount, needCount);
            else if (needCount < 100)
                return string.Format("{0}/{1}", currentCount, needCount);
            else
                return string.Format(" {0}/{1}", currentCount, needCount);
        }
        else
        {
            if (needCount < 10)
                return string.Format("{0}/{1}  ", currentCount, needCount);
            else if (needCount < 100)
                return string.Format("{0}/{1} ", currentCount, needCount);
            else
                return string.Format("{0}/{1}", currentCount, needCount);
        }

    }
}
