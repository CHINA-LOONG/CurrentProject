using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SelectPet : UIBase
{
    public delegate void SelectPetDelegate(int guid);
    public static string ViewName = "SelectPet";

    public Text titleText;
    public Text maoxianText;
    public Text zhushouText;
    public ScrollView contentView;
    public Button closeButton;
    public Image bgMask;
    private SelectPetDelegate callBack;

    public static SelectPet Instance = null;
    private List<GameUnit> mListUnit = new List<GameUnit>();
    public static void Open(SelectPetDelegate callBack)
    {
        Close();
        Instance = (SelectPet)UIMgr.Instance.OpenUI_(ViewName);
        Instance.callBack = callBack;
    }
    public static void Close()
    {
        if(null !=Instance)
        {
            UIMgr.Instance.CloseUI_(Instance);
            Instance = null;
        }
    }
    public override void Init(bool forbidGuide = false)
    {
        contentView.ClearAllElement();
        RefreshUI();
        base.Init(forbidGuide);
    }
    //删除界面，对子对象的清理操作
    public override void Clean()
    {
        contentView.ClearAllElement();
    }

    // Use this for initialization
    void Start ()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        EventTriggerListener.Get(bgMask.gameObject).onClick = OnBgMaskClick;
        titleText.text = StaticDataMgr.Instance.GetTextByID("sociaty_paibingxuanze");
        zhushouText.text = StaticDataMgr.Instance.GetTextByID("advanture_zhushouing");
        maoxianText.text = StaticDataMgr.Instance.GetTextByID("advanture_maoxianing");
	}
    public void SetCallBack(SelectPetDelegate callBack)
    {
        this.callBack = callBack;
    }

    void OnCloseButtonClick()
    {
        RequestCloseUi();
    }

    void OnBgMaskClick (GameObject go)
    {
        OnCloseButtonClick();
    }

    void RefreshUI()
    {
        GameDataMgr.Instance.PlayerDataAttr.GetAllPet(ref mListUnit);
        mListUnit.Sort();
        GameUnit subUnit = null;
        int count = mListUnit.Count;
        for (int i = 0; i < count; ++i)
        {
            subUnit = mListUnit[i];
            string monsterId = subUnit.pbUnit.id;

            MonsterIcon icon = MonsterIcon.CreateIcon();
            ScrollViewEventListener.Get(icon.iconButton.gameObject).onClick = OnMonsterClicked;
            contentView.AddElement(icon.gameObject);
            icon.SetMonsterStaticId(monsterId);
            icon.SetId(subUnit.pbUnit.guid.ToString());
            icon.SetLevel(subUnit.pbUnit.level);
            icon.SetStage(subUnit.pbUnit.stage);
            icon.ShowZhushouImage(subUnit.pbUnit.IsInAllianceBase());
            icon.ShowMaoxianImage(subUnit.pbUnit.IsInAdventure());
            icon.ShowLockImage(subUnit.pbUnit.IsInAllianceBase() || subUnit.pbUnit.IsInAdventure());
            icon.ShowMaskImage(subUnit.pbUnit.IsInAllianceBase() || subUnit.pbUnit.IsInAdventure());
        }
    }

    void OnMonsterClicked(GameObject go)
    {
        MonsterIcon micon = go.GetComponentInParent<MonsterIcon>();
        GameUnit gameUnit = GameDataMgr.Instance.PlayerDataAttr.GetPetWithKey(int.Parse(micon.Id));
        if(null!=gameUnit)
        {
            if(gameUnit.pbUnit.IsInAdventure())
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("arrayselect_count_005"), (int)PB.ImType.PROMPT);
                return;
            }
            else if (gameUnit.pbUnit.IsInAllianceBase())
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("adventure_record_009"), (int)PB.ImType.PROMPT);
                return;
            }
        }
        if (callBack != null)
        {
            callBack(int.Parse(micon.Id));
        }
    }

}
