using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyUnitUI : MonoBehaviour 
{
	public Text unitName;
	public Text unitLevel;
	public LifeBarUI lifeBar;

    public BattleObject Unit
    {
        get
        {
            return targetUnit;
        }
        set
        {
            targetUnit = value;
        }
    }
    private BattleObject targetUnit;
    private UIBuffView buffView;
	//RectTransform rectTrans;

    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        buffView = gameObject.GetComponent<UIBuffView>();
        buffView.Init();
		//rectTrans = transform as RectTransform;
    }
    //---------------------------------------------------------------------------------------------
    public void ChangeBuffState(SpellBuffArgs args)
    {
        buffView.OnBuffChanged(args);
    }
    //---------------------------------------------------------------------------------------------
    public void Show(BattleObject sUnit)
    {
        buffView.SetTargetUnit(sUnit);
        lifeBar.LifeTarget = sUnit;
        Unit = sUnit;
        if (sUnit == null)
        {
            Hide();
            return;
        }

        unitName.text = sUnit.unit.name;
        unitLevel.text = sUnit.unit.pbUnit.level.ToString();

        gameObject.SetActive(Unit.unit.isVisible == true);
    }
    //---------------------------------------------------------------------------------------------
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------
}
