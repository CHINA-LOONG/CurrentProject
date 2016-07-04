using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UINetRequest : UIBase
{

	public static string ViewName = "UINetRequest";
	public static string AssertName = "ui/netRequest";

	public Button closeButton;

	public static void Open()
	{
		UIMgr.Instance.OpenUI (UINetRequest.AssertName, UINetRequest.ViewName);
	}

	public static void Close()
	{
		UIMgr.Instance.CloseUI (UINetRequest.ViewName);
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
