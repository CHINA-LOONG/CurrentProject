using UnityEngine;
using System.Collections;
using System.IO;
using Csv.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    Dictionary<string, WeakPointData> weakPointData = new Dictionary<string, WeakPointData>();
    Dictionary<string, UnitData> unitData = new Dictionary<string, UnitData>();
    Dictionary<int, UnitBaseData> unitBaseData = new Dictionary<int, UnitBaseData>();
    Dictionary<int, int> spellUpLevelData = new Dictionary<int, int>();
    Dictionary<string, InstanceData> instanceData = new Dictionary<string, InstanceData>();
    Dictionary<string, BattleLevelData> battleLevelData = new Dictionary<string, BattleLevelData>();
	Dictionary<int,LazyData>	lazyData = new Dictionary<int, LazyData>();
	Dictionary<string,ItemStaticData> itemData = new Dictionary<string, ItemStaticData>();
	Dictionary<int,PlayerLevelAttr> playerLevelAttr = new Dictionary<int, PlayerLevelAttr>();
    Dictionary<int, QuestStaticData> questData = new Dictionary<int, QuestStaticData>();
    Dictionary<string, SpeechData> speechData = new Dictionary<string, SpeechData>();
    Dictionary<int, UnitStageData> unitStageData = new Dictionary<int, UnitStageData>();
   
    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        InitData();
    }

    public void InitData()
    {
        {
            var data = InitTable<UnitData>("unitData");
            foreach (var item in data)
                unitData.Add(item.index, item);
        }
        {
            var data = InitTable<UnitBaseData>("unitBaseData");
            foreach (var item in data)
                unitBaseData.Add(item.level, item);
        }
        #region instance data
        {
            var data = InitTable<InstanceProtoData>("instance");
            //foreach (var item in data)
            //{
            //    InstanceData curInstance = new InstanceData();
            //    curInstance.instanceProtoData = item;

            //    ArrayList battleArrayList = MiniJsonExtensions.arrayListFromJson(item.battleLevelList);
            //    for (int i = 0; i < battleArrayList.Count; ++i)
            //    {
            //        curInstance.battleLevelList.Add(battleArrayList[i] as string);
            //    }

            //    instanceData.Add(curInstance.instanceProtoData.id, curInstance);
            //}
        }
        #endregion
        #region battleLevel data
        {
            var data = InitTable<BattleLevelProtoData>("battleLevel");
            foreach (var item in data)
            {
                //BattleLevelData blData = new BattleLevelData();
                //blData.battleProtoData = item;
                //if (string.IsNullOrEmpty(item.winFunc) == false)
                //{
                //    var cls = typeof(NormalScript);
                //    blData.winFunc = cls.GetMethod(item.winFunc);
                //}
                //if (string.IsNullOrEmpty(item.loseFunc) == false)
                //{
                //    var cls = typeof(NormalScript);
                //    blData.loseFunc = cls.GetMethod(item.loseFunc);
                //}

                //Hashtable monsterTable = MiniJsonExtensions.hashtableFromJson(item.monsterList);
                //if (monsterTable != null)
                //{
                //    foreach (DictionaryEntry de in monsterTable)
                //    {
                //        blData.monsterList.Add(de.Key.ToString(), int.Parse(de.Value.ToString()));
                //    }
                //}

                //battleLevelData.Add(blData.battleProtoData.id, blData);
            }
        }
        #endregion
        {
            var data = InitTable<WeakPointData>("weakPointData");
            foreach (var item in data)
            {
                item.AdaptData();
                weakPointData.Add(item.id, item);
            }
        }

        {
            var data = InitTable<LazyData>("lazy");
            foreach (var item in data)
                lazyData.Add(item.index, item);
        }
        {
            var data = InitTable<ItemStaticData>("item");
            foreach (var item in data)
            {
                //Debug.Log(item.id);
                itemData.Add(item.id, item);
            }
        }

        {
            var data = InitTable<PlayerLevelAttr>("playerAttr");
            foreach (var item in data)
                playerLevelAttr.Add(item.level, item);
        }
        {
            #region static Quest
            var data = InitTable<QuestStaticData>("quest");
            foreach (var item in data)
            {
                #region Logger
                /*****************************************************************************
                Logger.Log("id:" + item.id + 
                          "\ngroup:" + item.group + 
                          "\ntype:" + item.type + 
                          "\nname:" + item.name + 
                          "\nicon:" + item.icon + 
                          "\nlevel:" + item.level + 
                          "\ncycle:" + item.cycle + 
                          "\ntimeBeginId:" + item.timeBeginId + 
                          "\ntimeEndId:" + item.timeEndId + 
                          "\ngoalType:" + item.goalType + 
                          "\ngoalParam:" + item.goalParam + 
                          "\ngoalCount:" + item.goalCount + 
                          "\nrewardId:" + item.rewardId + 
                          "\nexpK:" + item.expK + 
                          "\nexpB:" + item.expB + 
                          "\nspeechId:" + item.speechId);
/******************************************************************************/
                #endregion
                questData.Add(item.id, item);
            }
            #endregion
        }
        {
            #region speech
            var data = InitTable<SpeechStaticData>("speech");

            foreach (var item in data)
            {
                if (string.IsNullOrEmpty(item.id))
                    continue;

                if (speechData.ContainsKey(item.id))
                {
                    speechData[item.id].speechList.Add(item);
                    continue;
                }
                else if (!string.IsNullOrEmpty(item.id))
                {
                    SpeechData speech = new SpeechData();
                    speech.id = item.id;
                    speech.skip = item.skip;
                    speech.speechList.Add(item);
                    speechData.Add(speech.id, speech);
                }
                else if (speechData.Count > 0)
                {
                    speechData.Values.Last<SpeechData>().speechList.Add(item);
                }
                else
                {
                    continue;
                }
            }

            //Logger.Log(speechData.Count);
            //foreach (var item in speechData)
            //{
            //    Logger.Log(item.Key);
            //    foreach (var info in item.Value.speechList)
            //    {
            //        Logger.Log(info.name);
            //    }
            //}
            #endregion
        }
        {
            #region unitStage
            var data = InitTable<UnitStageData>("unitStage");
            foreach (var item in data)
            {
                //item.demandItemList = ItemInfo.getItemInfoList(item.demandItem, ItemParseType.DemandItemType);
                //item.demandMonsterList = ItemInfo.getItemInfoList(item.demandMonster, ItemParseType.DemandMonsterType);
                unitStageData.Add(item.stage, item);
                //Logger.Log(item.id + "\t" + item.english);
            }
            #endregion
        }
    }

    List<T> InitTable<T>(string filename) where T : new()
    {
        // Deserialization
        List<string> rowData = new List<string>();
        List<List<string>> rows = new List<List<string>>();
        using (var reader = new CsvFileReader(Path.Combine(Util.StaticDataPath, filename + ".csv"), Encoding.UTF8))
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
        return unitBaseData[level];
    }
    public UnitData GetUnitRowData(string unitID)
    {
        if (unitData.ContainsKey(unitID))
            return unitData[unitID];
        return null;
    }
    public WeakPointData GetWeakPointData(string id)
    {
        if (id == null)
            return null;

        WeakPointData wpData = null;
        weakPointData.TryGetValue(id, out wpData);
        return wpData;
    }

    public InstanceData GetInstanceData(string id)
    {
        InstanceData data;
        if (instanceData.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }

    public BattleLevelData GetBattleLevelData(string id)
    {
        BattleLevelData blData;
        if (battleLevelData.TryGetValue(id, out blData))
        {
            return blData;
        }

        return null;
    }

	public LazyData GetLazyData(int index)
	{
		LazyData ldata = null;
		lazyData.TryGetValue (index, out ldata);
		return ldata;
	}
	public ItemStaticData GetItemData(string id)
	{
		ItemStaticData item = null;
		itemData.TryGetValue (id, out item);
		return item;
	}
	public	PlayerLevelAttr GetPlayerLevelAttr(int level)
	{
		PlayerLevelAttr levelAttr = null;
		playerLevelAttr.TryGetValue (level, out levelAttr);
		return levelAttr;
	}

    public QuestStaticData GetQuestData(int id)
    {
        QuestStaticData item = null;
        questData.TryGetValue(id, out item);
        return item;
    }
    public SpeechData GetSpeechData(string id)
    {
        SpeechData item = null;
        speechData.TryGetValue(id, out item);
        return item;
    }

    public UnitStageData getUnitStageData(int stage)
    {
        UnitStageData item = null;
        unitStageData.TryGetValue(stage, out item);
        return item;
    }
    #endregion
}