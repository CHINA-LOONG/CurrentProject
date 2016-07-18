using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipData
{
    public int stage;
    public int level;
    public string equipId;

    public static EquipData valueof(string equipId, int stage, int level)
    {
        EquipData equipData = new EquipData();
        equipData.stage = stage;
        equipData.level = level;
        equipData.equipId = equipId;
        return equipData;
    }
}
//Modify: xiaolong   2015-8-18 09:51:17
public class AttrData
{
    // 属性id(参考Const.attr)
    public int attrId;
    // 属性值
    public float attrValue;
}

public class GameEquipData
{
    public Dictionary<long, EquipData> equipList = new Dictionary<long, EquipData>();

    public void AddEquip(long id, string equipId, int stage, int level)
    {
        EquipData equipData;
        if (equipList.TryGetValue(id, out equipData))
        {
            equipData.equipId = equipId;
            equipData.stage = stage;
            equipData.level = level;
        }
        else
        {
            equipList.Add(id, EquipData.valueof(equipId, stage, level));
        }
    }

    public bool RemoveEquip(long id)
    {
        if (equipList.ContainsKey(id))
        {
            equipList.Remove(id);
            return true;
        }

        return false;
    }

    public EquipData GetEquip(long id)
    {
        EquipData equipData;
        if (equipList.TryGetValue(id, out equipData))
        {
            return equipData;
        }

        return null;
    }
}

