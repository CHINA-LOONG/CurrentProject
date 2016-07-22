using UnityEngine;
using System.Collections;

public class UIPetEquipParam : PetRightParamBase
{
    public PartType part;
}

public class UIPetEquipList : PetDetailRightBase,IUIEquipListCallBack
{

    private UIEquipList uiEquipList;

    private UIPetEquipParam info;

    public override void ReloadData(PetRightParamBase param)
    {
        info = param as UIPetEquipParam;

        if (uiEquipList==null)
        {
            uiEquipList = UIEquipList.CreateEquipList();
            uiEquipList.listDelegate = this;
            UIUtil.SetParentReset(uiEquipList.transform,transform);
        }
        else
        {
            uiEquipList.gameObject.SetActive(true);
        }

        uiEquipList.Refresh(info.unit, (int)info.part);

    }

    public void OnSelectEquip(GameUnit unit, EquipData equip)
    {
        UIEquipPopup uiEquipDetails = UIMgr.Instance.OpenUI_(UIEquipPopup.ViewName, false) as UIEquipPopup;
        uiEquipDetails.Refresh(equip);
        uiEquipDetails.IEquipPopupDeletage = ParentNode;
    }

    public void OnEquipDressOrReplace()
    {
        ParentNode.ReloadData();
    }

}
