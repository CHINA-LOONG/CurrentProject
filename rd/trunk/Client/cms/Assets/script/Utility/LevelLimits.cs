using UnityEngine;
using System.Collections;
using System;
public enum LimitsType
{
    expJinbiLimits = 8,//经验和金币试炼
    towerLimits = 12,//通天塔
    pvpLimits = 0,//PVP
    guildLimits = 24,//公会
    adventureLimits = 0,//大冒险
    synthesisLimits = 0,//合成
    decomposeLimits = 0,//分解
    petPromotionLimits = 8,//宠物升阶
    petEvolutionLimits = 0,//宠物进化
    equipstrengthenLimits = 6,//装备强化、进阶
    equipinlayLimits = 20,//装备开孔、镶嵌宝石
    shopLimits = 8,//普通商店
    changePetLimits = 6,//换宠
    automaticFightLimits = 6,//自动战斗
    sweepLimits = 8//扫荡
}

public class LevelLimits
{
    public static bool IsOpen(LimitsType limits)
    {
        if (GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= (int)limits)
            return true;
        else
            return false;
    }
}
