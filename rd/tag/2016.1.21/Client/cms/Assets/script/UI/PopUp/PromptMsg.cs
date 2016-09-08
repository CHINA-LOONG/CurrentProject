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

		public static string ViewName = "PromptMsg";//单行
        public static string ViewName2 = "PromptMsg2";//双行

		public static PromptMsg Open(MsgBoxType msgType, string msg, PrompDelegate callback = null, bool autoClose = true, bool useTextmesh = false)
		{
            PromptMsg mInfo = UIMgr.Instance.OpenUI_(PromptMsg.ViewName,false) as PromptMsg;
            mInfo.SetData(msgType,null, msg, callback, autoClose, useTextmesh);
            return mInfo;
		}
        public static PromptMsg Open(MsgBoxType msgType, string title, string msg, PrompDelegate callback = null, bool autoClose = true, bool useTextmesh = false)
        {
            PromptMsg mInfo = UIMgr.Instance.OpenUI_(PromptMsg.ViewName2, false) as PromptMsg;
            mInfo.SetData(msgType,title, msg, callback, autoClose, useTextmesh);
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

        public TextMesh textWithImg;

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
			isFirst = false;
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

		public	void SetData(MsgBoxType msgType,string title, string msg, PrompDelegate callback, bool autoClose, bool useTextmesh)
		{
			conformPanel.gameObject.SetActive (msgType == MsgBoxType.Conform);
			conformCancelPanel.gameObject.SetActive (msgType == MsgBoxType.Conform_Cancel);

            this.buttonClick = callback;

            titleText.gameObject.SetActive(!string.IsNullOrEmpty(title));
            if (!string.IsNullOrEmpty(title))
            {
                titleText.text = title;
                titleText.alignment = (titleText.preferredWidth > ((titleText.transform as RectTransform).rect.width) ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter);
            }
            if (useTextmesh)
            {
                msgText.text = string.Empty;
                textWithImg.text = msg;
            }
            else
            {
                TextAnchor curAlignment = TextAnchor.MiddleCenter;
                RectTransform textRt = msgText.transform as RectTransform;
               
                msgText.text = msg;
				if (msgText.preferredWidth > textRt.rect.width)
				{
					curAlignment = TextAnchor.MiddleLeft;
				}
				msgText.alignment = curAlignment;
				textWithImg.text = string.Empty;
            }
			
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
