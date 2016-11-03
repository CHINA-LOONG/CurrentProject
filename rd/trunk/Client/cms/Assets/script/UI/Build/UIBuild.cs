using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Funplus;

public class UIBuild : UIBase,PopupListIndextDelegate,GuideBase
{
    public static string ViewName = "UIBuild";

    public BuildButton m_PetButton;
    public BuildButton m_BagButton;
	public BuildButton instanceButton;

    public BuildButton m_QuestButton;
	public BuildButton shopButton;

    public BuildButton m_ComposeButton;
    public BuildButton m_DecomposeButton;
    public BuildButton m_SigninButton;

    public Button huoliButton;
    public Text huoliText;
    public GameObject huoliTipButton;
    public HuoliCountDown huoliCountdown;
    public Button userCenter;

    public PopupList m_LangPopup;

    public BuildButton btnMail;

	public Text levelText;
	public Text nameText;
    public UIProgressbar playerProgress;

    [HideInInspector]
    public UIMail uiMail;
    [HideInInspector]
    public UIQuest uiQuest;
    [HideInInspector]
    public UIBag uiBag;
    [HideInInspector]
    public UIMonsters uiMonsters;
    [HideInInspector]
    public UIAdjustBattleTeam uiAdjustBattleTeam;
    [HideInInspector]
    public UIShop uiShop;
    [HideInInspector]
    public UIStore uiStore;
    [HideInInspector]
    public UICompose uiCompose;
    [HideInInspector]
    public UIDecompose uiDecompose;
    [HideInInspector]
    public UIAdventure uiAdventure;
    [HideInInspector]
    public UISignIn uiSignin;
    [HideInInspector]
    public PvpMain uiPvp;

    public Text mTestShowExp;

    void Awake()
    {
        BindListener();
    }

    void Start()
    {
        EventTriggerListener.Get(m_PetButton.gameObject).onClick = PetButtonClick;
        EventTriggerListener.Get(m_BagButton.gameObject).onClick = BagButtonClick;
        EventTriggerListener.Get(instanceButton.gameObject).onClick = OnInstanceButtonClick;
        EventTriggerListener.Get(m_QuestButton.gameObject).onClick = OnQuestButtonClick;
        EventTriggerListener.Get(btnMail.gameObject).onClick = OnMailButtonClick;
        EventTriggerListener.Get(shopButton.gameObject).onClick = OnShopButtonClick;
        EventTriggerListener.Get(userCenter.gameObject).onClick = OnUserCenterButtonClick;

        EventTriggerListener.Get(m_ComposeButton.gameObject).onClick = OnComposeButtonClick;
        EventTriggerListener.Get(m_DecomposeButton.gameObject).onClick = OnDecomposeButtonClick;
        EventTriggerListener.Get(m_SigninButton.gameObject).onClick = OnSigninButtonClick;
        EventTriggerListener.Get(huoliButton.gameObject).onClick = OnHuoliButtonClick;
        EventTriggerListener.Get(huoliTipButton).onClick = OnHuoliTipButtonClick;


        m_LangPopup.Initialize<PopupListIndextDelegate>(this);
        m_LangPopup.AddItem((int)Language.Chinese, StaticDataMgr.Instance.GetTextByID("ui_chinese"));
        m_LangPopup.AddItem((int)Language.English, StaticDataMgr.Instance.GetTextByID("ui_english"));
        m_LangPopup.SetSelection((int)LanguageMgr.Instance.Lang);
    }

	void OnDestroy()
	{
		UnBindListener ();
	}

