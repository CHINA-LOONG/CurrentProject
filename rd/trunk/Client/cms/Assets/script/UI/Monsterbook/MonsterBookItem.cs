using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MonsterBookItem : MonoBehaviour
{

    public Text textName;
    public Text textCount;
    public Transform posIcon;
    private MonsterIcon iconMonster;

    public void ReloadData(MonsterBookItemInfo data)
    {
        textName.text = StaticDataMgr.Instance.GetTextByID(data.unit.name);
        textCount.text = data.count;
        if (iconMonster == null)
        {
            iconMonster = MonsterIcon.CreateIcon();
            UIUtil.SetParentReset(iconMonster.transform, posIcon);
        }
        else
        {
            iconMonster.gameObject.SetActive(true);
            iconMonster.Init();
        }
        iconMonster.SetId(data.unit.pbUnit.guid.ToString());
        iconMonster.SetMonsterStaticId(data.unit.pbUnit.id);
        iconMonster.SetStage(data.unit.pbUnit.stage);
        iconMonster.SetLevel(data.unit.pbUnit.level);
        iconMonster.iconButton.gameObject.SetActive(false);

    }
}

public class MonsterBookItemInfo
{
    public string name;
    public string count = "100/200";
    public GameUnit unit;

    public MonsterBookItemInfo(GameUnit unit)
    {
        name = unit.name;

        this.unit = unit;
    }
}
