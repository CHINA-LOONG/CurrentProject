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
    private SelectPetDelegate callBack;

    public static SelectPet Instance = null;
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
    public override void Init()
    {
        contentView.ClearAllElement();
        RefreshUI();
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
	}
    public void SetCallBack(SelectPetDelegate callBack)
    {
        this.callBack = callBack;
    }

    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }
	
    void RefreshUI()
    {
        List<GameUnit> listUnit = GameDataMgr.Instance.PlayerDataAttr.GetAllPet();
        listUnit.Sort();
        GameUnit subUnit = null;
        for (int i = 0; i < listUnit.Count; ++i)
        {
            subUnit = listUnit[i];
            string monsterId = subUnit.pbUnit.id;

            MonsterIcon icon = MonsterIcon.CreateIcon();
            EventTriggerListener.Get(icon.iconButton.gameObject).onClick = OnMonsterClicked;
            contentView.AddElement(icon.gameObject);
            icon.SetMonsterStaticId(monsterId);
            icon.SetId(subUnit.pbUnit.guid.ToString());
            icon.SetLevel(subUnit.pbUnit.level);
            icon.SetStage(subUnit.pbUnit.stage);
        }
    }

    void OnMonsterClicked(GameObject go)
    {
        MonsterIcon micon = go.GetComponentInParent<MonsterIcon>();
        if(callBack != null)
        {
            callBack(int.Parse(micon.Id));
        }
    }

}
