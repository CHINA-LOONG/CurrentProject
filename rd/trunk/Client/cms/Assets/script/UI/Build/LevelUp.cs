using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelUp : UIBase
{
    public static string ViewName = "LevelUp";

    public Text title;
    public Text levelDesc;
    public Text curPilaoDesc;
    public Text curPilaoValue;
    public Text maxPilaoDesc;
    public Text maxPilaoValue;
    public ScrollView functionScrollView;
    public Button conformButton;


    public static void OpenWith(int oldLevel, int targetLevel,int oldPilao,int newPilao)
    {
        LevelUp lup = (LevelUp)UIMgr.Instance.OpenUI_(ViewName);
        lup.InitWith(oldLevel, targetLevel,oldPilao,newPilao);
    }

    public  void    InitWith(int oldLevel, int targetLevel, int oldPilao, int newPilao)
    {
        title.text = StaticDataMgr.Instance.GetTextByID("main_levelup_title");
        curPilaoDesc.text = StaticDataMgr.Instance.GetTextByID("main_levelup_dengji");
        maxPilaoDesc.text = StaticDataMgr.Instance.GetTextByID("main_levelup_shangxian");

        curPilaoValue.text = string.Format(StaticDataMgr.Instance.GetTextByID("main_levelup_pilao"), oldPilao, newPilao);

        levelDesc.text = string.Format(StaticDataMgr.Instance.GetTextByID("main_levelup_dengji"), targetLevel);

        InitFunctions(targetLevel);

        EventTriggerListener.Get(conformButton.gameObject).onClick = OnConformButtonClicked;
    }

    void InitFunctions(int targetLevel)
    {
        functionScrollView.ClearAllElement();
        List<FunctionData> listfunction = StaticDataMgr.Instance.GetFunctionNoSmallerLevel(targetLevel);
        if (null == listfunction || listfunction.Count ==0)
        {
            return;
        }

        for(int i =0;i<listfunction.Count;++i)
        {
            FunctionItem subItem = FunctionItem.CreateWith(listfunction[i]);
            functionScrollView.AddElement(subItem.gameObject);
        }
        
    }

    void    OnConformButtonClicked(GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }
	
}
