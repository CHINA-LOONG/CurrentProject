using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMonsterIconExp : MonoBehaviour {

    public UIProgressbar mExpBar;
    public Image mLvlUpUI;
    public Text mExpGainUI;

    private RectTransform mMonsterIconRoot;

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
        mMonsterIconRoot = transform as RectTransform;
    }
    //---------------------------------------------------------------------------------------------
    public void SetMonsterIconExpInfo(
        int guid,
        int expGain,
        int lvlOriginal,
        int lvlTarget
        )
    {
        BattleObject bo = ObjectDataMgr.Instance.GetBattleObject(guid);
        if (bo == null)
        {
            Logger.LogErrorFormat("unit not find id = {0}", guid);
            return;
        }

        MonsterIcon icon = MonsterIcon.CreateIcon();
        icon.transform.SetParent(mMonsterIconRoot.transform, false);
        icon.SetMonsterStaticId(bo.unit.pbUnit.id);
        icon.SetLevel(lvlTarget);
        //icon.SetStage(1);

        mExpBar.SetLoopCount(lvlTarget - lvlOriginal);
        //test only exp = lvl * 100
        mExpBar.SetCurrrentRatio(bo.unit.currentExp / lvlOriginal * 100);
        int expRemain = expGain + bo.unit.currentExp;
        for (int i = lvlOriginal; i <= lvlTarget; ++i)
        {
            expRemain -= i * 100;
        }
        mExpBar.SetTargetRatio(expRemain / lvlTarget * 100);
        bo.unit.currentExp = expRemain;
        bo.unit.LevelUp(lvlTarget);
    }
    //---------------------------------------------------------------------------------------------
}
