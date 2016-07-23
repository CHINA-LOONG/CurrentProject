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
        public delegate void PrompDelegate(PrompButtonClick state);
        private bool autoClose = true;

		public static string ViewName = "PromptMsg";

		public static PromptMsg Open(MsgBoxType msgType, string msg, PrompDelegate callback = null, bool autoClose = true)
		{
            PromptMsg mInfo = UIMgr.Instance.OpenUI_(PromptMsg.ViewName,false) as PromptMsg;
            mInfo.SetData(msgType, msg, callback, autoClose);
            return mInfo;
		}

        public void Close()
        {
            UIMgr.Instance.DestroyUI(this);
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

			SetButtonName (cancelButton, StaticDataMgr.Instance.GetTextByID("ui_quxiao"));
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

		public	void SetData(MsgBoxType msgType, string msg, PrompDelegate callback, bool autoClose)
		{
			conformPanel.gameObject.SetActive (msgType == MsgBoxType.Conform);
			conformCancelPanel.gameObject.SetActive (msgType == MsgBoxType.Conform_Cancel);

            this.buttonClick = callback;
			msgText.text = msg;
            this.autoClose = autoClose;
		}

		void OnCancelButtonClilck(GameObject go)
		{
            if (autoClose)
            {
                UIMgr.Instance.DestroyUI(this);
            }
			if (buttonClick != null)
			{
				buttonClick(PrompButtonClick.Cancle);
			}
		}

		void OnConformButtonClick(GameObject go)
		{
            if (autoClose)
            {
                UIMgr.Instance.DestroyUI(this);
            }
			if (buttonClick != null)
			{
				buttonClick(PrompButtonClick.OK);
			}
		}

	}

}