    void BindListener()
	{
		GameEventMgr.Instance.AddListener<int> (GameEventList.LevelChanged, OnLevelChanged);
        GameEventMgr.Instance.AddListener(GameEventList.QuestChanged, OnQuestChanged);
        GameEventMgr.Instance.AddListener<int>(GameEventList.MailAdd, OnMailChanged);
        GameEventMgr.Instance.AddListener<int>(GameEventList.MailRead, OnMailChanged);
        GameEventMgr.Instance.AddListener(GameEventList.SignInPopupChange, OnSigninChanged);
        GameEventMgr.Instance.AddListener(GameEventList.SignInChange, OnSigninChanged);
        GameEventMgr.Instance.AddListener<int, int,bool>(GameEventList.PlayerExpChanged, OnPlayerExpChanged);
        GameEventMgr.Instance.AddListener<int>(GameEventList.HuoliChanged, OnHuoliChanged);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SETTING_LANGUAGE_C.GetHashCode().ToString(), OnSettingLanguageRet);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.SETTING_LANGUAGE_S.GetHashCode().ToString(), OnSettingLanguageRet);
	}

	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<int> (GameEventList.LevelChanged, OnLevelChanged);
        GameEventMgr.Instance.RemoveListener(GameEventList.QuestChanged, OnQuestChanged);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.MailAdd, OnMailChanged);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.MailRead, OnMailChanged);
        GameEventMgr.Instance.RemoveListener(GameEventList.SignInPopupChange, OnSigninChanged);
        GameEventMgr.Instance.RemoveListener(GameEventList.SignInChange, OnSigninChanged);
        GameEventMgr.Instance.RemoveListener<int, int,bool>(GameEventList.PlayerExpChanged, OnPlayerExpChanged);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.HuoliChanged, OnHuoliChanged);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SETTING_LANGUAGE_C.GetHashCode().ToString(), OnSettingLanguageRet);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.SETTING_LANGUAGE_S.GetHashCode().ToString(), OnSettingLanguageRet);
    }
    void OnEnable()
    {
        GuideListener(true);
    }
    void OnDisable()
    {
        GuideListener(false);
    }

	void OnLevelChanged(int level)
	{
        levelText.text = string.Format("{0}", level);
		nameText.text = GameDataMgr.Instance.PlayerDataAttr.nickName;
	}

    void OnPlayerExpChanged(int oldExp,int newExp,bool withAni = false)
    {
        PlayerLevelAttr originalAttr = StaticDataMgr.Instance.GetPlayerLevelAttr(GameDataMgr.Instance.PlayerDataAttr.LevelAttr);
        if (null == originalAttr)
            return;
        float targetExp = newExp / (float)originalAttr.exp;
        if(oldExp != newExp && withAni)
        {
            playerProgress.SetLoopCount(1);
            float curExp = oldExp / (float)originalAttr.exp;
            playerProgress.SetCurrrentRatio(targetExp);
        }

        playerProgress.SetTargetRatio(targetExp);
        if(!withAni)
        {
            playerProgress.SkipAnimation();
        }
        int ratio = (int)Mathf.Ceil(targetExp * 100);
        if(GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= GameConfig.MaxPlayerLevel)
        {
            playerProgress.mProgressText.text = "MAXLEVEL";
        }
        else
        {
            playerProgress.mProgressText.text = string.Format("{0}%", ratio);
        }

        //test only
        mTestShowExp.text = newExp.ToString() + "/" + originalAttr.exp.ToString();
    }       

    void OnHuoliChanged(int newHuoli)
    {
        huoliText.text = string.Format("{0}/{1}", newHuoli, GameDataMgr.Instance.PlayerDataAttr.MaxHuoliAttr);
    }

    //邮件同步，新邮件事件
    void OnMailChanged(int mail)
    {
        btnMail.SetRemind(UIUtil.CheckHaveNewMail());
    }
    void OnQuestChanged()
    {
        GameQuestData questData = GameDataMgr.Instance.PlayerDataAttr.gameQuestData;
        m_QuestButton.SetRemind(questData.StoryFinish||questData.DailyFinish||questData.OtherFinish);
    }
    void OnSigninChanged()
    {
        //Logger.Log(!SigninDataMgr.Instance.isSigninDaily);
        //Logger.Log((!SigninDataMgr.Instance.isPopup));
        //Logger.Log(SigninDataMgr.Instance.canSigninFillTimes > 0);

        //Logger.Log(!SigninDataMgr.Instance.isSigninDaily || ((!SigninDataMgr.Instance.isPopup) && (SigninDataMgr.Instance.canSigninFillTimes > 0)));
        m_SigninButton.SetRemind(!SigninDataMgr.Instance.isSigninDaily || ((!SigninDataMgr.Instance.isPopup) && (SigninDataMgr.Instance.canSigninFillTimes > 0)));
    }
    
    void BagButtonClick(GameObject go)
    {
        uiBag = UIBag.OpenWith();
    }

    void PetButtonClick(GameObject go)
    {
        //uiMonsters = UIMgr.Instance.OpenUI_(UIMonsters.ViewName) as UIMonsters;
        UIMgr.Instance.OpenUIAsync(UIMonsters.ViewName, true, PetLoadCallback);
    }
    public void PetLoadCallback(GameObject instance, System.EventArgs args)
    {
        uiMonsters = instance.GetComponent<UIBase>() as UIMonsters;
        uiMonsters.Refresh(0, 0);
    }


    void OnInstanceButtonClick(GameObject go)
	{
        InstanceMap.OpenWith();
	}

    void OnQuestButtonClick(GameObject go)
    {
        uiQuest= UIMgr.Instance.OpenUI_(UIQuest.ViewName) as UIQuest;
        uiQuest.Refresh();
    }

    void OnMailButtonClick(GameObject go)
    {
        uiMail = UIMgr.Instance.OpenUI_(UIMail.ViewName) as UIMail;
    }

    void OnComposeButtonClick(GameObject go)
    {
        uiCompose = UIMgr.Instance.OpenUI_(UICompose.ViewName) as UICompose;
    }
    void OnDecomposeButtonClick(GameObject go)
    {
        uiDecompose = UIMgr.Instance.OpenUI_(UIDecompose.ViewName) as UIDecompose;
    }
    public void OnAdventureButtonClick(GameObject go)
    {
        //UISpeech.Open("speech_test");
        uiAdventure = UIMgr.Instance.OpenUI_(UIAdventure.ViewName) as UIAdventure;
        uiAdventure.Refresh(0);
    }
    public void OnPvpButtonClick(GameObject go)
    {
       uiPvp = PvpMain.Open();
    }

    void OnSigninButtonClick(GameObject go)
    {
        //签到作为弹出框层popup
        if (uiSignin == null)
        {
            uiSignin = UISignIn.Open();
        }
    }

    void    OnHuoliButtonClick (GameObject go)
    {
        UseHuoLi.Open();
    }

    void OnHuoliTipButtonClick(GameObject go)
    {
        huoliCountdown.SetShow(true);
    }

    public	UIShop OpenShop(int shopType)
	{
		uiShop = UIMgr.Instance.OpenUI_ (UIShop.ViewName) as UIShop;
		uiShop.RefreshShopData (shopType);
		return uiShop;
	}
    public UIStore OpenStore()
    {
        uiStore= UIMgr.Instance.OpenUI_(UIStore.ViewName) as UIStore;
        return uiStore;
    }


	void OnShopButtonClick( GameObject go)
	{
		OpenShop ((int)PB.shopType.NORMALSHOP);
	}

	void OnExchangeButtonclick(GameObject go)
	{
		UICoinExchange.Open ();
	}

    void OnUserCenterButtonClick(GameObject go)
    {
        FunplusAccount.Instance.ShowUserCenter();
    }

    public void OnPopupListChanged(int index)
    {
        Language lang = (Language)index;
        if (lang != LanguageMgr.Instance.Lang)
        {
            LanguageMgr.Instance.Lang = lang;
            m_LangPopup.RefrshItem((int)Language.Chinese, StaticDataMgr.Instance.GetTextByID("ui_chinese"));
            m_LangPopup.RefrshItem((int)Language.English, StaticDataMgr.Instance.GetTextByID("ui_english"));
            if (LanguageMgr.Instance.Lang == Language.Chinese)
            {
                PlayerPrefs.SetString("serverLanguage", "zh-CN");
                SettingLanguage();
            }
            else if (LanguageMgr.Instance.Lang == Language.English)
            {
                PlayerPrefs.SetString("serverLanguage", "en");
                SettingLanguage();
            }
        }
    }

    public override void Init(bool forbidGuide = false)
    {
        OnQuestChanged();
        OnMailChanged(0);
        OnLevelChanged(GameDataMgr.Instance.PlayerDataAttr.LevelAttr);
        OnPlayerExpChanged(0, GameDataMgr.Instance.PlayerDataAttr.ExpAttr,false);
        OnHuoliChanged(GameDataMgr.Instance.PlayerDataAttr.HuoliAttr);

        if (!SigninDataMgr.Instance.isSigninDaily && !SigninDataMgr.Instance.isPopup && SigninDataMgr.Instance.loginTimesDaily <= 0)
        {
            OnSigninButtonClick(null);
            GuideHelp.isSignAutoOpen = true;
        }
        else
        {
            GuideHelp.isSignAutoOpen = false;
        }
        OnSigninChanged();
        base.Init(forbidGuide);
        if(!forbidGuide)
        {
            GuideManager.Instance.RequestGuide(this);
        }
        GuideHelp.isSignAutoOpen = false;
    }
    public override void RefreshOnPreviousUIHide()
    {
        base.RefreshOnPreviousUIHide();
        GuideManager.Instance.RequestGuide(this);
    }
    public override void Clean()
    {
        UIMgr.Instance.DestroyUI(uiQuest);
        UIMgr.Instance.DestroyUI(uiMail);
        UIMgr.Instance.DestroyUI(uiBag);
        if(InstanceMap.Instance != null)
        {
            UIMgr.Instance.DestroyUI(InstanceMap.Instance);
            InstanceMap.Instance = null;
        }
        UIMgr.Instance.DestroyUI(uiMonsters);
        UIMgr.Instance.DestroyUI(uiAdjustBattleTeam);
		UIMgr.Instance.DestroyUI (uiShop);
        UIMgr.Instance.DestroyUI(uiStore);
        UIMgr.Instance.DestroyUI(uiCompose);
        UIMgr.Instance.DestroyUI(uiDecompose);
        UIMgr.Instance.DestroyUI(uiAdventure);
        UIMgr.Instance.DestroyUI(uiSignin);
        UIMgr.Instance.DestroyUI(uiPvp);

        if(null != UISociatyTask.Instance)
        {
            UIMgr.Instance.DestroyUI(UISociatyTask.Instance);
            UISociatyTask.Instance = null;
        }
        if(null != SociatyMain.Instance)
        {
            UIMgr.Instance.DestroyUI(SociatyMain.Instance);
            SociatyMain.Instance = null;
        }
    }

    void SettingLanguage()
    {
        PB.HSSettingLanguage param = new PB.HSSettingLanguage()
        {
            language = PlayerPrefs.GetString("serverLanguage")
        };
        GameApp.Instance.netManager.SendMessage(PB.code.SETTING_LANGUAGE_C.GetHashCode(), param,false);
    }
    void OnSettingLanguageRet(ProtocolMessage msg)
    {
        if (msg.GetProtocolBody<PB.HSSettingLanguageRet>().language == PlayerPrefs.GetString("serverLanguage"))
        {
            Logger.Log("设置成功");
        }
    }

    #region 为新手引导提供的接口
    /// <summary>
    /// 镜头切换到3D对象，并返回投影到屏幕的 2DUI坐标, （左下点为0,0）
    /// </summary>
    /// <param name="pos3DObject"></param>
    /// <returns></returns>
    public Vector2  Focus3DObject(string pos3DObject)
    {
        return new Vector2(100, 100);
    }
    /// <summary>
    /// 新手引导任务回调
    /// </summary>
    /// <param name="message"></param>
    protected override void OnGuideMessageCallback(string message)
    {
        if(message.Equals("gd_build_more"))
        {
            MoreButton.Instance.OnMoreButtonClicked(null);
        }
        else if (message.Equals("gd_build_instance"))
        {
            InstanceMap.OpenWith();
        }
        else if (message.Equals("gd_build_pet"))
        {
            PetButtonClick(null);
        }
        else if (message.Equals("gd_build_task"))
        {
            OnQuestButtonClick(null);
        }
        else if (message.Equals("3D_choudan_obj"))
        {
            MainStageController mainStage = UIMgr.Instance.MainstageInstance;
            if (mainStage != null)
            {
                mainStage.SetCurrentSelectGroup((int)SelectableGroupType.Select_Group_Summon);
            }
        }
        else if (message.Equals("gd_build_shop"))
        {
            OnShopButtonClick(null);
        }
        else if (message.Equals("gd_build_qiandao"))
        {
            OnSigninButtonClick(null);
        }
    }
    #endregion
}
