using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MsgBox
{
	public	enum MsgBoxType
	{
		Conform = 0,
		Conform_Cancel
	}
	public	enum PrompButtonClick
	{
		OK = 0,
		Cancle
	}
	public class PromptMsg : UIBase
	{
		public delegate void PrompDelegate (int state);

		public static string ViewName = "PromptMsg";

		public static	void Open(MsgBoxType msgType, string msg, PrompDelegate buttonClilck = null)
		{
            PromptMsg mInfo = UIMgr.Instance.OpenUI_(PromptMsg.ViewName,false) as PromptMsg;
			mInfo.SetData (msgType, msg, buttonClilck);
		}

		public  Text 	titleText;
		public	Text	msgText;
		public	Transform	conformPanel;
		public	Transform	conformCancelPanel;

		public	Button	conformButton1;

		public	Button	conformButton2;
		public	Button	cancelButton;

		private	PrompDelegate  buttonClick = null;
	
		bool	isFirst = true;
		public override void Init()
		{
			if (isFirst)
			{
				FirstInit();
			}
		}

		public override void Clean()
		{
			
		}

		private	void	FirstInit()
		{
			isFirst = true;
			EventTriggerListener.Get (cancelButton.gameObject).onClick = OnCancelButtonClilck;
			EventTriggerListener.Get (conformButton1.gameObject).onClick = OnConformButtonClick;
			EventTriggerListener.Get (conformButton2.gameObject).onClick = OnConformButtonClick;

			SetButtonName (cancelButton, "取消");
			SetButtonName (conformButton1, StaticDataMgr.Instance.GetTextByID("ui_queding"));
			SetButtonName (conformButton2, StaticDataMgr.Instance.GetTextByID("ui_queding"));

		}

		void SetButtonName(Button btn ,string name)
		{
			var buttonTitle = btn.GetComponentInChildren<Text> ();
			if (null != buttonTitle) 
			{
				buttonTitle.text = name;
			}
		}

		public	void SetData(MsgBoxType msgType, string msg, PrompDelegate buttonClilck)
		{
			conformPanel.gameObject.SetActive (msgType == MsgBoxType.Conform);
			conformCancelPanel.gameObject.SetActive (msgType == MsgBoxType.Conform_Cancel);

			this.buttonClick = buttonClick;
			msgText.text = msg;
		}

		void OnCancelButtonClilck(GameObject go)
		{
			UIMgr.Instance.DestroyUI (this);
			if (buttonClick != null)
			{
				buttonClick((int) PrompButtonClick.Cancle);
			}
		}

		void	OnConformButtonClick(GameObject go)
		{
			UIMgr.Instance.DestroyUI (this);
			if (buttonClick != null)
			{
				buttonClick((int) PrompButtonClick.OK);
			}
		}

	}

}
