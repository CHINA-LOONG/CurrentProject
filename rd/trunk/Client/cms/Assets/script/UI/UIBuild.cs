using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBuild : UIBase
{
    public static string ViewName = "UIBuild";
	public static string AssertName = "ui/build";

    public Button m_PatButton;
    public Button m_ItemButton;
	public	Button	instanceButton;

    public Button m_QuestButton;

	public Text levelText;
	public Text coinText;
	public Text nameText;

    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(m_PatButton.gameObject).onClick = PatButtonClick;
        EventTriggerListener.Get(m_ItemButton.gameObject).onClick = ItemButtonClick;
		EventTriggerListener.Get (instanceButton.gameObject).onClick = OnInstanceButtonClick;
        EventTriggerListener.Get(m_QuestButton.gameObject).onClick = OnQuestButtonClick;
        

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
        UIMgr.Instance.OpenUI(UIBag.AssertName, UIBag.ViewName);
    }

    void PatButtonClick(GameObject go)
    {
        UIMgr.Instance.OpenUI(UIPet.AssertName, UIPet.ViewName);
    }

	void OnInstanceButtonClick(GameObject go)
	{
		//PB.HSMonsterCatch mcache = new PB.HSMonsterCatch ();
	//	mcache.cfgId = "Unit_Demo_qingniao";// Unit_Demo_jiuweihu  Unit_Demo_qingniao.

	//	GameApp.Instance.netManager.SendMessage (ProtocolMessage.Create (PB.code.MONSTER_CATCH_C.GetHashCode(), mcache));

		UIMgr.Instance.OpenUI (UIInstance.AssertName, UIInstance.ViewName);
	}

    void OnQuestButtonClick(GameObject go)
    {
        UIMgr.Instance.OpenUI(UIQuest.AssertName,UIQuest.ViewName);
    }

	void OnCachMonsterFinished(ProtocolMessage msg)
	{

	}
}
