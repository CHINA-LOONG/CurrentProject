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
    //合成
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
    //分解
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
    //商店
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
    //商城
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
