using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SociatyPrayItem : MonoBehaviour
{
    public Text mSelfContributeText;
    public Text mSociatyLiveText;
    public Button mPrayBtn;
    public Text mPrayBtnText;
    public Text mTitle;
    public Image mCostImg;
    public Text mCostValue;

    public Sprite mZuanshiImg;
    public Sprite mJinbiImg;

    private SociatyPray mPrayUI;
    private SociatyPrayData mPrayData;
    private int mPrayIndex;

    //---------------------------------------------------------------------------------------------
    public static SociatyPrayItem Create()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("SociatyPrayItem");
        SociatyPrayItem item = go.GetComponent<SociatyPrayItem>();
        return item;
    }
    //---------------------------------------------------------------------------------------------
    void Start()
    {
        EventTriggerListener.Get(mPrayBtn.gameObject).onClick = OnPray;
        mPrayBtnText.text = StaticDataMgr.Instance.GetTextByID("sociaty_qifubtn");
    }
    //---------------------------------------------------------------------------------------------
    void OnPray(GameObject go)
    {
        if (mPrayData != null)
        {
            PlayerData playerData = GameDataMgr.Instance.PlayerDataAttr;
            if (GameDataMgr.Instance.SociatyDataMgrAttr.allianceParyCount > 0)
            {
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_027"),
                                               (int)PB.ImType.PROMPT);
            }
            else if (mPrayData.coinConsume > 0 && mPrayData.coinConsume > playerData.coin)
            {
                GameDataMgr.Instance.ShopDataMgrAttr.JinbiNoEnough();
            }
            else if (mPrayData.goldConsume > 0 && mPrayData.goldConsume > playerData.gold)
            {
                GameDataMgr.Instance.ShopDataMgrAttr.ZuanshiNoEnough();
            }
            else
            {
                ShowPrayRewadAni();
                PB.HSAlliancePray param = new PB.HSAlliancePray();
                param.prayIndex = mPrayIndex;
                GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_PRAY_C.GetHashCode(), param);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetPrayData(int index, SociatyPray prayUI)
    {
        mPrayUI = prayUI;
        StaticDataMgr sdMgr = StaticDataMgr.Instance;
        mPrayData = sdMgr.GetPrayData(index);
        if (mPrayData != null)
        {
            mPrayIndex = index;
            string titleString = string.Empty;
            switch (index)
            {
                case 1:
                    titleString = "sociaty_X1dang";
                    break;
                case 2:
                    titleString = "sociaty_X2dang";
                    break;
                case 3:
                    titleString = "sociaty_X3dang";
                    break;
            }
            mTitle.text = sdMgr.GetTextByID(titleString);
            mSelfContributeText.text = sdMgr.GetTextByID("sociaty_onecontribution") + mPrayData.memberReward.ToString();
            mSociatyLiveText.text = sdMgr.GetTextByID("sociaty_comcontribution") + mPrayData.allianceReward;
            if (mPrayData.coinConsume > 0)
            {
                mCostImg.sprite = mJinbiImg;
                mCostValue.text = mPrayData.coinConsume.ToString();
            }
            else if (mPrayData.goldConsume > 0)
            {
                mCostImg.sprite = mZuanshiImg;
                mCostValue.text = mPrayData.goldConsume.ToString();
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    private void ShowPrayRewadAni()
    {
    }
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
}
