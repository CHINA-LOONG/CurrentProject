using UnityEngine;
using System.Collections;

public class UIPetInlayParam : PetRightParamBase
{
    public EquipData equip;
    public int tabIndex;
    public int selIndex;
}

public class UIPetEquipInlay : PetDetailRightBase, IEquipInlayCallBack
{
    private UIEquipInlay uiEquipInlay;

    private UIPetInlayParam info;

    public override void ReloadData(PetRightParamBase param)
    {
        info = param as UIPetInlayParam;

        if (uiEquipInlay == null)
        {
            uiEquipInlay = UIEquipInlay.CreateEquipInlay();
            uiEquipInlay.ICallBackDelegate = this;
            UIUtil.SetParentReset(uiEquipInlay.transform, transform);
        }
        else
        {
            uiEquipInlay.gameObject.SetActive(true);
        }
        uiEquipInlay.Refresh(info.equip, info.tabIndex, info.selIndex, UIEquipInlay.UIType.Right);
    }

    public void OnSelectReturn(int tabIndex, int selIndex)
    {
        ParentNode.ReloadLeftData();
        ParentNode.uiEquipSetting = UIMgr.Instance.OpenUI_(UIEquipSetting.ViewName) as UIEquipSetting;
        ParentNode.uiEquipSetting.Refresh(info.equip, tabIndex, selIndex);
    }

    public void OnMosaicReturn(int tabIndex, int selIndex)
    {
        ParentNode.ReloadLeftData();
        info.tabIndex = tabIndex;
        info.selIndex = selIndex;
        ParentNode.ReloadRigthData(PetViewConst.UIPetEquipInlayAssetName, info);
    }
}
