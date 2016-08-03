using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class UIMonsterbook : UIBase,IScrollView,TabButtonDelegate
{
    public static string ViewName = "UIMonsterbook";

    public Text text_Title;
    public Text text_Collected;
    public Text textMonster;
    public Text textFragment;

    public Text textOption_1;
    public Text textOption_2;
    public Text textOption_3;
    public Text textOption_4;
    public Text textOption_5;

    public Button btnClose;


    public FixCountScrollView scrollView;
    public GameObject item;

    private List<GameUnit> curList=new List<GameUnit>();
    private List<MonsterBookItemInfo> listInfo = new List<MonsterBookItemInfo>();


    private int tabIndex = -1;
    private int selIndex = 0;
    private TabButtonGroup tabGroup;
    public TabButtonGroup TabGroup
    {
        get
        {
            if (tabGroup == null)
            {
                tabGroup = GetComponentInChildren<TabButtonGroup>();
                tabGroup.InitWithDelegate(this);
            }
            return tabGroup;
        }
    }
    void Start()
    {
        text_Title.text = StaticDataMgr.Instance.GetTextByID("handbook_btn");
        text_Collected.text = StaticDataMgr.Instance.GetTextByID("handbook_shouji");

        textOption_1.text = StaticDataMgr.Instance.GetTextByID("pet_list_option_property1");
        textOption_2.text = StaticDataMgr.Instance.GetTextByID("pet_list_option_property2");
        textOption_3.text = StaticDataMgr.Instance.GetTextByID("pet_list_option_property3");
        textOption_4.text = StaticDataMgr.Instance.GetTextByID("pet_list_option_property4");
        textOption_5.text = StaticDataMgr.Instance.GetTextByID("pet_list_option_property5");

        EventTriggerListener.Get(btnClose.gameObject).onClick = ClickCloseBtn;
    }
    public void Refresh(int select = -1)
    {
        selIndex = (select == -1 ? selIndex : select);
        if (tabIndex!=selIndex)
        {
            TabGroup.OnChangeItem(selIndex);
        }
        else
        {
            ReloadData(selIndex);
        }
    }
    void ReloadData(int index)
    {
        int curType=0;
        #region 标签与类型匹配

        switch (index)
        {
            case 0:
                curType = SpellConst.propertyGold;
                break;
            case 1:
                curType = SpellConst.propertyWood;
                break;
            case 2:
                curType = SpellConst.propertyWater;
                break;
            case 3:
                curType = SpellConst.propertyFire;
                break;
            case 4:
                curType = SpellConst.propertyEarth;
                break;
            default:
                Logger.LogError("选择图鉴标签出错");
                break;
        }
        #endregion

        List<GameUnit> list = GameDataMgr.Instance.PlayerDataAttr.GetAllPet();
        curList.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].property==curType)
            {
                curList.Add(list[i]);
            }
        }
        curList.Sort();

        listInfo.Clear();
        for (int i = 0; i < curList.Count; i++)
        {
            MonsterBookItemInfo info = new MonsterBookItemInfo(curList[i]);
            listInfo.Add(info);
        }
        scrollView.InitContentSize(listInfo.Count, this);
    }

    void ClickCloseBtn(GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }

    /// <summary>
    /// UIBase重写
    /// </summary>
    public override void Init()
    {
        Refresh();
    }
    public override void Clean()
    {
    }

    /// <summary>
    /// TabButtonDelegate接口
    /// </summary>
    /// <param name="index"></param>
    public void OnTabButtonChanged(int index)
    {
        if (tabIndex==index)
        {
            return;
        }
        selIndex = index;
        tabIndex = selIndex;
        ReloadData(tabIndex);
    }

    /// <summary>
    /// IScrollView接口
    /// </summary>
    public Transform CreateData(Transform parent, int index = 0)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("MonsterBookItem");
        UIUtil.SetParentReset(go.transform, parent);
        return go.transform;
    }
    public void ReloadData(Transform item, int index)
    {
        //throw new NotImplementedException();
        MonsterBookItem monster = item.GetComponent<MonsterBookItem>();
        monster.ReloadData(listInfo[index]);
    }
    public void CleanData(List<Transform> itemList)
    {
        itemList.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
        itemList.Clear();
    }
}
