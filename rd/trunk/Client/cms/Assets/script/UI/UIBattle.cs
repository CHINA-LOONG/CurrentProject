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
    public Button mSetting = null;

    public BattleGroupUI m_PlayerGroupUI;
    public PetSwitchPage m_PetPanel;
    public DazhaoTip dazhaoTip;
    //public Text levelIndex;
    public WeakpointUI wpUI;
    private UIBattleSetting mUIBattleSetting;

    [HideInInspector]
    //public UIFazhen uiFazhen;

    public MirrorDray m_MirrorDray = null;

    private float m_BattleSpeed = 1.0f;
	private	float m_MaxSpeed = 2.2f;
    private Animator animator;
    private MsgBox.PrompCostMsg reviveWnd;
    private RectTransform mRootTrans;
    //private int reviveIndex;
    class SpellVitalChangeData
    {
        public SpellVitalChangeArgs vitalChangeArgs;
        public float showTime;
        public GameObject vitalObj = null;
    }
    private List<SpellVitalChangeData> mVitalChangeList = new List<SpellVitalChangeData>();
    private List<SpellVitalChangeData> mSpellNameList = new List<SpellVitalChangeData>();

	static UIBattle instance = null;
	public static UIBattle Instance
	{
		get
		{
			return instance;
		}
	}

	// Use this for initialization
    public void Initialize()
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

        m_BattleSpeed = PlayerPrefs.GetFloat("battleSpeed");
        if (Mathf.Abs(m_BattleSpeed) <= BattleConst.floatZero)
        {
            m_BattleSpeed = 1.0f;
        }
        GameSpeedService.Instance.SetBattleSpeed(m_BattleSpeed);
		UpdateButton ();

		animator = GetComponent<Animator> ();
		AniControl.battleUIState = Animator.StringToHash ("battleUIState");

        mSpellNameList.Clear();
        mVitalChangeList.Clear();
    }

    public override void Init()
    {
        //TODO：战斗界面不会隐藏了，只会删除
        //gameObject.SetActive(true);
        mRootTrans = transform as RectTransform;
        m_PetPanel.Hide(BattleConst.closeSwitchPetUI);
    }

    public override void Clean()
    {
        //UIMgr.Instance.DestroyUI(uiFazhen);
        for (int i = mSpellNameList.Count - 1; i >= 0; i--)
        {
            SpellVitalChangeData vitalData = mSpellNameList[i];
            if (vitalData.vitalObj != null)
            {
                ResourceMgr.Instance.DestroyAsset(vitalData.vitalObj);
            }
        }
        mSpellNameList.Clear();

        for (int i = mVitalChangeList.Count - 1; i >= 0; i--)
        {
            SpellVitalChangeData vitalData = mVitalChangeList[i];
            if (vitalData.vitalObj != null)
            {
                ResourceMgr.Instance.DestroyAsset(vitalData.vitalObj);
            }
        }
        mVitalChangeList.Clear();
    }

    public void HideBattleUI()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        AddUIObjectEvent();
        BindListener();
    }

    void OnDestroy()
    {
        UnBindListener();
    }
    public void ShowReviveUI(int reviveCount)
    {
        switch (reviveCount)
        {
            case 0:
                reviveCount = 15;
                break;
            case 1:
                reviveCount = 30;
                break;
            case 2:
                reviveCount = 50;
                break;
        }
        //reviveWnd = MsgBox.PromptMsg.Open(
        //    MsgBox.MsgBoxType.Conform_Cancel,
        //    string.Format(StaticDataMgr.Instance.GetTextByID("battle_revive"), reviveCount),
        //    ChooseReviveOrNot,
        //    false,
        //    true
        //    );
        reviveWnd = MsgBox.PrompCostMsg.Open(
            reviveCount,
            StaticDataMgr.Instance.GetTextByID("battle_revive"),
            null,
            CostType.ZuanshiCoin,
            ChooseReviveOrNot,
            false
            );
        //reviveIndex = reviveWnd.transform.GetSiblingIndex();
        //this has text mesh render
        //Vector3 localPos = reviveWnd.transform.localPosition;
        //reviveWnd.transform.localPosition = new Vector3(localPos.x, localPos.y, 10.0f);
        if (m_MirrorDray != null)
        {
            m_MirrorDray.OnPointerUp(null);
        }
    }

    public void CloseReviveUI()
    {
        if (reviveWnd != null)
        {
            reviveWnd.Close();
        }
    }

    private void ChooseReviveOrNot(MsgBox.PrompButtonClick state)
    {
        if (state == MsgBox.PrompButtonClick.OK)
        {
            PB.HSInstanceRevive reviveParam = new PB.HSInstanceRevive();
            GameApp.Instance.netManager.SendMessage(PB.code.INSTANCE_REVIVE_C.GetHashCode(), reviveParam, false);
            ////test only
            //CloseReviveUI();
            //BattleController battleInstance = BattleController.Instance;
            //battleInstance.BattleGroup.RevivePlayerList();
            //battleInstance.Process.ReviveSuccess();
        }
        else if (state == MsgBox.PrompButtonClick.Cancle)
        {
            BattleController.Instance.OnBattleOver(false);
            CloseReviveUI();
        }
    }

    void OnReviveResult(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();
            switch (error.errCode)
            {
                case (int)PB.instanceError.INSTANCE_REVIVE_COUNT:
                    UIBattle.Instance.CloseReviveUI();
                    BattleController.Instance.OnBattleOver(false);
                    break;
                case (int)PB.PlayerError.GOLD_NOT_ENOUGH:
                    GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
                    break;
            }
        }
        else
        {
            PB.HSInstanceReviveRet reviveResult = msg.GetProtocolBody<PB.HSInstanceReviveRet>();
            BattleController battleInstance = BattleController.Instance;
            CloseReviveUI();
            battleInstance.BattleGroup.RevivePlayerList(battleInstance.Process.lastActionOrder);
            battleInstance.Process.ReviveSuccess(reviveResult.reviveCount);
            m_PetPanel.ForceRefresh();
        }
        //switch (reviveStatus)
        //{
        //    //0:success 1:count error 2:diamond not enough
        //    case 0:
        //        CloseReviveUI();
        //        battleInstance.BattleGroup.RevivePlayerList(reviveResult);
        //        battleInstance.Process.ReviveSuccess(reviveresu);
        //        break;
        //    case 1:
        //        CloseReviveUI();
        //        battleInstance.OnBattleOver(false);
        //        break;
        //    case 2:
        //        GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
        //        break;
        //    default:
        //        break;
        //}
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
        
        if (
            lifeChange.vitalType != (int)VitalType.Vital_Type_FixLife &&
            lifeChange.vitalType != (int)VitalType.Vital_Type_Shield 
            )
        {
            SpellVitalChangeData vitalChangeData = new SpellVitalChangeData();
            vitalChangeData.vitalChangeArgs = lifeChange;
            if (lifeChange.vitalType == (int)VitalType.Vital_Type_SpellName)
            {
                float startTime = mSpellNameList.Count > 0 ? mSpellNameList[0].showTime : Time.time;
                vitalChangeData.showTime = startTime + BattleConst.intervalTime * mSpellNameList.Count;
                mSpellNameList.Add(vitalChangeData);
            }
            else
            {
                if (
                    vitalChangeData.vitalChangeArgs.vitalType == (int)VitalType.Vital_Type_Default &&
                    vitalChangeData.vitalChangeArgs.vitalChange == 0
                    )
                {
                    return;
                }
                float startTime = mVitalChangeList.Count > 0 ? mVitalChangeList[0].showTime : Time.time;
                vitalChangeData.showTime = startTime + BattleConst.intervalTime * mVitalChangeList.Count;
                mVitalChangeList.Add(vitalChangeData);

                if (string.IsNullOrEmpty(lifeChange.wpID) == false && lifeChange.wpID != "e_shouji")
                {
                    BattleObject target = ObjectDataMgr.Instance.GetBattleObject(lifeChange.targetID);
                    WeakPointRuntimeData wpRuntime = null;
                    if (target.wpGroup != null)
                    {
                        target.wpGroup.allWpDic.TryGetValue(lifeChange.wpID, out wpRuntime);
                        if (wpRuntime != null)
                        {
                            wpUI.UpdateArmorProgress(wpRuntime);
                        }
                    }
                }
            }
        }
    }

    public void ChangeEnergy(SpellVitalChangeArgs energyChange)
    {
        m_PlayerGroupUI.ChangeEnergy(energyChange);
    }

    public void SetBattleUnitVisible(int id, bool visible)
    {
        m_PlayerGroupUI.SetBattleUnitVisible(id, visible);
    }

    public void ShowDazhaoReleateUI(bool isShow)
    {
        m_ButtonSpeed.gameObject.SetActive(isShow);
        RectTransform mirrorTrans = mirrorUI.gameObject.transform as RectTransform;
        if (isShow == true)
        {
            mirrorTrans.anchoredPosition -= BattleConst.uiFarDistance;
            OnSetMirrorModeState(false, false);
        }
        else
        {
            mirrorTrans.anchoredPosition += BattleConst.uiFarDistance;
        }
    }
	public void ShowUI(bool ishow)
    {
        //if (ishow == false)
        //{
        //    gameObject.BroadcastMessage("OnAnimationFinish", SendMessageOptions.DontRequireReceiver);
        //}
        bool isbossBattle = BattleController.Instance.battleType == BattleType.Boss;
        if (mirrorUI != null)
        {
            mirrorUI.gameObject.SetActive(isbossBattle);
        }

        //gameObject.SetActive (ishow);
		if (ishow) 
		{
            //InitMirrorDray();
            mRootTrans.anchoredPosition = Vector2.zero;
            GameSpeedService.Instance.SetBattleSpeed(m_BattleSpeed);
		}
		else 
		{
            mRootTrans.anchoredPosition = BattleConst.uiFarDistance;
			//DestroyMirrorDray();
        }

        if (null != m_MirrorDray)
        {
            m_MirrorDray.gameObject.SetActive(isbossBattle);
        }
        if (m_MirrorImage != null)
        {
            m_MirrorImage.gameObject.SetActive(isbossBattle);
        }
        
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

        for (int i = mSpellNameList.Count - 1; i >= 0; i--)
        {
            SpellVitalChangeData vitalData = mSpellNameList[i];
            if (vitalData.vitalObj == null && vitalData.showTime <= Time.time)
            {
                vitalData.vitalObj = ResourceMgr.Instance.LoadAsset("VitalChange");
                UIVitalChangeView uiVitalChangeView = vitalData.vitalObj.GetComponent<UIVitalChangeView>();
                uiVitalChangeView.ShowVitalChange(vitalData.vitalChangeArgs, gameObject.transform as RectTransform);
                //mSpellNameList.Remove(vitalData);
            }
        }

        for (int i = mVitalChangeList.Count - 1; i >= 0; i--)
        {
            SpellVitalChangeData vitalData = mVitalChangeList[i];
            if (vitalData.vitalObj == null && vitalData.showTime <= Time.time)
            {
                vitalData.vitalObj = ResourceMgr.Instance.LoadAsset("VitalChange");
                UIVitalChangeView uiVitalChangeView = vitalData.vitalObj.GetComponent<UIVitalChangeView>();
                uiVitalChangeView.ShowVitalChange(vitalData.vitalChangeArgs, gameObject.transform as RectTransform);
                //mVitalChangeList.Remove(vitalData);
            }
        }

        for (int i = mSpellNameList.Count - 1; i >= 0; i--)
        {
            SpellVitalChangeData vitalData = mSpellNameList[i];
            if (vitalData.vitalObj != null && vitalData.showTime + 1.0f <= Time.time)
            {
                ResourceMgr.Instance.DestroyAsset(vitalData.vitalObj);
                mSpellNameList.RemoveAt(i);
            }
        }

        for (int i = mVitalChangeList.Count - 1; i >= 0; i--)
        {
            SpellVitalChangeData vitalData = mVitalChangeList[i];
            if (vitalData.vitalObj != null && vitalData.showTime + 1.0f <= Time.time)
            {
                ResourceMgr.Instance.DestroyAsset(vitalData.vitalObj);
                mVitalChangeList.RemoveAt(i);
            }
        }
        //if (reviveWnd != null)
        //{
        //    reviveWnd.transform.SetAsLastSibling();
        //    int index = reviveWnd.transform.GetSiblingIndex();
        //    reviveWnd.gameObject.SetActive(index == reviveIndex);
        //}
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.ShowSwitchPetUI, OnShowSwitchPetUIAtIndex);
        GameEventMgr.Instance.AddListener(GameEventList.ShowDazhaoTip, OnShowDazhaoTip);
        GameEventMgr.Instance.AddListener(GameEventList.HideDazhaoTip, OnHideDazhaoTip);
		GameEventMgr.Instance.AddListener<bool,bool>(GameEventList.SetMirrorModeState, OnSetMirrorModeState);
        GameEventMgr.Instance.AddListener<int>(GameEventList.HideSwitchPetUI, OnHideSwitchPetUI);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_REVIVE_C.GetHashCode().ToString(), OnReviveResult);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_REVIVE_S.GetHashCode().ToString(), OnReviveResult);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.ShowSwitchPetUI, OnShowSwitchPetUIAtIndex);
        GameEventMgr.Instance.RemoveListener(GameEventList.ShowDazhaoTip, OnShowDazhaoTip);
        GameEventMgr.Instance.RemoveListener(GameEventList.HideDazhaoTip, OnHideDazhaoTip);
        GameEventMgr.Instance.RemoveListener<bool,bool>(GameEventList.SetMirrorModeState, OnSetMirrorModeState);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.HideSwitchPetUI, OnHideSwitchPetUI);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_REVIVE_C.GetHashCode().ToString(), OnReviveResult);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_REVIVE_S.GetHashCode().ToString(), OnReviveResult);
    }

    void AddUIObjectEvent()
    {
        EventTriggerListener.Get(m_ButtonLeft.gameObject).onClick = OnButtonLeftCllicked;
        EventTriggerListener.Get(m_ButtonSpeed.gameObject).onClick = OnButtonSpeedClicked;
        mSetting.onClick.AddListener(OnSettingClicked);
		//m_ButtonDaoju.onClick = OnButtonDaojuClicked;

		m_ButtonMirror.onClick = OnToggleMirrorClicked;
		m_ButtonTuoguan.onClick = OnTuoguanButtonClick;

		//m_ButtonMomo.onClick = OnMomoCliced;
    }


	void InitMirrorImage()
	{
		if (null != m_MirrorImage)
		{
			InitMirrorDray();
			//Logger.LogError("liwsTest: UIBattle is Load And MirrorDray component added");
		}
		else
		{
			Logger.LogError("You Should set MirrorImage in the UIBattle prefab!");
		}
        /*
        GameObject go = ResourceMgr.Instance.LoadAsset("MirrorFindMonsterInfo");
		//GameObject go = Instantiate (prefab) as GameObject;
		go.transform.SetParent (m_MirrorImage.gameObject.transform,false);
		MirrorFindMonsterInfo mminfo = go.GetComponent<MirrorFindMonsterInfo> ();
		go.SetActive (false);
		mminfo.Init ();

		//m_MirrorImage.gameObject.SetActive(false);
        */
		//
	}

	void InitMirrorDray()
	{
		DestroyMirrorDray ();
		if (null == m_MirrorDray) 
		{
			m_MirrorDray = m_MirrorImage.gameObject.AddComponent<MirrorDray> ();
		}
		
		GameObject uigo = ResourceMgr.Instance.LoadAsset("MirrorUI");
		uigo.name = "MirrorUI";
		uigo.transform.SetParent(mirrorUI.transform,false);
		uigo.transform.localPosition = Vector3.zero;
		
		var img = mirrorUI.GetComponent<Image>();
		if(null != img)
		{
			img.enabled = false;
		}
		
		m_MirrorDray.Init(uigo);
	}

	void DestroyMirrorDray()
	{
        if (null == mirrorUI)
            return;
		var v = mirrorUI.GetComponentsInChildren<Animator> (true);
		if (v != null && v.Length > 0)
		{
			Destroy(v[0]);
			Destroy(v[0].gameObject);
		}
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
		//GameEventMgr.Instance.FireEvent<bool> (GameEventList.SetMirrorModeState, false);
    }

    void OnButtonSpeedClicked(GameObject go)
    {
        if (BattleController.Instance.processStart == false)
            return;

        m_BattleSpeed += 0.6f;
        if (m_BattleSpeed > m_MaxSpeed)
        {
            m_BattleSpeed = 1.0f;
        }

        PlayerPrefs.SetFloat("battleSpeed", m_BattleSpeed);
        GameSpeedService.Instance.SetBattleSpeed(m_BattleSpeed);
        UpdateButton();
    }

    void OnSettingClicked()
    {
        BattleProcess curProcess = BattleController.Instance.Process;
        if (curProcess.IsProcessFinish == false)
        {
            curProcess.Pause(true, false);
            mUIBattleSetting = UIMgr.Instance.OpenUI_(UIBattleSetting.ViewName) as UIBattleSetting;
        }
    }

    public void CloseBattleSetting()
    {
        if (mUIBattleSetting != null)
        {
            mUIBattleSetting.CloseEnsureExitWnd();
            UIMgr.Instance.DestroyUI(mUIBattleSetting);
        }
    }

    void UpdateButton()
	{
		Image subImg = null;
		for (int i = 0; i < m_SpeedNumImageList.Count; ++i)
		{
			subImg = m_SpeedNumImageList[i] as Image;
			if (Mathf.Abs(m_BattleSpeed - 1.0f - 0.6f * i) <= BattleConst.floatZero)
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

	void OnSetMirrorModeState(bool isMirrorMode,bool isMirrExitEffect)
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
        if(null != m_MirrorDray)
        {
            m_MirrorDray.OnSetMirrorModeState(isMirrorMode, isMirrExitEffect);
        }
	}
    void OnTuoguanButtonClick(GameObject go)
    {

    }

	void OnMomoCliced(GameObject go)
	{
		//GameEventMgr.Instance.FireEvent<bool> (GameEventList.SetMirrorModeState, false);
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

	//void OnChangeUIState(UiState uiState)
	//{
		//animator.SetInteger (AniControl.battleUIState, (int)uiState);
	//}
#endregion
}
