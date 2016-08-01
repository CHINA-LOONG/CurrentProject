using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIBuild : UIBase,PopupListIndextDelegate
{
    public static string ViewName = "UIBuild";

    public BuildButton m_PetButton;
    public BuildButton m_BagButton;
	public BuildButton instanceButton;

    public BuildButton m_QuestButton;
    public Button m_SpeechButton;
	public BuildButton shopButton;


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
    public UIInstance uiInstance;
    [HideInInspector]
    public UIPetList uiPetList;
    [HideInInspector]
    public UIAdjustBattleTeam uiAdjustBattleTeam;
    [HideInInspector]
    public UIShop uiShop;

    void Start()
    {
        EventTriggerListener.Get(m_PetButton.gameObject).onClick = PetButtonClick;
        EventTriggerListener.Get(m_BagButton.gameObject).onClick = BagButtonClick;
        EventTriggerListener.Get(instanceButton.gameObject).onClick = OnInstanceButtonClick;
        EventTriggerListener.Get(m_QuestButton.gameObject).onClick = OnQuestButtonClick;
        EventTriggerListener.Get(m_SpeechButton.gameObject).onClick = OnSpeechButtonClick;
        EventTriggerListener.Get(btnMail.gameObject).onClick = OnMailButtonClick;
		EventTriggerListener.Get (shopButton.gameObject).onClick = OnShopButtonClick;


        m_LangPopup.Initialize<PopupListIndextDelegate>(this);
        m_LangPopup.AddItem((int)Language.Chinese, StaticDataMgr.Instance.GetTextByID("ui_chinese"));
        m_LangPopup.AddItem((int)Language.English, StaticDataMgr.Instance.GetTextByID("ui_english"));
        m_LangPopup.SetSelection((int)LanguageMgr.Instance.Lang);

		BindListener ();
    }

	void OnDestroy()
	{
		UnBindListener ();
	}

    void BindListener()
	{
		GameEventMgr.Instance.AddListener<int> (GameEventList.LevelChanged, OnLevelChanged);
        GameEventMgr.Instance.AddListener<int>(GameEventList.MailAdd, OnMailChanged);
        GameEventMgr.Instance.AddListener<int>(GameEventList.MailRead, OnMailChanged);
        GameEventMgr.Instance.AddListener<int, int>(GameEventList.PlayerExpChanged, OnPlayerExpChanged);
	}

	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<int> (GameEventList.LevelChanged, OnLevelChanged);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.MailAdd, OnMailChanged);
        GameEventMgr.Instance.RemoveListener<int>(GameEventList.MailRead, OnMailChanged);
        GameEventMgr.Instance.RemoveListener<int, int>(GameEventList.PlayerExpChanged, OnPlayerExpChanged);
	}

	void OnLevelChanged(int level)
	{
        levelText.text = string.Format("LVL {0}", level);
		nameText.text = GameDataMgr.Instance.PlayerDataAttr.nickName;
	}

    void OnPlayerExpChanged(int oldExp,int newExp)
    {
        PlayerLevelAttr originalAttr = StaticDataMgr.Instance.GetPlayerLevelAttr(GameDataMgr.Instance.PlayerDataAttr.level);
        if (null == originalAttr)
            return;
        float targetExp = newExp / (float)originalAttr.exp;
       // playerProgress.SetCurrrentRatio(targetExp);
        playerProgress.SetTargetRatio(targetExp);
        playerProgress.SkipAnimation();
        int ratio = (int)(targetExp * 100);
        playerProgress.mProgressText.text = string.Format("{0}%", ratio);
    }

    //邮件同步，新邮件事件
    void OnMailChanged(int mail)
    {
        //mailTips.SetActive(UIUtil.CheckHaveNewMail());
        btnMail.SetRemind(UIUtil.CheckHaveNewMail());
    }

  //  static int oldLevel = 1;
 //   static int newLevel = 2;
    void BagButtonClick(GameObject go)
    {
        uiBag = UIMgr.Instance.OpenUI_(UIBag.ViewName)as UIBag;

      //  LevelUp.OpenWith(oldLevel ++, oldLevel++, 200, 399);
    }

    void PetButtonClick(GameObject go)
    {
        uiPetList= UIMgr.Instance.OpenUI_(UIPetList.ViewName) as UIPetList;
    }

	void OnInstanceButtonClick(GameObject go)
	{
        OpenInstanceUI();
	}

    void OnQuestButtonClick(GameObject go)
    {
        uiQuest= UIMgr.Instance.OpenUI_(UIQuest.ViewName) as UIQuest;
    }

    void OnMailButtonClick(GameObject go)
    {
        uiMail = UIMgr.Instance.OpenUI_(UIMail.ViewName) as UIMail;
    }

	public	UIShop OpenShop(int shopType)
	{
		uiShop = UIMgr.Instance.OpenUI_ (UIShop.ViewName) as UIShop;
		uiShop.RefreshShopData (shopType);
		return uiShop;
	}

	void OnShopButtonClick( GameObject go)
	{
		OpenShop ((int)PB.shopType.NORMALSHOP);
	}

	void OnExchangeButtonclick(GameObject go)
	{
		UICoinExchange.Open ();
	}

    void OnSpeechButtonClick(GameObject go)
    {
        //UISpeech.Open(m_SpeechInput.text);
        //AudioSystemMgr.Instance.PlaySound(go,SoundType.Click);
        PB.HSMonsterCatch mcache = new PB.HSMonsterCatch();

      //  mcache.cfgId = m_SpeechInput.text.ToString();// Unit_Demo_jiuweihu  Unit_Demo_qingniao.
        UnitData monster = StaticDataMgr.Instance.GetUnitRowData(mcache.cfgId);
        if (monster == null) return;
        ArrayList spellArrayList = MiniJsonExtensions.arrayListFromJson(monster.spellIDList);
        for (int i = 0; i < spellArrayList.Count; ++i)
        {
            string spellID = spellArrayList[i] as string;
            mcache.skill.Add(new PB.HSSkill() { skillId = spellID, level = 2 });
        }	

        GameApp.Instance.netManager.SendMessage(ProtocolMessage.Create(PB.code.MONSTER_CATCH_C.GetHashCode(), mcache));
    }


    public void OnPopupListChanged(int index)
    {
        Language lang = (Language)index;
        if (lang != LanguageMgr.Instance.Lang)
        {
            LanguageMgr.Instance.Lang = lang;
            m_LangPopup.RefrshItem((int)Language.Chinese, StaticDataMgr.Instance.GetTextByID("ui_chinese"));
            m_LangPopup.RefrshItem((int)Language.English, StaticDataMgr.Instance.GetTextByID("ui_english"));
            //GameMain.Instance.ChangeModule<LoginModule>();
        }
    }

    public override void Init()
    {
        OnMailChanged(0);
        OnLevelChanged(GameDataMgr.Instance.PlayerDataAttr.level);
        OnPlayerExpChanged(0, GameDataMgr.Instance.PlayerDataAttr.ExpAttr);
    }
    public override void Clean()
    {
        UIMgr.Instance.DestroyUI(uiQuest);
        UIMgr.Instance.DestroyUI(uiMail);
        UIMgr.Instance.DestroyUI(uiBag);
        UIMgr.Instance.DestroyUI(uiInstance);
        UIMgr.Instance.DestroyUI(uiPetList);
        UIMgr.Instance.DestroyUI(uiAdjustBattleTeam);
		UIMgr.Instance.DestroyUI (uiShop);
    }

    public UIInstance OpenInstanceUI()
    {
        uiInstance = UIMgr.Instance.OpenUI_(UIInstance.ViewName) as UIInstance;
        return uiInstance;
    }
}
