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
            ResourceMgr.Instance.DestroyAsset(bo.gameObject, bo.type == BattleObjectType.Unit);

            battleObjectList.Remove(guid);
        }
    }
    //---------------------------------------------------------------------------------------------
    public BattleObject CreateBattleObject(GameUnit unit, GameObject parent, Vector3 pos, Quaternion rot)
    {
        var go = ResourceMgr.Instance.LoadAsset("monster", unit.assetID);
        GameObject unitObject = GameObject.Instantiate(go);
        if (parent != null)
        {
            unitObject.transform.parent = parent.transform;
        }
        unitObject.transform.localPosition = pos;
        unitObject.transform.localRotation = rot;
        BattleObject bo = unitObject.AddComponent<BattleObject>();
        bo.camp = unit.pbUnit.camp;
        bo.guid = unit.pbUnit.guid;
        unit.battleUnit = bo;
        bo.unit = unit;
        bo.aniControl = unitObject.AddComponent<AnimControl>();

		bo.shifaGo = Util.FindChildByName(unitObject,"e_shifa_01");
		if(bo.shifaGo != null)
		{
			bo.shifaNodeEffect = bo.shifaGo.AddComponent<SimpleEffect>();
		}

        AddBattleObject(bo);

        return bo;
    }
    //---------------------------------------------------------------------------------------------
    public BattleObject CreateSceneObject(int guid, string bundleName, string prefab)
    {
        var go = ResourceMgr.Instance.LoadAsset(bundleName, prefab);
        GameObject sceneObj = GameObject.Instantiate(go);
        BattleObject bo = sceneObj.AddComponent<BattleObject>();
        bo.guid = guid;
        bo.type = BattleObjectType.Scene;

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
