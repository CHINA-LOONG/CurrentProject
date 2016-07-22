using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIBuild : UIBase,PopupListIndextDelegate
{
    public static string ViewName = "UIBuild";

    public Button m_PatButton;
    public Button m_ItemButton;
	public	Button	instanceButton;

    public Button m_QuestButton;
    public Button m_SpeechButton;
	public Button shopButton;
    public InputField m_SpeechInput;

    public PopupList m_LangPopup;

    public Button btnMail;

	public Text levelText;
	public Text coinText;
	public Text nameText;

    public UIMail uiMail;
    public UIQuest uiQuest;
    public UIBag uiBag;
    public UIInstance uiInstance;
    public UIPetList uiPetList;
    public UIAdjustBattleTeam uiAdjustBattleTeam;
	public UIShop uiShop;


    void Start()
    {
        EventTriggerListener.Get(m_PatButton.gameObject).onClick = PatButtonClick;
        EventTriggerListener.Get(m_ItemButton.gameObject).onClick = ItemButtonClick;
        EventTriggerListener.Get(instanceButton.gameObject).onClick = OnInstanceButtonClick;
        EventTriggerListener.Get(m_QuestButton.gameObject).onClick = OnQuestButtonClick;
        EventTriggerListener.Get(m_SpeechButton.gameObject).onClick = OnSpeechButtonClick;
        EventTriggerListener.Get(btnMail.gameObject).onClick = OnMailButtonClick;
		EventTriggerListener.Get (shopButton.gameObject).onClick = OnShopButtonClick;

        m_LangPopup.Initialize<PopupListIndextDelegate>(this);
        m_LangPopup.AddItem((int)Language.Chinese, StaticDataMgr.Instance.GetTextByID("ui_chinese"));
        m_LangPopup.AddItem((int)Language.English, StaticDataMgr.Instance.GetTextByID("ui_english"));
        m_LangPopup.SetSelection((int)LanguageMgr.Instance.Lang);

        //levelText.text = GameDataMgr.Instance.PlayerDataAttr.level.ToString ();
        //coinText.text = GameDataMgr.Instance.PlayerDataAttr.coin.ToString ();
        //nameText.text = GameDataMgr.Instance.PlayerDataAttr.nickName;
		BindListener ();
    }

	void OnDestroy()
	{
		UnBindListener ();
	}

	void BindListener()
	{
		GameEventMgr.Instance.AddListener<int> (GameEventList.LevelChanged, OnLevelChanged);
		GameEventMgr.Instance.AddListener<long> (GameEventList.CoinChanged, OnCoinChanged);

		GameEventMgr.Instance.AddListener<ProtocolMessage> (PB.code.MONSTER_CATCH_S.GetHashCode().ToString(),OnCachMonsterFinished);
	}

	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<int> (GameEventList.LevelChanged, OnLevelChanged);
		GameEventMgr.Instance.RemoveListener<long> (GameEventList.CoinChanged, OnCoinChanged);

		GameEventMgr.Instance.RemoveListener<ProtocolMessage> (PB.code.MONSTER_CATCH_S.GetHashCode().ToString(),OnCachMonsterFinished);
	}

	void OnLevelChanged(int level)
	{
		levelText.text = level.ToString ();
		nameText.text = GameDataMgr.Instance.PlayerDataAttr.nickName;
	}

	void OnCoinChanged(long coin)
	{
		coinText.text = coin.ToString ();
	}

    void ItemButtonClick(GameObject go)
    {
        uiBag = UIMgr.Instance.OpenUI_(UIBag.ViewName)as UIBag;
    }

    void PatButtonClick(GameObject go)
    {
        uiPetList= UIMgr.Instance.OpenUI_(UIPetList.ViewName) as UIPetList;
    }

	void OnInstanceButtonClick(GameObject go)
	{
		//PB.HSMonsterCatch mcache = new PB.HSMonsterCatch ();
	//	mcache.cfgId = "Unit_Demo_qingniao";// Unit_Demo_jiuweihu  Unit_Demo_qingniao.

	//	GameApp.Instance.netManager.SendMessage (ProtocolMessage.Create (PB.code.MONSTER_CATCH_C.GetHashCode(), mcache));

        uiInstance = UIMgr.Instance.OpenUI_(UIInstance.ViewName) as UIInstance;
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

    void OnSpeechButtonClick(GameObject go)
    {
        //UISpeech.Open(m_SpeechInput.text);
        //AudioSystemMgr.Instance.PlaySound(go,SoundType.Click);
        PB.HSMonsterCatch mcache = new PB.HSMonsterCatch();

        mcache.cfgId = m_SpeechInput.text.ToString();// Unit_Demo_jiuweihu  Unit_Demo_qingniao.
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

	void OnCachMonsterFinished(ProtocolMessage msg)
    {
        if (msg == null)
            return;
        PB.HSMonsterCatchRet result = msg.GetProtocolBody<PB.HSMonsterCatchRet>();
        if (result!=null&&result.status==1)
        {
            GameObject go = new GameObject("tips");
            go.transform.parent = m_SpeechButton.transform;
            go.transform.localPosition = new Vector3(50, 50, -10);
            Text tex = go.AddComponent<Text>();
            tex.text = "获取成功";

            Destroy(go, 5.0f);
        }
        
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
}
