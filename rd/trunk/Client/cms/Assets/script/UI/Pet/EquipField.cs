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

    public void SetField(GameUnit unit, int part)
    {
        this.unit=unit;
        this.part = (PartType)part;
        this.Data = unit.equipList[part];
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
            equipNull.ShowAdd(UIUtil.CheckIsEnoughEquip(unit,(int)part));

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
        iClickBack.OnSelectEquipField(part, null);
    }

    void OnClickEquipSlot(GameObject go)
    {
        iClickBack.OnSelectEquipField(part, data);
    }


}
