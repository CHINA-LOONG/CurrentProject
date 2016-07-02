using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleUnitUI : MonoBehaviour
{
    public Button dazhaoBtn;
    public Text unitName;
    public Scrollbar lifeBar;

    GameUnit unit;
    public GameUnit Unit
    {
        set { unit = value; }
    }

    // Use this for initialization
    void Start()
    {

    }

    public void Show(GameUnit sUnit)
    {
        unit = sUnit;

        unitName.text = unit.name;
        lifeBar.size = unit.curLife / (float)unit.maxLife;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
