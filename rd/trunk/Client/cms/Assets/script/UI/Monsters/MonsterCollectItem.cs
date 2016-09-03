using UnityEngine;
using UnityEngine.UI;


public interface ICollectItem
{
    void OnClickCollectPet(CollectUnit data);
}
public class MonsterCollectItem : MonoBehaviour
{
    public Text textName;
    public Text textCount;
    public Transform posIcon;
    private MonsterIcon iconMonster;
    public GameObject mask;
    //public Button btnCollect;

    public CollectUnit curData;
    public ICollectItem iCollectDelegate;
    void Start()
    {
        ScrollViewEventListener.Get(/*btnCollect.*/gameObject).onClick = OnClickItem;
    }

    public void ReloadData(CollectUnit unit)
    {
        curData = unit;

        textName.text = StaticDataMgr.Instance.GetTextByID(curData.unit.nickName);
        iconMonster = InitMonsterIcon();
        iconMonster.SetId("");
        iconMonster.SetMonsterStaticId(curData.unit.id);
        iconMonster.SetStage(0);
        iconMonster.SetLevel(0,false);
        iconMonster.iconButton.gameObject.SetActive(false);

        SetFragmentCount(curData.unit);

        mask.gameObject.SetActive(!curData.isExist);
    }

    MonsterIcon InitMonsterIcon()
    {
        if (iconMonster == null)
        {
            iconMonster = MonsterIcon.CreateIcon();
            UIUtil.SetParentReset(iconMonster.transform, posIcon);
        }
        else
        {
            iconMonster.gameObject.SetActive(true);
            iconMonster.Init();
        }
        return iconMonster;
    }

    void SetFragmentCount(UnitData unit)
    {
        ItemData fragment = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(unit.fragmentId);
        int need = unit.fragmentCount;
        int count = (fragment == null ? 0 : fragment.count);
        textCount.text = string.Format("{0}/{1}", count, need);
        if (count>=need)
        {
            textCount.color = ColorConst.text_color_fullFragment;
        }
        else
        {
            textCount.color = ColorConst.system_color_white;
        }
    }

    void OnClickItem(GameObject go)
    {
        if (null!=iCollectDelegate)
        {
            iCollectDelegate.OnClickCollectPet(curData);
        }
    }

}
