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
    public void UpdateShow(BattleObject battleUnit)
    {
        lifeBar.LifeTarget = battleUnit;

        buffView.SetTargetUnit(battleUnit);

        //Vector3 viewPosition = BattleCamera.Instance.CameraAttr.WorldToViewportPoint (battleUnit.gameObject.transform.position);
        //Vector3 screenPosition = UICamera.Instance.CameraAttr.ViewportToScreenPoint (viewPosition);
        //rectTrans.anchoredPosition = screenPosition;

        unitName.text = battleUnit.name;
        //if (battleUnit.gameObject.activeSelf)
        //{
        //    lifeBar.value = battleUnit.unit.curLife / (float)battleUnit.unit.maxLife;
        //    //battleUnit.isBorn = false;
        //}
        unitLevel.text = battleUnit.unit.pbUnit.level.ToString();
    }
    //---------------------------------------------------------------------------------------------
}
