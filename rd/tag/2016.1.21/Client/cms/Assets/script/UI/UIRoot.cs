using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIRoot : MonoBehaviour {
    
    public Button gmButton;
    [HideInInspector]
    public UIGM uiGM;
    static string chineseTxt = null;
    public UnityEngine.Font baseFont;
    //------------------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start () {
        ScrollViewEventListener.Get(gmButton.gameObject).onClick = OnGMButtonClick;
        FixBrokenWord();
	}
    //------------------------------------------------------------------------------------------------------
	// Update is called once per frame
	void Update () {
	
	}
    //------------------------------------------------------------------------------------------------------
    void OnGMButtonClick(GameObject go)
    {
        uiGM = UIMgr.Instance.OpenUI_(UIGM.ViewName) as UIGM;
    }
    //------------------------------------------------------------------------------------------------------
    public void FixBrokenWord()
    {
        if (chineseTxt == null)
        {
            TextAsset txt = Resources.Load("commonText") as TextAsset;
            chineseTxt = txt.ToString();
        }

        baseFont.RequestCharactersInTexture(chineseTxt);
        //Texture texture = baseFont.material.mainTexture;    // Font的内部纹理
    }
    //------------------------------------------------------------------------------------------------------
}
