using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuideHelp 
{
    public static bool isSignAutoOpen = false;
    private static  GuideHelp _instance = null;
    public static GuideHelp Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new GuideHelp();
            }
            return _instance;
        }
    }
    /// <summary>
    /// 图鉴中，碎片是否充足来合并一个宠物，不考虑万能碎片
    /// </summary>
    /// <returns></returns>
    public bool IsPetFragmentEnough()
    {
        return GameDataMgr.Instance.PlayerDataAttr.GetMonsterCountByCompose(false) > 0;
    }
    /// <summary>
    /// 获得当前月签到次数
    /// </summary>
    /// <returns></returns>
    public int GetSignTimesThisMonth()
    {
        return SigninDataMgr.Instance.signinTimesMonthly;
    }
    /// <summary>
    /// 获得当前 能够穿戴武器（并拥有向右武器） 的宠物数量
    /// </summary>
    /// <returns></returns>
    public int GetCanWeaponPetNumber()
    {
        return GameDataMgr.Instance.PlayerDataAttr.GetMonsterCountByPart(1);
    }
    /// <summary>
    /// 获得玩家等级
    /// </summary>
    /// <returns></returns>
    public int GetPlayerLevel()
    {
        return GameDataMgr.Instance.PlayerDataAttr.LevelAttr;
    }
    /// <summary>
    /// 获得打开商店的次数
    /// </summary>
    /// <returns></returns>
    public int GetOpenShopTimes()
    {
        return  StatisticsDataMgr.Instance.ShopOpenTimesAttr;
    }
    /// <summary>
    /// 获得大于 某一等级的宠物数量
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int GetPetNumberWithLevelMore(int level)
    {
        List<GameUnit> listUnit = new List<GameUnit>();
        GameDataMgr.Instance.PlayerDataAttr.GetAllPet(ref listUnit);
        int count = 0;
        for (int i = 0; i < listUnit.Count;++i)
        {
            if(listUnit[i].pbUnit.level > level)
            {
                count++;
            }
        }
        return count;
    }
    /// <summary>
    /// 获得 玩家拥有怪物的最高等级
    /// </summary>
    /// <returns></returns>
    public int GetPetMaxLevel()
    {
        List<GameUnit> listUnit = new List<GameUnit>();
        GameDataMgr.Instance.PlayerDataAttr.GetAllPet(ref listUnit);
        int maxLevel = 0;
        for (int i = 0; i < listUnit.Count; ++i)
        {
            if (listUnit[i].pbUnit.level > maxLevel)
            {
                maxLevel = listUnit[i].pbUnit.level;
            }
        }
        return maxLevel;
    }
    /// <summary>
    /// 获得经验药 物品个数
    /// </summary>
    /// <returns></returns>
    public int GetExpItemCount()
    {
        string[] expItemId = { "50002","50003","50004"};
        GameItemData itemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData;
        return itemData.GetItemsCount(expItemId);
    }
    /// <summary>
    /// 获得强化石数量
    /// </summary>
    /// <returns></returns>
    public int GetStrongStoneCount()
    {
        string[] stonesId = { "11001", "11002", "11003" };
        GameItemData itemData = GameDataMgr.Instance.PlayerDataAttr.gameItemData;
        return itemData.GetItemsCount(stonesId);
    }
    /// <summary>
    /// 获得第一关副本通关次数
    /// </summary>
    /// <returns></returns>
    public int GetInstance1SuccCount()
    {
       InstanceEntryRuntimeData instanceData =  InstanceMapService.Instance.GetRuntimeInstance("dajie11");
        if(null == instanceData || instanceData.star < 1)
        {
            return 0;
        }
        else
        {
            return Mathf.Max(1, instanceData.countDaily);
        }
    }
    /// <summary>
    /// 第一个任务是否完成
    /// </summary>
    /// <returns></returns>
    public bool IsFirstTaskFinished()
    {
        PB.HSStatisticsSyncGuide guideSync = StatisticsDataMgr.Instance.guideSyncData;
        if (null == guideSync)
        {
            return false;
        }
        else
        {
            return guideSync.guideQuestState[0];
        }
    }
    /// <summary>
    /// 获得经验药使用次数
    /// </summary>
    /// <returns></returns>
    public int GetExpUseCount()
    {
        PB.HSStatisticsSyncGuide guideSync = StatisticsDataMgr.Instance.guideSyncData;
        if (null == guideSync)
        {
            return 0;
        }
        else
        {
            return  guideSync.expItemUseCount;
        }
    }
    /// <summary>
    /// 获得宠物升级技能的次数
    /// </summary>
    /// <returns></returns>
    public int GetPetSkilledTimes()
    {
        PB.HSStatisticsSyncGuide guideSync = StatisticsDataMgr.Instance.guideSyncData;
        if (null == guideSync)
        {
            return 0;
        }
        else
        {
            return guideSync.upSkillTimes;
        }
    }
    /// <summary>
    /// 获得宠物穿戴的次数
    /// </summary>
    /// <returns></returns>
    public bool IsPetHasWear()
    {
        PB.HSStatisticsSyncGuide guideSync = StatisticsDataMgr.Instance.guideSyncData;
        if (null == guideSync)
        {
            return false;
        }
        else
        {
            return  guideSync.hasWear;
        }
    }
    /// <summary>
    /// 主界面更多按钮是否展开
    /// </summary>
    /// <returns></returns>
    public bool IsBuidMoreButtonExpand()
    {
        if (MoreButton.Instance == null)
            return true;
        return MoreButton.Instance.IsMoreButtonExpand;
    }

    public bool IsSigninAutoOpen()
    {
        return isSignAutoOpen;
    }
}
