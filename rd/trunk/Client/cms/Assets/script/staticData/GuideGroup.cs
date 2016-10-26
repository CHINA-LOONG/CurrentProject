using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuideGroup
{
    public int Id;
    public string guidPurpose;
    public string finishOther;
    public string steps;
    public int finishAtStep;
    public int conditionId;

    private List<int> stepList = null;
    public List<int>StepListAttr
    {
        get
        {
            if(null == stepList)
            {
                stepList = new List<int>();
                string[] stepsArray = steps.Split('-');
                for(int  i =0;i<stepsArray.Length;++i)
                {
                    stepList.Add(int.Parse(stepsArray[i]));
                }
            }
            return stepList;
        }
    }

    private List<int> finishOtherGuide = null;
    public List<int>FinishOtherGuideAttr
    {
        get
        {
            if(null == finishOtherGuide)
            {
                finishOtherGuide = new List<int>();
                if(!string.IsNullOrEmpty(finishOther))
                {
                    string[] others = finishOther.Split('-');
                    for(int i=0;i<others.Length;++i)
                    {
                        finishOtherGuide.Add(int.Parse(others[i]));
                    }
                }
            }
            return finishOtherGuide;
        }
    }
}
