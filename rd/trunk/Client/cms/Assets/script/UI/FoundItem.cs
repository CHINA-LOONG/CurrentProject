using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public enum FoundType
{
    /// <summary> 副本 </summary>
    Stage = 1,                  
    /// <summary> 合成 </summary>
    Compose = 2,                
    /// <summary> 商店 </summary>
    Shop = 3,                   
    /// <summary> 商城 </summary>
    Store = 4,                  
    /// <summary> 任务 </summary>
    Quest = 5,                  
    /// <summary> 抽蛋 </summary>
    Lucky = 6,                  
    /// <summary> 公会 </summary>
    Guild = 7,                  
    /// <summary> 宠物 </summary>
    Monster = 8,                
    /// <summary> 章节 </summary>
    Chapter = 9,                
    /// <summary> 背包 </summary>
    Bag = 10,                   
    /// <summary> 购买金币 </summary>
    BuyCoin = 11,               
    /// <summary> 分解 </summary>
    Decompose = 12,             
    /// <summary> 活动 </summary>
    Activity = 13               
}

#region 跳转子类枚举
public enum FoundComposeType
{
    /// <summary> 宝石合成 </summary>
    Gems = 1,                 
    /// <summary> 材料合成 </summary>
    Materials = 2             
}
public enum FoundDecomposeType
{
    /// <summary> 装备合成 </summary>
    Equipments = 1,          
    /// <summary> 副本 </summary>
    Monsters = 2             
}
public enum FoundShopType
{
    /// <summary> 普通商店 </summary>
    Normal = 1,              
    /// <summary> 公会商店 </summary>
    Guild = 2,               
    /// <summary> 通天塔商店 </summary>
    Wishing = 3,             
    /// <summary> PVP商店 </summary>
    PVPShop = 4              
}
public enum FoundQuestType
{
    /// <summary> 剧情任务 </summary>
    Story = 1,                   
    /// <summary> 日常任务 </summary>
    Daily = 2,                   
    /// <summary> 成就任务 </summary>
    Achievement = 3              
}
public enum FoundGuildType
{
    /// <summary> 公会Boss </summary>
    Boss = 1,                   
    /// <summary> 公会任务 </summary>
    Mission = 2,                
    /// <summary> 公会成员 </summary>
    Member = 3,                 
    /// <summary> 公会列表 </summary>
    GuildList = 4,              
    /// <summary> 公会主界面 </summary>
    GuildMain = 5,              
    /// <summary> 公会许愿 </summary>
    GuildLucky = 6              
}
public enum FoundMonsterType
{
    /// <summary> 拥有宠物 </summary>
    Owned = 1,
    /// <summary> 宠物图鉴 </summary>
    Collection = 2
}
public enum FoundBagType
{
    /// <summary> 宝箱背包 </summary>
    Treasure = 1,              
    /// <summary> 消耗品背包 </summary>
    Consumable = 2,            
    /// <summary> 宝石背包 </summary>
    Gem = 3,                   
    /// <summary> 材料背包 </summary>
    Material = 4,              
}
public enum FoundActivityType
{
    /// <summary> 大冒险 </summary>
    Adventure = 1,      
    /// <summary> 竞技场 </summary>      
    Arena = 2,
    /// <summary> 试炼 </summary> 
    Hole = 3,            
    /// <summary> 通天塔 </summary>
    Trials = 4                
}
#endregion

public class FoundItem : MonoBehaviour
{
    public static FoundItem CreateItem(List<string> list, UIBase tips = null)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("FoundItem");
        FoundItem found = go.GetComponent<FoundItem>();
        found.ReLoadData(list, tips);
        return found;
    }

    public Text textName;
    public Image imgButton;
    public GameObject btnClick;

    public List<string> curInfo;

    string name = "";
    bool condition = false;
    Action onClickEvent = null;
    UIBase uiTips;

    void Start()
    {
        EventTriggerListener.Get(btnClick.gameObject).onClick = OnClickButton;
    }

    public void ReLoadData(List<string> info, UIBase tips = null)
    {
        curInfo = info;
        uiTips = tips;

        name = "";
        condition = false;
        onClickEvent = null;
        ParseBase parse = ParseFactory.CreateParse(info);
        parse.GetResult(info, out name, out onClickEvent, out condition);

        textName.text = name;
        imgButton.gameObject.SetActive(condition);
        if (condition)
        {
            textName.color = ColorConst.system_color_black;
        }
        else
        {
            textName.color = ColorConst.text_color_nReq;
        }
    }

    void OnClickButton(GameObject go)
    {
        if (uiTips != null)
        {
            UIMgr.Instance.CloseUI_(uiTips);
        }
        if (onClickEvent != null)
        {
            onClickEvent();
        }
    }
}

