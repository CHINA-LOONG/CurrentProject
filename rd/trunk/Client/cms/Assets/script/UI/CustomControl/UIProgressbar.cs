using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIProgressbar : MonoBehaviour {

    public Image mProgressImage;
    public Text mProgressText;
    public float mProgressSpeed = 1.0f;
    public bool mNeedAni = true;
    public bool mShowText = false;

    private int mLoopCount = 0;
    private float mCurrentRatio = 0.0f;
    private float mTargetRatio = 0.0f;

    private int mTotalValue;
    private int mTargetValue;
    private int mCurrentValue = 0;

    void Awake()
    {
        mProgressText.gameObject.SetActive(mShowText);
    }
    //---------------------------------------------------------------------------------------------
	void Update () 
    {
        if (mCurrentRatio != mTargetRatio || mLoopCount > 0)
        {
            if (mNeedAni == false)
            {
                mCurrentValue = mTargetValue;
                mLoopCount = 0;
                mCurrentRatio = mTargetRatio = 1.0f;
                if (mTotalValue > 0)
                {
                    mCurrentRatio = mTargetRatio = (float)mCurrentValue / mTotalValue;
                }

                if (mShowText)
                {
                    mProgressText.gameObject.SetActive(true);
                }
            }
            else
            {
                //hide the progress in ani
                if (mShowText)
                {
                    mProgressText.gameObject.SetActive(false);
                }
                mCurrentRatio += mProgressSpeed * Time.deltaTime;
                if (mLoopCount > 0)
                {
                    if (mCurrentRatio >= 1.0f)
                    {
                        --mLoopCount;
                        mCurrentRatio = 0.0f;
                    }
                }
                else if (mCurrentRatio > mTargetRatio)
                {
                    mCurrentRatio = mTargetRatio;
                }
            }

            mProgressImage.fillAmount = mCurrentRatio;
            
            if (mShowText && mLoopCount == 0 && mCurrentRatio == mTargetRatio)
            {
                mProgressText.gameObject.SetActive(true);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetLoopCount(int loopCount)
    {
        mLoopCount = loopCount;
    }
    //---------------------------------------------------------------------------------------------
    public void SetValueText(int totalValue, int currentValue, int targetValue)
    {
        mTotalValue = totalValue;
        mCurrentValue = currentValue;
        mTargetValue = targetValue;
        mProgressText.text = mTargetValue.ToString() + "/" + mTotalValue.ToString();
    }
    //---------------------------------------------------------------------------------------------
    public void SetTargetRatio(int targetRatio)
    {
        mTargetRatio = targetRatio;
    }
    //---------------------------------------------------------------------------------------------
    public void SetCurrrentRatio(int currentRatio)
    {
        mCurrentRatio = currentRatio;
    }
    //---------------------------------------------------------------------------------------------
}
