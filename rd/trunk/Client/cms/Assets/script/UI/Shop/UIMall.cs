using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIMall : UIBase 
{
	public static string ViewName = "UIMall";

	public	Text	mallNameText;
	public	Button	closeButton;
	public	ScrollView	itemScrollView;
	// Use this for initialization
	void Start () 
	{
		EventTriggerListener.Get (closeButton.gameObject).onClick = OnCloseButtonClick;
	}
	
	#region ------------ override method---------------
	public override void Init()
	{
		RefreshMallItems ();
	}
	
	public override void Clean()
	{
	}

	void	RefreshMallItems()
	{
		itemScrollView.ClearAllElement ();
		Dictionary<string,RechargeStaticData> itemsDic = StaticDataMgr.Instance.GetAllRechageStaticData ();

		foreach (var subItem in itemsDic)
		{
			var subRechage = subItem.Value;
			var subGo = MallItem.Create(subRechage);
			itemScrollView.AddElement (subGo.gameObject);
		}
	}

	void OnCloseButtonClick(GameObject go)
	{
		UIMgr.Instance.CloseUI_ (this);
	}
	#endregion
}
