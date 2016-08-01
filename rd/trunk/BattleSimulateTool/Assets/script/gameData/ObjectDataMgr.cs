using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectDataMgr : MonoBehaviour
{
    //---------------------------------------------------------------------------------------------
    static ObjectDataMgr mInst = null;
    public static ObjectDataMgr Instance
    {
        get
        {
            if (mInst == null)
            {
                GameObject go = new GameObject("ObjectDataMgr");
                mInst = go.AddComponent<ObjectDataMgr>();
            }
            return mInst;
        }
    }
    //---------------------------------------------------------------------------------------------
    public void Init()
    {
        DontDestroyOnLoad(gameObject);
    }
    //---------------------------------------------------------------------------------------------
    private Dictionary<int, BattleObject> battleObjectList = new Dictionary<int, BattleObject>();

    public BattleObject GetBattleObject(int guid)
    {
        BattleObject bo = null;
        if (battleObjectList.TryGetValue(guid, out bo) == true)
        {
            return bo;
        }

        return null;
    }
    //---------------------------------------------------------------------------------------------
    public bool AddBattleObject(BattleObject bo)
    {
        if (battleObjectList.ContainsKey(bo.guid) == true)
        {
            return false;
        }

        battleObjectList.Add(bo.guid, bo);
        return true;
    }
    //---------------------------------------------------------------------------------------------
    public void RemoveBattleObject(int guid)
    {
        BattleObject bo = null;
        if (battleObjectList.TryGetValue(guid, out bo) == true)
        {
            battleObjectList.Remove(guid);
        }
    }
    //---------------------------------------------------------------------------------------------
    public BattleObject CreateBattleObject(GameUnit unit, GameObject parent, Vector3 pos, Quaternion rot)
    {
        BattleObject bo = null;
        if (battleObjectList.TryGetValue(unit.pbUnit.guid, out bo) == true)
        {
            return bo;
        }
        //bo = unitObject.AddComponent<BattleObject>();

        bo = new BattleObject();
        bo.camp = unit.pbUnit.camp;
        bo.guid = unit.pbUnit.guid;
        unit.battleUnit = bo;
        bo.unit = unit;
        if (unit.pbUnit.camp == UnitCamp.Enemy)
        {
            bo.wpGroup = WeakPointGroup.CreateWeakpointGroup(bo);
        }
        AddBattleObject(bo);

        return bo;
    }
    //---------------------------------------------------------------------------------------------
    void OnDestroy()
    {
        Destroy(gameObject);
    }
    //---------------------------------------------------------------------------------------------
}
