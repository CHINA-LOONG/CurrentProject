using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipData
{
    public int stage;
    public int level;
    public int equipId;

    public static EquipData valueof(int equipId, int stage, int level)
    {
        EquipData equipData = new EquipData();
        equipData.stage = stage;
        equipData.level = level;
        equipData.equipId = equipId;
        return equipData;
    }
}

public class GameEquipData
{
    public Dictionary<long, EquipData> equipList = new Dictionary<long, EquipData>();

    public void AddEquip(long id, int equipId, int stage, int level)
    {
        EquipData equipData;
        if (equipList.TryGetValue(equipId, out equipData))
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
}

