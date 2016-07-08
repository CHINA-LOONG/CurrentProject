using UnityEngine;
using System.Collections;
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

        if (SortType.ALLTYPE == (SortType)index)
        {
            foreach (BattleObject monsterObject in GameDataMgr.Instance.PlayerDataAttr.mainUnitList)
            {
                AddPatItme(monsterObject.unit);
            }
        }
        else if (SortType.GOLDTYPE == (SortType)index)
        {
            foreach (BattleObject monsterObject in GameDataMgr.Instance.PlayerDataAttr.mainUnitList)
            {
                if (monsterObject.unit.property == SpellConst.propertyGold)
                {
                    AddPatItme(monsterObject.unit);
                }
            }
        }
        else if (SortType.WOODTYPE == (SortType)index)
        {
            foreach (BattleObject monsterObject in GameDataMgr.Instance.PlayerDataAttr.mainUnitList)
            {
                if (monsterObject.unit.property == SpellConst.propertyWood)
                {
                    AddPatItme(monsterObject.unit);
                }
            }
        }
        else if (SortType.WATERTYPE == (SortType)index)
        {
            foreach (BattleObject monsterObject in GameDataMgr.Instance.PlayerDataAttr.mainUnitList)
            {
                if (monsterObject.unit.property == SpellConst.propertyWater)
                {
                    AddPatItme(monsterObject.unit);
                }
            }
        }
        else if (SortType.FIRETYPE == (SortType)index)
        {
            foreach (BattleObject monsterObject in GameDataMgr.Instance.PlayerDataAttr.mainUnitList)
            {
                if (monsterObject.unit.property == SpellConst.propertyFire)
                {
                    AddPatItme(monsterObject.unit); ;
                }
            }
        }
        else if (SortType.EARTHTYPE == (SortType)index)
        {
            foreach (BattleObject monsterObject in GameDataMgr.Instance.PlayerDataAttr.mainUnitList)
            {
                if (monsterObject.unit.property == SpellConst.propertyEarth)
                {
                    AddPatItme(monsterObject.unit);
                }
            }
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
            patScript.guid = unit.pbUnit.guid;
            patScript.ReloadPatData(unit);
            ScrollViewEventListener.Get(go).onClick = SinglePetClick;
        }
    }

    void SinglePetClick(GameObject go)
    {
        Debug.Log(go.GetComponent<SinglePet>().guid);
        UIMgr.Instance.OpenUI(UIPetDetail.AssertName, UIPetDetail.ViewName);
    }
}
