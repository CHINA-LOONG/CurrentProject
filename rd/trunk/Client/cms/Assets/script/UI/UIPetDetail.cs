using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIPetDetail : UIBase{

    public static string ViewName = PetViewConst.UIPetDetailAssetName;

    public Button closeButton;
    public Button preButton;
    public Button nextButton;
    public Button skillButton;
    public Button attrButton;
    public Button stageButton;
    public Button advanceButton;
   // public Button equipButton;

    public Button addExpButton;
    public PetDetailLeft leftView;
    public GameObject rightView;

    PetDetailRightBase m_rightDetail = null;
    GameObject m_cameraObject = null;

    List<GameUnit> m_curTypeList = null;
    int m_currentIndex = 0;
    
    PetViewConst.RightPanelType currentRightType = PetViewConst.RightPanelType.NULL_RIGHT_TYPE;

    void Start()
    {
        EventTriggerListener.Get(closeButton.gameObject).onClick = CloseButtonDown;
        EventTriggerListener.Get(preButton.gameObject).onClick = PreButtonDown;
        EventTriggerListener.Get(nextButton.gameObject).onClick = NextButtonDown;
        EventTriggerListener.Get(skillButton.gameObject).onClick = SkillButtonDown;
        EventTriggerListener.Get(attrButton.gameObject).onClick = DetailAttrButtonDown;
        EventTriggerListener.Get(stageButton.gameObject).onClick = StageButtonDown;
        EventTriggerListener.Get(advanceButton.gameObject).onClick = AdvanceButtonDown;
        //EventTriggerListener.Get(equipButton.gameObject).onClick = EquipButtonDown;
    }

    void CloseButtonDown(GameObject go)
    {
        UIMgr.Instance.CloseUI(UIPetDetail.ViewName);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
    }

    public override void OnOpenUI()
    {
        m_cameraObject = ResourceMgr.Instance.LoadAsset(PetViewConst.UIPetModelCameraAssetName);
        m_cameraObject.name = PetViewConst.UIPetModelCameraAssetName;
    }

    public override void OnCloseUI()
    {
        if (m_cameraObject != null)
        {
            ResourceMgr.Instance.DestroyAsset(m_cameraObject);
        }
    }

    void AddRightView(string assetName)
    {
        //clear
        if (m_rightDetail != null)
        {
            ResourceMgr.Instance.DestroyAsset(m_rightDetail.gameObject);
            m_rightDetail = null;
        }

        m_rightDetail = ResourceMgr.Instance.LoadAsset(assetName).GetComponent<PetDetailRightBase>();
        m_rightDetail.transform.SetParent(rightView.transform, false);
        m_rightDetail.transform.localScale = Vector3.one;
        m_rightDetail.gameObject.name = "contentView";
    }

    void SkillButtonDown(GameObject go)
    {
        if (currentRightType == PetViewConst.RightPanelType.SKILL_PANEL_TYPE)
        {
            return;
        }

        currentRightType = PetViewConst.RightPanelType.SKILL_PANEL_TYPE;
        AddRightView(PetViewConst.UIPetSkillAssetName);
        m_rightDetail.ReloadData(m_curTypeList[m_currentIndex]);
    }

    void DetailAttrButtonDown(GameObject go)
    {
        if (currentRightType == PetViewConst.RightPanelType.DETAIL_ATTR_TYPE)
        {
            return;
        }

        currentRightType = PetViewConst.RightPanelType.DETAIL_ATTR_TYPE;
        AddRightView(PetViewConst.UIPetAttrAssetName);
        m_rightDetail.ReloadData(m_curTypeList[m_currentIndex]);
    }

    void StageButtonDown(GameObject go)
    {
        if (currentRightType == PetViewConst.RightPanelType.STAGE_PANEL_TYPE)
        {
            return;
        }

        currentRightType = PetViewConst.RightPanelType.STAGE_PANEL_TYPE;
        AddRightView(PetViewConst.UIPetStageAssetName);
        m_rightDetail.ReloadData(m_curTypeList[m_currentIndex]);
    }

    void AdvanceButtonDown(GameObject go)
    {
        if (currentRightType == PetViewConst.RightPanelType.ADVANCE_PANEL_TYPE)
        {
            return;
        }

        currentRightType = PetViewConst.RightPanelType.ADVANCE_PANEL_TYPE;
        AddRightView(PetViewConst.UIPetAdvanceAssetName);
        m_rightDetail.ReloadData(m_curTypeList[m_currentIndex]);
    }

    void EquipButtonDown(GameObject go)
    {
        if (currentRightType == PetViewConst.RightPanelType.EQUIP_PANEL_TYPE)
        {
            return;
        }

        currentRightType = PetViewConst.RightPanelType.EQUIP_PANEL_TYPE;
        AddRightView(PetViewConst.UIPetEquipAssetName);
        m_rightDetail.ReloadData(m_curTypeList[m_currentIndex]);
    }

    void PreButtonDown(GameObject go)
    {
        if (m_curTypeList.Count == 1)
        {
            return;
        }

        m_currentIndex = (m_currentIndex - 1 + m_curTypeList.Count) % m_curTypeList.Count;
        ReloadData(m_curTypeList[m_currentIndex]);
    }

    void NextButtonDown(GameObject go)
    {
        if (m_curTypeList.Count == 1)
        {
            return;
        }

        m_currentIndex = (m_currentIndex + 1) % m_curTypeList.Count;
        ReloadData(m_curTypeList[m_currentIndex]);
    }

    public void SetTypeList(GameUnit unit, List<GameUnit> unitList)
    {
        m_curTypeList = unitList;
        m_currentIndex = m_curTypeList.Count;
        for (int i = 0; i < m_curTypeList.Count; ++i)
        {
            if (m_curTypeList[i] == unit)
            {
                m_currentIndex = i;
                break;
            }
        }

        if (m_currentIndex == m_curTypeList.Count)
        {
            return;
        }

        //默认选中属性界面
        SkillButtonDown(null);
        leftView.ReloadData(unit);
    }

    void ReloadData(GameUnit unit)
    {
        leftView.ReloadData(unit);
        m_rightDetail.ReloadData(unit);
    }
}
