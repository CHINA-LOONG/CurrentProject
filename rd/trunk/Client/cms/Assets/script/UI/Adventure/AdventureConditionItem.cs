using UnityEngine;
using UnityEngine.UI;

public interface IAdventureConditionItem
{
    void onSelectionCondition(AdventureConditionItem condition,bool filter);
}
public class AdventureConditionItem : MonoBehaviour
{

    public Text textDesc;
    public HomeButton btnFilter;

    public bool IsFilter
    {
        get { return btnFilter.IsOn; }
        set{ btnFilter.IsOn = value; }
    }

    public PB.HSAdventureCondition curData;
    public AdventureConditionTypeData conditionData;

    public IAdventureConditionItem IAdventureConditionItemDelegate;
    void Start()
    {
        btnFilter.onClick = OnClickFilterBtn;
    }

    public void ReloadData(PB.HSAdventureCondition condition)
    {
        curData = condition;
        conditionData = StaticDataMgr.Instance.GetAdventureConditionType(condition.conditionTypeCfgId);
        textDesc.text =condition.monsterCount+ conditionData.desc;
    }

    void OnClickFilterBtn(GameObject go)
    {
        if (IAdventureConditionItemDelegate!=null)
        {
            IAdventureConditionItemDelegate.onSelectionCondition(this, IsFilter);
        }
    }

}
