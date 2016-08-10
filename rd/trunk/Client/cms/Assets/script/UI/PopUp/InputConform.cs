using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MsgBox
{
    public class InputConform : UIBase
    {
        public static string ViewName = "InputConform";
        public delegate void ModifyDelegate(PrompButtonClick state, string modifyMsg);

        public Text titleText;
        public InputField inputField;
        public Button conformbutton;
        public Button cancelButton;

        private bool isAutoClose = false;
        private ModifyDelegate callBack = null;

        private static InputConform instance = null;
        public  static  void    OpenWith(string title,string defalutContent,bool isAutoClose,ModifyDelegate callBack)
        {
            InputConform mInfo = UIMgr.Instance.OpenUI_(ViewName, false) as InputConform;
            mInfo.InitWith(title, defalutContent, isAutoClose, callBack);
            instance = mInfo;
        }

        public static   void    Close()
        {
            if(null != instance)
            {
                UIMgr.Instance.CloseUI_(instance);
                instance = null;
            } 
        }
        // Use this for initialization
        void Start()
        {
            cancelButton.onClick.AddListener(OnCancelClick);
            conformbutton.onClick.AddListener(OnConformClick);

            UIUtil.SetButtonTitle(cancelButton.transform, StaticDataMgr.Instance.GetTextByID("ui_quxiao"));
            UIUtil.SetButtonTitle(conformbutton.transform, StaticDataMgr.Instance.GetTextByID("ui_queding"));
        }

        public  void InitWith(string title, string defalutContent, bool isAutoClose, ModifyDelegate callBack)
        {
            titleText.text = title;
            inputField.text = defalutContent;
            this.isAutoClose = isAutoClose;
            this.callBack = callBack;
        }

        void OnCancelClick()
        {
            UIMgr.Instance.CloseUI_(this);
        }

        void OnConformClick()
        {
            if (isAutoClose)
            {
                UIMgr.Instance.CloseUI_(this);
            }
            if (callBack != null)
            {
                callBack(PrompButtonClick.OK, inputField.text);
            }
        }
    }
}
