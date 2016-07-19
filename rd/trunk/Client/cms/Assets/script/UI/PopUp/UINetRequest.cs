using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UINetRequest : UIBase
{

	public static string ViewName = "UINetRequest";

	public Button closeButton;

	public static void Open()
	{
		UIMgr.Instance.OpenUI_(UINetRequest.ViewName);
	}

	public static void Close()
	{
		UIMgr.Instance.CloseUI_(UINetRequest.ViewName);
	}

    public override void Init()
    {
        //前置
        
    }

	// Use this for initialization
	void Start () 
	{
		EventTriggerListener.Get (closeButton.gameObject).onClick = OnCloseButtonCliced;
	}
	
	void OnCloseButtonCliced(GameObject go)
	{
		UINetRequest.Close ();
	}
}
