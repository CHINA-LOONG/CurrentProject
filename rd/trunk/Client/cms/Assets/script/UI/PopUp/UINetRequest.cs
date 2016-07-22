using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UINetRequest : UIBase
{

	public static string ViewName = "UINetRequest";
    public static UINetRequest Inst = null;

	public Button closeButton;

	public static void Open()
	{
        if (Inst != null)
        {
            Inst.gameObject.SetActive(true);
        }
        else
        {
            Inst = UIMgr.Instance.OpenUI_(UINetRequest.ViewName) as UINetRequest;
        }
	}

	public static void Close()
	{
        if (Inst != null)
        {
            Inst.gameObject.SetActive(false);
        }
		//UIMgr.Instance.CloseUI_(UINetRequest.ViewName);
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
