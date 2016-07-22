using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILoading : UIBase
{
	public static string ViewName = "loading";
    public Image loadingProgress;
    public Text loadingText;

    private bool beginLoading = false;
    private int totalAssetCount = 0;

    //---------------------------------------------------------------------------------------------
    void Update()
    {
        if (beginLoading == true)
        {
            int remainCount = ResourceMgr.Instance.GetAssetRequestCount();
            if (totalAssetCount == 0)
            {
                loadingProgress.fillAmount = 1.0f;
            }
            else
            {
                loadingProgress.fillAmount = 1.0f - (float)remainCount / totalAssetCount;
            }
            loadingText.text = (totalAssetCount - remainCount).ToString() + "/" + totalAssetCount.ToString();
        }
    }
    //---------------------------------------------------------------------------------------------
    public void UpdateTotalAssetCount()
    {
        beginLoading = true;
        loadingProgress.fillAmount = 0.0f;
        totalAssetCount = ResourceMgr.Instance.GetAssetRequestCount();
    }
    //---------------------------------------------------------------------------------------------
    public void SetProgress(float ratio)
    {
        loadingProgress.fillAmount = ratio;
    }
    //---------------------------------------------------------------------------------------------
    void OnEnable()
    {
        beginLoading = false;
        loadingProgress.fillAmount = 0.0f;
        loadingText.text = string.Empty;
    }
    //---------------------------------------------------------------------------------------------
}
