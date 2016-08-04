using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelUp : UIBase
{
    public static string ViewName = "LevelUp";

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
       // title.text = StaticDataMgr.Instance.GetTextByID("main_levelup_title");
        curPilaoDesc.text = StaticDataMgr.Instance.GetTextByID("main_levelup_pilao");
        maxPilaoDesc.text = StaticDataMgr.Instance.GetTextByID("main_levelup_shangxian");

        PlayerLevelAttr oldLevelAttr = StaticDataMgr.Instance.GetPlayerLevelAttr(oldLevel);
        PlayerLevelAttr newLevelAttr = StaticDataMgr.Instance.GetPlayerLevelAttr(targetLevel);
       
        curPilaoValue.text = string.Format("{0}-{1}", oldPilao, newPilao);
        maxPilaoValue.text = string.Format("{0}-{1}", oldLevelAttr.fatigue, newLevelAttr.fatigue);

        levelDesc.text = string.Format(StaticDataMgr.Instance.GetTextByID("main_levelup_dengji"), targetLevel);

        int nextLevel = oldLevel + 1;
        if(targetLevel < nextLevel)
        {
            nextLevel = targetLevel;
        }
        InitFunctions(nextLevel);

        EventTriggerListener.Get(conformButton.gameObject).onClick = OnConformButtonClicked;
        UIUtil.SetButtonTitle(conformButton.transform, StaticDataMgr.Instance.GetTextByID("ui_queding"));
    }

    void InitFunctions(int targetLevel)
    {
        functionScrollView.ClearAllElement();
        List<FunctionData> listfunction = StaticDataMgr.Instance.GetFunctionNoSmallerLevel(targetLevel);
        if (null == listfunction || listfunction.Count ==0)
        {
            return;
        }

        int noOpenCount = 0;
        for(int i =0;i<listfunction.Count;++i)
        {
            var subFunction = listfunction[i];
            int playerLevel = GameDataMgr.Instance.PlayerDataAttr.LevelAttr;
            if (playerLevel < subFunction.needlevel)
            {
                noOpenCount++;
                if (noOpenCount > 2)
                {
                    break;
                }
            }
            FunctionItem subItem = FunctionItem.CreateWith(subFunction);
            functionScrollView.AddElement(subItem.gameObject);
        }
        
    }

    void    OnConformButtonClicked(GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }
	
}
