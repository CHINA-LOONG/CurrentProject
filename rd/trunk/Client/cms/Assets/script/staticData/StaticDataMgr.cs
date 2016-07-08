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
    Dictionary<string, BuffPrototype> buffData = new Dictionary<string, BuffPrototype>();
    Dictionary<string, EffectPrototype> effectData = new Dictionary<string, EffectPrototype>();
    Dictionary<string, SpellProtoType> spellData = new Dictionary<string, SpellProtoType>();
    Dictionary<int, int> spellUpLevelData = new Dictionary<int, int>();
    Dictionary<string, InstanceData> instanceData = new Dictionary<string, InstanceData>();
    Dictionary<string, BattleLevelData> battleLevelData = new Dictionary<string, BattleLevelData>();
	Dictionary<int,LazyData>	lazyData = new Dictionary<int, LazyData>();
	Dictionary<int,CharacterData> characterData = new Dictionary<int, CharacterData>();
	Dictionary<int,Chapter>	chapterData = new Dictionary<int, Chapter>();
	Dictionary<string,InstanceEntry> instanceEntryData = new Dictionary<string, InstanceEntry>();
	Dictionary<int,ItemStaticData> itemData = new Dictionary<int, ItemStaticData>();
	Dictionary<int,PlayerLevelAttr> playerLevelAttr = new Dictionary<int, PlayerLevelAttr>();
    Dictionary<int, QuestStaticData> questData = new Dictionary<int, QuestStaticData>();
    Dictionary<int, RewardData> rewardData = new Dictionary<int, RewardData>();
    Dictionary<int, TimeStaticData> timeData = new Dictionary<int, TimeStaticData>();

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
        {
            var data = InitTable<SpellProtoType>("spell");
            foreach (var item in data)
                spellData.Add(item.id, item);
        }
        {
            var data = InitTable<SpellUpLevelPrice>("skillUpPrice");
            foreach (var item in data)
                spellUpLevelData.Add(item.level, item.coin);
        }
        {
            var data = InitTable<BuffPrototype>("buff");
            foreach (var item in data)
                buffData.Add(item.id, item);
        }
        {
            //effectData = new Dictionary<string, EffectPrototype>();
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
                            ArrayList effectArrayList = MiniJsonExtensions.arrayListFromJson(wholeData.effectList);
                            for (int i = 0; i < effectArrayList.Count; ++i)
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
                    case EffectType.Effect_Type_Switch:
                        {
                            effectPt = new EffectSwitchPrototype();
                            EffectSwitchPrototype switchPt = effectPt as EffectSwitchPrototype;

                            string[] effectList = wholeData.periodEffectList.Split(';');
                            ///ArrayList effectArrayList = MiniJsonExtensions.arrayListFromJson (wholeData.periodEffectList);
                            for (int i = 0; i < effectList.Length; ++i)
                            {
                                string[] effectKV = effectList[i].Split('|');
                                if (effectKV.Length != 2)
                                    continue;

                                switchPt.effectList.Add(new KeyValuePair<string, string>(effectKV[0], effectKV[1]));
                            }

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
        #region instance data
        {
            var data = InitTable<InstanceProtoData>("instance");
            foreach (var item in data)
            {
                InstanceData curInstance = new InstanceData();
                curInstance.instanceProtoData = item;

                ArrayList battleArrayList = MiniJsonExtensions.arrayListFromJson(item.battleLevelList);
                for (int i = 0; i < battleArrayList.Count; ++i)
                {
                    curInstance.battleLevelList.Add(battleArrayList[i] as string);
                }

                instanceData.Add(curInstance.instanceProtoData.id, curInstance);
            }
        }
        #endregion
        #region battleLevel data
        {
            var data = InitTable<BattleLevelProtoData>("battleLevel");
            foreach (var item in data)
            {
                BattleLevelData blData = new BattleLevelData();
                blData.battleProtoData = item;
                if (string.IsNullOrEmpty(item.winFunc) == false)
                {
                    var cls = typeof(NormalScript);
                    blData.winFunc = cls.GetMethod(item.winFunc);
                }
                if (string.IsNullOrEmpty(item.loseFunc) == false)
                {
                    var cls = typeof(NormalScript);
                    blData.loseFunc = cls.GetMethod(item.loseFunc);
                }

                Hashtable monsterTable = MiniJsonExtensions.hashtableFromJson(item.monsterList);
                if (monsterTable != null)
                {
                    foreach (DictionaryEntry de in monsterTable)
                    {
                        blData.monsterList.Add(de.Key.ToString(), int.Parse(de.Value.ToString()));
                    }
                }

                battleLevelData.Add(blData.battleProtoData.id, blData);
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
            var data = InitTable<CharacterData>("character");
            foreach (var item in data)
                characterData.Add(item.index, item);
        }

        {
            var data = InitTable<Chapter>("chapter");
            foreach (var item in data)
                chapterData.Add(item.chapter, item);
        }
		
		{
			var data = InitTable<InstanceEntry>("instanceEntry");
			foreach(var item in data)
			{
				item.AdaptData();

                instanceEntryData.Add(item.id, item);
            }
        }

        {
            var data = InitTable<ItemStaticData>("item");
            foreach (var item in data)
                itemData.Add(item.id, item);
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
            #region static Reward
            var data = InitTable<RewardStaticData>("reward");
            foreach (var item in data)
            {
                RewardData reward = new RewardData();
                reward.id = item.id;
                reward.itemList = new List<RewardItemData>();

                string[] rewardList = item.reward.Split(',');
                for (int i = 0; i < rewardList.Length; i++)
                {
                    string[] rewardAttr = rewardList[i].Split('_');

                    if ((PB.itemType)int.Parse(rewardAttr[0]) == PB.itemType.EQUIP)
                    {
                        if (rewardAttr.Length != 6) continue;
                    }
                    else if (rewardAttr.Length != 4) continue;

                    RewardItemData rewardItem=null;
                    switch ((PB.itemType)int.Parse(rewardAttr[0]))
                    {
                        case PB.itemType.NONE_ITEM:
                            break;
                        case PB.itemType.PLAYER_ATTR:
                        case PB.itemType.MONSTER_ATTR:
                            rewardItem = new RewardItemData(int.Parse(rewardAttr[0]),
                                                        int.Parse(rewardAttr[1]),
                                                        int.Parse(rewardAttr[2]),
                                                        float.Parse(rewardAttr[3]));
                            break;
                        case PB.itemType.ITEM:
                            rewardItem = new RewardItemData(int.Parse(rewardAttr[0]),
                                                        int.Parse(rewardAttr[1]),
                                                        int.Parse(rewardAttr[2]),
                                                        float.Parse(rewardAttr[3]));
                            break;
                        case PB.itemType.EQUIP:
                            rewardItem = new RewardItemData(int.Parse(rewardAttr[0]),
                                                        int.Parse(rewardAttr[1]),
                                                        int.Parse(rewardAttr[2]),
                                                        int.Parse(rewardAttr[3]),
                                                        int.Parse(rewardAttr[4]),
                                                        float.Parse(rewardAttr[5]));
                            break;
                        case PB.itemType.SKILL:
                            break;
                        case PB.itemType.GROUP:
                            break;
                        case PB.itemType.MONSTER:
                            break;
                        default:
                            Logger.Log("not found this itemType");
                            break;
                    }
                    if (rewardItem != null)
                    {
                        //Logger.Log("\nitemType :" + rewardItem.itemType + "\nitemId :" + rewardItem.itemId + "\ncount :" + rewardItem.count + "\nstage :" + rewardItem.stage + "\nlevel :" + rewardItem.level + "\nprob :" + rewardItem.prob);
                        reward.itemList.Add(rewardItem);
                    }
                }
                rewardData.Add(reward.id, reward);
            }
            #endregion
        }
        {
            #region static Time
            var data = InitTable<TimeStaticData>("time");
            foreach (var item in data)
            {
                timeData.Add(item.type, item);
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
    public SpellProtoType GetSpellProtoData(string id)
    {
        if (spellData.ContainsKey(id))
            return spellData[id];

        return null;
    }
    public int GetSPellLevelPrice(int nextLevel)
    {
        if (spellUpLevelData.ContainsKey(nextLevel))
            return spellUpLevelData[nextLevel];
       
        return 0;
    }
    public EffectPrototype GetEffectProtoData(string id)
    {
        if (effectData.ContainsKey(id))
            return effectData[id];

        return null;
    }
    public BuffPrototype GetBuffProtoData(string id)
    {
        if (buffData.ContainsKey(id))
            return buffData[id];

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

	public CharacterData GetCharacterData(int index)
	{
		CharacterData cData = null;
		characterData.TryGetValue (index, out cData);
		return cData;
	}

	public Chapter GetChapterData(int chapterIndex)
	{
		Chapter chapter = null;
		chapterData.TryGetValue (chapterIndex, out chapter);
		return chapter;
	}

	public InstanceEntry GetInstanceEntry(string instanceId)
	{
		InstanceEntry entry = null;
		instanceEntryData.TryGetValue (instanceId, out entry);
		return	entry;
	}

	public List<InstanceEntry> GetInstanceEntryList(int diffculty,int chapter)
	{
		List<InstanceEntry> listReturn = new List<InstanceEntry> ();

		foreach (InstanceEntry subEntry in instanceEntryData.Values) 
		{
			if(subEntry.difficulty == diffculty &&
			   subEntry.chapter == chapter)
			{
				listReturn.Add(subEntry);
			}
		}
		return listReturn;
	}

	public ItemStaticData GetItemData(int id)
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

    public RewardData GetRewardData(int id)
    {
        RewardData item = null;
        rewardData.TryGetValue(id, out item);
        return item;
    }

    public TimeStaticData GetTimeData(int id)
    {
        TimeStaticData item = null;
        timeData.TryGetValue(id, out item);
        return item;
    }


    #endregion
}