using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

public class GemInfo
{
    int type;
    string gemId;

    public GemInfo(int type, string gemId) {
        this.type = type;
        this.gemId = gemId;
    }

    public GemInfo(PB.GemPunch gem)
    {
        this.type = gem.type;
        this.gemId = gem.gemItemId;
    }
}

public class EquipData
{
    public int stage;
    public int level;
    public string equipId;
    public List<GemInfo> gemList;

    public static EquipData valueof(string equipId, int stage, int level, List<PB.GemPunch> gemList)
    {
        EquipData equipData = new EquipData();
        equipData.stage = stage;
        equipData.level = level;
        equipData.equipId = equipId;
        equipData.gemList = new List<GemInfo>();

        foreach (PB.GemPunch element in gemList)
        {
            GemInfo gemInfo = new GemInfo(element);
            equipData.gemList.Add(gemInfo);
        }

        return equipData;
    }
}
////Modify: xiaolong   2015-8-18 09:51:17
//public class AttrData
//{
//    // 属性id(参考Const.attr)
//    public int attrId;
//    // 属性值
//    public float attrValue;

//    public AttrData(PB)
//}

public class GameEquipData
{
    public Dictionary<long, EquipData> equipList = new Dictionary<long, EquipData>();

    public void AddEquip(long id, string equipId, int stage, int level, List<PB.GemPunch> gemList)
    {
        EquipData equipData;
        if (equipList.TryGetValue(id, out equipData))
        {
            equipData.equipId = equipId;
            equipData.stage = stage;
            equipData.level = level;
            equipData.gemList.Clear();
            foreach (PB.GemPunch element in gemList)
            {
                GemInfo gemInfo = new GemInfo(element);
                equipData.gemList.Add(gemInfo);
            }
        }
        else
        {
            equipList.Add(id, EquipData.valueof(equipId, stage, level, gemList));
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

