using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILevelInfo : UIBase
{
    public static string ViewName = "LevelInforRoot";
    public Image mLevelBoss;
    public Text mLevelInfoName;
    public Text mLevelInfoIndex;
    public Animator mLevelInfoAnimator;

    //------------------------------------------------------------------------------------------------------  
    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
    //------------------------------------------------------------------------------------------------------  
    public void SetInstanceName(string instanceName)
    {
        //only need to set once
        mLevelInfoName.text = instanceName;
    }
    //------------------------------------------------------------------------------------------------------  
    public void SetBattleLevelProcess(int curIndex, int maxIndex)
    {
        gameObject.SetActive(true);
        mLevelInfoAnimator.Play("levelinfo");
        mLevelInfoIndex.text = curIndex.ToString() + "/" + maxIndex.ToString();
        mLevelBoss.gameObject.SetActive(curIndex == maxIndex);
    }
    //------------------------------------------------------------------------------------------------------  
}
