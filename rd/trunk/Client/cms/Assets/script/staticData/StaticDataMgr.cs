using UnityEngine;
using System.Collections;
using System.IO;
using Csv.Serialization;
using System.Collections.Generic;
using System.Linq;

public class StaticDataMgr : MonoBehaviour
{
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

    Dictionary<string, WeakPointData> m_WeakPointData;
    Dictionary<string, UnitData> m_UnitData;
    Dictionary<int, UnitBaseData> m_UnitBaseData;
    Dictionary<string, BuffPrototype> buffData;
    Dictionary<string, EffectPrototype> effectData;
    Dictionary<string, SpellProtoType> spellData;
    Dictionary<string, BattleData> m_BattleData;
	Dictionary<string, BattleUnitAiData> m_BattleUnitAiData;
   
    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        InitData();
    }

    public void InitData()
    {
        {
            var data = InitTable<UnitData>("unitData");
            m_UnitData = data.ToDictionary(p => p.index);
        }
        {
            var data = InitTable<UnitBaseData>("unitBaseData");
            m_UnitBaseData = data.ToDictionary(p => p.level);
        }
        {
             var data = InitTable<SpellProtoType>("spell");
             spellData = data.ToDictionary(p => p.id);
        }
        {
             var data = InitTable<BuffPrototype>("buff");
             buffData = data.ToDictionary(p => p.id);
        }
        {
            effectData = new Dictionary<string, EffectPrototype>();
            var data = InitTable<EffectWholeData>("effect");
            EffectPrototype effectPt = null;
            foreach (EffectWholeData wholeData in data)
            {
                EffectType et = (EffectType)(wholeData.effectType);
                switch (et)
                {
                    case EffectType.Effect_Type_Set:
                        {
                            effectPt = new EffectSetPrototype();
                            EffectSetPrototype setPt = effectPt as EffectSetPrototype;
                            //string[] effects = wholeData.effectList.Split(';');
							ArrayList effectArrayList = MiniJsonExtensions.arrayListFromJson ( wholeData.effectList);
							for (int i = 0; i <  effectArrayList.Count; ++i)
                            {
								setPt.effectList.Add(effectArrayList[i] as string);
                            }
                        }
                        break;
                    case EffectType.Effect_Type_Search:
                        {
                            effectPt = new EffectSearchPrototype();
                            EffectSearchPrototype searchPt = effectPt as EffectSearchPrototype;

                            searchPt.count = wholeData.count;
                            searchPt.camp = wholeData.camp;
                            searchPt.effectID = wholeData.searchEffect;
                        }
                        break;
                    case EffectType.Effect_Type_Persistent:
                        {
                            effectPt = new EffectPersistentProtoType();
                            EffectPersistentProtoType persistPt = effectPt as EffectPersistentProtoType;

                            persistPt.effectStartID = wholeData.effectStartID;
                            persistPt.startDelayTime = wholeData.startDelayTime;
                            string[] effectList = wholeData.periodEffectList.Split(';');
							///ArrayList effectArrayList = MiniJsonExtensions.arrayListFromJson (wholeData.periodEffectList);
							for (int i = 0; i < effectList.Length; ++i)
                            {
								string[] effectKV = effectList[i].Split('|');
                                if (effectKV.Length != 2)
                                    continue;

                                persistPt.effectList.Add(new KeyValuePair<float, string>(float.Parse(effectKV[0]), effectKV[1]));
                            }
                        }
                        break;
                    case EffectType.Effect_Type_Damage:
                        {
                            effectPt = new EffectDamageProtoType();
                            EffectDamageProtoType damagePt = effectPt as EffectDamageProtoType;

                            damagePt.damageType = wholeData.damageType;
                            damagePt.attackFactor = wholeData.attackFactor;
                            damagePt.isHeal = wholeData.isHeal > 0;
                            damagePt.damageProperty = wholeData.damageProperty;
                        }
                        break;
                    case EffectType.Effect_Type_Buff:
                        {
                            effectPt = new EffectApplyBuffPrototype();
                            EffectApplyBuffPrototype buffPt = effectPt as EffectApplyBuffPrototype;

                            buffPt.buffID = wholeData.buffID;
                        }
                        break;
                }
                effectPt.id = wholeData.id;
                effectPt.effectType = et;
                effectPt.targetType = wholeData.targetType;
                effectPt.casterType = wholeData.casterType;
                effectData.Add(effectPt.id, effectPt);
            }
        }
        {
            var data = InitTable<BattleData>("battle");
            m_BattleData = data.ToDictionary(p => p.id);
        }
        {
            var data = InitTable<WeakPointData>("weakPointData");
            m_WeakPointData = data.ToDictionary(p => p.id);
        }

		{
			var data = InitTable<BattleUnitAiData>("battleUnitAi");
			m_BattleUnitAiData = data.ToDictionary(p => p.index);
		}
    }

    List<T> InitTable<T>(string filename) where T : new()
    {
        // Deserialization
        List<string> rowData = new List<string>();
        List<List<string>> rows = new List<List<string>>();
        using (var reader = new CsvFileReader(Path.Combine(Util.StaticDataPath, filename + ".csv")))
        {
            while (reader.ReadRow(rowData))
            {
                rows.Add(rowData);
                rowData = new List<string>();
            }

            var serializer = new CsvSerializer<T>();
            var data = serializer.Deserialize(rows[1], rows.GetRange(3, rows.Count - 3));

            return data;
        }
    }

#region Get Data
    public UnitBaseData GetUnitBaseRowData(int level)
    {
        return m_UnitBaseData[level];
    }
    public UnitData GetUnitRowData(string unitID)
    {
        return m_UnitData[unitID];
    }
    public SpellProtoType GetSpellProtoData(string id)
    {
        return spellData[id];
    }
    public EffectPrototype GetEffectProtoData(string id)
    {
        return effectData[id];
    }
    public BuffPrototype GetBuffProtoData(string id)
    {
        return buffData[id];
    }

    public WeakPointData GetWeakPointData(string id)
    {
        return m_WeakPointData[id];
    }

    public BattleData GetBattleDataFromLevel(string id)
    {
        return m_BattleData[id];
    }

	public BattleUnitAiData GetBattleUnitAiData(string index)
	{
		return m_BattleUnitAiData [index];
	}
#endregion    
}