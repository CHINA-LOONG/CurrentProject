using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBuild : UIBase
{
    public static string ViewName = "UIBuild";

    public Button m_BattleButton;
    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(m_BattleButton.gameObject).onClick = BattleButtonClick;
    }

    void BattleButtonClick(GameObject go)
    {
        GameEventMgr.Instance.FireEvent(GameEventList.BattleBtnClick);
    }

}
