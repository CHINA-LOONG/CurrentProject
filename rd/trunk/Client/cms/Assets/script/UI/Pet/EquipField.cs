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

    private EquipData data;

    private PartType part;

    public IEquipField iClickBack;

    public PartType Part
    {
        get { return part; }
        set { part = value; }
    }

    public EquipData Data
    {
        get
        {
            return data;
        }
        set
        {
            if (value != data||data==null)
            {
                data = value;
                SetEquip(data);
            }
        }
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
            if (equipIcon != null)
            {
                equipIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            if (equipIcon==null)
            {
                equipIcon = ItemIcon.CreateItemIcon(data);
                UIUtil.SetParentReset(equipIcon.transform, transform);
                EventTriggerListener.Get(equipIcon.iconButton.gameObject).onClick = OnClickEquipSlot;
            }
            else
            {
                equipIcon.gameObject.SetActive(true);
                equipIcon.RefreshWithEquipInfo(data);
            }
            if (equipNull!=null)
            {
                equipNull.gameObject.SetActive(false);
            }
        }
    }

    void OnClickNullSlot(GameObject go)
    {
        iClickBack.OnSelectEquipField(Part, null);
    }

    void OnClickEquipSlot(GameObject go)
    {
        iClickBack.OnSelectEquipField(Part, data);
    }


}
