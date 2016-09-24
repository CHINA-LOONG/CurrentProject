using UnityEngine;
using System.Collections;


public class FoundMgr
{
    static FoundMgr mInst = null;
    public static FoundMgr Instance
    {
        get
        {
            if (mInst==null)
            {
                mInst = new FoundMgr();
            }
            return mInst;
        }
    }

    public UIBase curUIPanel;
    
    public UIBuild uibuild
    {
        get
        {
            return UIMgr.Instance.GetUI(UIBuild.ViewName)as UIBuild;
        }
    }

    //副本
    public void GoToUIStage(InstanceEntry entry)
    {
        InstanceEntryRuntimeData runtime = InstanceMapService.Instance.GetRuntimeInstance(entry.id);
        UIAdjustBattleTeam.OpenWith(entry.id, runtime.star,false, (InstanceType)entry.type);
    }
    /// <summary>
    /// 合成
    /// </summary>
    /// <param name="index">0--装备/1--宠物</param>
    public void GoToUICompose(int index)
    {
        if (curUIPanel as UICompose == null)
        {
            uibuild.uiCompose = UIMgr.Instance.OpenUI_(UICompose.ViewName) as UICompose;

            uibuild.uiCompose.Refresh(index);
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_item_found2"), (int)PB.ImType.PROMPT);
            //(curUIPanel as UICompose).Refresh(index);
        }
    }
    /// <summary>
    /// 分解
    /// </summary>
    /// <param name="index">0--装备/1--宠物</param>
    public void GoToUIDecompose(int index)
    {
        if (curUIPanel as UIDecompose == null)
        {
            uibuild.uiDecompose = UIMgr.Instance.OpenUI_(UIDecompose.ViewName) as UIDecompose;
            uibuild.uiDecompose.Refresh(index);
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_item_found2"), (int)PB.ImType.PROMPT);
            //(curUIPanel as UIDecompose).Refresh(index);
        }
    }
    /// <summary>
    /// 商店
    /// </summary>
    /// <param name="type">商店类型</param>
    public void GoToUIShop(PB.shopType type)
    {
        if (curUIPanel as UIShop == null)
        {
            uibuild.OpenShop((int)type);
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_item_found2"), (int)PB.ImType.PROMPT);
        }
    }
    /// <summary>
    /// 商城
    /// </summary>
    public void GoToUIStore()
    {
        if (curUIPanel as UIStore == null)
        {
            uibuild.OpenStore();
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_item_found2"), (int)PB.ImType.PROMPT);
        }
    }
    /// <summary>
    /// 任务
    /// </summary>
    /// <param name="index">0--剧情/1--日常/2--成就</param>
    public void GoToQuest(int index)
    {
        if (curUIPanel as UIQuest == null)
        {
            uibuild.uiQuest = UIMgr.Instance.OpenUI_(UIQuest.ViewName) as UIQuest;
            uibuild.uiQuest.Refresh(index);
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_item_found2"), (int)PB.ImType.PROMPT);
            //(curUIPanel as UIDecompose).Refresh(index);
        }
    }
    /// <summary>
    /// 抽蛋
    /// </summary>
    public void GoToLucky()
    {
        Logger.Log("打开抽奖界面");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index">1--boss/2--任务/3--成员/4--列表/5--主界面/6--许愿</param>
    public void GoToGuild(int index)
    {
        if (GameDataMgr.Instance.SociatyDataMgrAttr.allianceID > 0)
        {
            switch (index)
            {
                case 1:
                    //公会boss   未开发
                    break;
                case 2:
                    //任务 
                    UISociatyTask.Open();
                    break;
                case 3:
                    //成员   
                    SociatyMain.OpenWith(SociatyContenType.Member);
                    break;
                case 4:
                    //公会列表 (其它公会)
                    SociatyMain.OpenWith(SociatyContenType.OtherSociaty);
                    break;
                case 5:
                    //公会主界面
                    SociatyMain.OpenWith();
                    break;
                case 6:
                    //公会许愿
                    UIMgr.Instance.OpenUI_(SociatyPray.ViewName);
                    break;
            }
        }
        else
        {
            // 申请公会
            SociatyList.OpenWith(null);
        }
    }
    /// <summary>
    /// 背包
    /// </summary>
    public void GoToBag(BagType type)
    {
        UIBag.OpenWith(type);
    }
    /// <summary>
    /// 宠物
    /// </summary>
    /// <param name="index">0--拥有/1--收藏</param>
    public void GoToMonster(int index)
    {
        if (curUIPanel as UIMonsters == null)
        {
            uibuild.uiMonsters = UIMgr.Instance.OpenUI_(UIMonsters.ViewName) as UIMonsters;
            uibuild.uiQuest.Refresh(index);
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_item_found2"), (int)PB.ImType.PROMPT);
            //(curUIPanel as UIDecompose).Refresh(index);
        }
    }
    /// <summary>
    /// 钻石购买金币
    /// </summary>
    public void GoToBuyCoin()
    {
        GameDataMgr.Instance.ShopDataMgrAttr.JinbiNoEnough();
    }
    /// <summary>
    /// 章節
    /// </summary>
    /// <param name="chapter">章節ID</param>
    /// <param name="instanceDiff">難度</param>
    public void GoToChapter(int chapter, InstanceDifficulty instanceDiff)
    {
        bool isOpened = false;
        if (InstanceDifficulty.Normal == instanceDiff)
        {
            isOpened = InstanceMapService.Instance.IsChapterOpened(chapter);
        }
        else
        {
            isOpened = InstanceMapService.Instance.IsHardChapterOpend(chapter);
        }
        if (!isOpened)
        {
            Logger.LogError("章节为开启 for Xiaolong");
            return;
        }
        InstanceMap.OpenMapAndInstanceList(chapter, instanceDiff);
    }
    /// <summary>
    /// 通天塔
    /// </summary>
    public void GoToTower()
    {
        MainStageController mainStage = UIMgr.Instance.MainstageInstance;
        if (mainStage != null)
        {
            mainStage.SetCurrentSelectGroup((int)InstanceType.Tower);
        }

    }
    /// <summary>
    /// 试炼
    /// </summary>
    public void GoToHole()
    {
        MainStageController mainStage = UIMgr.Instance.MainstageInstance;
        if (mainStage != null)
        {
            mainStage.SetCurrentSelectGroup((int)InstanceType.Hole);
        }
    }
    /// <summary>
    /// 大冒险
    /// </summary>
    public void GoToAdventure()
    {
        if (curUIPanel as UIAdventure == null)
        {
            uibuild.uiAdventure = UIMgr.Instance.OpenUI_(UIAdventure.ViewName) as UIAdventure;
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_item_found2"), (int)PB.ImType.PROMPT);
        }
    }
    /// <summary>
    /// 竞技场
    /// </summary>
    public void GoToArena()
    {

    }
}
