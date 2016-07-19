using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PetListElement : MonoBehaviour {

    public Text nameLable;
    public Image proIcon;
    public Text typeLable;
    public MonsterIcon avatar;
    public Image badge;

    public GameUnit unit;

    public void Init()
    {
 
    }

    public void ReloadPatData(GameUnit unit)
    {
        this.unit = unit;
        UIUtil.SetStageColor(nameLable, unit);

        var image = ResourceMgr.Instance.LoadAssetType<Sprite>("property_" + unit.property) as Sprite;
        if (image != null)
        {
            proIcon.sprite = image;
        }

        typeLable.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetListType);
        avatar.Init();
        avatar.SetId(unit.pbUnit.guid.ToString());
        avatar.SetMonsterStaticId(unit.pbUnit.id);
        avatar.SetStage(unit.pbUnit.stage);
        avatar.SetLevel(unit.pbUnit.level);
        avatar.iconButton.gameObject.SetActive(false);
        CheckStageStatus();
    }

    public void CheckStageStatus()
    {
        badge.gameObject.SetActive(UIUtil.CheckIsEnoughMaterial(unit));
    }
}
