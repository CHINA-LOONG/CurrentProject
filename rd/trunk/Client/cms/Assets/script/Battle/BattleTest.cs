using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PbStartBattle
{
    public int battleId;
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
        proto.battleId = 2;

        //enemy list
        for (int i = 0; i < 5; i++)
        {
            PbUnit pbUnit = new PbUnit();
            pbUnit.guid = i;
            pbUnit.id = "soul";
            pbUnit.level = 15;
            //pbUnit.name = "Enemy" + (i + 1);
            pbUnit.slot = i;
            proto.enemyList.Add(pbUnit);
        }

        //player list
        for (int i = 0; i < 5; i++)
        {
            PbUnit pbUnit = new PbUnit();
            pbUnit.guid = 10 + i;
            pbUnit.level = 16;
            //unit.name = "Player"+(i+1);
            pbUnit.id = "soul";
            pbUnit.slot = i;
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
