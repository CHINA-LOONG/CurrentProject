using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIPetList :  UIBase, TabButtonDelegate {

    public static string ViewName = PetViewConst.UIPetListAssetName;

    public Button closeButton;
    public GameObject scrollRect;
    public TabButtonGroup tabGroup;
    public Image srcollIcon;

    int m_currentIndex = 0;
    GridLayoutGroup m_patContainer = null;
    List<GameUnit> m_typeList = new List<GameUnit>();

    void Start()
    {
        EventTriggerListener.Get(closeButton.gameObject).onClick = CloseButtonDown;

        GameEventMgr.Instance.AddListener(PetViewConst.ReloadPetStageNotify, ReloadPetList);

        // 默认选中第一栏
        tabGroup.InitWithDelegate(this);
        ReloadPetList();
    }

    void OnDestroy()
    {
        GameEventMgr.Instance.RemoveListener(PetViewConst.ReloadPetStageNotify, ReloadPetList);
    }

    public override void OnOpenUI()
    {
       if (GameDataMgr.Instance.PlayerDataAttr.GetAllPet().Count >= GameConfig.MaxMonsterCount)
       {
           MsgBox.PromptMsg.Open("提示", "拥有过多宠物,快去消耗一些吧\n 有可能不会继续获得宠物", "确定");
       }
    }

    void ReloadPetList()
    {
        OnTabButtonChanged(m_currentIndex);
    }

    void ShowScrollIcon()
    {
        if (m_patContainer != null && m_patContainer.transform.childCount > 6)
        {
            srcollIcon.gameObject.SetActive(true);
        }
        else
        {
            srcollIcon.gameObject.SetActive(false);
        }
    }

    public void OnTabButtonChanged(int index)
    {
        m_currentIndex = index;

        GameObject container = Util.FindChildByName(scrollRect, PetViewConst.UIPetListContainerAssetName);
        if (container != null)
        {
            Destroy(container);
            m_patContainer = null;
        }

        container = ResourceMgr.Instance.LoadAsset(PetViewConst.UIPetListContainerAssetName, false);
        container.transform.localScale = Vector3.one;
        container.transform.SetParent(scrollRect.transform, false);
        container.name = PetViewConst.UIPetListContainerAssetName;
        scrollRect.GetComponent<ScrollRect>().content = container.GetComponent<RectTransform>();
        m_patContainer = container.GetComponent<GridLayoutGroup>();

        m_typeList.Clear();

        if (PetViewConst.SortType.ALLTYPE == (PetViewConst.SortType)index)
        {
            List<GameUnit> list = GameDataMgr.Instance.PlayerDataAttr.GetAllPet();
            m_typeList.AddRange(list);

        }
        else if (PetViewConst.SortType.GOLDTYPE == (PetViewConst.SortType)index)
        {
            foreach (GameUnit unit in GameDataMgr.Instance.PlayerDataAttr.GetAllPet())
            {
                if (unit.property == SpellConst.propertyGold)
                {
                    m_typeList.Add(unit);
                }
            }
        }
        else if (PetViewConst.SortType.WOODTYPE == (PetViewConst.SortType)index)
        {
            foreach (GameUnit unit in GameDataMgr.Instance.PlayerDataAttr.GetAllPet())
            {
                if (unit.property == SpellConst.propertyWood)
                {
                    m_typeList.Add(unit);
                }
            }
        }
        else if (PetViewConst.SortType.WATERTYPE == (PetViewConst.SortType)index)
        {
            foreach (GameUnit unit in GameDataMgr.Instance.PlayerDataAttr.GetAllPet())
            {
                if (unit.property == SpellConst.propertyWater)
                {
                    m_typeList.Add(unit);
                }
            }
        }
        else if (PetViewConst.SortType.FIRETYPE == (PetViewConst.SortType)index)
        {
            foreach (GameUnit unit in GameDataMgr.Instance.PlayerDataAttr.GetAllPet())
            {
                if (unit.property == SpellConst.propertyFire)
                {
                    m_typeList.Add(unit); ;
                }
            }
        }
        else if (PetViewConst.SortType.EARTHTYPE == (PetViewConst.SortType)index)
        {
            foreach (GameUnit unit in GameDataMgr.Instance.PlayerDataAttr.GetAllPet())
            {
                if (unit.property == SpellConst.propertyEarth)
                {
                    m_typeList.Add(unit);
                }
            }
        }

        m_typeList.Sort();

        foreach (GameUnit unit in m_typeList)
        {
            AddPatItme(unit);
        }

        ShowScrollIcon();
    }

    void AddPatItme(GameUnit unit)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset(PetViewConst.UIPetListElementAssetName, false);
        if (go != null)
        {
            go.transform.localScale = Vector3.one;
            go.transform.SetParent(m_patContainer.transform, false);
            PetListElement petScript = go.GetComponent<PetListElement>();
            petScript.ReloadPatData(unit);
            ScrollViewEventListener.Get(go).onClick = SinglePetClick;
        }
    }

    void SinglePetClick(GameObject go)
    {
        UIMgr.Instance.OpenUI(UIPetDetail.ViewName).GetComponent<UIPetDetail>().SetTypeList(go.GetComponent<PetListElement>().unit, m_typeList);
    }

    void CloseButtonDown(GameObject go)
    {
        UIMgr.Instance.CloseUI(UIPetList.ViewName);
    }
}
