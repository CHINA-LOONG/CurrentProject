using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBuild : UIBase
{
    public static string ViewName = "UIBuild";
	public static string AssertName = "ui/build";

    public Button m_NormalBattleButton;
    public Button m_BossBattleButton;
    public Button m_RareBattleButton;

    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(m_NormalBattleButton.gameObject).onClick = NormalBattleButtonClick;
        EventTriggerListener.Get(m_BossBattleButton.gameObject).onClick = BossBattleButtonClick;
        EventTriggerListener.Get(m_RareBattleButton.gameObject).onClick = RareBattleButtonClick;
    }

    void NormalBattleButtonClick(GameObject go)
    {
        var proto = BattleTest.GenerateNormalProto("demo");
        GameEventMgr.Instance.FireEvent<PbStartBattle>(GameEventList.BattleBtnClick, proto);
    }

    void BossBattleButtonClick(GameObject go)
    {
        var proto = BattleTest.GenerateBossProto("demo");
        GameEventMgr.Instance.FireEvent<PbStartBattle>(GameEventList.BattleBtnClick, proto);
    }

    void RareBattleButtonClick(GameObject go)
    {
        var proto = BattleTest.GenerateRareProto("demo");
        GameEventMgr.Instance.FireEvent<PbStartBattle>(GameEventList.BattleBtnClick, proto);
    }

}
