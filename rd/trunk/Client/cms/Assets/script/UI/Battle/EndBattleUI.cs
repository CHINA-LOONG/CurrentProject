using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndBattleUI : MonoBehaviour {

    public StarUI[] mStarList;
    //public float mShowStarInterval;
    public Image mScoreResultImg;
    public GameObject mScoreResultRoot;
    public Sprite mVictorySprite;
    public Sprite mFailedSprite;
    public Animator mEndBattleAni;

    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Awake () {
    }
    //---------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update () {

    }
    //---------------------------------------------------------------------------------------------
    public void SetStar(int starCount)
    {
        for (int i = 0; i < mStarList.Length; ++i)
        {
            StartCoroutine(ShowStarInternal(i, i < starCount));
        }
    }
    //---------------------------------------------------------------------------------------------
    private IEnumerator ShowStarInternal(int index, bool hasStar)
    {
        yield return new WaitForSeconds(index * BattleConst.scoreStarInterval);
        mStarList[index].ShowStar(hasStar);
    }
    //---------------------------------------------------------------------------------------------
    public void SkipShowStarAni()
    {
        for (int i = 0; i < mStarList.Length; ++i)
        {
            if (mStarList[i].gameObject.activeSelf == true)
            {
                mStarList[i].SkipAni();
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetStarVisiblebool (bool isVisible)
    {
        for (int i = 0; i < mStarList.Length; ++i)
        {
            mStarList[i].gameObject.SetActive(isVisible);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetSuccess(bool success)
    {
        mScoreResultRoot.SetActive(success);
        if (success)
        {
            mScoreResultImg.sprite = mVictorySprite;
            mEndBattleAni.Play("endbattle_guang");
        }
        else
        {
            mScoreResultImg.sprite = mFailedSprite;
        }
        mScoreResultImg.SetNativeSize();
    }
    //---------------------------------------------------------------------------------------------
}
