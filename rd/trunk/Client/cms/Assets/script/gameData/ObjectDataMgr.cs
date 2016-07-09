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
            Destroy(bo.aniControl);
            Destroy(bo);

            ResourceMgr.Instance.DestroyAsset(bo.gameObject, bo.type != BattleObjectType.Scene);

            battleObjectList.Remove(guid);
        }
    }
    //---------------------------------------------------------------------------------------------
    public BattleObject CreateBattleObject(GameUnit unit, GameObject parent, Vector3 pos, Quaternion rot)
    {
        GameObject unitObject = ResourceMgr.Instance.LoadAsset("monster", unit.assetID);
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

		bo.shifaGo = Util.FindChildByName(unitObject,"e_shifa");

		//weakpoint
		if (bo.camp == UnitCamp.Enemy) {

			BoxCollider bc = unitObject.GetComponent<BoxCollider>();
			if( unit.isBoss)
			{
				if(null !=bc)
				{
					Destroy(bc);
				}
			}
			else
			{
				if(null == bc)
				{
					unitObject.AddComponent<BoxCollider>();
				}
			}

			bo.wpGroup = WeakPointGroup.CreateWeakpointGroup(bo);
		}
		else 
		{

			BoxCollider bc = unitObject.GetComponent<BoxCollider>();
			if(null == bc)
			{
				unitObject.AddComponent<BoxCollider>();
			}
		}
        AddBattleObject(bo);

        return bo;
    }
    //---------------------------------------------------------------------------------------------
    public BattleObject CreateSceneObject(int guid, string bundleName, string prefab)
    {
        GameObject sceneObj = ResourceMgr.Instance.LoadAsset(bundleName, prefab, false);
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
