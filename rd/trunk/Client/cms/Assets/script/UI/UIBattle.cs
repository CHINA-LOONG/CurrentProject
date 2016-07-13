using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIBattle : UIBase
{
	public enum UiState:int
	{
		Normal = 0,
		Dazhao
	}

	public class AniControl
	{
		public static int	battleUIState = 0;
	}

    public static string ViewName = "UIBattle";

	public Transform bottomLayer = null;
	public Transform dazhaoGroup = null;
    public Transform publicTopGroup = null;

	public Image mirrorUI = null;
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
    public DazhaoTip dazhaoTip;
    public Text levelIndex;

    public Sprite victorySprite;
    public Sprite failedSprite;

    private MirrorDray m_MirrorDray = null;

    private int m_BattleSpeed = 1;
	private	int	m_MaxSpeed = 3;

    GameObject startBattleUI = null;
    GameObject endBattleUI = null;
	Animator animator;

	static UIBattle instance = null;
	public static UIBattle Instance
	{
		get
		{
			return instance;
		}
	}

	// Use this for initialization
    public void Init()
    {
		instance = this;
        
		m_ButtonMirror.IsOn = false;
        
		InitMirrorImage ();
		InitFindWpInfoGroup ();
		InitDazhaoTip ();
        m_PetPanel.gameObject.SetActive(false);
        //dazhaoTip.gameObject.SetActive(false);
		dazhaoTip.Hide ();

        m_PlayerGroupUI.gameObject.SetActive(true);
        m_PlayerGroupUI.Init(
            BattleController.Instance.BattleGroup.PlayerFieldList,
            BattleController.Instance.BattleGroup.EnemyFieldList
            );

        AddUIObjectEvent();
        BindListener();

        m_BattleSpeed = (int)(PlayerPrefs.GetFloat("battleSpeed"));
		UpdateButton ();

		animator = GetComponent<Animator> ();
		AniControl.battleUIState = Animator.StringToHash ("battleUIState");
    }

    void OnDestroy()
    {
        UnBindListener();
    }

    public void SetBattleLevelProcess(int curIndex, int maxIndex)
    {
        levelIndex.text = curIndex.ToString() + "/" + maxIndex.ToString();
    }

    public void ShowStartBattleUI()
    {
        //GameObject starBattlePrefab = ResourceMgr.Instance.LoadAsset("startBattle") as GameObject;
        startBattleUI = ResourceMgr.Instance.LoadAsset("startBattle");
        startBattleUI.transform.SetParent(publicTopGroup, false);
        //Animator startBattleUIAni = startBattleUI.GetComponent<Animator>();
        //startBattleUIAni.gameObject.SetActive(true);
    }

    public void DestroyStartBattleUI()
    {
        ResourceMgr.Instance.DestroyAsset(startBattleUI);
    }

    public void ShowEndBattleUI(bool success)
    {
        //GameObject endBattlePrefab = ResourceMgr.Instance.LoadAsset("endBattle") as GameObject;
        endBattleUI = ResourceMgr.Instance.LoadAsset("endBattle");
        endBattleUI.transform.SetParent(publicTopGroup, false);
        Image endImage = endBattleUI.GetComponent<Image>();
        if (success)
        {
            endImage.sprite = victorySprite;
        }
        else 
        {
            endImage.sprite = failedSprite;
        }
    }

    public void DestroyEndBattleUI()
    {
        ResourceMgr.Instance.DestroyAsset(endBattleUI);
    }

    public void ChangeBuffState(SpellBuffArgs args)
    {
        m_PlayerGroupUI.ChangeBuffState(args);
    }

    public void ShowUnitUI(BattleObject unit, int slot)
    {
        m_PlayerGroupUI.ShowUnit(unit, slot);
    }

    public void HideUnitUI(int id)
    {
        m_PlayerGroupUI.HideUnit(id);
    }

    public void ChangeLife(SpellVitalChangeArgs lifeChange)
    {
        m_PlayerGroupUI.ChangeLife(lifeChange);

        //GameObject prefab = ResourceMgr.Instance.LoadAsset("ui/battle", "VitalChange");
        GameObject go = ResourceMgr.Instance.LoadAsset("VitalChange");
        UIVitalChangeView uiVitalChangeView = go.GetComponent<UIVitalChangeView>();
        uiVitalChangeView.ShowVitalChange(lifeChange, gameObject.transform as RectTransform);
    }

    public void ChangeEnergy(SpellVitalChangeArgs energyChange)
    {
        m_PlayerGroupUI.ChangeEnergy(energyChange);
    }

    public void SetBattleUnitVisible(int id, bool visible)
    {
        m_PlayerGroupUI.SetBattleUnitVisible(id, visible);
    }

	public void ShowUI(bool ishow)
    {
        //if (ishow == false)
        //{
        //    gameObject.BroadcastMessage("OnAnimationFinish", SendMessageOptions.DontRequireReceiver);
        //}
		gameObject.SetActive (ishow);
	}

    void Update()
    {
        if (dazhaoTip.IsShow())
        {
			//dazhaoTip.text = "大招模式点点点！剩余时间：" + (int)PhyDazhaoController.Instance.DazhaoLeftTime + "秒 剩余次数：" + PhyDazhaoController.Instance.DazhaoLeftCount;
			dazhaoTip.SetTipInfo((int)PhyDazhaoController.Instance.DazhaoLeftTime,
			                     PhyDazhaoController.Instance.DazhaoUseCount,
			                     PhyDazhaoController.Instance.DazhaoAllCount);
        }
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.ShowSwitchPetUI, OnShowSwitchPetUIAtIndex);
        GameEventMgr.Instance.AddListener(GameEventList.ShowDazhaoTip, OnShowDazhaoTip);
        GameEventMgr.Instance.AddListener(GameEventList.HideDazhaoTip, OnHideDazhaoTip);
		GameEventMgr.Instance.AddListener<bool>(GameEventList.SetMirrorModeState, OnSetMirrorModeState);
        GameEventMgr.Instance.AddListener<UiState>(GameEventList.ChangeUIBattleState, OnChangeUIState);
        GameEventMgr.Instance.AddListener<int>(GameEventList.HideSwitchPetUI, OnHideSwitchPetUI);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.ShowSwitchPetUI, OnShowSwitchPetUIAtIndex);
        GameEventMgr.Instance.RemoveListener(GameEventList.ShowDazhaoTip, OnShowDazhaoTip);
        GameEventMgr.Instance.RemoveListener(GameEventList.HideDazhaoTip, OnHideDazhaoTip);
        GameEventMgr.Instance.RemoveListener<bool>(GameEventList.SetMirrorModeState, OnSetMirrorModeState);
        GameEventMgr.Instance.RemoveListener<UiState>(GameEventList.ChangeUIBattleState, OnChangeUIState);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.HideSwitchPetUI, OnHideSwitchPetUI);
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


	void InitMirrorImage()
	{
		if (null != m_MirrorImage)
		{
			m_MirrorDray = m_MirrorImage.gameObject.AddComponent<MirrorDray>();

			GameObject uigo = ResourceMgr.Instance.LoadAsset("MirrorUI");
			uigo.transform.SetParent(mirrorUI.transform,false);
			uigo.transform.localPosition = Vector3.zero;

			var img = mirrorUI.GetComponent<Image>();
			if(null != img)
			{
				img.enabled = false;
			}

			m_MirrorDray.Init(uigo);
			//Debug.LogError("liwsTest: UIBattle is Load And MirrorDray component added");
		}
		else
		{
			Logger.LogError("You Should set MirrorImage in the UIBattle prefab!");
		}

        GameObject go = ResourceMgr.Instance.LoadAsset("MirrorFindMonsterInfo");
		//GameObject go = Instantiate (prefab) as GameObject;
		go.transform.SetParent (m_MirrorImage.gameObject.transform,false);
		MirrorFindMonsterInfo mminfo = go.GetComponent<MirrorFindMonsterInfo> ();
		go.SetActive (false);
		mminfo.Init ();

		//m_MirrorImage.gameObject.SetActive(false);

		//
	}

	void InitFindWpInfoGroup()
	{
		//照妖镜发现弱点
		WpInfoGroup wpInfoGroup = gameObject.AddComponent<WpInfoGroup> ();
		wpInfoGroup.InitWithParent (bottomLayer);
	}	

	void InitDazhaoTip()
	{
		GameObject go = ResourceMgr.Instance.LoadAsset ("dazhaoTip");
		//GameObject go = Instantiate (prefab) as GameObject;
		go.transform.SetParent (dazhaoGroup.transform, false);
		dazhaoTip = go.GetComponent<DazhaoTip> ();
		dazhaoTip.Hide ();
	}

	//--------------------------------------------------------------------------------------------------
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
        if (BattleController.Instance.processStart == false)
            return;

        m_BattleSpeed++;
		if (m_BattleSpeed > m_MaxSpeed)
        {
            m_BattleSpeed = 1;
        }

        GameSpeedService.Instance.SetBattleSpeed(m_BattleSpeed);
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
		
		Logger.Log("battle Speed = " + m_BattleSpeed);
	}

    void OnToggleMirrorClicked(GameObject go)
    {
		//bool isMirrorMode = m_ButtonMirror.IsOn;
		//OnSetMirrorModeState (isMirrorMode);
		//GameEventMgr.Instance.FireEvent<bool> (GameEventList.SetMirrorModeState, isMirrorMode);
    }

	void OnSetMirrorModeState(bool isMirrorMode)
	{
		/*
		m_ButtonMirror.IsOn = isMirrorMode;
	//	m_MirrorImage.gameObject.SetActive(isMirrorMode);
		if (isMirrorMode)
		{
			float rootWidth = Screen.width /UIMgr.Instance.CanvasAttr.scaleFactor ;
			float rootHeight =   Screen.height/UIMgr.Instance.CanvasAttr.scaleFactor;

			Vector2 mirrorSize = m_MirrorImage.rectTransform.sizeDelta;
			Vector2 mirrorPiviot = m_MirrorImage.rectTransform.pivot;
			Vector2 tempPos = new Vector2(0,0);
			tempPos.x = rootWidth/2.0f - (0.5f - mirrorPiviot.x)*mirrorSize.x;
			tempPos.y = rootHeight/2.0f - (0.5f - mirrorPiviot.y)*mirrorSize.y;
			
			m_MirrorImage.rectTransform.anchoredPosition = tempPos;
		}
		*/
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

        //GameEventMgr.Instance.AddListener<int>(GameEventList.HideSwitchPetUI, OnHideSwitchPetUI);
    }

    void OnHideSwitchPetUI(int targetId)
    {
        m_PetPanel.Hide(targetId);
            //GameEventMgr.Instance.RemoveListener<int>(GameEventList.HideSwitchPetUI, OnHideSwitchPetUI);
    }

    //dazhao
    void OnShowDazhaoTip()
    {
      //  dazhaoTip.gameObject.SetActive(true);
		dazhaoTip.Show ();
    }

    void OnHideDazhaoTip()
    {
       // dazhaoTip.gameObject.SetActive(false);
		dazhaoTip.Hide ();
    }

	void OnChangeUIState(UiState uiState)
	{
		animator.SetInteger (AniControl.battleUIState, (int)uiState);
	}
#endregion
}
