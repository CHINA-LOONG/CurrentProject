using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndBattleUI : MonoBehaviour {

    public StarUI[] mStarList;
    //public float mShowStarInterval;
    public Image mScoreResultImg;
    //public GameObject mScoreResultRoot;
    public Sprite mVictorySprite;
    public Sprite mFailedSprite;
    public Sprite mDrawSprite;
    //public Animator mEndBattleAni;

    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Awake () {
    }
    //---------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update () {

    }
    //---------------------------------------------------------------------------------------------
    public void SetStarCount(int starCount)
    {
        for (int i = 0; i < mStarList.Length; ++i)
        {
            mStarList[i].SetHasStar(i < starCount);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void ShowStar()
    {
        for (int i = 0; i < mStarList.Length; ++i)
        {
            StartCoroutine(ShowStarInternal(i));
        }
    }
    //---------------------------------------------------------------------------------------------
    private IEnumerator ShowStarInternal(int index)
    {
        yield return new WaitForSeconds(index * BattleConst.scoreStarInterval);
        mStarList[index].ShowStar();
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
    public void SetSuccess(byte battleResult)
    {
        //mScoreResultRoot.SetActive(battleResult == 0);
        ResourceMgr resMgr = ResourceMgr.Instance;
        if (battleResult == 0)
        {
            mScoreResultImg.sprite = mVictorySprite;
            //mEndBattleAni.Play("endbattle_guang");

            GameObject shengliEffect1 = resMgr.LoadAsset("pve_shengli_chixu");
            GameObject shengliEffect2 = resMgr.LoadAsset("pve_shengli_shanguang");
            if (shengliEffect1 != null)
            {
                shengliEffect1.transform.SetParent(transform, false);
            }
            if (shengliEffect2 != null)
            {
                shengliEffect2.transform.SetParent(transform, false);
            }
        }
        else if (battleResult == 1)
        {
            mScoreResultImg.sprite = mDrawSprite;
            GameObject pingjuEffect = resMgr.LoadAsset("pvp_pingju_chixu");
            if (pingjuEffect != null)
            {
                pingjuEffect.transform.SetParent(transform, false);
            }
        }
        else
        {
            mScoreResultImg.sprite = mFailedSprite;
            GameObject failedEffect = resMgr.LoadAsset("pvp_shibai_chixu");
            if (failedEffect != null)
            {
                failedEffect.transform.SetParent(transform, false);
            }
        }

        mScoreResultImg.SetNativeSize();
    }
    //---------------------------------------------------------------------------------------------
}
