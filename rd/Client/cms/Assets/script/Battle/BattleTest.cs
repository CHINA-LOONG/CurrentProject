using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 模拟战斗unit协议，只做测试用途
/// </summary>
public class PbBattleUnit
{
    public int guid;
    //静态数据id，可以通过此id获取prefab等
    public int id;
    public string name;
    //性格
    public int personality;

    //--基础属性，不加buff
    public int level;
    public int hp;
    public int strength;
    public int intelligence;
    public int speed;
    public int resistence;
    public int endurance;
    public float hitRate;
    public float criticalRate;
    public float criticalDamageFactor;
    public float recovery;

    public string spellList;
}

/// <summary>
/// 模拟开始战斗协议，只做测试用途
/// </summary>
public class PbStartBattle
{
    public List<PbBattleUnit> enemyList = new List<PbBattleUnit>();
    public List<PbBattleUnit> playerList = new List<PbBattleUnit>();
}

public class BattleTest : MonoBehaviour
{
    static bool m_IsTest = true;
    static PbStartBattle proto;

    static void InitBattleGroup()
    {
        //init enemy
        proto = new PbStartBattle();
        PbBattleUnit unit = new PbBattleUnit();
        unit.name = "Enemy1";
        proto.enemyList.Add(unit);

        unit.name = "Player1";
        proto.playerList.Add(unit);
    }

    // Use this for initialization
    public static void Test()
    {
        if (!m_IsTest)
            return;

        InitBattleGroup();

        BattleController.Instance.StartBattle(proto);
    }
}
