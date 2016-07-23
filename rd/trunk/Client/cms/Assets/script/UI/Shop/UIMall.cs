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
	
	}
	
	#region ------------ override method---------------
	public override void Init()
	{
	}
	
	public override void Clean()
	{
	}
	#endregion
}
