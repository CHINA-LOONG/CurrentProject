using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PvpOtherDefenseInfo : UIBase
{
    public static string ViewName = "PvpOtherDefenseInfo";

    public GameObject closeGameObj;
    public Text titleText;
    public Text bpLabelText;
    public Text bpValueText;

    public RectTransform[] petParentArray;
    float monsterSclae = -1;
    List<PB.HSMonster> listMonster;
    public static void OpenWith(PB.HSMonsterDefence defenseMonster,int bp)
    {
        PvpOtherDefenseInfo defenseInfo = UIMgr.Instance.OpenUI_(ViewName) as PvpOtherDefenseInfo;
        defenseInfo.InitWith(defenseMonster,bp);
    }

    public void InitWith(PB.HSMonsterDefence defenseMonster,int bp)
    {
        listMonster = defenseMonster.monsterInfo;
        PB.HSMonster subMonster = null;
        for (int i =0;i<listMonster.Count;++i)
        {
            subMonster = listMonster[i];
            MonsterIcon subIcon = MonsterIcon.CreateIcon();
            subIcon.SetMonsterStaticId(subMonster.cfgId);
            subIcon.SetStage(subMonster.stage);
            subIcon.SetLevel(subMonster.level);
            subIcon.SetId(i.ToString());
            EventTriggerListener.Get(subIcon.iconButton.gameObject).onClick = OnItemIconClick;

            subIcon.transform.SetParent(petParentArray[i]);
            subIcon.transform.localPosition = Vector3.zero;
            if(monsterSclae < 0)
            {
                RectTransform iconRt = subIcon.transform as RectTransform;
                monsterSclae = petParentArray[0].rect.width / iconRt.rect.width;
            }
            subIcon.transform.localScale = new Vector3(monsterSclae, monsterSclae, monsterSclae);
        }
        
        bpValueText.text = bp.ToString();
    }

    void OnItemIconClick(GameObject go)
    {
        MonsterIcon itemIcon = go.GetComponentInParent<MonsterIcon>();
        int monsterIndex = int.Parse(itemIcon.Id);
        PB.HSMonster clickMonster = listMonster[monsterIndex];
        if (null != clickMonster)
        {
            UIMonsterInfo.Open(-1, clickMonster.cfgId,clickMonster.level, clickMonster.stage);
        }
    }

	// Use this for initialization
	void Start ()
    {
        titleText.text = StaticDataMgr.Instance.GetTextByID("pvp_defense");
        bpLabelText.text = StaticDataMgr.Instance.GetTextByID("arrayselect_bp_001");
        EventTriggerListener.Get(closeGameObj).onClick = OnCloseImgClick;
	}
    void OnCloseImgClick (GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }
}
