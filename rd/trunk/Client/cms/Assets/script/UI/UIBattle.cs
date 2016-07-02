using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIBattle : UIBase
{
    public static string ViewName = "UIBattle";
	public static string AssertName = "ui/battle";

    public Image m_MirrorImage = null;
    public Button m_ButtonLeft = null;
    public Button m_ButtonDaoju = null;
    public Button m_ButtonSpeed = null;
    public List<Image> m_SpeedNumImageList = new List<Image>();
    public Toggle m_ToggleMirror = null;
    public Toggle m_ToggleBattleStyle = null;

    public PetSwitchPage m_PetPanel;

    private MirrorDray m_MirrorDray = null;

    private int m_BattleSpeed = 1;

    // Use this for initialization
    void Start()
    {
        if (null != m_MirrorImage)
        {
            m_MirrorDray = m_MirrorImage.gameObject.AddComponent<MirrorDray>();
        }
        else
        {
            Debug.LogError("You Should set MirrorImage in the UIBattle prefab!");
        }

        m_ToggleMirror.isOn = false;
        m_MirrorImage.gameObject.SetActive(false);
        m_PetPanel.gameObject.SetActive(false);

        AddUIObjectEvent();
        BindListener();
    }

    void OnDestory()
    {
        UnBindListener();
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.ShowSwitchPetUI, OnShowSwitchPetUIAtIndex);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.ShowSwitchPetUI, OnShowSwitchPetUIAtIndex);
    }

    void AddUIObjectEvent()
    {
        EventTriggerListener.Get(m_ButtonLeft.gameObject).onClick = OnButtonLeftCllicked;
        EventTriggerListener.Get(m_ButtonDaoju.gameObject).onClick = OnButtonDaojuClicked;
        EventTriggerListener.Get(m_ButtonSpeed.gameObject).onClick = OnButtonSpeedClicked;

        EventTriggerListener.Get(m_ToggleMirror.gameObject).onClick = OnToggleMirrorClicked;
        EventTriggerListener.Get(m_ToggleBattleStyle.gameObject).onClick = OnToggleBattleStyleClicked;
    }

    void OnButtonLeftCllicked(GameObject go)
    {
        //m_PetPanelGameObject.SetActive (true);
        GameEventMgr.Instance.FireEvent<int>(GameEventList.ShowSwitchPetUI, 1);
    }

    void OnButtonDaojuClicked(GameObject go)
    {
        GameEventMgr.Instance.FireEvent(GameEventList.HideSwitchPetUI);
    }

    void OnButtonSpeedClicked(GameObject go)
    {
        m_BattleSpeed++;
        if (m_BattleSpeed > 3)
        {
            m_BattleSpeed = 1;
        }

        Image subImg = null;
        for (int i = 0; i < m_SpeedNumImageList.Count; ++i)
        {
            subImg = m_SpeedNumImageList[i] as Image;
            if (i + 1 == m_BattleSpeed)
            {
                subImg.gameObject.SetActive(true);
            }
            else
            {
                subImg.gameObject.SetActive(false);
            }
        }

        Debug.Log("battle Speed = " + m_BattleSpeed);
    }

    void OnToggleMirrorClicked(GameObject go)
    {
        bool isMirrorMode = m_ToggleMirror.isOn;
        m_MirrorImage.gameObject.SetActive(isMirrorMode);
        if (isMirrorMode)
        {
            Vector3 tempPos = m_MirrorImage.transform.localPosition;
            tempPos.x = 0;
            tempPos.y = 0;

            m_MirrorImage.transform.localPosition = tempPos;
        }
    }

    void OnToggleBattleStyleClicked(GameObject go)
    {

    }

    //switch pet
    void OnShowSwitchPetUIAtIndex(EventArgs sArgs)
    {
        var args = sArgs as ShowSwitchPetUIArgs;
        m_PetPanel.Show(args);

        GameEventMgr.Instance.AddListener(GameEventList.HideSwitchPetUI, OnHideSwitchPetUI);
    }

    void OnHideSwitchPetUI()
    {
        m_PetPanel.Hide();

        GameEventMgr.Instance.RemoveListener(GameEventList.HideSwitchPetUI, OnHideSwitchPetUI);
    }
}
