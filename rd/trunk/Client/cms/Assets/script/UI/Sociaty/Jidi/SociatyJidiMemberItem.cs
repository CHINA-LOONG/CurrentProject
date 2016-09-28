using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SociatyJidiMemberItem : MonoBehaviour
{
    public Text nameText;
    public Text bpText;
    public RectTransform iconParent;
    public Button itemButton;

    private MonsterIcon itemIcon = null;
    private PB.AllianceBaseMonster baseMonster = null;

    public static SociatyJidiMemberItem CreateWith(PB.AllianceBaseMonster baseMonster)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("SociatyJidiMemberItem");
        SociatyJidiMemberItem memberItem = go.GetComponent<SociatyJidiMemberItem>();
        memberItem.RefreshWith(baseMonster);
        return memberItem;
    }

    // Use this for initialization
    public void RefreshWith(PB.AllianceBaseMonster baseMonster)
    {
        this.baseMonster = baseMonster;
        nameText.text = baseMonster.nickname;
        bpText.text = string.Format(StaticDataMgr.Instance.GetTextByID("equip_forge_zhanli"), baseMonster.bp);
        itemIcon = MonsterIcon.CreateIcon();
        itemIcon.SetMonsterStaticId(baseMonster.cfgId);
        itemIcon.SetId(baseMonster.id.ToString());
        itemIcon.SetLevel(baseMonster.level);
        itemIcon.SetStage(baseMonster.stage);

        RectTransform itemRt = itemIcon.transform as RectTransform;
        float scale = iconParent.rect.width / itemRt.rect.width;
        itemIcon.transform.SetParent(iconParent);
        itemIcon.transform.localScale = new Vector3(scale, scale, scale);
        itemIcon.transform.localPosition = Vector3.zero;

        itemButton.onClick.AddListener(OnMonsterIconClicked);
    }
    void OnMonsterIconClicked()
    {
        UIMonsterInfo.Open(-1, baseMonster.cfgId, baseMonster.level, baseMonster.stage);
    }
    
    public void Clear()
    {
        if(null != itemIcon)
        {
            ResourceMgr.Instance.DestroyAsset(itemIcon.gameObject);
        }
    }
}
