using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIPetList :  UIBase, TabButtonDelegate 
{

    public static string ViewName = PetViewConst.UIPetListAssetName;

    public Text title;
    public Button closeButton;
    public TabButtonGroup tabGroup;
    public Image srcollIcon;

    public ScrollRect scrollView;
    public GameObject content;

    int m_currentIndex = 0;
    GridLayoutGroup m_patContainer = null;
    private List<GameUnit> m_typeList = new List<GameUnit>();
    private List<PetListElement> items = new List<PetListElement>();
    private List<PetListElement> itemsPool = new List<PetListElement>();

    private UIPetDetail uiPetDetail;
    public UIPetDetail UIPetDetail
    {
        get { return uiPetDetail; }
    }

    void Start()
    {
        EventTriggerListener.Get(closeButton.gameObject).onClick = CloseButtonDown;

        GameEventMgr.Instance.AddListener(PetViewConst.ReloadPetStageNotify, ReloadPetList);

        // 默认选中第一栏
        tabGroup.InitWithDelegate(this);
        ReloadPetList();
    }

    void Update()
    {
        ShowScrollIcon();
    }

    void OnDestroy()
    {
        GameEventMgr.Instance.RemoveListener(PetViewConst.ReloadPetStageNotify, ReloadPetList);
    }
    public override void Init()
    {
        if (GameDataMgr.Instance.PlayerDataAttr.GetAllPet().Count >= GameConfig.MaxMonsterCount)
        {
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,
			                      StaticDataMgr.Instance.GetTextByID(PetViewConst.PetListFull));
        }

        title.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetListTitle);
    }

    public override void Clean()
    {
        UIMgr.Instance.DestroyUI(UIPetDetail);
    }

    void ReloadPetList()
    {
        OnTabButtonChanged(m_currentIndex);
    }

    void ShowScrollIcon()
    {
        if (m_patContainer != null && m_patContainer.transform.childCount > 6 && scrollView.normalizedPosition.y > 0.01)
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
        DeleteAllElement();
        for (int i = 0; i < m_typeList.Count; i++)
        {
            GetPatItme().ReloadPatData(m_typeList[i]);
        }
    }

    PetListElement GetPatItme()
    {
        PetListElement item = null;
        if (itemsPool.Count <= 0)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset(PetViewConst.UIPetListElementAssetName);
            if (null != go)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(content.transform, false);
                item = go.GetComponent<PetListElement>();
                ScrollViewEventListener.Get(go).onClick = SinglePetClick;
            }
        }
        else
        {
            item = itemsPool[itemsPool.Count - 1];
            item.gameObject.SetActive(true);
            itemsPool.Remove(item);
        }
        items.Add(item);
        return item;

    }

    public void DeleteAllElement()
    {
        PetListElement item = null;
        for (int i = items.Count - 1; i >= 0; i--)
        {
            item = items[i];
            item.gameObject.SetActive(false);
            items.Remove(item);
            itemsPool.Add(item);
        }
    }

    void SinglePetClick(GameObject go)
    {
        uiPetDetail = UIMgr.Instance.OpenUI_(UIPetDetail.ViewName) as UIPetDetail;
        UIPetDetail.SetTypeList(go.GetComponent<PetListElement>().unit, m_typeList);
    }

    void CloseButtonDown(GameObject go)
    {
        UIMgr.Instance.CloseUI_(UIPetList.ViewName);
    }
}
