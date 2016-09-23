using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AdventureExtraConditon : MonoBehaviour
{

    public enum ConditionStep
    {
        STEP_0,
        STEP_33,
        STEP_50,
        STEP_66,
        STEP_100
    }
    private ConditionStep conditionStep;

    private Animator animator;
    public Animator Animator
    {
        get
        {
            if (animator==null)
            {
                animator = gameObject.GetComponent<Animator>();
            }
            return animator;
        }
    }
    
    public float changeTime = 1.0f;
    public Text textStep;

    private int tarIndex;
    private int curIndex;
    void Awake()
    {
        conditionStep = ConditionStep.STEP_0;
        curIndex = 0;
        textStep.text = string.Format("{0}%", curIndex);
    }
    public void RefreshStep(int step)
    {
        ConditionStep temp;
        if (step>=100)
        {
            temp = ConditionStep.STEP_100;
        }
        else if (step>=66)
        {
            temp = ConditionStep.STEP_66;
        }
        else if (step>=50)
        {
            temp = ConditionStep.STEP_50;
        }
        else if(step>=33)
        {
            temp = ConditionStep.STEP_33;
        }
        else
        {
            temp = ConditionStep.STEP_0;
        }

        if (temp != conditionStep)//改变条件段位
        {
            conditionStep = temp;
            StopCoroutine("UpdateIndex");
            StartCoroutine(UpdateIndex(step));
        }
    }

    IEnumerator UpdateIndex(int target)
    {
        tarIndex = target;
        if (tarIndex > curIndex)
        {
            //TODO:播放动画

            float curtime = Time.time;
            int tempIndex = curIndex;
            while (curIndex<tarIndex)
            {
                curIndex = (int)Mathf.Lerp(tempIndex, tarIndex, (Time.time - curtime) / changeTime);
                textStep.text = string.Format("{0}%", curIndex);
                yield return null;
            }

            //float delay = changeTime / (tarIndex - curIndex);
            //while (curIndex < tarIndex)
            //{
            //    curIndex += 1;
            //    textStep.text = string.Format("{0}%", curIndex);
            //    yield return new WaitForSeconds(delay);
            //}
        }
        else
        {
            curIndex = tarIndex;
            textStep.text = string.Format("{0}%", curIndex);
        }
    }

}
