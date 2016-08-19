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

        uiEquipDetails.Show(info.unit.equipList[(int)info.part-1], info.unit, EquipState.show);

    }

    //强化
    public void Reinforced(EquipData data)
    {
        ParentNode.ReloadRigthData(PetViewConst.UIPetEquipInlayAssetName, new UIPetInlayParam() { equip = data, tabIndex = 0, selIndex = -1 });
    }
    //镶嵌
    public void Inlay(EquipData data)
    {
        ParentNode.ReloadRigthData(PetViewConst.UIPetEquipInlayAssetName, new UIPetInlayParam() { equip = data, tabIndex = 1, selIndex = -1 });
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
        ParentNode.ReloadRigthData(PetViewConst.UIPetEquipListAssetName);
    }
}
