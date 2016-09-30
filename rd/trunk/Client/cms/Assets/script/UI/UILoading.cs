using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILoading : UIBase
{
    public delegate IEnumerator LoadingFinishCallback();
    public static string ViewName = "loading";
    public Image loadingProgress;
    public Text loadingText;
    public LoadingFinishCallback mLoadingCallback = null;
    public Image loadingBackground;
    public Text gamePrompt;
    private bool beginLoading = false;
    private int totalAssetCount = 0;

    //-------------------------------------------------------------------------------------------
    public override void Init()
    {
        mLoadingCallback = null;
    }
    //---------------------------------------------------------------------------------------------
    public void SetLoadingCallback(LoadingFinishCallback callback)
    {
        mLoadingCallback = callback;
    }
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

            if (remainCount == 0)
            {
                beginLoading = false;
                if (mLoadingCallback != null)
                {
                    StartCoroutine(mLoadingCallback());
                }
            }
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
    public void SetLoading(int loadingType = 1000)
    {
        LoadingData loadingData = StaticDataMgr.Instance.GetLoadingData(loadingType);
        int randomNum = Random.Range(0, (loadingData.imageResource.Length - 1));
        loadingBackground.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(loadingData.imageResource[randomNum]);
        randomNum = Random.Range(0, (loadingData.content.Length - 1));
        gamePrompt.text = StaticDataMgr.Instance.GetTextByID(loadingData.content[randomNum]);
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
