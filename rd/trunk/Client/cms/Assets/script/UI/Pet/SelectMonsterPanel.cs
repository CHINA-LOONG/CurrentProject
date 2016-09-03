using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SelectMonsterPanel : UIBase
{
    public static string ViewName = "SelectMonsterPanel";


    public ScrollViewContainer scrollView;
    public ScrollRect scrollRect;

    public Button closeButton;
    public Text textClose;
    public Text countLabel;
    //public Text demandLabel;
    public Image scrollIcon;
    public GameObject elementContainer;
    public Transform monIconPos;
    private MonsterIcon avatar;
    public Text textEmpty;

    private List<GameUnit> mValidatePetList = new List<GameUnit>();

    List<int> m_currentSelectMonster = null;
    ItemInfo m_itemInfo = null;

	void Update () {
	
        if (m_itemInfo != null && m_currentSelectMonster != null)
        {
            countLabel.text = string.Format("{0}/{1}", m_currentSelectMonster.Count, m_itemInfo.count);
        }

        //ShowScrollIcon();
	}

    public void init(ItemInfo itemInfo, List<List<int>> demandList, List<int> selectMonster, GameUnit curUnit/* int selfId*/)
    {
        int selfId=curUnit.pbUnit.guid;
        //demandLabel.text = StaticDataMgr.Instance.GetTextByID("pet_detail_stage_monster");
        textClose.text = StaticDataMgr.Instance.GetTextByID("ui_queding");
        textEmpty.text = StaticDataMgr.Instance.GetTextByID("list_empty");
        m_currentSelectMonster = selectMonster;
        m_itemInfo = itemInfo;

        GameDataMgr.Instance.PlayerDataAttr.GetAllPet(
            (itemInfo.itemId.Equals(BattleConst.stageSelfId) ? curUnit.pbUnit.id : itemInfo.itemId),
            itemInfo.stage,
            ref mValidatePetList
            );
        int showCount = 0;
        foreach (GameUnit unit in mValidatePetList)
        {
            if (unit.pbUnit.guid != selfId && !unit.pbUnit.IsLocked() && !CheckMonsterIsSelect(unit, demandList, selectMonster))
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("UIPetStageMonsterElement", false);
                scrollView.AddElement(go);
                go.GetComponent<SelectMonsterElement>().ReloadData(unit, m_currentSelectMonster.Contains(unit.pbUnit.guid));
                ScrollViewEventListener.Get(go.GetComponent<SelectMonsterElement>().eventObject).onClick = SelectButtonDown;
                showCount++;
            }
        }
        textEmpty.gameObject.SetActive(showCount == 0);

        //if (elementContainer != null && elementContainer.transform.childCount > 0)
        //{
        //    nonoMonsterLabel.gameObject.SetActive(false);
        //}
        //else
        //{
        //    nonoMonsterLabel.gameObject.SetActive(true);
        //}

        if (avatar == null)
        {
            avatar = MonsterIcon.CreateIcon();
            UIUtil.SetParentReset(avatar.transform, monIconPos);
        }
        else
        {
            avatar.Init();
        }
        avatar.SetMonsterStaticId(itemInfo.itemId.Equals(BattleConst.stageSelfId)?curUnit.pbUnit.id:itemInfo.itemId);
        avatar.SetStage(itemInfo.stage);

        UpdateSelectState();
    }
    bool CheckMonsterIsSelect(GameUnit unit, List<List<int>> demand, List<int> current)
    {
        for (int i = 0; i < demand.Count; i++)
        {
            if (demand[i]==current)
            {
                continue;
            }
            else
            {
                for (int j = 0; j < demand[i].Count; j++)
                {
                    if (demand[i][j]==unit.pbUnit.guid)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void UpdateSelectState()
    {
        if (elementContainer != null)
        {
            for (int i = 0; i < elementContainer.transform.childCount; ++i)
            {
                Transform element = elementContainer.transform.GetChild(i);
                SelectMonsterElement elementScript = element.GetComponent<SelectMonsterElement>();
                if (elementScript != null)
                {
                    if (m_currentSelectMonster.Count == m_itemInfo.count)
                    {
                       if (m_currentSelectMonster.Contains(elementScript.guid) == false)
                       {
                           elementScript.showMask = true;
                       } 
                       else
                       {
                           elementScript.showMask = false;
                       }
                    }
                    else
                    {
                        elementScript.showMask = false;
                    }
                }
            }            
        }
    }

    void SelectButtonDown(GameObject go)
    {
        SelectMonsterElement element = go.transform.parent.GetComponent<SelectMonsterElement>();

        if (element.select == true)
        {
            element.select = false;
            m_currentSelectMonster.Remove(element.guid);
        }
        else
        {
            element.select = true;
            m_currentSelectMonster.Add(element.guid);
        }

        UpdateSelectState();
    }

    //void ShowScrollIcon()
    //{
    //    if (scrollRect != null && elementContainer.transform.childCount > 3 && scrollRect.GetComponent<ScrollRect>().normalizedPosition.y > 0.01)
    //    {
    //        scrollIcon.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        scrollIcon.gameObject.SetActive(false);
    //    }
    //}
}
