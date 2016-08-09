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
            textName.color = Color.white;
        }
        else
        {
            textName.color = Color.red;
        }
    }

    void OnClickButton(GameObject go)
    {
        if (uiTips!=null)
        {
            UIMgr.Instance.CloseUI_(uiTips);
        }
        if (condition&&onClickEvent!=null)
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
    public virtual void ClickCallBack()
    {
        if (FoundMgr.Instance.curUIPanel != UIMgr.Instance.GetCurrentUI())
        {
            UIMgr.Instance.CloseUI_(FoundMgr.Instance.curUIPanel);
            FoundMgr.Instance.curUIPanel = UIMgr.Instance.GetCurrentUI();
        }
    }
    public abstract void GetResult(List<string> info, out string name, out Action action, out bool condition);

}

public class StageParse : ParseBase
{
    public override void ClickCallBack()
    {
        base.ClickCallBack();
        //TODO：前往副本
        Logger.Log("副本");
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
            return;
        }
        name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID(entry.name));
    }
    
}
public class ComposeParse : ParseBase
{
    public override void ClickCallBack()
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

    }
}
public class DecomposeParse : ParseBase
{
    public override void ClickCallBack()
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

    }
}
public class ShopParse : ParseBase
{
    public override void ClickCallBack()
    {
        base.ClickCallBack();
        //TODO:
        Logger.Log("商店");
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
                break;
            case FoundShopType.Guild:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("shop_gonghui"));
                break;
            case FoundShopType.Wishing:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("towerBoss_instance_shop"));
                break;
            default:
                break;
        }
    }
}
public class StoreParse : ParseBase
{
    public override void ClickCallBack()
    {
        base.ClickCallBack();
        //TODO:
        Logger.Log("商城");
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = false;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("shop_store"));
        name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("tips_go"));
    }
}
public class DailyParse : ParseBase
{
    public override void ClickCallBack()
    {
        base.ClickCallBack();
        //TODO:
        Logger.Log("日常");
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = true;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("tips_main5"));
        name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("tips_go"));
    }
}
public class LuckyParse : ParseBase
{
    public override void ClickCallBack()
    {
        base.ClickCallBack();
        //TODO:
        Logger.Log("抽奖");
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = false;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("tips_main6"));
        name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("tips_go"));
    }
}
public class GuildParse : ParseBase
{
    public override void ClickCallBack()
    {
        base.ClickCallBack();
        //TODO:
        Logger.Log("公会");
    }

    public override void GetResult(List<string> info, out string name, out Action action, out bool condition)
    {
        Info = info;
        action = ClickCallBack;
        condition = true;
        name = string.Format("[{0}]", StaticDataMgr.Instance.GetTextByID("sociaty_title"));

        switch ((FoundGuildType)int.Parse(Info[1]))
        {
            case FoundGuildType.Boss:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("sociaty_boss"));
                break;
            case FoundGuildType.Mission:
                name = string.Format("{0}:{1}", name, StaticDataMgr.Instance.GetTextByID("sociaty_task"));
                break;
            default:
                break;
        }
    }
}