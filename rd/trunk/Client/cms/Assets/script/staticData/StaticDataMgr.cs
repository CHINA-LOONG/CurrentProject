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
    Dictionary<string, InstanceData> instanceData = new Dictionary<string, InstanceData>();
	Dictionary<int,LazyData>	lazyData = new Dictionary<int, LazyData>();
	Dictionary<int,CharacterData> characterData = new Dictionary<int, CharacterData>();
	Dictionary<int,Chapter>	chapterData = new Dictionary<int, Chapter>();
	Dictionary<string,InstanceEntry> instanceEntryData = new Dictionary<string, InstanceEntry>();
	Dictionary<int,ItemStaticData> itemData = new Dictionary<int, ItemStaticData>();
	Dictionary<int,PlayerLevelAttr> playerLevelAttr = new Dictionary<int, PlayerLevelAttr>();

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
            var data = InitTable<BuffPrototype>("buff");
            foreach (var item in data)
                buffData.Add(item.id, item);
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
            var data = InitTable<InstanceData>("instance");
            foreach (var item in data)
            {
                ProcessData process;

                if (!string.IsNullOrEmpty(item.bossValiP1))
                {
                    process = new ProcessData();
                    process.index = 1;
                    process.preAnim = item.pre1Animation;
                    process.processAnim = item.process1Animation;
                    process.needClearBuff = item.is1ClearBuff != 0;
                    process.ParseCondition(item.bossValiP1);
                    item.bossProcess.Add(process);
                }

                if (!string.IsNullOrEmpty(item.bossValiP2))
                {
                    process = new ProcessData();
                    process.index = 2;
                    process.preAnim = item.pre2Animation;
                    process.processAnim = item.process2Animation;
                    process.needClearBuff = item.is2ClearBuff != 0;
                    process.ParseCondition(item.bossValiP2);
                    item.bossProcess.Add(process);
                }

                if (!string.IsNullOrEmpty(item.bossValiP3))
                {
                    process = new ProcessData();
                    process.index = 3;
                    process.preAnim = item.pre3Animation;
                    process.processAnim = item.process3Animation;
                    process.needClearBuff = item.is3ClearBuff != 0;
                    process.ParseCondition(item.bossValiP3);
                    item.bossProcess.Add(process);
                }

                if (!string.IsNullOrEmpty(item.bossValiP4))
                {
                    process = new ProcessData();
                    process.index = 4;
                    process.preAnim = item.pre4Animation;
                    process.processAnim = item.process4Animation;
                    process.needClearBuff = item.is4ClearBuff != 0;
                    process.ParseCondition(item.bossValiP4);
                    item.bossProcess.Add(process);
                }

                if (!string.IsNullOrEmpty(item.bossValiP5))
                {
                    process = new ProcessData();
                    process.index = 5;
                    process.preAnim = item.pre5Animation;
                    process.processAnim = item.process5Animation;
                    process.needClearBuff = item.is5ClearBuff != 0;
                    process.ParseCondition(item.bossValiP5);
                    item.bossProcess.Add(process);
                }

                if (!string.IsNullOrEmpty(item.rareValiP1))
                {
                    process = new ProcessData();
                    process.index = 1;
                    process.preAnim = item.preRare1Animation;
                    process.processAnim = item.processRare1Animation;
                    process.needClearBuff = item.isRare1ClearBuff != 0;
                    process.ParseCondition(item.rareValiP1);
                    item.rareProcess.Add(process);
                }


                if (!string.IsNullOrEmpty(item.rareValiP2))
                {
                    process = new ProcessData();
                    process.index = 2;
                    process.preAnim = item.preRare2Animation;
                    process.processAnim = item.processRare2Animation;
                    process.needClearBuff = item.isRare2ClearBuff != 0;
                    process.ParseCondition(item.rareValiP2);
                    item.rareProcess.Add(process);
                }


                if (!string.IsNullOrEmpty(item.rareValiP3))
                {
                    process = new ProcessData();
                    process.index = 3;
                    process.preAnim = item.preRare3Animation;
                    process.processAnim = item.processRare3Animation;
                    process.needClearBuff = item.isRare3ClearBuff != 0;
                    process.ParseCondition(item.rareValiP3);
                    item.rareProcess.Add(process);
                }


                if (!string.IsNullOrEmpty(item.rareValiP4))
                {
                    process = new ProcessData();
                    process.index = 4;
                    process.preAnim = item.preRare4Animation;
                    process.processAnim = item.processRare4Animation;
                    process.needClearBuff = item.isRare4ClearBuff != 0;
                    process.ParseCondition(item.rareValiP4);
                    item.rareProcess.Add(process);
                }


                if (!string.IsNullOrEmpty(item.rareValiP5))
                {
                    process = new ProcessData();
                    process.index = 5;
                    process.preAnim = item.preRare5Animation;
                    process.processAnim = item.processRare5Animation;
                    process.needClearBuff = item.isRare5ClearBuff != 0;
                    process.ParseCondition(item.rareValiP5);
                    item.rareProcess.Add(process);
                }

                instanceData.Add(item.id, item);
            }
        }
        #endregion
        {
            var data = InitTable<WeakPointData>("weakPointData");
            foreach (var item in data)
                weakPointData.Add(item.id, item);
        }

		{
			var data = InitTable<LazyData>("lazy");
			foreach(var item in data)
				lazyData.Add(item.index,item);
		}

		{
			var data = InitTable<CharacterData>("character");
			foreach(var item in data)
				characterData.Add(item.index,item);
		}

		{
			var data = InitTable<Chapter>("chapter");
			foreach(var item in data)
				chapterData.Add(item.chapter,item);
		}

		{
			var data = InitTable<InstanceEntry>("instanceEntry");
			foreach(var item in data)
			{
				item.enemyList = new List<string>();
				item.rewardList = new List<int>();

				if(!string.IsNullOrEmpty(item.enemy1))
					item.enemyList.Add(item.enemy1);

				if(!string.IsNullOrEmpty(item.enemy2))
					item.enemyList.Add(item.enemy2);

				if(!string.IsNullOrEmpty(item.enemy3))
					item.enemyList.Add(item.enemy3);

				if(!string.IsNullOrEmpty(item.enemy4))
					item.enemyList.Add(item.enemy4);

				if(!string.IsNullOrEmpty(item.enemy5))
					item.enemyList.Add(item.enemy5);

				if(!string.IsNullOrEmpty(item.enemy6))
					item.enemyList.Add(item.enemy6);

				if(item.reward1 > 0)
					item.rewardList.Add(item.reward1);

				if(item.reward2 > 0)
					item.rewardList.Add(item.reward2);

				if(item.reward3 > 0)
					item.rewardList.Add(item.reward3);
				
				if(item.reward4 > 0)
					item.rewardList.Add(item.reward4);
				
				if(item.reward5 > 0)
					item.rewardList.Add(item.reward5);
				
				if(item.reward6 > 0)
					item.rewardList.Add(item.reward6);


				instanceEntryData.Add(item.id,item);
			}
		}

		{
			var data = InitTable<ItemStaticData>("item");
			foreach(var item in data)
				itemData.Add(item.id,item);
		}

		{
			var data = InitTable<PlayerLevelAttr>("playerAttr");
			foreach(var item in data)
				playerLevelAttr.Add(item.level,item);
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
        return instanceData[id];
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

    #endregion
}