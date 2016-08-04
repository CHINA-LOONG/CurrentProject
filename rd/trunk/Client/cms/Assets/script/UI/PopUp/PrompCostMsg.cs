using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MsgBox
{
    public class PrompCostMsg : UIBase
    {
        public static string ViewName = "PrompCostMsg";
        public Text titleText;
        public Text costText;
        public Text opthionDesc;

        public Button cancelButton;
        public Button conformButton;

        private PromptMsg.PrompDelegate callBack;
        private bool autoClose; 


        public static PrompCostMsg Open(int cost,string title, string opthionMsg, PromptMsg.PrompDelegate callback = null, bool autoClose = true)
        {
            PrompCostMsg mInfo = UIMgr.Instance.OpenUI_(ViewName, false) as PrompCostMsg;
            mInfo.SetData(cost,title, opthionMsg, callback,autoClose);
            return mInfo;
        }

        public void Close()
        {
            UIMgr.Instance.DestroyUI(this);
        }

        void    Start()
        {
            cancelButton.onClick.AddListener(OnCancelButtonClick);
            conformButton.onClick.AddListener(OnConformButtonClick);
        }

        public  void SetData(int cost, string title, string opthionMsg, PromptMsg.PrompDelegate callback = null, bool autoClose = true)
        {
            this.callBack = callback;
            this.autoClose = autoClose;

            costText.text = cost.ToString();
            titleText.text = title;
            if(string.IsNullOrEmpty(opthionMsg))
            {
                opthionDesc.text = "";
            }
            else
            {
                opthionDesc.text = opthionMsg;
            }
        }

        void OnCancelButtonClick()
        {
            if (autoClose)
            {
                UIMgr.Instance.DestroyUI(this);
            }
            if (callBack != null)
            {
                callBack(PrompButtonClick.Cancle);
            }
        }

        void OnConformButtonClick()
        {
            if (autoClose)
            {
                UIMgr.Instance.DestroyUI(this);
            }
            if (callBack != null)
            {
                callBack(PrompButtonClick.OK);
            }
        }
       
    }
}