public class ParseFactory
{
    public static ParseBase CreateParse(List<string> list)
    {
        ParseBase parse = null;

        switch ((FoundType)int.Parse(list[0]))
        {
            case FoundType.Stage:
                parse = new StageParse();
                break;
            case FoundType.Compose:
                parse = new ComposeParse();
                break;
            case FoundType.Shop:
                parse = new ShopParse();
                break;
            case FoundType.Store:
                parse = new StoreParse();
                break;
            case FoundType.Quest:
                parse = new QuestParse();
                break;
            case FoundType.Lucky:
                parse = new LuckyParse();
                break;
            case FoundType.Guild:
                parse = new GuildParse();
                break;
            case FoundType.Monster:
                parse = new MonsterParse();
                break;
            case FoundType.Chapter:
                parse = new ChapterParse();
                break;
            case FoundType.Bag:
                parse = new BagParse();
                break;
            case FoundType.BuyCoin:
                parse = new BuyCoinParse();
                break;
            case FoundType.Decompose:
                parse = new DecomposeParse();
                break;
            case FoundType.Activity:
                parse = new ActivityParse();
                break;
        }
        return parse;
    }
}

public abstract class ParseBase
{
    private List<string> info;
    public List<string> Info
    {
        get { return info; }
        set { info = value; }
    }

    protected bool condition;
    public virtual void ClickCallBack()
    {
        if (FoundMgr.Instance.curUIPanel != UIMgr.Instance.GetCurrentUI())
        {
            UIMgr.Instance.CloseUI_(FoundMgr.Instance.curUIPanel);
            FoundMgr.Instance.curUIPanel = UIMgr.Instance.GetCurrentUI();
        }
        //else
        //{
        //    UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_item_found2"), (int)PB.ImType.PROMPT);
        //}
    }
    public abstract void GetResult(List<string> info, out string name, out Action action, out bool condition);

}

