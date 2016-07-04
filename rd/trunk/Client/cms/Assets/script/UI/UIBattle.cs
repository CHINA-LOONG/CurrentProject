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
    public HomeButton m_ButtonDaoju = null;
    public Button m_ButtonSpeed = null;
    public List<Image> m_SpeedNumImageList = new List<Image>();
	public HomeButton m_ButtonMirror = null;
    public HomeButton m_ButtonTuoguan = null;
	public HomeButton m_ButtonMomo = null;

    public BattleGroupUI m_PlayerGroupUI;
    public PetSwitchPage m_PetPanel;
    public Text dazhaoTip;

    private MirrorDray m_MirrorDray = null;

    private int m_BattleSpeed = 1;
	private	int	m_MaxSpeed = 3;

    // Use this for initialization
    public void Init()
    {
        if (null != m_MirrorImage)
        {
            m_MirrorDray = m_MirrorImage.gameObject.AddComponent<MirrorDray>();
			m_MirrorDray.Init();
			//Debug.LogError("liwsTest: UIBattle is Load And MirrorDray component added");
        }
        else
        {
            Debug.LogError("You Should set MirrorImage in the UIBattle prefab!");
        }

		m_ButtonMirror.IsOn = false;
        m_MirrorImage.gameObject.SetActive(false);
        m_PetPanel.gameObject.SetActive(false);
        dazhaoTip.gameObject.SetActive(false);

        m_PlayerGroupUI.gameObject.SetActive(true);
        m_PlayerGroupUI.Init(BattleController.Instance.BattleGroup.PlayerFieldList);

        AddUIObjectEvent();
        BindListener();

		UpdateButton ();
    }

    void OnDestroy()
    {
        UnBindListener();
    }

    void Update()
    {
        if (dazhaoTip.gameObject.activeSelf)
        {
            dazhaoTip.text = "大招模式点点点！剩余时间：" + (int)BattleController.Instance.Process.DazhaoLeftTime + "秒 剩余次数：" + BattleController.Instance.Process.DazhaoLeftCount;
        }
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.ShowSwitchPetUI, OnShowSwitchPetUIAtIndex);
        GameEventMgr.Instance.AddListener(GameEventList.ShowDazhaoTip, OnShowDazhaoTip);
        GameEventMgr.Instance.AddListener(GameEventList.HideDazhaoTip, OnHideDazhaoTip);
		GameEventMgr.Instance.AddListener<bool>(GameEventList.SetMirrorModeState, OnSetMirrorModeState);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.ShowSwitchPetUI, OnShowSwitchPetUIAtIndex);
        GameEventMgr.Instance.RemoveListener(GameEventList.ShowDazhaoTip, OnShowDazhaoTip);
        GameEventMgr.Instance.RemoveListener(GameEventList.HideDazhaoTip, OnHideDazhaoTip);
        GameEventMgr.Instance.RemoveListener<bool>(GameEventList.SetMirrorModeState, OnSetMirrorModeState);
    }

    void AddUIObjectEvent()
    {
        EventTriggerListener.Get(m_ButtonLeft.gameObject).onClick = OnButtonLeftCllicked;
		m_ButtonDaoju.onClick = OnButtonDaojuClicked;
        EventTriggerListener.Get(m_ButtonSpeed.gameObject).onClick = OnButtonSpeedClicked;

		m_ButtonMirror.onClick = OnToggleMirrorClicked;
		m_ButtonTuoguan.onClick = OnTuoguanButtonClick;

		m_ButtonMomo.onClick = OnMomoCliced;
    }

    void OnButtonLeftCllicked(GameObject go)
    {
        //m_PetPanelGameObject.SetActive (true);
        //GameEventMgr.Instance.FireEvent<int>(GameEventList.ShowSwitchPetUI, 1);
    }

    void OnButtonDaojuClicked(GameObject go)
    {
		GameEventMgr.Instance.FireEvent<bool> (GameEventList.SetMirrorModeState, false);
    }

    void OnButtonSpeedClicked(GameObject go)
    {
        m_BattleSpeed++;
		if (m_BattleSpeed > m_MaxSpeed)
        {
            m_BattleSpeed = 1;
        }

		UpdateButton ();
    }

	void UpdateButton()
	{
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
		bool isMirrorMode = m_ButtonMirror.IsOn;
		OnSetMirrorModeState (isMirrorMode);
		//GameEventMgr.Instance.FireEvent<bool> (GameEventList.SetMirrorModeState, isMirrorMode);
    }

	void OnSetMirrorModeState(bool isMirrorMode)
	{
		m_ButtonMirror.IsOn = isMirrorMode;
		m_MirrorImage.gameObject.SetActive(isMirrorMode);
		if (isMirrorMode)
		{
			Vector3 tempPos = m_MirrorImage.transform.localPosition;
			tempPos.x = 0;
			tempPos.y = 0;
			
			m_MirrorImage.transform.localPosition = tempPos;
		}
		
		//
		m_MirrorDray.OnSetMirrorModeState (isMirrorMode);
	}
    void OnTuoguanButtonClick(GameObject go)
    {

    }

	void OnMomoCliced(GameObject go)
	{
		GameEventMgr.Instance.FireEvent<bool> (GameEventList.SetMirrorModeState, false);
	}

#region Event

    //switch pet
    void OnShowSwitchPetUIAtIndex(EventArgs sArgs)
    {
        var args = sArgs as ShowSwitchPetUIArgs;
        m_PetPanel.Show(args);

        GameEventMgr.Instance.AddListener<int>(GameEventList.HideSwitchPetUI, OnHideSwitchPetUI);
    }

    void OnHideSwitchPetUI(int targetId)
    {
        if (m_PetPanel.Hide(targetId))
            GameEventMgr.Instance.RemoveListener<int>(GameEventList.HideSwitchPetUI, OnHideSwitchPetUI);
    }

    //dazhao
    void OnShowDazhaoTip()
    {
        dazhaoTip.gameObject.SetActive(true);
    }

    void OnHideDazhaoTip()
    {
        dazhaoTip.gameObject.SetActive(false);
    }
#endregion
}
