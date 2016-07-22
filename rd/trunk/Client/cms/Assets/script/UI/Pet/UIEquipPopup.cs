using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public interface IEquipPopupCallBack
{
    void OnQiangHuaCallBack(EquipData equip);
    void OnXiangqianCallBack(EquipData equip);
}

public class UIEquipPopup : UIBase, IEquipCallBack
{
    public static string ViewName = "UIEquipPopup";

    public Button btnClose;
    public Transform uiEquipNode;

    public IEquipPopupCallBack IEquipPopupDeletage;

    private UIEquipDetails uiEquipDetails;

    private EquipData info;

    void Start()
    {
        EventTriggerListener.Get(btnClose.gameObject).onClick = OnClickClose;
    }

    public void Refresh(EquipData equip)
    {
        info = equip;

        if (uiEquipDetails == null)
        {
            uiEquipDetails = UIEquipDetails.CreateEquip();
            uiEquipDetails.equipCallBack = this;
            UIUtil.SetParentReset(uiEquipDetails.transform, uiEquipNode);
        }
        else
        {
            uiEquipDetails.gameObject.SetActive(true);
        }

        uiEquipDetails.Show(info, null, EquipState.showGem);
    }


    void OnClickClose(GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }

    #region 实现接口
    
    public void Reinforced(EquipData data)
    {
        if (IEquipPopupDeletage!=null)
        {
            IEquipPopupDeletage.OnQiangHuaCallBack(data);
        }
        UIMgr.Instance.DestroyUI(this);
    }

    public void Inlay(EquipData data)
    {
        if (IEquipPopupDeletage != null)
        {
            IEquipPopupDeletage.OnXiangqianCallBack(data);
        }
        UIMgr.Instance.DestroyUI(this);
    }

    public void Unload()
    {
        //do nothing
    }

    public void Replacement(GameUnit unit, int equipPart)
    {
        //do nothing
    }

    #endregion
}
