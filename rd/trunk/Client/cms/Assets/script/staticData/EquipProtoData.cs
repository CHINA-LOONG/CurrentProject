using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EquipProtoData
{
    public string id;
    public int stage;
    public int levelAttrId;
    public int stageAttrId;
    public string stageDemand;
    public string levelDemand;
    public string punchDemand;
    public int rollCount;
    public string additionAttr;

    public void  GetStageDemand(ref List<ItemInfo> stageinfo)//传来一个作为存储用的
    {
        ItemInfo.getItemInfoList1(stageinfo, this.stageDemand, ItemParseType.DemandItemType);
    }
    public void GetLevelDemand(ref List<ItemInfo> leveinfo)
    {
        ItemInfo.getItemInfoList1(leveinfo, this.levelDemand, ItemParseType.DemandItemType);
    }
    public void GetPunchDemand(ref List<ItemInfo> punchinfo)
    {
        ItemInfo.getItemInfoList1(punchinfo, this.punchDemand, ItemParseType.DemandItemType);
    }
    //强化数据
    public Dictionary<AttrType,int> leveAttribute(int level)
    {
        EquipLevelData baseAttr = StaticDataMgr.Instance.GetEquipLevelData(this.stageAttrId);
        Dictionary<AttrType, int> attr = new Dictionary<AttrType, int>();
        if (baseAttr.health!=0)        
            attr.Add(AttrType.Health, baseAttr.health * level);
        if (baseAttr.strength != 0)
            attr.Add(AttrType.Strength, baseAttr.strength * level);
        if (baseAttr.intelligence != 0)
            attr.Add(AttrType.Intelligence, baseAttr.intelligence * level);
        if (baseAttr.defense != 0)
            attr.Add(AttrType.Defense, baseAttr.defense * level);
        if (baseAttr.speed != 0)
            attr.Add(AttrType.Speed, baseAttr.speed * level);
        return attr;        
    }
}
