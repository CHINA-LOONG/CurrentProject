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
        string unitStaticID,
        int expCurrent,
        int expTarget,
        int lvlOriginal,
        int lvlTarget,
        int stage,
        int expGain = 0
        )
    {
        MonsterIcon icon = MonsterIcon.CreateIcon();
        icon.transform.SetParent(mMonsterIconRoot.transform, false);
        icon.SetMonsterStaticId(unitStaticID);
        icon.SetLevel(lvlTarget);
        icon.SetStage(stage);

        mExpBar.SetLoopCount(lvlTarget - lvlOriginal);
        UnitData curUnitData = StaticDataMgr.Instance.GetUnitRowData(unitStaticID);
        int originalMaxExp = (int)(StaticDataMgr.Instance.GetUnitBaseRowData(lvlOriginal).experience * curUnitData.levelUpExpRate);
        int targetMaxExp = (int)(StaticDataMgr.Instance.GetUnitBaseRowData(lvlTarget).experience * curUnitData.levelUpExpRate);
        mExpBar.SetCurrrentRatio(expCurrent / (float)originalMaxExp);
        mExpBar.SetTargetRatio(expTarget / (float)targetMaxExp);

        mLvlUpUI.gameObject.SetActive(lvlTarget > lvlOriginal);
        if (UIUtil.CheckPetIsMaxLevel(lvlTarget) == true)
        {
            mExpGainUI.text = "MAX LVL";
            mExpBar.SetTargetRatio(0.0f);
        }
        else
        {
            mExpGainUI.text = "+" + expGain.ToString();
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetExpGain(string expGain)
    {
        mExpGainUI.text = expGain;
    }
    //---------------------------------------------------------------------------------------------
    public void SkipAnimation()
    {
        mExpBar.SkipAnimation();
    }
    //---------------------------------------------------------------------------------------------
}
