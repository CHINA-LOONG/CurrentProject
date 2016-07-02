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
    static bool m_IsTest = true;

    static PbStartBattle InitBattleGroup()
    {
        var proto = new PbStartBattle();
        proto.instanceId = "demo";

        //enemy list
        for (int i = 0; i < 1; i++)
        {
            PbUnit pbUnit = new PbUnit();
            pbUnit.guid = i;
            pbUnit.id = "soul";
            pbUnit.level = 15;
            pbUnit.slot = i;
            if (i > 2)
                pbUnit.slot = BattleConst.offsiteSlot;

            proto.enemyList.Add(pbUnit);
        }

        //player list
        for (int i = 0; i < 1; i++)
        {
            PbUnit pbUnit = new PbUnit();
            pbUnit.guid = 10 + i;
            pbUnit.level = 16;
            pbUnit.id = "cat";
            pbUnit.slot = i;
            if (i > 2)
                pbUnit.slot = BattleConst.offsiteSlot;

            proto.playerList.Add(pbUnit);
        }

        return proto;
    }

    // Use this for initialization
    public static void Test()
    {
        if (!m_IsTest)
            return;

        var proto = InitBattleGroup();

        GameEventMgr.Instance.FireEvent(GameEventList.StartBattle, proto);
    }
}
