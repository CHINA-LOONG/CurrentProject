using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

public class GemInfo
{
    public int type;
    public string gemId;

    public GemInfo(int type, string gemId) {
        this.type = type;
        this.gemId = gemId;
    }

    public GemInfo(PB.GemPunch gem)
    {
        this.type = gem.type;
        this.gemId = gem.gemItemId;
    }
}

public class EquipData
{
    public long id;
    public int stage;
    public int level;
    public string equipId;
    public int monsterId;
    public List<GemInfo> gemList;

    //基础属性
    public int health;
    public int strength;
    public int intelligence;
    public int defense;
    public int speed;
    //强化属性
    public int healthStrengthen;
    public int strengthStrengthen;
    public int intelligenceStrengthen;
    public int defenseStrengthen;
    public int speedStrengthen;
    //宝石属性
    public int healthGem;
    public int strengthGem;
    public int intelligenceGem;
    public int defenseGem;
    public int speedGem;

    //---------------------------------------------------------------------------------------------
    public static EquipData valueof(long id, string equipId, int stage, int level,int monsterId, List<PB.GemPunch> gemList)
    {
        EquipData equipData = new EquipData();
        equipData.id = id;
        equipData.stage = stage;
        equipData.level = level;
        equipData.equipId = equipId;
        equipData.monsterId = monsterId;
        equipData.gemList = new List<GemInfo>();
        //基础属性id
        equipData.SetStageLvl(stage, true);
        equipData.SetStrengthLvl(level, true);
        foreach (PB.GemPunch element in gemList)
        {
            GemInfo gemInfo = new GemInfo(element);
            equipData.gemList.Add(gemInfo);
        }
        //
        return equipData;
    }
    //---------------------------------------------------------------------------------------------
    public void SetStrengthLvl(int lvl, bool forceRefresh = false)
    {
        if (forceRefresh || level != lvl)
        {
            level = lvl;
            if (lvl != 0)
            {
                EquipProtoData item = StaticDataMgr.Instance.GetEquipProtoData(equipId, stage);
                EquipLevelData lvlAttr = StaticDataMgr.Instance.GetEquipLevelData(item.levelAttrId);
                healthStrengthen = lvlAttr.health * level;
                strengthStrengthen = lvlAttr.strength * level;
                intelligenceStrengthen = lvlAttr.intelligence * level;
                defenseStrengthen = lvlAttr.defense * level;
                speedStrengthen = lvlAttr.speed * level;
            }
            else
            {
                healthStrengthen = 0;
                strengthStrengthen = 0;
                intelligenceStrengthen = 0;
                defenseStrengthen = 0;
                speedStrengthen = 0;
            }

            if (monsterId != BattleConst.invalidMonsterID)
            {
                GameUnit ownedMonster = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(monsterId);
                if (ownedMonster != null)
                {
                    ownedMonster.ForceRefreshAttr();
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetStageLvl(int lvl, bool forceRefresh = false)
    {
        if (forceRefresh || stage != lvl)
        {
            stage = lvl;
            EquipProtoData item = StaticDataMgr.Instance.GetEquipProtoData(equipId, stage);
            EquipLevelData lvlAttr = StaticDataMgr.Instance.GetEquipLevelData(item.stageAttrId);
            health = lvlAttr.health;
            strength = lvlAttr.strength;
            intelligence = lvlAttr.intelligence;
            defense = lvlAttr.defense;
            speed = lvlAttr.speed;

            if (monsterId != BattleConst.invalidMonsterID)
            {
                GameUnit ownedMonster = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(monsterId);
                if (ownedMonster != null)
                {
                    ownedMonster.ForceRefreshAttr();
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void RefreshGemAttr()
    {
        int count = gemList.Count;
        ItemStaticData curItemData = null;
        EquipLevelData curLevlData = null;
        healthGem = 0;
        strengthGem = 0;
        intelligenceGem = 0;
        defenseGem = 0;
        speedGem = 0;
        for (int i = 0; i < count; ++i)
        {
            if (gemList[i].gemId != "0")
            {
                curItemData = StaticDataMgr.Instance.GetItemData(gemList[i].gemId);
                curLevlData = StaticDataMgr.Instance.GetEquipLevelData(curItemData.gemId);
                healthGem += curLevlData.health;
                strengthGem += curLevlData.strength;
                intelligenceGem += curLevlData.intelligence;
                defenseGem += curLevlData.defense;
                speedGem += curLevlData.speed;
            }
        }

        if (monsterId != BattleConst.invalidMonsterID)
        {
            GameUnit ownedMonster = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(monsterId);
            if (ownedMonster != null)
            {
                ownedMonster.ForceRefreshAttr();
            }
        }
    }
    //---------------------------------------------------------------------------------------------
}
////Modify: xiaolong   2015-8-18 09:51:17
//public class AttrData
//{
//    // 属性id(参考Const.attr)
//    public int attrId;
//    // 属性值
//    public float attrValue;

//    public AttrData(PB)
//}

public class GameEquipData
{
    public Dictionary<long, EquipData> equipList = new Dictionary<long, EquipData>();

    //modify: xiaolong 2015-9-9 18:41:28
    //public void AddEquip(long id, string equipId, int stage, int level, List<PB.GemPunch> gemList)
    //{
    //    EquipData equipData;
    //    if (equipList.TryGetValue(id, out equipData))
    //    {
    //        equipData.id = id;
    //        equipData.equipId = equipId;
    //        equipData.stage = stage;
    //        equipData.level = level;
    //        equipData.gemList.Clear();
    //        foreach (PB.GemPunch element in gemList)
    //        {
    //            GemInfo gemInfo = new GemInfo(element);
    //            equipData.gemList.Add(gemInfo);
    //        }
    //    }
    //    else
    //    {
    //        equipList.Add(id, EquipData.valueof(id,equipId, stage, level, gemList));
    //    }
    //}
    public void AddEquip(EquipData data)
    {
        if (equipList.ContainsKey(data.id))
        {
            equipList[data.id] = data;
        }
        else
        {
            equipList.Add(data.id, data);
        }
    }

    public bool RemoveEquip(long id)
    {
        if (equipList.ContainsKey(id))
        {
            equipList.Remove(id);
            return true;
        }

        return false;
    }

    public EquipData GetEquip(long id)
    {
        EquipData equipData;
        if (equipList.TryGetValue(id, out equipData))
        {
            return equipData;
        }
        return null;
    }
}

