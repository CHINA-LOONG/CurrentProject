using UnityEngine;
using System.Collections;

public class StarUI : MonoBehaviour
{
    public RectTransform mStar;

    private bool mHasStar;
    private Animator mAniControl;
    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start () {
        mAniControl = gameObject.GetComponent<Animator>();
    }
    //---------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update () {

    }
    //---------------------------------------------------------------------------------------------
    public void ShowStar(bool hasStar)
    {
        mHasStar = hasStar;
        //mStar.gameObject.SetActive(hasStar);
        if (hasStar == true)
        {
            mAniControl.Play("star_show");
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SkipAni()
    {
        if (mHasStar == true)
        {
            mStar.anchoredPosition = new Vector2(0.0f, 0.0f);
            mStar.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            mAniControl.Stop();
        }
    }
    //---------------------------------------------------------------------------------------------
}
