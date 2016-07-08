using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PetList : MonoBehaviour, TabButtonDelegate
{
    enum SortType 
    {
        ALLTYPE = 0,
        GOLDTYPE,
        WOODTYPE,
        WATERTYPE,
        FIRETYPE,
        EARTHTYPE
    }

    public GameObject m_scrollRect = null;
    private GridLayoutGroup m_patContainer = null;
    private List<GameUnit> typeList = new List<GameUnit>();

    // Use this for initialization
    void Start()
    {
        GetComponentInChildren<TabButtonGroup>().InitWithDelegate(this);
        OnTabButtonChanged(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTabButtonChanged(int index)
    {
        GameObject container = Util.FindChildByName(m_scrollRect, "petContainer");
        if (container != null)
        {
            Destroy(container);
            m_patContainer = null;
        }

        container = ResourceMgr.Instance.LoadAsset("ui/pet", "petContainer", false);
        container.transform.localScale = Vector3.one;
        container.transform.SetParent(m_scrollRect.transform, false);
        container.name = "petContainer";
        m_scrollRect.GetComponent<ScrollRect>().content = container.GetComponent<RectTransform>();
        m_patContainer = container.GetComponent<GridLayoutGroup>();

        typeList.Clear();

        if (SortType.ALLTYPE == (SortType)index)
        {
            List<GameUnit> list = GameDataMgr.Instance.PlayerDataAttr.GetAllPet();
            typeList.AddRange(list);
                 
        }
        else if (SortType.GOLDTYPE == (SortType)index)
        {
            foreach (GameUnit unit in GameDataMgr.Instance.PlayerDataAttr.GetAllPet())
            {
                if (unit.property == SpellConst.propertyGold)
                {
                    typeList.Add(unit);
                }
            }
        }
        else if (SortType.WOODTYPE == (SortType)index)
        {
            foreach (GameUnit unit in GameDataMgr.Instance.PlayerDataAttr.GetAllPet())
            {
                if (unit.property == SpellConst.propertyWood)
                {
                    typeList.Add(unit);
                }
            }
        }
        else if (SortType.WATERTYPE == (SortType)index)
        {
            foreach (GameUnit unit in GameDataMgr.Instance.PlayerDataAttr.GetAllPet())
            {
                if (unit.property == SpellConst.propertyWater)
                {
                    typeList.Add(unit);
                }
            }
        }
        else if (SortType.FIRETYPE == (SortType)index)
        {
            foreach (GameUnit unit in GameDataMgr.Instance.PlayerDataAttr.GetAllPet())
            {
                if (unit.property == SpellConst.propertyFire)
                {
                    typeList.Add(unit); ;
                }
            }
        }
        else if (SortType.EARTHTYPE == (SortType)index)
        {
            foreach (GameUnit unit in GameDataMgr.Instance.PlayerDataAttr.GetAllPet())
            {
                if (unit.property == SpellConst.propertyEarth)
                {
                    typeList.Add(unit);
                }
            }
        }

        typeList.Sort();

        foreach (GameUnit unit in typeList)
        {
            AddPatItme(unit);
        } 
    }

    void AddPatItme(GameUnit unit)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("ui/pet", "singlePet", false);
        if (go != null)
        {
            go.transform.localScale = Vector3.one;
            go.transform.SetParent(m_patContainer.transform, false);
            SinglePet patScript = go.GetComponent<SinglePet>();
            patScript.unit = unit;
            patScript.ReloadPatData(unit);
            ScrollViewEventListener.Get(go).onClick = SinglePetClick;
        }
    }

    void SinglePetClick(GameObject go)
    {
        UIMgr.Instance.OpenUI(UIPetDetail.AssertName, UIPetDetail.ViewName).GetComponent<UIPetDetail>().SetTypeList(go.GetComponent<SinglePet>().unit, typeList);
    }
}
