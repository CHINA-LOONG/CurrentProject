using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DecomposeItemInfo
{
    public UIDecompose.type type;

    public EquipData curEquip;
    public GameUnit curMonster;
    public System.Action Refresh;

    private bool isSelect=false;
    public bool IsSelect
    {
        get { return isSelect; }
        set
        {
            if (isSelect != value)
            {
                isSelect = value;
                if (Refresh!=null)
                {
                    Refresh();
                }
            }
        }
    }
    public long ItemId
    {
        get
        {
            if (type==UIDecompose.type.Equipment)
            {
                return curEquip.id;
            }
            else
            {
                return curMonster.pbUnit.guid;
            }
        }
    }

    public DecomposeItemInfo(EquipData equip)
    {
        type = UIDecompose.type.Equipment;
        curEquip = equip;
    }
    public DecomposeItemInfo(GameUnit monster)
    {
        type = UIDecompose.type.Monsters;
        curMonster = monster;
    }

}
public class DecomposeItem : MonoBehaviour
{
    public Button btnSelect;

    public Image imgSelect;
    private Sprite sprNormal;
    private Sprite sprSelect;
    private System.Action<DecomposeItem> onClickBack;

    private DecomposeItemInfo curData;
    public DecomposeItemInfo CurData
    {
        get { return curData; }
        set
        {
            if (curData != null)
            {
                curData.Refresh = null;
            }
            curData = value;
            curData.Refresh = RefreshInfo;
        }
    }
    #region monster

    public GameObject monsterUI;

    public Transform transMonsterIcon;
    private MonsterIcon monsterIcon;

    public Text monsterName;
    public Image proIcon;
    public Text textMonsterBP;
    public Text text_MonsterBP;

    #endregion

    #region equipment
    public GameObject equipUI;

    public Transform transEquipIcon;
    private ItemIcon equipIcon;

    public Text equipName;
    public Text textEquipBP;
    public Text textDengji;

    public Text text_EquipBP;
    public Text text_Dengji;

    public Transform content;
    [HideInInspector]
    public List<EquipGemItem> items = new List<EquipGemItem>();
    [HideInInspector]
    public List<EquipGemItem> itemPool = new List<EquipGemItem>();

    void RefreshGem(List<GemInfo> gems)
    {
        RemoveAllElement();
        foreach (var info in gems)
        {
            EquipGemItem item = GetElement();
            item.Refresh(info);
            item.transform.SetAsLastSibling();
        }
    }

    public EquipGemItem GetElement()
    {
        EquipGemItem item = null;
        if (itemPool.Count <= 0)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("EquipGemItem");
            if (go != null)
            {
                go.transform.localScale = Vector3.one;
                go.transform.SetParent(content, false);
                item = go.GetComponent<EquipGemItem>();
            }
        }
        else
        {
            item = itemPool[itemPool.Count - 1];
            item.gameObject.SetActive(true);
            itemPool.Remove(item);
        }
        items.Add(item);
        return item;
    }
    public void RemoveElement(EquipGemItem item)
    {
        if (items.Contains(item))
        {
            item.gameObject.SetActive(false);
            items.Remove(item);
            itemPool.Add(item);
        }
    }
    public void RemoveAllElement()
    {
        items.ForEach(delegate(EquipGemItem item) { item.gameObject.SetActive(false); });
        itemPool.AddRange(items);
        items.Clear();
    }
    
    #endregion

    void Start()
    {
        ScrollViewEventListener.Get(btnSelect.gameObject).onClick = OnClickItem;

        text_MonsterBP.text = StaticDataMgr.Instance.GetTextByID("equip_forge_zhanli");

        text_Dengji.text = StaticDataMgr.Instance.GetTextByID("equip_List_xianzhidengji");
        text_EquipBP.text = StaticDataMgr.Instance.GetTextByID("equip_forge_zhanli");
    }

    public void Init(System.Action<DecomposeItem> clickBack)
    {
        this.onClickBack = clickBack;
    }

    public void ReloadData(DecomposeItemInfo data)
    {
        CurData = data;
        if (CurData.type == UIDecompose.type.Equipment)
        {
            ReloadData(CurData.curEquip);
        }
        else
        {
            ReloadData(CurData.curMonster);
        }
        RefreshInfo();
    }

    public void RefreshInfo()
    {
        SetSelect(CurData.IsSelect);
    }

    void ReloadData(EquipData data)
    {
        equipUI.SetActive(true);
        monsterUI.SetActive(false);

        EquipData curEquip = CurData.curEquip;
        ItemStaticData staticData = StaticDataMgr.Instance.GetItemData(curEquip.equipId);

        if (equipIcon==null)
        {
            equipIcon = ItemIcon.CreateItemIcon(curEquip, true, true);
            UIUtil.SetParentReset(equipIcon.transform, transEquipIcon);
        }
        else
        {
            equipIcon.RefreshWithEquipInfo(CurData.curEquip,true,true);
        }
        UIUtil.SetStageColor(equipName, staticData.name, curEquip.stage);
        //TODO:设置战力
        //textZhanli.text=StaticDataMgr.Instance.GetTextByID(itemInfo.z)
        textDengji.text = staticData.minLevel.ToString();
        RefreshGem(curEquip.gemList);
    }
    void ReloadData(GameUnit data)
    {
        monsterUI.SetActive(true);
        equipUI.SetActive(false);

        GameUnit curMonster = CurData.curMonster;
        UIUtil.SetStageColor(monsterName, curMonster);

        proIcon.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>("property_" + curMonster.property) as Sprite;

        if (monsterIcon==null)
        {
            monsterIcon = MonsterIcon.CreateIcon();
            UIUtil.SetParentReset(monsterIcon.transform, transMonsterIcon);
        }
        else
        {
            monsterIcon.Init();
        }
        monsterIcon.SetId(curMonster.pbUnit.guid.ToString());
        monsterIcon.SetMonsterStaticId(curMonster.pbUnit.id);
        monsterIcon.SetStage(curMonster.pbUnit.stage);
        monsterIcon.SetLevel(curMonster.pbUnit.level);
        monsterIcon.iconButton.gameObject.SetActive(false);

    }

    void SetSelect(bool select)
    {
        if (sprNormal == null)
            sprNormal = ResourceMgr.Instance.LoadAssetType<Sprite>("beibao_duigou");
        if (sprSelect == null)
            sprSelect = ResourceMgr.Instance.LoadAssetType<Sprite>("beibao_duigou_1");

        imgSelect.sprite = select ? sprSelect : sprNormal;
    }

    void OnClickItem(GameObject go)
    {
        if (onClickBack != null)
        {
            onClickBack(this);
        }
    }

}