/// <summary>
/// 副本
/// </summary>
public class StageParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            InstanceEntry entry = StaticDataMgr.Instance.GetInstanceEntry(Info[1]);
            if (entry == null)
            {
                return;
            }
            else
            {
                switch ((InstanceType)entry.type)
                {
                    case InstanceType.Normal:
                        FoundMgr.Instance.GoToUIStage(entry);
                        break;
                    //case InstanceType.Hole:
                    //    FoundMgr.Instance.GoToHole();
                    //    break;
                    //case InstanceType.Tower:
                    //    FoundMgr.Instance.GoToTower();
                    //    break;
                    default:
                        Logger.LogError("不要在普通副本中配置通天塔和试炼");
                        break;
                }
            }
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_notopen1"), (int)PB.ImType.PROMPT);
        }
    }
    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("tips_main1"));

        InstanceEntry entry = StaticDataMgr.Instance.GetInstanceEntry(Info[1]);
        if (entry == null)
        {
            name = string.Format("{0}:{1}", name, Info[1]);
            condition = false;
            return;
        }
        else
        {
            name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID(entry.name));
            switch ((InstanceType)entry.type)
            {
                case InstanceType.Normal:
                    InstanceEntryRuntimeData runtime = InstanceMapService.Instance.GetRuntimeInstance(entry.id);

                    condition = (runtime != null) && (runtime.isOpen);
                    break;
                //case InstanceType.Hole:
                //    condition = true;
                //    //TODO: 检测功能是否开启
                //    break;
                //case InstanceType.Tower:
                //    condition = true;
                //    //TODO: 检测功能是否开启
                //    break;
                default:
                    condition = false;
                    Logger.LogError("不要在普通副本中配置通天塔和试炼");
                    break;
            }
        }
        this.condition = condition;
    }

}
/// <summary>
/// 合成
/// </summary>
public class ComposeParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            switch ((FoundComposeType)int.Parse(Info[1]))
            {
                case FoundComposeType.Gems:
                    FoundMgr.Instance.GoToUICompose(0);
                    break;
                case FoundComposeType.Materials:
                    FoundMgr.Instance.GoToUICompose(1);
                    break;
                default:
                    break;
            }
            Logger.Log("合成");
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_notopen2"), (int)PB.ImType.PROMPT);
        }
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = UIUtil.CheckIsComposeOpened();
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("compose_title"));

        switch ((FoundComposeType)int.Parse(Info[1]))
        {
            case FoundComposeType.Gems:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("compose_gem"));
                break;
            case FoundComposeType.Materials:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("compose_item"));
                break;
            default:
                break;
        }
        this.condition = condition;
    }
}
/// <summary>
/// 商店
/// </summary>
public class ShopParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();

            switch ((FoundShopType)int.Parse(Info[1]))
            {
                case FoundShopType.Normal:
                    FoundMgr.Instance.GoToUIShop(PB.shopType.NORMALSHOP);
                    break;
                case FoundShopType.Guild:
                    FoundMgr.Instance.GoToUIShop(PB.shopType.ALLIANCESHOP);
                    break;
                case FoundShopType.Wishing:
                    FoundMgr.Instance.GoToUIShop(PB.shopType.TOWERSHOP);
                    break;
                case FoundShopType.PVPShop:
                    //TODO： 还没做
                    break;
            }
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_notopen4"), (int)PB.ImType.PROMPT);
        }
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("shop_main"));

        switch ((FoundShopType)int.Parse(Info[1]))
        {
            case FoundShopType.Normal:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("shop_putong"));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundShopType.Guild:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("shop_gonghui"));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundShopType.Wishing:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("towerBoss_instance_shop"));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundShopType.PVPShop:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID(""));
                condition = false;
                //TODO： 判断是否开启功能
                break;
            default:
                condition = false;
                break;
        }
        this.condition = condition;
    }
}
/// <summary>
/// 商城
/// </summary>
public class StoreParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            FoundMgr.Instance.GoToUIStore();
            Logger.Log("商城");
        }
        else
        {
            //UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(""), (int)PB.ImType.PROMPT);
        }
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = true;
        //TODO： 判断是否开启功能
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("shop_store"));
        name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("tips_go"));

        this.condition = condition;
    }
}
/// <summary>
/// 任务
/// </summary>
public class QuestParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            switch ((FoundQuestType)int.Parse(Info[1]))
            {
                case FoundQuestType.Story:
                    FoundMgr.Instance.GoToQuest(0);
                    break;
                case FoundQuestType.Daily:
                    FoundMgr.Instance.GoToQuest(1);
                    break;
                case FoundQuestType.Achievement:
                    FoundMgr.Instance.GoToQuest(2);
                    break;
            }
        }
        else
        {
            //UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(""), (int)PB.ImType.PROMPT);
        }
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("quest_title"));

        switch ((FoundQuestType)int.Parse(Info[1]))
        {
            case FoundQuestType.Story:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("quest_juqingrenwu"));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundQuestType.Daily:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("quest_richangrenwu"));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundQuestType.Achievement:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("quest_liezhuanrenwu"));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            default:
                condition = false;
                break;
        }
        this.condition = condition;
    }
}
/// <summary>
/// 抽蛋
/// </summary>
public class LuckyParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            FoundMgr.Instance.GoToLucky();
        }
        else
        {
            //UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(""), (int)PB.ImType.PROMPT);
        }
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = true;
        //TODO： 判断是否开启功能
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("tips_main6"));
        name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("tips_go"));
        this.condition = condition;
    }
}
/// <summary>
/// 公会
/// </summary>
public class GuildParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            switch ((FoundGuildType)int.Parse(Info[1]))
            {
                case FoundGuildType.Boss:
                case FoundGuildType.Mission:
                case FoundGuildType.Member:
                case FoundGuildType.GuildList:
                case FoundGuildType.GuildMain:
                case FoundGuildType.GuildLucky:
                    FoundMgr.Instance.GoToGuild(int.Parse(Info[1]));
                    break;
            }
        }
        else
        {
            //UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_notopen5"), (int)PB.ImType.PROMPT);
        }
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("sociaty_title"));

        switch ((FoundGuildType)int.Parse(Info[1]))
        {
            case FoundGuildType.Boss:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("sociaty_boss"));
                condition = false;
                //TODO： 判断是否开启功能
                break;
            case FoundGuildType.Mission:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("sociaty_task"));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundGuildType.Member:
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundGuildType.GuildList:
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundGuildType.GuildMain:
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundGuildType.GuildLucky:
                condition = true;
                //TODO： 判断是否开启功能
                break;
            default:
                condition = false;
                break;
        }
        this.condition = condition;
    }
}
/// <summary>
/// 宠物
/// </summary>
public class MonsterParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            switch ((FoundMonsterType)int.Parse(Info[1]))
            {
                case FoundMonsterType.Owned:
                    FoundMgr.Instance.GoToMonster(0);
                    break;
                case FoundMonsterType.Collection:
                    FoundMgr.Instance.GoToMonster(1);
                    break;
            }
        }
        else
        {
            //UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_notopen5"), (int)PB.ImType.PROMPT);
        }
    }
    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID(""));

        switch ((FoundMonsterType)int.Parse(Info[1]))
        {
            case FoundMonsterType.Owned:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID(""));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundMonsterType.Collection:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID(""));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            default:
                condition = false;
                break;
        }
        this.condition = condition;
    }
}
/// <summary>
/// 章节
/// </summary>
public class ChapterParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            int chapter;
            int difficulty;
            if (int.TryParse(Info[1], out chapter) && int.TryParse(Info[2], out difficulty))
            {
                base.ClickCallBack();
                FoundMgr.Instance.GoToChapter(chapter,(InstanceDifficulty)difficulty);
            }
        }
        else
        {
            //UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("instanceselect_open_004"), (int)PB.ImType.PROMPT);
        }
    }
    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID(""));
        name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("tips_go"));

        condition = true;
        //TODO： 判断是否开启功能
        this.condition = condition;

    }
}
/// <summary>
/// 背包
/// </summary>
public class BagParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            switch ((FoundBagType)int.Parse(Info[1]))
            {
                case FoundBagType.Treasure:
                    FoundMgr.Instance.GoToBag(BagType.BaoXiang);
                    break;
                case FoundBagType.Consumable:
                    FoundMgr.Instance.GoToBag(BagType.Xiaohao);
                    break;
                case FoundBagType.Gem:
                    FoundMgr.Instance.GoToBag(BagType.Baoshi);
                    break;
                case FoundBagType.Material:
                    FoundMgr.Instance.GoToBag(BagType.Cailiao);
                    break;
            }
        }
        else
        {
            //UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(""), (int)PB.ImType.PROMPT);
        }
    }
    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("quest_title"));
        switch ((FoundBagType)int.Parse(Info[1]))
        {
            case FoundBagType.Treasure:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("quest_juqingrenwu"));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundBagType.Consumable:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("quest_juqingrenwu"));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundBagType.Gem:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("quest_juqingrenwu"));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundBagType.Material:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("quest_juqingrenwu"));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            default:
                condition = false;
                break;
        }
        this.condition = condition;
    }
}
/// <summary>
/// 购买金币
/// </summary>
public class BuyCoinParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            FoundMgr.Instance.GoToBuyCoin();
        }
        else
        {
            //UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(""), (int)PB.ImType.PROMPT);
        }
    }
    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID(""));
        name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("tips_go"));
        condition = true;
        //TODO： 判断是否开启功能
        this.condition = condition;
    }
}
/// <summary>
/// 分解
/// </summary>
public class DecomposeParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            switch ((FoundDecomposeType)int.Parse(Info[1]))
            {
                case FoundDecomposeType.Equipments:
                    FoundMgr.Instance.GoToUIDecompose(0);
                    break;
                case FoundDecomposeType.Monsters:
                    FoundMgr.Instance.GoToUIDecompose(1);
                    break;
            }
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_notopen3"), (int)PB.ImType.PROMPT);
        }
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = UIUtil.CheckIsDecomposeOpened();
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("decompose_title"));

        switch ((FoundDecomposeType)int.Parse(Info[1]))
        {
            case FoundDecomposeType.Equipments:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("decompose_item"));
                break;
            case FoundDecomposeType.Monsters:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("tips_monster"));
                break;
            default:
                condition = false;
                break;
        }
        this.condition = condition;
    }
}
/// <summary>
/// 活动
/// </summary>
public class ActivityParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            switch ((FoundActivityType)int.Parse(Info[1]))
            {
                case FoundActivityType.Adventure:
                    FoundMgr.Instance.GoToAdventure();
                    break;
                case FoundActivityType.Arena:
                    FoundMgr.Instance.GoToArena();
                    break;
                case FoundActivityType.Hole:
                    FoundMgr.Instance.GoToHole();
                    break;
                case FoundActivityType.Trials:
                    FoundMgr.Instance.GoToTower();
                    break;
            }
        }
        else
        {
            //UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID(""), (int)PB.ImType.PROMPT);
        }
    }
    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID(""));
        switch ((FoundActivityType)int.Parse(Info[1]))
        {
            case FoundActivityType.Adventure:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID(""));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundActivityType.Arena:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID(""));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundActivityType.Hole:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID(""));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            case FoundActivityType.Trials:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID(""));
                condition = true;
                //TODO： 判断是否开启功能
                break;
            default:
                condition = false;
                break;
        }
        this.condition = condition;
    }
}
