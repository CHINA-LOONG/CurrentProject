using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectMonsterElement : MonoBehaviour {

    public Text nameLabel;
    public Text characterLabel;
    public Text battleLabel;
    public Text battle;
    public GameObject maskView;
    public GameObject eventObject;
    public Transform monIconPos;
    private MonsterIcon avatar;

    public MonsterIcon Avatar
    {
        get
        {
            if (avatar == null)
            {
                avatar = MonsterIcon.CreateIcon();
                UIUtil.SetParentReset(avatar.transform, monIconPos);
            }
            return avatar; 
        }
        set { avatar = value; }
    }

    [HideInInspector]
    public int guid;

    [HideInInspector]
    public bool showMask
    {
        set
        {
            maskView.SetActive(value);
        }
    }

    private bool isSelect = false;
    [HideInInspector]
    public bool select
    {
        get
        {
            return isSelect;
        }
        set
        {
            avatar.ShowSelectImage(value);
            isSelect = value;
        }
    }

    public void ReloadData(GameUnit unit, bool isSelect)
    {
        this.isSelect = isSelect;
        guid = unit.pbUnit.guid;
        nameLabel.text = unit.name;
        characterLabel.text = StaticDataMgr.Instance.GetTextByID("pet_character_" + unit.character);
        battleLabel.text = StaticDataMgr.Instance.GetTextByID("equip_forge_zhanli");
        battle.text = unit.mBp.ToString();

        Avatar.Init();
        Avatar.SetMonsterStaticId(unit.pbUnit.id);
        Avatar.SetStage(unit.pbUnit.stage);
        Avatar.SetLevel(unit.pbUnit.level);
        Avatar.iconButton.gameObject.SetActive(false);
        Avatar.ShowSelectImage(isSelect);
    }
}
