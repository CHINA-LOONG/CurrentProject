using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public class InstanceButton : MonoBehaviour 
{
	public	Text 	starText;
	public	Text	nameText;
	public	Button	button;

	[HideInInspector]
	public int index =0;

	[HideInInspector]
	public	string	instanceId;

	// Use this for initialization
	void Start () 
	{
		//EventTriggerListener.Get (button.gameObject).onClick = OnButtonClick;		
	}

	void OnButtonClick(GameObject go)
	{
	}

	public	void SetStar(int star)
	{
		if (star > 0)
			starText.text = star.ToString () + "星星";
		else
			starText.text = "未开打";
	}

	public	void SetName(string name)
	{
		nameText.text = name;
	}

}
