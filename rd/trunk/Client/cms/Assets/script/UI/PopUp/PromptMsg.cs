using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MsgBox
{
	public	enum PrompButtonClick
	{
		OK = 0,
		Cancle
	}
	public class PromptMsg : UIBase
	{
		public delegate void PrompDelegate (int state);

		public static string ViewName = "PromptMsg";

		public static	void Open(string title,string msg,string buttonName,PrompDelegate buttonClick = null)
		{
            PromptMsg mInfo = UIMgr.Instance.OpenUI_(PromptMsg.ViewName,false) as PromptMsg;
			mInfo.SetData (title, msg, buttonName);
			mInfo.buttonClick = buttonClick;
		}

		public  Text 	titleText;
		public	Text	msgText;
		public	Button	titleButton;

		public	PrompDelegate  buttonClick = null;
	
		// Use this for initialization
		void Start ()
		{
			EventTriggerListener.Get (titleButton.gameObject).onClick = OnClose;
		}

		public	void SetData(string title,string msg, string buttonName)
		{
			var buttonTitle = titleButton.GetComponentInChildren<Text> ();
			if (null != buttonTitle) 
			{
				buttonTitle.text = buttonName;
			}

			titleText.text = title;
			msgText.text = msg;
		}

		public void OnClose(GameObject go)
		{
			UIMgr.Instance.DestroyUI (this);
			if (buttonClick != null)
			{
				buttonClick((int) PrompButtonClick.OK);
			}
		}
	}

}
