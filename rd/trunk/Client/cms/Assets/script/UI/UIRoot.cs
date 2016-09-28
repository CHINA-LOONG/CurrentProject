using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIRoot : MonoBehaviour {
    
    public Button gmButton;
    [HideInInspector]
    public UIGM uiGM;
    //------------------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start () {
        ScrollViewEventListener.Get(gmButton.gameObject).onClick = OnGMButtonClick;
	}
    //------------------------------------------------------------------------------------------------------
    void OnGMButtonClick(GameObject go)
    {
        uiGM = UIMgr.Instance.OpenUI_(UIGM.ViewName) as UIGM;
    }
    //------------------------------------------------------------------------------------------------------
}
