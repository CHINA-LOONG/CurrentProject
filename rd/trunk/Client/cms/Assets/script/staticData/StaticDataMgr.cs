using UnityEngine;
using System.Collections;
using System.IO;

public class StaticDataMgr : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }
    static StaticDataMgr mInst = null;
    public static StaticDataMgr Instance
    {
        get
        {
            if (mInst == null)
            {
                GameObject go = new GameObject("StaticDataMgr");
                mInst = go.AddComponent<StaticDataMgr>();
            }
            return mInst;
        }
    }
    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        InitData();
    }

    public UnitBaseData.RowData GetUnitBaseRowData(int level)
    {
        return m_UnitBaseData.getRowDataFromLevel(level);
    }
    public UnitData.RowData GetUnitRowData(string unitID)
    {
        return m_UnitData.getRowDataFromIndex(unitID);
    }
    public SpellProtoType GetSpellProtoData(string id)
    {
        return spellData.GetSpellRawData(id);
    }
    public EffectPrototype GetEffectProtoData(string id)
    {
        return effectData.GetEffectRawData(id);
    }
    public BuffPrototype GetBuffProtoData(string id)
    {
        return buffData.GetBuffRawData(id);
    }

	[SerializeField]
	UnitData  m_UnitData;
	public UnitData UnitDataAttr
	{
		get
		{
			return m_UnitData;
		}
	}

	[SerializeField]
	UnitBaseData m_UnitBaseData;
    public UnitBaseData UnitBaseDataEAttr
	{
		get
		{
            return m_UnitBaseData;
		}
	}

    SpellStaticData spellData;
    BuffStaticData buffData;
    EffectStaticData effectData;

    [SerializeField]
    BattleData m_BattleData;
    public BattleData BattleData
    {
        get
        {
            return m_BattleData;
        }
    }

    [SerializeField]
    ProcessData m_ProcessData;
    public ProcessData ProcessData
    {
        get
        {
            return m_ProcessData;
        }
    }

	[SerializeField]
	WeakPointData m_WeakPointData;
	public WeakPointData WeakPointData
	{
		get
		{
			return m_WeakPointData;
		}
	}

    public void InitData()
    {
        GameObject unitDataGo = new GameObject ("UnitData");
        unitDataGo.transform.parent = transform;
        m_UnitData = unitDataGo.AddComponent<UnitData>();
        m_UnitData.InitWithTableFile(Util.ResPath + "/staticData/" + "unitData.csv");

		GameObject unitBaseDataGo = new GameObject ("UnitBaseData");
        unitBaseDataGo.transform.parent = transform;
        m_UnitBaseData = unitBaseDataGo.AddComponent<UnitBaseData>();
        m_UnitBaseData.InitWithTableFile(Util.ResPath + "/staticData/" + "unitBaseData.csv");

        //Spell releated
        GameObject spellDataGo = new GameObject("SpellStaticData");
        spellDataGo.transform.parent = transform;
        spellData = spellDataGo.AddComponent<SpellStaticData>();
        spellData.InitWithTableFile(Util.ResPath + "/staticData/" + "spell.csv");

        GameObject buffDataGo = new GameObject("BuffStaticData");
        buffDataGo.transform.parent = transform;
        buffData = buffDataGo.AddComponent<BuffStaticData>();
        buffData.InitWithTableFile(Util.ResPath + "/staticData/" + "buff.csv");

        GameObject effectDataGo = new GameObject("EffectStaticData");
        effectDataGo.transform.parent = transform;
        effectData = effectDataGo.AddComponent<EffectStaticData>();
        effectData.InitWithTableFile(Util.ResPath + "/staticData/" + "effect.csv");

        m_BattleData = InitTable<BattleData>("battle") as BattleData;
        m_ProcessData = InitTable<ProcessData>("process") as ProcessData;

		m_WeakPointData = InitTable<WeakPointData> ("weakPointData") as WeakPointData;
    }

    StaticDataBase InitTable<T>(string filename) where T : StaticDataBase
    {
        GameObject go = new GameObject(filename);
        go.transform.parent = transform;
        var target = go.AddComponent<T>();
        target.InitWithTableFile(Path.Combine(Util.StaticDataPath, filename + ".csv"));

        return target;
    }
}