using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleUnitUI : MonoBehaviour
{
    public Button dazhaoBtn;
    public Text unitName;
    public LifeBarUI lifeBar;
    public EnegyBarUI enegyBar;

    public GameUnit Unit { get; set; }
    private UIBuffView buffView;

    // Use this for initialization
    void Awake()
    {
       EventTriggerListener.Get(dazhaoBtn.gameObject).onClick = OnDazhaoClick;
       buffView = gameObject.GetComponent<UIBuffView>();
       buffView.Init();
    }

    public void Show(GameUnit sUnit)
    {
        buffView.SetTargetUnit(sUnit);
        if (sUnit == null)
        {
            Hide();
            return;
        }

        Unit = sUnit;

        dazhaoBtn.gameObject.SetActive(false);
        unitName.text = Unit.name;
        lifeBar.value = Unit.curLife / (float)Unit.maxLife;
        enegyBar.value = Mathf.Clamp01(Unit.energy / (float)BattleConst.enegyMax);

        if (Unit.energy >= BattleConst.enegyMax)
        {
            dazhaoBtn.gameObject.SetActive(true);
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void OnDazhaoClick(GameObject go)
    {
        GameEventMgr.Instance.FireEvent<GameUnit>(GameEventList.HitDazhaoBtn, Unit);
		GameEventMgr.Instance.FireEvent<bool> (GameEventList.SetMirrorModeState, false);
    }
}
