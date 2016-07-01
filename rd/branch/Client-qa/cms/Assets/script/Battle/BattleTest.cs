using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//////////////////////////////////////////////////////////////////////////
/// <summary>
/// 模拟战斗协议，只做测试用途
/// </summary>
public class PbBattleUnit
{
    //站位，1，2，3表示，0表示在场下
    public int slot;
    public int guid;
    //静态数据id，可以通过此id获取prefab等
    public int id;
    public string name;
    //性格
    public int personality;

    //--基础属性，不加buff
    public int level;
    public int hp = 100;
    public int strength;
    public int intelligence;
    public int speed = 50;
    public int resistence;
    public int endurance;
    public float hitRate;
    public float criticalRate;
    public float criticalDamageFactor;
    public float recovery;

    public string spellList;
}

//暂时不知道里面会填什么东西
public class PbBattleProcess
{
    public int id;
    public int processAnim;
    public int preAnim;
    public bool needClearBuff;
}

public class PbStartBattle
{
    public List<PbBattleProcess> processList = new List<PbBattleProcess>();
    public List<PbBattleUnit> enemyList = new List<PbBattleUnit>();
    public List<PbBattleUnit> playerList = new List<PbBattleUnit>();
}
//////////////////////////////////////////////////////////////////////////

public class BattleTest : MonoBehaviour
{
    static bool m_IsTest = true;

    static PbStartBattle InitBattleGroup()
    {
        //init enemy
        var proto = new PbStartBattle();

        proto.processList.Add(new PbBattleProcess());

        for (int i = 0; i < 5; i++)
        {
            PbBattleUnit unit = new PbBattleUnit();
            unit.guid = i;
            unit.name = "Enemy" + (i + 1);
            unit.slot = i;
            proto.enemyList.Add(unit);
        }

        for (int i = 0; i < 5; i++)
        {
            PbBattleUnit unit = new PbBattleUnit();
            unit.guid = 10+i;
            unit.name = "Player"+(i+1);
            unit.slot = i;
            proto.playerList.Add(unit);
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
