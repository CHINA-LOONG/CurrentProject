using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarUI : MonoBehaviour
{
    public RectTransform mStar;
    public Image mStarImg;
    public ParticleSystem mStarEffect;

    private bool mHasStar;
    private Animator mAniControl;
    private bool mSkipped;
    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Awake () {
        mAniControl = gameObject.GetComponent<Animator>();
        mSkipped = false;
    }
    //---------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update () {

    }
    //---------------------------------------------------------------------------------------------
    public void SetHasStar(bool hasStar)
    {
        mHasStar = hasStar;
    }
    //---------------------------------------------------------------------------------------------
    public void ShowStar()
    {
        //mHasStar = hasStar;
        //mStar.gameObject.SetActive(hasStar);
        if (mHasStar == true)
        {
            mAniControl.Play("star_show");
            mStarEffect.gameObject.SetActive(mSkipped == false);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SkipAni()
    {
        if (mHasStar == true)
        {
            mStar.gameObject.SetActive(true);
            mStar.anchoredPosition = new Vector2(0.0f, 0.0f);
            mStar.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            mStarImg.color = Color.white;
            mAniControl.Stop();
            mSkipped = true;
        }
    }
    //---------------------------------------------------------------------------------------------
}
