using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RewardStaticData
{
    public string id;
    public string reward;
}

public class RewardItemData
{
    public int type;
    //database id
    public int id;
    //if it is PLAYER_ATTR or MONSTER_ATTR 
    public string itemId;
    public int count;
    public float prob;

    //If it is EQUIP
    public int stage;
    public int level;
    public List<AttrData> attrDatas=new List<AttrData>();
    /////////////////

    public RewardItemData(int type, string itemId, int count, float prob)
    {
        this.type = type;
        this.itemId = itemId;
        this.count = count;
        this.prob = prob;
    }

    public RewardItemData(int type, string itemId, int count, int stage, int level, float prob)
    {
        this.type = type;
        this.itemId = itemId;
        this.count = count;
        this.stage = stage;
        this.level = level;
        this.prob = prob;
    }

}


public class RewardData
{
    public string id;
    public List<RewardItemData> itemList;
}