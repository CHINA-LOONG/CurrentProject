using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GuideManager 
{
    public static GuideManager _instance = null;
    public static GuideManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new GuideManager();
            }
            return _instance;
        }
    }

    private GuideGroup curGuideGroup = null;
    private GuideStep nextGuideStep = null;
    private int finishStepCount = 0;
    public GuideBase curRequestGuide;

    public void FinishStep()
    {
        GameEventMgr.Instance.FireEvent(nextGuideStep.finishEvent);

        finishStepCount++;
        if(curGuideGroup.finishAtStep == nextGuideStep.Id)
        {
            List<int> finishGuide = new List<int>();
            finishGuide.Add(nextGuideStep.Id);
            if(curGuideGroup.FinishOtherGuideAttr.Count > 0)
            {
                finishGuide.AddRange(curGuideGroup.FinishOtherGuideAttr);
            }
            GameDataMgr.Instance.GuideDataMgrAttr.RequestFinishGuide(finishGuide, OnFinishRequestFinishGuide);
        }
        if(curGuideGroup.StepListAttr.Count == finishStepCount)
        {
            //finish
            curGuideGroup = null;
            nextGuideStep = null;
            finishStepCount = 0;
        }
        else
        {
            GuideStep preGuideStep = nextGuideStep;
            nextGuideStep = StaticDataMgr.Instance.GetGuideStep(curGuideGroup.StepListAttr[finishStepCount]);
            if(preGuideStep.trigerView.Equals(nextGuideStep.trigerView))
            {

            }
        }
    }
    void OnFinishRequestFinishGuide(bool succ)
    {

    }
    public void RequestGuide(GuideBase guide)
    {
        return;
        curRequestGuide = guide;
        if(null != curGuideGroup)
        {
            GuideRunningGuide();
        }
        else
        {
            GuideNewGuide();
        }
    }

    void GuideRunningGuide()
    {
        if (CheckStep(nextGuideStep))
        {
            ShowGuideUi(nextGuideStep);
        }
        else
        {
            Logger.LogErrorFormat("guide break error! guide group = {0},guide step = {1}", curGuideGroup.Id, nextGuideStep.Id);
            curGuideGroup = null;
            nextGuideStep = null;
            finishStepCount = 0;
        }
    }
    void GuideNewGuide()
    {
        List<GuideGroup> allGuide = StaticDataMgr.Instance.GetGuideGroupList();

        GuideGroup subGuide = null;
        for(int i =0;i < allGuide.Count;++i)
        {
            subGuide = allGuide[i];
            if (GameDataMgr.Instance.GuideDataMgrAttr.IsGuideFinished(subGuide.Id))
            {
                continue;
            } 
            GuideStep firstStep = StaticDataMgr.Instance.GetGuideStep(subGuide.StepListAttr[0]);
            if(!CheckStep(firstStep))
            {
                continue;
            }
            if(CheckGroupCondition(subGuide))
            {
                curGuideGroup = subGuide;
                nextGuideStep = firstStep;
                finishStepCount = 0;
                ShowGuideUi(firstStep);
                break;
            }
        }
    }

    bool CheckStep(GuideStep step)
    {
        UIBase curUi = curRequestGuide as UIBase;
        if (null == curUi)
            return false;
        bool result = curUi.uiViewName.Equals(step.trigerView);
        return result;
    }

    bool  CheckGroupCondition(GuideGroup ggroup)
    {
        return GuideCondition.IsConditionOK(ggroup.Id);
    }

    void ShowGuideUi(GuideStep step)
    {

    }

}
