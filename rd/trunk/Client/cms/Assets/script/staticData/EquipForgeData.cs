using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipForgeData
{
    public string stageLevel;
    public int playerlevelDemand;
    public float successRate;
    public string levelDemand;
    public string punchDemand;
    public string decompose;

    public void GetLevelDemand(ref List<ItemInfo> leveinfo)
    {
        ItemInfo.getItemInfoList1(leveinfo, this.levelDemand, ItemParseType.DemandItemType);
    }

    public void GetPunchDemand(ref List<ItemInfo> punchinfo)
    {
        ItemInfo.getItemInfoList1(punchinfo, this.punchDemand, ItemParseType.DemandItemType);
    }
}
