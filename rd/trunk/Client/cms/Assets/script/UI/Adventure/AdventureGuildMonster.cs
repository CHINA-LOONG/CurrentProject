using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AdventureGuildMonsterInfo
{
    public PB.AllianceBaseMonster unit;
    public UnitData unitData;
    public Sociatybase sociatyData;

    private bool isSelect;
    public bool IsSelect
    {
        get { return isSelect; }
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

    public System.Action<bool,AdventureGuildMonsterInfo> setSelect;

    public static bool operator ==(AdventureGuildMonsterInfo a, AdventureGuildMonsterInfo b)
    {
        if ((a as object) != null && (b as object) != null)
        {
            if (a.unit.monsterId == b.unit.monsterId)
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
            if ((a as object) == null && (b as object) == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public static bool operator !=(AdventureGuildMonsterInfo a, AdventureGuildMonsterInfo b)
    {
        return !(a == b);
    }
}

public interface IAdventureGuildMonster
{
    void OnClickGuildMonster(AdventureGuildMonsterInfo monster);
}
public class AdventureGuildMonster : MonoBehaviour
{
    public Text textName;
    public Text textCoin;

    public Transform iconPos;
    private MonsterIcon monsterIcon;

    private AdventureGuildMonsterInfo curData;
    public AdventureGuildMonsterInfo CurData
    {
        get { return curData; }
        set
        {
            curData = value;
            curData.setSelect = SetIconState;
        }
    }

    public IAdventureGuildMonster IAdventureGuildMonsterDelegate;
    
    public void ReloadData(AdventureGuildMonsterInfo info)
    {
        CurData = info;

        textName.text = CurData.unit.nickname;
        textCoin.text = CurData.sociatyData.coinHire.ToString();

        if (monsterIcon==null)
        {
            monsterIcon = MonsterIcon.CreateIcon();
            UIUtil.SetParentReset(monsterIcon.transform, iconPos);
            ScrollViewEventListener.Get(monsterIcon.iconButton.gameObject).onClick = OnClickGuildIcon;
        }
        else
        {
            monsterIcon.Init();
        }
        monsterIcon.SetMonsterStaticId(CurData.unit.cfgId.ToString());
        monsterIcon.SetId(CurData.unit.id.ToString());
        monsterIcon.SetLevel(CurData.unit.level);
        monsterIcon.SetStage(CurData.unit.stage);

        monsterIcon.ShowMaoxianImage(false);
        monsterIcon.ShowZhushouImage(false);
        monsterIcon.ShowLockImage(false);

        SetIconState(CurData.IsSelect,CurData);
    }
    void SetIconState(bool isSelect,AdventureGuildMonsterInfo info)
    {
        if (curData == info)
        {
            monsterIcon.ShowSelectImage(isSelect);
            monsterIcon.ShowMaskImage(isSelect);
        }
    }

    void OnClickGuildIcon(GameObject go)
    {
        if (IAdventureGuildMonsterDelegate!=null)
        {
            IAdventureGuildMonsterDelegate.OnClickGuildMonster(CurData);
        }
    }



}
