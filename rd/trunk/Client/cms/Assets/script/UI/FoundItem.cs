using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public enum FoundType
{
    Stage = 1,
    Compose,
    Decompose,
    Shop,
    Store,
    Daily,
    Lucky,
    Guild
}
public enum FoundComposeType
{
    Gems = 1,
    Materials
}
public enum FoundDecomposeType
{
    Equipments = 1,
    Monsters
}
public enum FoundShopType
{
    Normal = 1,
    Guild,
    Wishing
}
public enum FoundGuildType
{
    Boss = 1,
    Mission
}

public class FoundItem : MonoBehaviour
{
    public static FoundItem CreateItem(List<string> list, UIBase tips = null)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("FoundItem");
        FoundItem found = go.GetComponent<FoundItem>();
        found.ReLoadData(list,tips);
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

    public void ReLoadData(List<string> info, UIBase tips=null)
    {
        curInfo = info;
        uiTips = tips;

        name = "";
        condition = false;
        onClickEvent =null;
        ParseBase parse = ParseFactory.CreateParse(info);
        parse.GetResult(info,out name, out onClickEvent,out condition);

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
        if (uiTips!=null)
        {
            UIMgr.Instance.CloseUI_(uiTips);
        }
        if (onClickEvent!=null)
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
            case FoundType.Decompose:
                parse = new DecomposeParse();
                break;
            case FoundType.Shop:
                parse = new ShopParse();
                break;
            case FoundType.Store:
                parse = new StoreParse();
                break;
            case FoundType.Daily:
                parse = new DailyParse();
                break;
            case FoundType.Lucky:
                parse = new LuckyParse();
                break;
            case FoundType.Guild:
                parse = new GuildParse();
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
                    case InstanceType.Hole:
                        FoundMgr.Instance.GoToHole();
                        break;
                    case InstanceType.Tower:
                        FoundMgr.Instance.GoToTower();
                        break;
                }
            }
            Logger.Log("副本");
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_notopen1"), (int)PB.ImType.PROMPT);
        }
    }
    public override void GetResult(List<string> info,out string name,out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = false;
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

                    condition = (runtime != null)&&(runtime.isOpen);
                    break;
                case InstanceType.Hole:
                    condition = true;
                    //TODO: 检测功能是否开启
                    break;
                case InstanceType.Tower:
                    condition = true;
                    //TODO: 检测功能是否开启
                    break;
                default:
                    condition = false;
                    break;
            }
        }
        this.condition = condition;
    }
    
}
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
                default:
                    break;
            }
            Logger.Log("分解");
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
                break;
        }
        this.condition = condition;
    }
}
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
                    //TODO： 设置通天塔商店
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
        condition = false;
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
                condition = false;
                //TODO： 判断是否开启功能
                break;
            case FoundShopType.Wishing:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("towerBoss_instance_shop"));
                condition = false;
                //TODO： 判断是否开启功能
                break;
            default:
                break;
        }
        this.condition = condition;
    }
}
public class StoreParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            //TODO:
            Logger.Log("商城");
        }
        else
        {

        }
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = false;
        //TODO： 判断是否开启功能
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("shop_store"));
        name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("tips_go"));

        this.condition = condition;
    }
}
public class DailyParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            //TODO:
            Logger.Log("日常");
        }
        else
        {

        }
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = false;
        //TODO： 判断是否开启功能
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("tips_main5"));
        name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("tips_go"));

        this.condition = condition;
    }
}
public class LuckyParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            //TODO:
            Logger.Log("抽奖");
        }
        else
        {

        }
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = false;
        //TODO： 判断是否开启功能
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("tips_main6"));
        name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("tips_go"));
        this.condition = condition;
    }
}
public class GuildParse : ParseBase
{
    public override void ClickCallBack()
    {
        if (condition)
        {
            base.ClickCallBack();
            //TODO:
            Logger.Log("公会");
        }
        else
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("record_notopen5"), (int)PB.ImType.PROMPT);
        }
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = false;
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
                condition = false;
                //TODO： 判断是否开启功能
                break;
            default:
                break;
        }
        this.condition = condition;
    }
}