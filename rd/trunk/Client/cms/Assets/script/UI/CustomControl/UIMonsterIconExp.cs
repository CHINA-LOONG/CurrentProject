using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMonsterIconExp : MonoBehaviour {

    public UIProgressbar mExpBar;
    public Image mLvlUpUI;
    public Text mExpGainUI;
    public RectTransform mMonsterIconRoot;

    //---------------------------------------------------------------------------------------------
    public static UIMonsterIconExp Create()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("monsterExpIcon");
        UIMonsterIconExp icon = go.GetComponent<UIMonsterIconExp>();
        return icon;
    }
    //---------------------------------------------------------------------------------------------
	void Awake () 
    {
        mLvlUpUI.gameObject.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------
    public void SetMonsterIconExpInfo(
        GameUnit unit,
        int expTarget,
        int expGain,
        int lvlOriginal,
        int lvlTarget
        )
    {
        MonsterIcon icon = MonsterIcon.CreateIcon();
        icon.transform.SetParent(mMonsterIconRoot.transform, false);
        icon.SetMonsterStaticId(unit.pbUnit.id);
        icon.SetLevel(lvlTarget);
        //icon.SetStage(1);

        mExpBar.SetLoopCount(lvlTarget - lvlOriginal);
        UnitData curUnitData = StaticDataMgr.Instance.GetUnitRowData(unit.pbUnit.id);
        int originalMaxExp = (int)(StaticDataMgr.Instance.GetUnitBaseRowData(lvlOriginal).experience * curUnitData.levelUpExpRate);
        int targetMaxExp = (int)(StaticDataMgr.Instance.GetUnitBaseRowData(lvlTarget).experience * curUnitData.levelUpExpRate);
        mExpBar.SetCurrrentRatio(unit.currentExp / (float)originalMaxExp);
        mExpBar.SetTargetRatio(expTarget / (float)targetMaxExp);

        mLvlUpUI.gameObject.SetActive(lvlTarget > lvlOriginal);
        mExpGainUI.text = "+ " + expGain.ToString();
    }
    //---------------------------------------------------------------------------------------------
}
