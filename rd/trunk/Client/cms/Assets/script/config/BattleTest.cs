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
    //public List<PbUnit> playerList = new List<PbUnit>();
}
//////////////////////////////////////////////////////////////////////////

public class BattleTest : MonoBehaviour
{

    public static PbStartBattle GenerateNormalProto(string instanceId)
    {
        var proto = new PbStartBattle();
        proto.battleType = (int)BattleType.Normal;
        proto.instanceId = instanceId;
        //var instanceData = StaticDataMgr.Instance.GetInstanceData(instanceId);

        //enemy list
        for (int i = 0; i < 6; i++)
        {
            PbUnit pbUnit = new PbUnit();
            pbUnit.guid = i;
            pbUnit.camp = UnitCamp.Enemy;
			if (i == 0)
                pbUnit.id = "Unit_Demo_zhuyan";
			if (i == 1)
				pbUnit.id = "Unit_Demo_qingniao";
			if (i == 2)
				pbUnit.id = "Unit_Demo_ershu";
			if (i == 3)
				pbUnit.id = "Unit_Demo_qingniao";
			if (i == 4)
				pbUnit.id = "Unit_Demo_zhuyan";
			if (i == 5)
				pbUnit.id = "Unit_Demo_ershu";
            pbUnit.level = 50;// instanceData.level;
            pbUnit.slot = i;
            if (i > 2)
                pbUnit.slot = BattleConst.offsiteSlot;

			pbUnit.character = 1;
			pbUnit.lazy = 4;

            proto.enemyList.Add(pbUnit);
        }

        //proto.playerList = PlayerList;

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
        //pbUnit.guid = Random.Range(100, 1000);
        pbUnit.guid = 100;
        if (StaticDataMgr.Instance.GetUnitRowData(instanceData.bossID) != null)
            pbUnit.id = instanceData.bossID;
        else
            pbUnit.id = "Unit_Demo_jiuweihu"; //instanceData.rareID;
        pbUnit.level = 50;
        pbUnit.camp = UnitCamp.Enemy;
        pbUnit.slot = 1;
		pbUnit.character = 2;
		pbUnit.lazy = 2;

		pbUnit.testBossType = 1;//九尾狐
		//pbUnit.testBossType = 2;//混沌

        proto.enemyList.Add(pbUnit);

        //player list
        //proto.playerList = PlayerList;

        return proto;
    }

	public static PbStartBattle GenerateHundunBossProto(string instanceId)
	{
		var proto = new PbStartBattle();
		proto.battleType = (int)BattleType.Boss;
		proto.instanceId = instanceId;
		var instanceData = StaticDataMgr.Instance.GetInstanceData(instanceId);
		
		//enemy list
		PbUnit pbUnit = new PbUnit();
		//pbUnit.guid = Random.Range(100, 1000);
		pbUnit.guid = 101;
		if (StaticDataMgr.Instance.GetUnitRowData(instanceData.bossID) != null)
			pbUnit.id = instanceData.bossID;
		else
			pbUnit.id = "Unit_Demo_hundun"; //instanceData.rareID;
		pbUnit.level = 50;
		pbUnit.camp = UnitCamp.Enemy;
		pbUnit.slot = 1;
		pbUnit.character = 2;
		pbUnit.lazy = 2;
		
		//pbUnit.testBossType = 1;//九尾狐
		pbUnit.testBossType = 2;//混沌
		
		proto.enemyList.Add(pbUnit);
		
		//player list
		//proto.playerList = PlayerList;
		
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
		pbUnit.character = 2;
        pbUnit.lazy = 2;
        pbUnit.camp = UnitCamp.Enemy;

        proto.enemyList.Add(pbUnit);

        //player list
        //proto.playerList = PlayerList;

        return proto;
    }
}
