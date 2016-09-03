using UnityEngine;
using UnityEngine.UI;

public interface IOwnedPetItem
{
    void OnClickOwnedPet(GameUnit data);
}
public class MonsterListItem : MonoBehaviour
{
    public Text textName;
    public Image imgPro;
    public Transform posIcon;
    private MonsterIcon iconMonster;
    //public Image imgBadge;

    public Transform[] equipSlot = new Transform[6];
    private ItemIcon[] equipIcon = new ItemIcon[6];
    private GameObject[] equipAdd = new GameObject[6];
    
    public GameUnit curData;

    public IOwnedPetItem iOwnedPetDelegate;

    void Start()
    {
        ScrollViewEventListener.Get(/*btnCollect.*/gameObject).onClick = OnClickItem;
    }


    public void MonsterIconLoadCallback(GameObject icon, System.EventArgs args)
    {
        iconMonster = icon.GetComponent<MonsterIcon>();
        UIUtil.SetParentReset(iconMonster.transform, posIcon);
        //TODO: duplicate code
        iconMonster.SetId(curData.pbUnit.guid.ToString());
        iconMonster.SetMonsterStaticId(curData.pbUnit.id);
        iconMonster.SetStage(curData.pbUnit.stage);
        iconMonster.SetLevel(curData.pbUnit.level);
        iconMonster.iconButton.gameObject.SetActive(false);
    }

    public void ReloadData(GameUnit unit)
    {
        curData = unit;
        UIUtil.SetStageColor(textName, curData);
        imgPro.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>("property_" + unit.property) as Sprite;

        if (iconMonster == null)
        {
            MonsterIcon.CreateIconAsync(MonsterIconLoadCallback);        
            //iconMonster = MonsterIcon.CreateIcon();
            //UIUtil.SetParentReset(iconMonster.transform, posIcon);
        }
        else
        {
            iconMonster.gameObject.SetActive(true);
            iconMonster.Init();
            iconMonster.SetId(curData.pbUnit.guid.ToString());
            iconMonster.SetMonsterStaticId(curData.pbUnit.id);
            iconMonster.SetStage(curData.pbUnit.stage);
            iconMonster.SetLevel(curData.pbUnit.level);
            iconMonster.iconButton.gameObject.SetActive(false);
        }

        UnitData petData = StaticDataMgr.Instance.GetUnitRowData(unit.pbUnit.id);
        for (int i = 0; i < unit.equipList.Length; i++)
        {
            if (unit.equipList[i] == null)
            {
                if (equipIcon[i] != null)
                {
                    equipIcon[i].gameObject.SetActive(false);
                }
                if (equipAdd[i] == null)
                {
                    equipAdd[i] = equipSlot[i].FindChild("ImageAdd").gameObject;
                }
                if (GameDataMgr.Instance.PlayerDataAttr.CheckEquipTypePart(petData.equip, i + 1))
                {
                    equipAdd[i].SetActive(true);
                }
                else
                {
                    equipAdd[i].SetActive(false);
                }
            }
            else
            {
                EquipData curEquip = unit.equipList[i];
                EquipData equipData = EquipData.valueof(curEquip.id, curEquip.equipId, curEquip.stage, curEquip.level, BattleConst.invalidMonsterID, null);
                if (equipIcon[i] == null)
                {
                    equipIcon[i] = ItemIcon.CreateItemIcon(equipData, false);
                    UIUtil.SetParentReset(equipIcon[i].transform, equipSlot[i].FindChild("equipPos"));
                    //equipIcon[i].HideExceptIcon();
                }
                else
                {
                    equipIcon[i].gameObject.SetActive(true);
                    equipIcon[i].RefreshWithEquipInfo(equipData);
                    //equipIcon[i].HideExceptIcon();
                }
            }
        }
    }

    //public void CheckStageStatus()
    //{
    //    imgBadge.gameObject.SetActive(UIUtil.CheckIsEnoughMaterial(curData));
    //}


    void OnClickItem(GameObject go)
    {
        if (null != iOwnedPetDelegate)
        {
            iOwnedPetDelegate.OnClickOwnedPet(curData);
        }
    }
}
