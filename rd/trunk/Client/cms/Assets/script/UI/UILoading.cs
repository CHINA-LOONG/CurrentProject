using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILoading : UIBase
{
	public static string ViewName = "loading";
    public Image loadingProgress;
	
    public void SetProgress(float ratio)
    {
        loadingProgress.fillAmount = ratio;
    }
}
