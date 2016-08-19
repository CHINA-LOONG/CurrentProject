using UnityEngine;
using System.Collections;

public interface IEquipField
{
    void OnSelectEquipField(PartType part, EquipData data);
}
public class EquipField : MonoBehaviour
{

    private ItemIcon equipIcon;
    private NullEquip equipNull;


    //private 

    public IEquipField iClickBack;

    private GameUnit unit;

    private PartType part;
    //public PartType Part
    //{
    //    get { return part; }
    //    set { part = value; }
    //}

    private EquipData data;
    public EquipData Data
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
            SetEquip(data);
        }
    }

    public bool CanEquip
    {
        get
        {
            return UIUtil.CheckIsEnoughEquip(unit, part);
        }
    }

    public void SetField(GameUnit unit, PartType part)
    {
        this.unit=unit;
        this.part = part;
        this.Data = unit.equipList[(int)part - 1];
    }

    void SetEquip(EquipData data)
    {
        if (data==null)
        {
            if (equipNull==null)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset(NullEquip.AssetName);
                UIUtil.SetParentReset(go.transform, transform);
                equipNull = go.GetComponent<NullEquip>();
                EventTriggerListener.Get(equipNull.button.gameObject).onClick = OnClickNullSlot;
            }
            else
            {
                equipNull.gameObject.SetActive(true);
            }
            equipNull.ShowAdd(CanEquip);

            if (equipIcon != null)
            {
                equipIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            if (equipIcon==null)
            {
                equipIcon = ItemIcon.CreateItemIcon(data,false);
                UIUtil.SetParentReset(equipIcon.transform, transform);
                EventTriggerListener.Get(equipIcon.iconButton.gameObject).onClick = OnClickEquipSlot;
            }
            else
            {
                equipIcon.gameObject.SetActive(true);
                equipIcon.RefreshWithEquipInfo(data,false);
            }
            if (equipNull!=null)
            {
                equipNull.gameObject.SetActive(false);
            }
        }
    }

    void OnClickNullSlot(GameObject go)
    {
        if (CanEquip)
        {
            iClickBack.OnSelectEquipField(part, null);
        }
    }

    void OnClickEquipSlot(GameObject go)
    {
        iClickBack.OnSelectEquipField(part, data);
    }


}
