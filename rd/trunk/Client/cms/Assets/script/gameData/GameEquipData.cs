using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

public class GemInfo
{
    public int type;
    public string gemId;

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
    public long id;
    public int stage;
    public int level;
    public string equipId;
    public int monsterId;
    public List<GemInfo> gemList;

    //显示用数据
    public int health;
    public int strength;
    public int intelligence;
    public int defense;
    public int speed;
    //强化属性
    public int healthStrengthen;
    public int strengthStrengthen;
    public int intelligenceStrengthen;
    public int defenseStrengthen;
    public int speedStrengthen;

    public static EquipData valueof(long id, string equipId, int stage, int level,int monsterId, List<PB.GemPunch> gemList)
    {
        EquipData equipData = new EquipData();
        equipData.id = id;
        equipData.stage = stage;
        equipData.level = level;
        equipData.equipId = equipId;
        equipData.monsterId = monsterId;
        equipData.gemList = new List<GemInfo>();
        EquipProtoData item = StaticDataMgr.Instance.GetEquipProtoData(equipId, stage);
        EquipLevelData baseAttr = StaticDataMgr.Instance.GetEquipLevelData(item.stageAttrId);
        //基础属性id
        equipData.health = baseAttr.health;
        equipData.strength = baseAttr.strength;
        equipData.intelligence = baseAttr.intelligence;
        equipData.defense = baseAttr.defense;
        equipData.speed = baseAttr.speed;
        //强化属性
        if (level != 0)
        {
            baseAttr = StaticDataMgr.Instance.GetEquipLevelData(item.levelAttrId);
            //基础属性id
            equipData.healthStrengthen = baseAttr.health * level;
            equipData.strengthStrengthen = baseAttr.strength * level;
            equipData.intelligenceStrengthen = baseAttr.intelligence * level;
            equipData.defenseStrengthen = baseAttr.defense * level;
            equipData.speedStrengthen = baseAttr.speed * level;
        }
        foreach (PB.GemPunch element in gemList)
        {
            GemInfo gemInfo = new GemInfo(element);
            equipData.gemList.Add(gemInfo);
        }
        //
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

    //modify: xiaolong 2015-9-9 18:41:28
    //public void AddEquip(long id, string equipId, int stage, int level, List<PB.GemPunch> gemList)
    //{
    //    EquipData equipData;
    //    if (equipList.TryGetValue(id, out equipData))
    //    {
    //        equipData.id = id;
    //        equipData.equipId = equipId;
    //        equipData.stage = stage;
    //        equipData.level = level;
    //        equipData.gemList.Clear();
    //        foreach (PB.GemPunch element in gemList)
    //        {
    //            GemInfo gemInfo = new GemInfo(element);
    //            equipData.gemList.Add(gemInfo);
    //        }
    //    }
    //    else
    //    {
    //        equipList.Add(id, EquipData.valueof(id,equipId, stage, level, gemList));
    //    }
    //}
    public void AddEquip(EquipData data)
    {
        if (equipList.ContainsKey(data.id))
        {
            equipList[data.id] = data;
        }
        else
        {
            equipList.Add(data.id, data);
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

