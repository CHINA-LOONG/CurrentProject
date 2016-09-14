using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class WebScript : MonoBehaviour {

	public Transform mWebParent;
    public GameObject mLogServer;
   #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
	private UniWebView mWebView;
    //--------------------------------
    void Start()
    {
        mWebView = mWebParent.GetComponent<UniWebView>();
        if (mWebView == null)
        {
            mWebView = mWebParent.gameObject.AddComponent<UniWebView>();
            mWebView.OnLoadComplete += OnLoadComplete;
            //int bottomInset = (int)(UniWebViewHelper.screenHeight * 0.5f);
            mWebView.insets = new UniWebViewEdgeInsets(1, 1, 40, 1);
            mWebView.url = "https://www.baidu.com";
            mWebView.Load();
            //_webView.Show();
        }
        else
        {
            mWebView.Show();
        }
    }
	//--------------------------------
	void OnLoadComplete(UniWebView webView, bool success, string errorMessage) {
		if (success) {
			webView.Show();
		} else {
			Logger.Log("error: " + errorMessage);
		}
	}
#endif
    //--------------------------------
    public void OpenServer()
    {
         #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
        Destroy(mWebView);
        #endif
        gameObject.SetActive(false);
        mLogServer.SetActive(true);
    }
}