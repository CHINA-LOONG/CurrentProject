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

	public	void SetStar(int star)
	{
        //modify: xuelong 先把显示几星关闭掉---功能需要打开    2015-9-1 10:24:00
        //TODO： 开启多语言模式
        //if (star > 0)
        //    starText.text = star.ToString() + "星星";
        //else
        //    starText.text = "未开打";

        starText.text = "";
	}

	public	void SetName(string name)
	{
		nameText.text = name;
	}

}
