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

    public void OnEquipDressOrReplace()
    {
        ParentNode.ReloadData();
    }
}
