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
        UIAdjustBattleTeam.OpenWith(entry.id, runtime.star, (InstanceType)entry.type);
    }
    //合成
    public void GoToUICompose(int index)
    {
        uibuild.uiCompose = UIMgr.Instance.OpenUI_(UICompose.ViewName) as UICompose;
        uibuild.uiCompose.Refresh(index);
    }
    //分解
    public void GoToUIDecompose(int index)
    {
        uibuild.uiDecompose = UIMgr.Instance.OpenUI_(UIDecompose.ViewName) as UIDecompose;
        uibuild.uiDecompose.Refresh(index);
    }
    //商店
    public void GoToUIShop(PB.shopType type)
    {
        uibuild.OpenShop((int)type);
    }
    //商城
    public void GoToUIStore()
    {
        Logger.Log("打开分解界面");
    }
    //日常
    public void GoToDaily()
    {
        Logger.Log("打开日常界面");
    }
    //抽奖
    public void GoToLucky()
    {
        Logger.Log("打开抽奖界面");
    }
    //公会
    public void GoToGuild()
    {
        Logger.Log("打开公会界面");
    }

    //通天塔
    public void GoToTower()
    {
        MainStageController mainStage = UIMgr.Instance.MainstageInstance;
        if (mainStage != null)
        {
            mainStage.SetCurrentSelectGroup((int)InstanceType.Tower);
        }
    }
    //洞
    public void GoToHole()
    {
        MainStageController mainStage = UIMgr.Instance.MainstageInstance;
        if (mainStage != null)
        {
            mainStage.SetCurrentSelectGroup((int)InstanceType.Hole);
        }
    }

}
