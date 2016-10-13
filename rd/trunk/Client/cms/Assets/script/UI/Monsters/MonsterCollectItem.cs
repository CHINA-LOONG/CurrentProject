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
    public GameObject unMask;
    public GameObject tips;
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

        textName.text = curData.unit.NickNameAttr;
        iconMonster = InitMonsterIcon();
        iconMonster.SetId("0");
        iconMonster.SetMonsterStaticId(curData.unit.id);
        iconMonster.SetStage(0, false);
        iconMonster.SetLevel(0, false);
        iconMonster.ShowXiyouduImage();
        iconMonster.iconButton.gameObject.SetActive(false);

        SetFragmentCount(curData.unit);

        mask.SetActive(!curData.isExist);
        unMask.SetActive(curData.isExist);
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

    void SetComposeTips(bool show)
    {
        tips.SetActive(show);
    }

    void SetFragmentCount(UnitData unit)
    {
        ItemData fragment = GameDataMgr.Instance.PlayerDataAttr.gameItemData.getItem(unit.fragmentId);
        int need = unit.fragmentCount;
        int count = (fragment == null ? 0 : fragment.count);
        textCount.text = string.Format("{0}/{1}", count, need);
        if (count >= need)
        {
            SetComposeTips(true);
            textCount.color = ColorConst.text_color_fullFragment;
        }
        else
        {
            SetComposeTips(false);
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
