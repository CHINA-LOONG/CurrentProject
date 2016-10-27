using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GuideUi : UIBase
{
    public static string ViewName = "GuideUI";

    public GameObject exitObject;
    public GameObject focusEffect;
    public Button focusButton;
    public RectTransform focusRt;

    private GuideStep guideStep;
    public static void OpenWith(GuideStep gstep)
    {
        GuideUi guideUi = (GuideUi)UIMgr.Instance.OpenUI_(ViewName);
        guideUi.InitWithStep(gstep);
    }

    void    Start()
    {
        focusButton.onClick.AddListener(OnFocusButtonClick);
        EventTriggerListener.Get(exitObject).onClick = OnExitGuidObjectClick;
    }
	public void InitWithStep(GuideStep gstep)
    {
        name = string.Format("GuideUI:guideStep {0}", gstep.Id);
        focusRt.gameObject.SetActive(false);
        guideStep = gstep;
        StartCoroutine(RefreshUI());
    }
    IEnumerator RefreshUI()
    {
        yield return new WaitForSeconds(0.5f);
        if (!string.IsNullOrEmpty(guideStep.talkId))
        {
            InitGuideWithTalk();
        }
        else if (guideStep.posObject.Contains("3D_"))
        {
            InitGuideWithFocus3DObject();
        }
        else
        {
            InitGuideWithFocus2DObject();
        }
    }
    void  InitGuideWithTalk()
    {
        focusRt.gameObject.SetActive(false);
        UISpeech.Open(guideStep.talkId, OnTalkFinish);
    }
    void InitGuideWithFocus3DObject()
    {
        focusRt.gameObject.SetActive(true);
        if(guideStep.trigerView.Equals(UIBuild.ViewName))
        {
            UIBuild buidUi = (UIBuild)GuideManager.Instance.curRequestGuide;
            Vector2 focusPos = buidUi.Focus3DObject(guideStep.posObject);
            focusRt.anchoredPosition = focusPos;
        }
    }
    void InitGuideWithFocus2DObject()
    {
        focusRt.gameObject.SetActive(true);
        UIBase uBase = (UIBase)GuideManager.Instance.curRequestGuide;
        List<GuidePositionObject> listPos = uBase.posObjectList;
        RectTransform targetRt = null;
        for(int  i =0;i<listPos.Count;i++)
        {
            if(guideStep.posObject.Equals(listPos[i].Id))
            {
                targetRt = listPos[i].transform as RectTransform;
                break;
            }
        }
        if(null == targetRt)
        {
            Logger.LogErrorFormat("位置列表中未发现 {0},配置UI名字{1}", guideStep.posObject, uBase.uiViewName);
            return;
        }
        Vector3 spacePosition = UIUtil.GetSpacePos(targetRt, UIMgr.Instance.CanvasAttr, UICamera.Instance.CameraAttr);
        float fscale = UIMgr.Instance.CanvasAttr.scaleFactor;
        focusRt.anchoredPosition = new Vector2(spacePosition.x / fscale, spacePosition.y / fscale);
    }

    void OnFocusButtonClick()
    {
        FinishStep();
    }
    void OnTalkFinish(float d)
    {
        FinishStep();
    }
	
    void OnExitGuidObjectClick(GameObject go)
    {
        if(guideStep.exitNoEvent == 1)
        {
            FinishStep(false);
        }
    }

    void FinishStep(bool sendEvent = true)
    {
        RequestCloseUi();
        GuideManager.Instance.FinishStep(sendEvent);
    }
}
