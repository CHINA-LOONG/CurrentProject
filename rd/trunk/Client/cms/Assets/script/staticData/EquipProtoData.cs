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

    public List<ItemInfo> stageDemand1(List<ItemInfo> stageinfo)
    {
        return ItemInfo.getItemInfoList1(stageinfo, stageDemand, ItemParseType.DemandItemType);
    }
    public List<ItemInfo> levelDemand1(List<ItemInfo> leveinfo)
    {
        return ItemInfo.getItemInfoList1(leveinfo, levelDemand, ItemParseType.DemandItemType);
    }
    public List<ItemInfo> punchDemand1(List<ItemInfo> punchinfo)
    {
        return ItemInfo.getItemInfoList1(punchinfo, punchDemand, ItemParseType.DemandItemType);
    }
}
