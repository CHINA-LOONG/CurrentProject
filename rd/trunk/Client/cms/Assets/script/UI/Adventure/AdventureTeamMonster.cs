using UnityEngine;
using UnityEngine.UI;

public interface IAdventureTeamMonster
{
    void OnClickTeamMonster<T>(T monster);
}

public class AdventureTeamMonster : MonoBehaviour
{
    public Image imgSelect;

    public Transform iconPos;
    private MonsterIcon monsterIcon;

    private bool isSelect;
    public bool IsSelect
    {
        get{return isSelect; }
        set
        {
            isSelect = value;
            imgSelect.gameObject.SetActive(isSelect);
        }
    }

    public enum MonsterType
    {
        NULL,
        SELF,
        GUILD
    }
    public MonsterType type = MonsterType.NULL;

    public IAdventureTeamMonster IAdventureTeamMonsterDelegate;

    public AdventureSelfMonsterInfo selfMonster;
    public AdventureGuildMonsterInfo guildMonster;
    
    public void ReloadData<T>(T monster)
    {
        if (monster == null)
        {
            type = MonsterType.NULL;
            if (monsterIcon != null)
            {
                monsterIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            #region 创建MonsterIcon
            if (monsterIcon == null)
            {
                monsterIcon = MonsterIcon.CreateIcon();
                UIUtil.SetParentReset(monsterIcon.transform, iconPos);
                ScrollViewEventListener.Get(monsterIcon.iconButton.gameObject).onClick = OnClickTeamIcon;
            }
            else
            {
                monsterIcon.gameObject.SetActive(true);
                monsterIcon.Init();
            }
            #endregion

            if (monster is AdventureSelfMonsterInfo)
            {
                selfMonster = monster as AdventureSelfMonsterInfo;
                type = MonsterType.SELF;
                monsterIcon.SetId(selfMonster.unit.pbUnit.guid.ToString());
                monsterIcon.SetMonsterStaticId(selfMonster.unit.pbUnit.id);
                monsterIcon.SetStage(selfMonster.unit.pbUnit.stage);
                monsterIcon.SetLevel(selfMonster.unit.pbUnit.level);
            }
            else if (monster is AdventureGuildMonsterInfo)
            {
                guildMonster = monster as AdventureGuildMonsterInfo;
                type = MonsterType.GUILD;
                monsterIcon.SetId(guildMonster.unit.monsterId.ToString());
                monsterIcon.SetMonsterStaticId(guildMonster.unit.cfgId);
                monsterIcon.SetStage(guildMonster.unit.stage);
                monsterIcon.SetLevel(guildMonster.unit.level);
            }
        }
    }

    void OnClickTeamIcon(GameObject go)
    {
        if (type==MonsterType.SELF)
        {
            if (IAdventureTeamMonsterDelegate!=null)
            {
                IAdventureTeamMonsterDelegate.OnClickTeamMonster(selfMonster);
            }
        }
        else if(type==MonsterType.GUILD)
        {
            IAdventureTeamMonsterDelegate.OnClickTeamMonster(guildMonster);
        }
    }
}
