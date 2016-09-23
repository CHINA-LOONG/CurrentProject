using UnityEngine;
using System.Collections;

public class AdventureSelfMonsterInfo
{
    public GameUnit unit;
    public UnitData unitData;

    private bool isSelect;
    public bool IsSelect
    {
        get   {return isSelect; }
        set
        {
            if (isSelect!=value)
            {
                isSelect = value;
                if (setSelect!=null)
                {
                    setSelect(isSelect,this);
                }
            }
        }
    }

    public System.Action<bool,AdventureSelfMonsterInfo> setSelect;

    public static bool operator ==(AdventureSelfMonsterInfo a, AdventureSelfMonsterInfo b)
    {
        if ((a as object) != null && (b as object) != null)
        {
            if (a.unit.pbUnit.guid == b.unit.pbUnit.guid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if ((a as object) == null && (b as object)== null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public static bool operator !=(AdventureSelfMonsterInfo a, AdventureSelfMonsterInfo b)
    {
        return !(a == b);
    }

}


public interface IAdventureSelfMonster
{
    void OnClickSelfMonster(AdventureSelfMonsterInfo monster);
}
public class AdventureSelfMonster : MonoBehaviour
{

    private MonsterIcon icon;
    public MonsterIcon Icon
    {
        get
        {
            if (icon==null)
            {
                icon = transform.GetComponent<MonsterIcon>();
            }
            return icon;
        }
    }

    private AdventureSelfMonsterInfo curData;
    public AdventureSelfMonsterInfo CurData
    {
        get   {return curData; }
        set
        {
            curData = value;
            curData.setSelect = SetIconState;
        }
    }

    public IAdventureSelfMonster IAdventureSelfMonsterDelegate;
    void Start()
    {
        ScrollViewEventListener.Get(Icon.iconButton.gameObject).onClick = OnClickSelfIcon;
    }
    public void ReloadData(AdventureSelfMonsterInfo info)
    {
        CurData = info;

        Icon.Init();
        icon.SetMonsterStaticId(CurData.unit.pbUnit.id);
        icon.SetId(CurData.unit.pbUnit.guid.ToString());
        icon.SetLevel(CurData.unit.pbUnit.level);
        icon.SetStage(CurData.unit.pbUnit.stage);
        
        icon.ShowMaoxianImage(CurData.unit.pbUnit.IsInAdventure());
        icon.ShowZhushouImage(CurData.unit.pbUnit.IsInAllianceBase());
        icon.ShowLockImage(CurData.unit.pbUnit.IsLocked());

        if (CurData.unit.pbUnit.IsInAdventure()|| CurData.unit.pbUnit.IsInAllianceBase())
        {
            icon.ShowMaskImage(true);
            icon.ShowSelectImage(false);
        }
        else
        {
            SetIconState(CurData.IsSelect,CurData);
        }
    }

    void SetIconState(bool isSelect,AdventureSelfMonsterInfo info)
    {
        if (CurData == info)
        {
            icon.ShowSelectImage(isSelect);
            icon.ShowMaskImage(isSelect);
        }
    }

    void OnClickSelfIcon(GameObject go)
    {
        //todo
        if (CurData.unit.pbUnit.IsInAdventure())
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("arrayselect_count_005"), (int)PB.ImType.PROMPT);
        }
        else if (CurData.unit.pbUnit.IsInAllianceBase())
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("adventure_record_009"), (int)PB.ImType.PROMPT);
        }
        else
        {
            if (IAdventureSelfMonsterDelegate != null)
            {
                IAdventureSelfMonsterDelegate.OnClickSelfMonster(CurData);
            }
        }
    }

}
