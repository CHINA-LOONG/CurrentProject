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
    //public int type;
    ////database id
    //public int id;
    ////if it is PLAYER_ATTR or MONSTER_ATTR 
    //public string itemId;
    //public int count;

    ////If it is EQUIP
    //public int stage;
    //public int level;
    //public List<PB.Attr> attrDatas = new List<PB.Attr>();

    public PB.RewardItem protocolData=new PB.RewardItem();
    public float prob;

    public RewardItemData(int type, string itemId, int count, float prob)
    {
        this.protocolData.type = type;
        this.protocolData.itemId = itemId;
        this.protocolData.count = count;
        this.prob = prob;
    }

    public RewardItemData(int type, string itemId, int count, int stage, int level, float prob)
    {
        this.protocolData.type = type;
        this.protocolData.itemId = itemId;
        this.protocolData.count = count;
        this.protocolData.stage = stage;
        this.protocolData.level = level;
        this.prob = prob;

        if(type == (int)PB.itemType.MONSTER)
        {
            PB.HSMonster monster = new PB.HSMonster();
            monster.cfgId = itemId;
            monster.stage = stage;
            monster.monsterId = -1;
            monster.level = level;

            this.protocolData.monster = monster;
        }
    }

}


public class RewardData
{
    public string id;
    public List<RewardItemData> itemList;
}