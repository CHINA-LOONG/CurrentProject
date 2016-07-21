using UnityEngine;
using System.Collections;

//public class UIPetEquipInfoParam : PetRightParamBase
//{
//    public EquipData equip;
//}

public class UIPetEquipInfo : PetDetailRightBase,IEquipCallBack
{

    private UIEquipDetails uiEquipDetails;

    private UIPetEquipParam info;

    public override void ReloadData(PetRightParamBase param)
    {
        info = param as UIPetEquipParam;

        if (uiEquipDetails == null)
        {
            uiEquipDetails = UIEquipDetails.CreateEquip();
            uiEquipDetails.equipCallBack = this;
            UIUtil.SetParentReset(uiEquipDetails.transform, transform);
        }
        else
        {
            uiEquipDetails.gameObject.SetActive(true);
        }

        uiEquipDetails.Show(info.unit.equipList[(int)info.part], info.unit, EquipState.show);

    }

    //强化
    public void Reinforced(EquipData data)
    {
        //TODO:
    }
    //镶嵌
    public void Inlay(EquipData data)
    {
        //TODO:
    }
    //卸载
    public void Unload()
    {
        ParentNode.ReloadLeftData();
        ParentNode.ReloadRigthData(PetViewConst.UIPetEquipListAssetName);
    }
    //打开列表
    public void Replacement(GameUnit unit, int equipPart)
    {

    }
}
