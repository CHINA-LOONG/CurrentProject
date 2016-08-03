using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIPetList : UIBase, TabButtonDelegate
{

    public static string ViewName = PetViewConst.UIPetListAssetName;

    public Text title;
    public Button closeButton;
    public Button btnMonsterbook;

    public TabButtonGroup tabGroup;
    public Image srcollIcon;

    public ScrollRect scrollView;
    public GameObject content;

    public Text textOption_0;
    public Text textOption_1;
    public Text textOption_2;
    public Text textOption_3;
    public Text textOption_4;
    public Text textOption_5;

    int m_currentIndex = 0;
    GridLayoutGroup m_patContainer = null;
    private List<GameUnit> m_typeList = new List<GameUnit>();
    private List<PetListElement> items = new List<PetListElement>();
    private List<PetListElement> itemsPool = new List<PetListElement>();

    private UIMonsterbook uiMonsterbook;
    public UIMonsterbook UIMonsterbook
    {
        get{return uiMonsterbook;}
    }
    private UIPetDetail uiPetDetail;
    public UIPetDetail UIPetDetail
    {
        get { return uiPetDetail; }
    }


    void Start()
    {
        EventTriggerListener.Get(closeButton.gameObject).onClick = CloseButtonDown;
        EventTriggerListener.Get(btnMonsterbook.gameObject).onClick = ClickMonsterbookBtn;

        scrollView.onValueChanged.AddListener(ShowScrollIcon);

        textOption_0.text = StaticDataMgr.Instance.GetTextByID("pet_list_option_all");
        textOption_1.text = StaticDataMgr.Instance.GetTextByID("pet_list_option_property1");
        textOption_2.text = StaticDataMgr.Instance.GetTextByID("pet_list_option_property2");
        textOption_3.text = StaticDataMgr.Instance.GetTextByID("pet_list_option_property3");
        textOption_4.text = StaticDataMgr.Instance.GetTextByID("pet_list_option_property4");
        textOption_5.text = StaticDataMgr.Instance.GetTextByID("pet_list_option_property5");
        
        // 默认选中第一栏
        tabGroup.InitWithDelegate(this);
        ReloadPetList();
    }

    void ShowScrollIcon(Vector2 vec2)
    {
        if (vec2.y > 0 && scrollView.content.rect.height > (scrollView.transform as RectTransform).rect.height)
        {
            srcollIcon.gameObject.SetActive(true);
        }
        else
        {
            srcollIcon.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        GameEventMgr.Instance.AddListener(GameEventList.ReloadPetStageNotify, ReloadPetList);
        GameEventMgr.Instance.AddListener(GameEventList.ReloadPetEquipNotify, ReloadPetList);
    }

    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener(GameEventList.ReloadPetStageNotify, ReloadPetList);
        GameEventMgr.Instance.RemoveListener(GameEventList.ReloadPetEquipNotify, ReloadPetList);
    }
    public override void Init()
    {
        if (GameDataMgr.Instance.PlayerDataAttr.GetAllPet().Count >= GameConfig.MaxMonsterCount)
        {
            MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,
                       StaticDataMgr.Instance.GetTextByID("pet_tip_full1"),
                       StaticDataMgr.Instance.GetTextByID("pet_tip_full2"));
        }

        tabGroup.OnChangeItem(0);

        title.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetListTitle);
    }

    public override void Clean()
    {
        UIMgr.Instance.DestroyUI(UIPetDetail);
        UIMgr.Instance.DestroyUI(UIMonsterbook);
    }

    void ReloadPetList()
    {
        OnTabButtonChanged(m_currentIndex);
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
        scrollView.verticalNormalizedPosition = 1.0f;
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
    void ClickMonsterbookBtn(GameObject go)
    {
        UIMgr.Instance.OpenUI_(UIMonsterbook.ViewName);
    }
}
