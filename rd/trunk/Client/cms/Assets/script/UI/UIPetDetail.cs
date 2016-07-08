using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIPetDetail : UIBase{

    public static string ViewName = "UIPetDetail";
    public static string AssertName = "ui/petdetail";

    public Button m_closeButton;

    public PetDetailLeft m_leftView;
    public PetDetailRight m_rightView;

    private GameObject m_cameRaObject;
    public Button m_preButton;
    public Button m_nextButton;

    public Button m_skillButton;
    public Button m_attrButton;
    public Button m_stageButton;

    private List<GameUnit> m_curTypeList = null;
    private int m_currentIndex = 0;

    void Start()
    {
        EventTriggerListener.Get(m_closeButton.gameObject).onClick = CloseBagButtonDown;
        EventTriggerListener.Get(m_preButton.gameObject).onClick = PreButtonDown;
        EventTriggerListener.Get(m_nextButton.gameObject).onClick = NextButtonDown;
        EventTriggerListener.Get(m_skillButton.gameObject).onClick = SkillButtonDown;
        EventTriggerListener.Get(m_attrButton.gameObject).onClick = DetailAttrButtonDown;
        EventTriggerListener.Get(m_stageButton.gameObject).onClick = StageButtonDown;
    }

    void CloseBagButtonDown(GameObject go)
    {
        UIMgr.Instance.CloseUI(ViewName);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnOpenUI()
    {
        m_cameRaObject = ResourceMgr.Instance.LoadAsset("ui/petdetail", "petCamera");
        m_cameRaObject.name = "petCamera";
    }

    public override void OnCloseUI()
    {
        if (m_cameRaObject != null)
        {
            ResourceMgr.Instance.DestroyAsset(m_cameRaObject);
        }
    }

    void AddRightView(string assetName)
    {
        //clear
        GameObject rightView = Util.FindChildByName(m_rightView.gameObject, "contentView");
        if (rightView != null)
        {
            ResourceMgr.Instance.DestroyAsset(rightView);
        }

        GameObject newView = ResourceMgr.Instance.LoadAsset("ui/petdetail", assetName);
        newView.transform.SetParent(m_rightView.transform);
        newView.transform.localPosition = Vector3.zero;
        newView.transform.localScale = Vector3.one;
        newView.name = "contentView";
    }

    void SkillButtonDown(GameObject go)
    {
        AddRightView("skillPanel");
    }

    void DetailAttrButtonDown(GameObject go)
    {
        AddRightView("attrPanel");
    }

    void StageButtonDown(GameObject go)
    {
        AddRightView("stagePanel");
    }

    void PreButtonDown(GameObject go)
    {
        m_currentIndex = (m_currentIndex - 1 + m_curTypeList.Count) % m_curTypeList.Count;
        ReloadUnitData(m_curTypeList[m_currentIndex]);
    }

    void NextButtonDown(GameObject go)
    {
        m_currentIndex = (m_currentIndex + 1) % m_curTypeList.Count;
        ReloadUnitData(m_curTypeList[m_currentIndex]);
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

        ReloadUnitData(unit);
    }

    void ReloadUnitData(GameUnit unit)
    {
        m_leftView.ReloadData(unit);
        m_rightView.ReloadData(unit);
    }
}
