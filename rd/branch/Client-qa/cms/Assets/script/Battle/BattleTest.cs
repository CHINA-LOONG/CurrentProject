using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PbStartBattle
{
    //0小怪 1boss 2稀有
    public int battleType;
    //副本id
    public string instanceId;
    public List<PbUnit> enemyList = new List<PbUnit>();
    public List<PbUnit> playerList = new List<PbUnit>();
}
//////////////////////////////////////////////////////////////////////////

public class BattleTest : MonoBehaviour
{
    static List<PbUnit> PlayerList
    {
        get
        {
            var list = new List<PbUnit>();
            //player list
            for (int i = 0; i < 6; i++)
            {
                PbUnit pbUnit = new PbUnit();
                pbUnit.guid = 10 + i;
                pbUnit.level = 16;
                pbUnit.id = "soul";
                pbUnit.slot = i;
                if (i > 2)
                    pbUnit.slot = BattleConst.offsiteSlot;

                list.Add(pbUnit);
            }

            return list;
        }
    }

    public static PbStartBattle GenerateNormalProto(string instanceId)
    {
        var proto = new PbStartBattle();
        proto.battleType = (int)BattleType.Normal;
        proto.instanceId = instanceId;
        //var instanceData = StaticDataMgr.Instance.GetInstanceData(instanceId);

        //enemy list
        for (int i = 0; i < 1; i++)
        {
            PbUnit pbUnit = new PbUnit();
            pbUnit.guid = i;
            pbUnit.id = "soul";
            pbUnit.level = 20;// instanceData.level;
            pbUnit.slot = i;
            if (i > 2)
                pbUnit.slot = BattleConst.offsiteSlot;

            proto.enemyList.Add(pbUnit);
        }

        proto.playerList = PlayerList;

        return proto;
    }

    public static PbStartBattle GenerateBossProto(string instanceId)
    {
        var proto = new PbStartBattle();
        proto.battleType = (int)BattleType.Boss;
        proto.instanceId = instanceId;
        var instanceData = StaticDataMgr.Instance.GetInstanceData(instanceId);

        //enemy list
        PbUnit pbUnit = new PbUnit();
        pbUnit.guid = Random.Range(100, 1000);
        if (StaticDataMgr.Instance.GetUnitRowData(instanceData.bossID) != null)
            pbUnit.id = instanceData.bossID;
        else
            pbUnit.id = "soul"; //instanceData.rareID;
        pbUnit.level = 28;
        pbUnit.slot = 1;

        proto.enemyList.Add(pbUnit);

        //player list
        proto.playerList = PlayerList;

        return proto;

    }

    public static PbStartBattle GenerateRareProto(string instanceId)
    {
        var proto = new PbStartBattle();
        proto.battleType = (int)BattleType.Rare;
        proto.instanceId = instanceId;
        var instanceData = StaticDataMgr.Instance.GetInstanceData(instanceId);

        //enemy list
        PbUnit pbUnit = new PbUnit();
        pbUnit.guid = 1;
        if (StaticDataMgr.Instance.GetUnitRowData(instanceData.rareID) != null)
            pbUnit.id = instanceData.rareID;
        else
            pbUnit.id = "soul"; //instanceData.rareID;
        pbUnit.level = 28;
        pbUnit.slot = 1;

        proto.enemyList.Add(pbUnit);

        //player list
        proto.playerList = PlayerList;

        return proto;
    }
}
