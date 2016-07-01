using UnityEngine;
using System.Collections;

public class BattleController : MonoBehaviour
{
    private static BattleController m_Instance;
    public static BattleController Instance
    {
        get
        {
            return m_Instance;
        }
    }

    BattleGroup battleGroup;

    // Use this for initialization
    void Awake()
    {
        m_Instance = this;
        battleGroup = new BattleGroup();
    }

    public void StartBattle(PbStartBattle proto)
    {
        battleGroup.SetEnemyList(proto.enemyList);
        battleGroup.SetPlayerList(proto.playerList);

        CreateAllUnits();
    }

    void CreateAllUnits()
    {
        var all = battleGroup.GetAllUnits();
        foreach (var item in all)
        {
            CreateUnit(item);
        }
    }

    void CreateUnit(BattleUnit unit)
    {
        Logger.LogFormat("[Battle]Create unit %s", unit.name);
    }
}
