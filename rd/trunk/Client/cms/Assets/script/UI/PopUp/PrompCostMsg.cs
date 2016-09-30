using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CostType
{
    ZuanshiCoin =0,
    JinBiCoin
}
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
        public GameObject[] costTypeObjectArray;

        private PromptMsg.PrompDelegate callBack;
        private bool autoClose;

        


        public static PrompCostMsg Open(int cost,string title, string opthionMsg, CostType costType, PromptMsg.PrompDelegate callback = null, bool autoClose = true)
        {
            PrompCostMsg mInfo = UIMgr.Instance.OpenUI_(ViewName, false) as PrompCostMsg;
            mInfo.SetData(cost,title, opthionMsg, costType, callback,autoClose);
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

            UIUtil.SetButtonTitle(cancelButton.transform, StaticDataMgr.Instance.GetTextByID("ui_quxiao"));
            UIUtil.SetButtonTitle(conformButton.transform, StaticDataMgr.Instance.GetTextByID("ui_queding"));
        }

        public  void SetData(int cost, string title, string opthionMsg, CostType costType, PromptMsg.PrompDelegate callback = null, bool autoClose = true)
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
            costTypeObjectArray[0].SetActive(costType == CostType.ZuanshiCoin);
            costTypeObjectArray[1].SetActive(costType == CostType.JinBiCoin);
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
