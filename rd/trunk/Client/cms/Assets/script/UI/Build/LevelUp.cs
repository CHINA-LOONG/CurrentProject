using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelUp : UIBase
{
    public static string ViewName = "LevelUp";

    public Text levelDesc;
    public GameObject openTaskPanel;
    public Text openTask;
    public ScrollView functionScrollView;
    public Button conformButton;
    void Awake()
    {
        openTask.text = StaticDataMgr.Instance.GetTextByID("main_levelup_open");

        EventTriggerListener.Get(conformButton.gameObject).onClick = OnConformButtonClicked;
        UIUtil.SetButtonTitle(conformButton.transform, StaticDataMgr.Instance.GetTextByID("ui_queding"));
    }

    public static void OpenWith(int oldLevel, int targetLevel)
    {
        LevelUp lup = (LevelUp)UIMgr.Instance.OpenUI_(ViewName);
        lup.InitWith(oldLevel, targetLevel);
    }

    public  void    InitWith(int oldLevel, int targetLevel)
    {
        PlayerLevelAttr oldLevelAttr = StaticDataMgr.Instance.GetPlayerLevelAttr(oldLevel);
        PlayerLevelAttr newLevelAttr = StaticDataMgr.Instance.GetPlayerLevelAttr(targetLevel);

        levelDesc.text = string.Format(StaticDataMgr.Instance.GetTextByID("main_levelup_dengji"), targetLevel);

        bool isNewOpen = InitOpenFunctions(oldLevel,targetLevel);
        openTaskPanel.SetActive(isNewOpen);
        if (isNewOpen)
        {
            levelDesc.alignment = TextAnchor.UpperCenter;
        }
        else
        {
            levelDesc.alignment = TextAnchor.LowerCenter;
        }
    }

    bool InitOpenFunctions(int oldLevel,int targetLevel)
    {
        functionScrollView.ClearAllElement();
        List<FunctionData> listfunction = StaticDataMgr.Instance.GetNewOpenFunction(oldLevel, targetLevel);
        if (null == listfunction || listfunction.Count == 0)
        {
            return false;
        }
        for (int i = 0; i < listfunction.Count; ++i)
        {
            var subFunction = listfunction[i];
            FunctionItem subItem = FunctionItem.CreateWith(subFunction);
            functionScrollView.AddElement(subItem.gameObject);
        }
        return true;
    }
    void    OnConformButtonClicked(GameObject go)
    {
        RequestCloseUi();
    }
}
