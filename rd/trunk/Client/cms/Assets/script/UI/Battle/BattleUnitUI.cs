using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleUnitUI : MonoBehaviour
{
    public Button dazhaoBtn;
    public Text unitName;
    public LifeBarUI lifeBar;

    public GameUnit Unit { get; set; }

    // Use this for initialization
    void Start()
    {
       EventTriggerListener.Get(dazhaoBtn.gameObject).onClick = OnDazhaoClick;
    }

    public void Show(GameUnit sUnit)
    {
        if (sUnit == null)
        {
            Hide();
            return;
        }

        Unit = sUnit;

        dazhaoBtn.gameObject.SetActive(false);
        unitName.text = Unit.name;
        lifeBar.value = Unit.curLife / (float)Unit.maxLife;

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
        BattleController.Instance.OnUnitCastDazhao(Unit);
    }
}
