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
    public Text countLabel;
    public Text nonoMonsterLabel;
    public Text demandLabel;
    public Image scrollIcon;
    public GameObject elementContainer;
    public Transform monIconPos;
    private MonsterIcon avatar;

    List<int> m_currentSelectMonster = null;
    ItemInfo m_itemInfo = null;

	// Update is called once per frame
	void Update () {
	
        if (m_itemInfo != null && m_currentSelectMonster != null)
        {
            countLabel.text = string.Format("{0}/{1}", m_currentSelectMonster.Count, m_itemInfo.count);
        }

        ShowScrollIcon();
	}

    public void init(ItemInfo itemInfo, List<int> selectMonster,GameUnit curUnit/* int selfId*/)
    {
        int selfId=curUnit.pbUnit.guid;
        nonoMonsterLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageNoneMonster);
        demandLabel.text = StaticDataMgr.Instance.GetTextByID(PetViewConst.PetDetailStageMonster);
        m_currentSelectMonster = selectMonster;
        m_itemInfo = itemInfo;

        List<GameUnit> petList = GameDataMgr.Instance.PlayerDataAttr.GetAllPet((itemInfo.itemId.Equals(BattleConst.stageSelfId) ? curUnit.pbUnit.id : itemInfo.itemId), itemInfo.stage);
        foreach (GameUnit unit in petList)
        {
            if (unit.pbUnit.guid != selfId)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset(PetViewConst.UIPetStageMonsterElementAssetName, false);
                scrollView.AddElement(go);
                go.GetComponent<SelectMonsterElement>().ReloadData(unit, m_currentSelectMonster.Contains(unit.pbUnit.guid));
                ScrollViewEventListener.Get(go.GetComponent<SelectMonsterElement>().eventObject).onClick = SelectButtonDown;
            }
        }

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

    void ShowScrollIcon()
    {
        if (scrollRect != null && elementContainer.transform.childCount > 3 && scrollRect.GetComponent<ScrollRect>().normalizedPosition.y > 0.01)
        {
            scrollIcon.gameObject.SetActive(true);
        }
        else
        {
            scrollIcon.gameObject.SetActive(false);
        }
    }
}
