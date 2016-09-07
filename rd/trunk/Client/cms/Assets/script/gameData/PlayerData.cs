using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerData : MonoBehaviour
{
	public	int	playerId = 0;
	public	string nickName;
	public	int career;
	private	int level = 1;
    public  int LevelAttr
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
            GameEventMgr.Instance.FireEvent<int>(GameEventList.LevelChanged, level);
        }
    }


	private	int	exp = 0;
    public  int ExpAttr
    {
        get
        {
            return exp;
        }
        set
        {
            int oldExp = value - exp;
            exp = value;
            GameEventMgr.Instance.FireEvent<int, int,bool>(GameEventList.PlayerExpChanged, oldExp, exp,false);
        }
    }
	public	int	gold;//钻石
	public	long coin;//金币
	private	int	gonghuiCoin;//公会币
    public int GonghuiCoinAttr
    {
        get { return gonghuiCoin; }
        set
        {
            gonghuiCoin = value;
            GameEventMgr.Instance.FireEvent<int>(GameEventList.GonghuiCoinChanged, gonghuiCoin);
        }
    }
    private int towerCoin;//通天塔币
    public int TowerCoinAttr
    {
        get { return towerCoin; }
        set
        {
            towerCoin = value;
            GameEventMgr.Instance.FireEvent<int>(GameEventList.TowerCoinChanged, gonghuiCoin);
        }
    }
	public	int	gender;
	public	int	eye;
	public	int	hair;
	public	int	hairColor;
	public	int	recharget;
	public	int	vipLevel;
    private int huoli = 0;//活力值
    public  int HuoliAttr
    {
        get
        {
            return huoli;
        }
    }

    private int huoliBeginTime = 0;
    public  int HuoliBegintimeAttr
    {
        get
        {
            return huoliBeginTime;
        }
    }

    public  void    UpdateHuoli(int newhuoli,int huoliBeginTime)
    {
        huoli = newhuoli;
        this.huoliBeginTime = huoliBeginTime;
        GameEventMgr.Instance.FireEvent<int>(GameEventList.HuoliChanged, huoli);

        bool isNeedRestore = this.huoli < MaxHuoliAttr;
        
        if(isNeedRestore != GameDataMgr.Instance.HuoliRestoreAtrr.IsRestoring)
        {
            GameDataMgr.Instance.HuoliRestoreAtrr.IsRestoring = isNeedRestore;
        }
        GameEventMgr.Instance.FireEvent(GameEventList.RefreshSaodangTimes);
    }

    public  int MaxHuoliAttr
    {
        get
        {
            PlayerLevelAttr levelAttr = StaticDataMgr.Instance.GetPlayerLevelAttr(level);
            if (null == levelAttr)
                return 0;
            return levelAttr.fatigue;
        }
    }

    //主角装备加成
    public int equipHealth;
    public int equipStrength;
    public int equipIntelligence;
    public int equipSpeed;
    public int equipDefense;
    public int equipEndurance;
    public float criticalRatio;
    public float hitRatio;

    public float equipHealthRatio;
    public float equipStrengthRatio;
    public float equipIntelligenceRatio;
    public float equipSpeedRatio;
    public float equipDefenseRatio;
    public float equipEnduranceRatio;
    //五行加成
    public float goldDamageRatio;
    public float woodDamageRatio;
    public float waterDamageRatio;
    public float fireDamageRatio;
    public float earthDamageRatio;
    
    //上阵宠物
    public List<int> mainUnitID = new List<int>();
    public List<BattleObject> mainUnitList = new List<BattleObject>();
    public Dictionary<int, PbUnit> unitPbList = new Dictionary<int, PbUnit>();
	public Dictionary<int,GameUnit> allUnitDic = new Dictionary<int, GameUnit> ();
    //图鉴收藏
    public List<string> petCollect = new List<string>();
    public List<CollectUnit> collectUnit = new List<CollectUnit>();

    //屏蔽列表
    public List<int> mBlockPlayerList = new List<int>();

    //道具
    public GameItemData gameItemData = new GameItemData();
    //装备
    public GameEquipData gameEquipData = new GameEquipData();
    //任务
    public GameQuestData gameQuestData = new GameQuestData();
    //邮件
    public GameMailData gameMailData = new GameMailData();

    public int[,] equipTypePart = new int[BattleConst.equipTypeCount, (int)PartType.NUM_EQUIP_PART] { {0,0,0,0,0,0 },
                                                                                                    {0,0,0,0,0,0 },
                                                                                                    {0,0,0,0,0,0 },
                                                                                                    {0,0,0,0,0,0 }};
    //---------------------------------------------------------------------------------------------
    public void InitMainUnitList()
    {
        mainUnitList.Clear();
        foreach (int unitID in mainUnitID)
        {
            //PbUnit pb = null;
            //if (unitPbList.TryGetValue(unitID, out pb))
            //{
            //    GameUnit curUnit = GameUnit.FromPb(pb, true);
            //    //TODO: use event to create battleobject
            //    //Vector3 testPos = new Vector3(-0.5f + (i % 3) * 0.5f, 0.5f - (int)(i / 3) * 0.5f);
            //    BattleObject bo = ObjectDataMgr.Instance.CreateBattleObject(curUnit, null, Vector3.zero, Quaternion.identity);
            //    bo.gameObject.SetActive(false);
            //    mainUnitList.Add(bo);
            //}
            GameUnit curGameUnit = null;
            if (allUnitDic.TryGetValue(unitID, out curGameUnit))
            {
                BattleObject bo = ObjectDataMgr.Instance.CreateBattleObject(curGameUnit, null, Vector3.zero, Quaternion.identity);
                bo.gameObject.SetActive(false);
                mainUnitList.Add(bo);
            }
            else
            {
                Logger.LogErrorFormat("can not find monster id={0}", unitID);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void ClearData()
    {
        mainUnitID.Clear();
        BattleObject subObj = null;
        for (int i =0;i< mainUnitList.Count;++i)
        {
            subObj = mainUnitList[i];
            ObjectDataMgr.Instance.RemoveBattleObject(subObj.guid);
        }
		mainUnitList.Clear();
        unitPbList.Clear();
        allUnitDic.Clear();
        petCollect.Clear();
        collectUnit.Clear();
        mBlockPlayerList.Clear();
        gameItemData.ClearData();
        gameEquipData.ClearData();
        gameQuestData.ClearData();
        gameMailData.ClearMail();

        for (int i = 0; i < BattleConst.equipTypeCount; ++i)
        {
            for (int j = 0; j < (int)PartType.NUM_EQUIP_PART; j++)
            {
                equipTypePart[i, j] = 0;
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetMainUnits(List<int> playerTeam)
    {
        mainUnitID.Clear();
        foreach (int unitID in playerTeam)
        {
            mainUnitID.Add(unitID);
        }
    }
    //---------------------------------------------------------------------------------------------
    public List<BattleObject> GetMainUnits()
    {
        return mainUnitList;
    }
    //---------------------------------------------------------------------------------------------
    public bool RemoveUnit(int guid)
    {
        if (allUnitDic.ContainsKey(guid))
        {
            allUnitDic.Remove(guid);
            unitPbList.Remove(guid);
            mainUnitID.Remove(guid);
            InitMainUnitList();

            return true;
        }

        return false;
    }
    //---------------------------------------------------------------------------------------------
    public void AddBlockPlayer(int blockID)
    {
        mBlockPlayerList.Add(blockID);
    }
    //---------------------------------------------------------------------------------------------
    public void RemoveBlockPlayer(int blockID)
    {
        int count = mBlockPlayerList.Count;
        for (int i = 0; i < count; ++i)
        {
            if (mBlockPlayerList[i] == blockID)
            {
                mBlockPlayerList.RemoveAt(i);
                break;
            }
        }
    }
    //---------------------------------------------------------------------------------------------
	public void GetAllPet(ref List<GameUnit> allPet)
	{
        allPet.Clear();
		allPet.AddRange (allUnitDic.Values);
    }
    //---------------------------------------------------------------------------------------------
    public int GetPetCount()
    {
        return allUnitDic.Count;
    }
    //---------------------------------------------------------------------------------------------
    public void GetAllPet(string monsterId, int stage, ref List<GameUnit> petList)
    {
        petList.Clear();
        foreach (GameUnit unit in allUnitDic.Values)
        {
            if (unit.pbUnit.id.Equals(monsterId) == true && unit.pbUnit.stage >= stage)
            {
                petList.Add(unit);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
	public GameUnit GetPetWithKey(int guid)
	{
		GameUnit unit = null;
		allUnitDic.TryGetValue (guid, out unit);
		return unit;
	}
    //---------------------------------------------------------------------------------------------
    public void SyncPlayerInof(ProtocolMessage msg)
    {
        //TODO: test only
        //create fake main player
        //for (int i = 0; i < 6; i++)
        //{
        //    PbUnit pbUnit = new PbUnit();
        //    pbUnit.camp = UnitCamp.Player;
        //    pbUnit.guid = 10 + i;
        //    pbUnit.level = 50;
        //    if (i == 0)
        //        pbUnit.id = "Unit_Demo_qingniao";
        //    if (i == 1)
        //        pbUnit.id = "Unit_Demo_zhuyan";
        //    if (i == 2)
        //        pbUnit.id = "Unit_Demo_zhuyan";
        //    if (i == 3)
        //        pbUnit.id = "Unit_Demo_ershu";
        //    if (i == 4)
        //        pbUnit.id = "Unit_Demo_qingniao";
        //    if (i == 5)
        //        pbUnit.id = "Unit_Demo_ershu";
        //    pbUnit.slot = i;
        //    if (i > 2)
        //        pbUnit.slot = BattleConst.offsiteSlot;
        //    pbUnit.character = 4;
        //    pbUnit.lazy = 4;

        //    mainUnitPb.Add(pbUnit);
        //}
        //PB

    }
    //---------------------------------------------------------------------------------------------
    public void AddCollectPet(string monsterId)
    {
        if (petCollect.Contains(monsterId))
        {
            return;
        }

        petCollect.Add(monsterId);
        for (int i = 0; i < collectUnit.Count; i++)
        {
            if (collectUnit[i].unit.id==monsterId)
            {
                collectUnit[i].isExist = true;
            }
        }
        GameEventMgr.Instance.FireEvent(GameEventList.ReloadPetCollectNotify);
    }
    public void InitCollectPet(List<string> list)
    {
        petCollect = list;

        List<UnitData> unitList = StaticDataMgr.Instance.GetPlayerUnitData();
        collectUnit.Clear();
        for (int i = 0; i < unitList.Count; i++)
        {
            CollectUnit collect = new CollectUnit();
            collect.unit = unitList[i];
            if (petCollect.Contains(unitList[i].id))
            {
                collect.isExist = true;
            }
            else
            {
                collect.isExist = false;
            }
            collectUnit.Add(collect);
        }
        collectUnit.Sort(SortCollect);
    }
    static int SortCollect(CollectUnit a, CollectUnit b)
    {
        int result = 0;
        if (a.unit.rarity>b.unit.rarity)
        {
            result = 1;
        }
        else if(a.unit.rarity < b.unit.rarity)
        {
            result = -1;
        }
        return result;
    }
    public int GetCollectCount()
    {
        return petCollect.Count;
    }

    //---------------------------------------------------------------------------------------------

    //获得或卸下装备
    public void AddEquipTypePart(long equipId)
    {
        AddEquipTypePart(gameEquipData.GetEquip(equipId));
    }
    public void AddEquipTypePart(EquipData equip)
    {
        //if (equip.monsterId == BattleConst.invalidMonsterID)
        {
            ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(equip.equipId);
            equipTypePart[itemInfo.subType-1, itemInfo.part-1] += 1;
        }
    }
    //被装备或移除该装备
    public void RemoveEquipTypePart(long equipId)
    {
        RemoveEquipTypePart(gameEquipData.GetEquip(equipId));
    }
    public void RemoveEquipTypePart(EquipData equip)
    {
        ItemStaticData itemInfo = StaticDataMgr.Instance.GetItemData(equip.equipId);

        equipTypePart[itemInfo.subType - 1, itemInfo.part - 1] -= 1;
        if (equipTypePart[itemInfo.subType - 1, itemInfo.part - 1] < 0)
        {
            Logger.LogError("装备出现异常");
        }
    }
    public void RefreshEquipTypePart()
    {
        for (int i = 0; i < equipTypePart.GetLength(0); i++)
        {
            for (int j = 0; j < equipTypePart.GetLength(1); j++)
            {
                equipTypePart[i, j] = 0;
            }
        }

        Dictionary<long, EquipData> equipList = GameDataMgr.Instance.PlayerDataAttr.gameEquipData.equipList;
        foreach (var item in equipList)
        {
            if (item.Value.monsterId != BattleConst.invalidMonsterID)
            {
                continue;
            }
            ItemStaticData itemTemp = StaticDataMgr.Instance.GetItemData(item.Value.equipId);
            equipTypePart[itemTemp.subType - 1, itemTemp.part - 1] += 1;
        }
    }

    public bool CheckEquipTypePart(int type, int part)
    {
        if (equipTypePart[type-1,part-1]>0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //-----------------------------------------------------------------------
}

public class CollectUnit
{
    public UnitData unit;
    public bool isExist;
}