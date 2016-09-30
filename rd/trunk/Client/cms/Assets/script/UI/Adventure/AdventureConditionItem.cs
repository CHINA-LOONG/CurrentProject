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
    public Text textFilter;
    private Color defaultColor;
    public Color meetColor;

    public bool IsFilter
    {
        get { return btnFilter.IsOn; }
        set{ btnFilter.IsOn = value; }
    }
    [HideInInspector]
    public PB.HSAdventureCondition curData;
    [HideInInspector]
    public AdventureConditionTypeData conditionData;

    public IAdventureConditionItem IAdventureConditionItemDelegate;

    void Awake()
    {
        defaultColor = textDesc.color;
    }
    void Start()
    {
        btnFilter.onClick = OnClickFilterBtn;
        textFilter.text = StaticDataMgr.Instance.GetTextByID("adventure_filter");
    }

    public void ReloadData(PB.HSAdventureCondition condition)
    {
        curData = condition;
        conditionData = StaticDataMgr.Instance.GetAdventureConditionType(condition.conditionTypeCfgId);
        textDesc.text = string.Format(StaticDataMgr.Instance.GetTextByID(conditionData.desc), condition.monsterCount);
    }

    void OnClickFilterBtn(GameObject go)
    {
        if (IAdventureConditionItemDelegate!=null)
        {
            IAdventureConditionItemDelegate.onSelectionCondition(this, IsFilter);
        }
    }
    /// <summary>
    /// 设置条件状态
    /// </summary>
    /// <param name="state">是否达成状态</param>
    public void SetState(bool state)
    {
        textDesc.color = state ? meetColor : defaultColor;
    }

}
