using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleUnitUI : MonoBehaviour
{
    public Button dazhaoBtn;
    public Text unitName;
    public LifeBarUI lifeBar;
    public EnegyBarUI enegyBar;

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

    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Awake()
    {
       EventTriggerListener.Get(dazhaoBtn.gameObject).onClick = OnDazhaoClick;
       buffView = gameObject.GetComponent<UIBuffView>();
       buffView.Init();
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

        dazhaoBtn.gameObject.SetActive(false);
        unitName.text = Unit.unit.name;
        //if (sUnit.gameObject.activeSelf)
        //{
        //    lifeBar.value = Unit.unit.curLife / (float)Unit.unit.maxLife;
        //}
        enegyBar.value = Mathf.Clamp01(Unit.unit.energy / (float)BattleConst.enegyMax);

        if (Unit.unit.energy >= BattleConst.enegyMax)
        {
            dazhaoBtn.gameObject.SetActive(true);
        }

        gameObject.SetActive(Unit.unit.isVisible == true);
    }
    //---------------------------------------------------------------------------------------------
    public void SetEnergy(int currentVital)
    {
        enegyBar.value = Mathf.Clamp01(currentVital / (float)BattleConst.enegyMax);
        dazhaoBtn.gameObject.SetActive(currentVital >= BattleConst.enegyMax);
    }
    //---------------------------------------------------------------------------------------------
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------
    void OnDazhaoClick(GameObject go)
    {
        GameEventMgr.Instance.FireEvent<BattleObject>(GameEventList.HitDazhaoBtn, Unit);
		GameEventMgr.Instance.FireEvent<bool> (GameEventList.SetMirrorModeState, false);
    }
    //---------------------------------------------------------------------------------------------
}
