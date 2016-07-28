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
    public int rollCount;
    public string additionAttr;

    //强化数据
    public Dictionary<AttrType,int> leveAttribute(int level)
    {
        EquipLevelData baseLevelAttr = StaticDataMgr.Instance.GetEquipLevelData(this.levelAttrId);
        EquipLevelData baseAttr = StaticDataMgr.Instance.GetEquipLevelData(this.stageAttrId);
        Dictionary<AttrType, int> attr = new Dictionary<AttrType, int>();
        if (baseAttr.health!=0)
            attr.Add(AttrType.Health, (baseLevelAttr.health * level) + baseAttr.health);
        if (baseAttr.strength != 0)
            attr.Add(AttrType.Strength, (baseLevelAttr.strength * level) + baseAttr.strength);
        if (baseAttr.intelligence != 0)
            attr.Add(AttrType.Intelligence, (baseLevelAttr.intelligence * level) + baseAttr.intelligence);
        if (baseAttr.defense != 0)
            attr.Add(AttrType.Defense, (baseLevelAttr.defense * level) + baseAttr.defense);
        if (baseAttr.speed != 0)
            attr.Add(AttrType.Speed, (baseLevelAttr.speed * level) + baseAttr.speed);
        return attr;        
    }
}
