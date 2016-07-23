using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PetListElement : MonoBehaviour {

    public Text nameLable;
    public Image proIcon;
    //public Text typeLable;
    public MonsterIcon avatar;
    public Image badge;

    public Transform[] equipPos = new Transform[6];
    private ItemIcon[] equipIcon = new ItemIcon[6];

    public GameUnit unit;

    public void ReloadPatData(GameUnit unit)
    {
        this.unit = unit;
        UIUtil.SetStageColor(nameLable, unit);

        var image = ResourceMgr.Instance.LoadAssetType<Sprite>("property_" + unit.property) as Sprite;
        if (image != null)
        {
            proIcon.sprite = image;
        }

        //typeLable.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetListType);
        avatar.Init();
        avatar.SetId(unit.pbUnit.guid.ToString());
        avatar.SetMonsterStaticId(unit.pbUnit.id);
        avatar.SetStage(unit.pbUnit.stage);
        avatar.SetLevel(unit.pbUnit.level);
        avatar.iconButton.gameObject.SetActive(false);

        for (int i = 0; i < unit.equipList.Length; i++)
        {
            if (unit.equipList[i]==null)
            {
                if (equipIcon[i]!=null)
                {
                    equipIcon[i].gameObject.SetActive(false);
                }
            }
            else
            {
                EquipData equipData=new EquipData() { equipId = unit.equipList[i].equipId,stage=unit.equipList[i].stage };
                if (equipIcon[i]==null)
                {
                    equipIcon[i] = ItemIcon.CreateItemIcon(equipData);
                    UIUtil.SetParentReset(equipIcon[i].transform, equipPos[i]);
                    equipIcon[i].HideExceptIcon();
                }
                else
                {
                    equipIcon[i].gameObject.SetActive(true);
                    equipIcon[i].RefreshWithEquipInfo(equipData);
                    equipIcon[i].HideExceptIcon();
                }
            }
        }


        //CheckStageStatus();
    }

    public void CheckStageStatus()
    {
        badge.gameObject.SetActive(UIUtil.CheckIsEnoughMaterial(unit));
    }
}
