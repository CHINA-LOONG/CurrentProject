using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SociatyTechnologyItem : MonoBehaviour
{
    public Text mTecName;
    public Text mTecCurLvl;
    public Text mFunctionDescript;
    public Text mCurLvlDescript;
    public Text mNextLvlDescript;
    public Text mContributeConsumValue;
    public Text mTecLvlUpText;
    public Text mMaxLvlText;

    public Button mTecLvlUp;
    public Image mTecIcon;
    public Image mContributeConsumIcon;
    public Image mMaxLvlBackImg;

    private SociatyTechnologyData mTechData;
    private int mCurrentType;
    //---------------------------------------------------------------------------------------------
    public static SociatyTechnologyItem Create()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("SociatyTechnologyItem");
        SociatyTechnologyItem item = go.GetComponent<SociatyTechnologyItem>();
        return item;
    }
    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        EventTriggerListener.Get(mTecLvlUp.gameObject).onClick = OnLvlUp;
        StaticDataMgr sdMgr = StaticDataMgr.Instance;
        mTecLvlUpText.text = sdMgr.GetTextByID("sociaty_levelupup");
        mMaxLvlBackImg.gameObject.SetActive(false);
    }
    //---------------------------------------------------------------------------------------------
    void OnLvlUp(GameObject go)
    {
        PB.HSAllianceLevelUp param = new PB.HSAllianceLevelUp();
        param.type = mCurrentType;
        GameApp.Instance.netManager.SendMessage(PB.code.ALLIANCE_LEVEL_UP_C.GetHashCode(), param);
    }
    //---------------------------------------------------------------------------------------------
    public void SetTechnologyData(int techType, int lvl)
    {
        mCurrentType = techType;
        StaticDataMgr sdMgr = StaticDataMgr.Instance;
        //technology not open
        if (lvl == 0)
        {
            SociatyTechnologyData data = sdMgr.GetSociatyTechData(techType, 1);
            if (data != null)
            {
                mTecName.text = sdMgr.GetTextByID(data.tecName);
                mCurLvlDescript.text = string.Format(sdMgr.GetTextByID("sociaty_startlevel"), data.sociatyLevel);
                mNextLvlDescript.text = string.Empty;
                mFunctionDescript.text = sdMgr.GetTextByID(data.tecDescript);
                mContributeConsumValue.text = data.levelUp.ToString();
                mTecCurLvl.text = string.Empty;
            }

            return;
        }

        int sociatyLvl = GameDataMgr.Instance.SociatyDataMgrAttr.allianceSelfData.level;
        mTechData = sdMgr.GetSociatyTechData(techType, lvl);
        SociatyTechnologyData nextTechData = sdMgr.GetSociatyTechData(techType, lvl + 1);
        if (mTechData != null)
        {
            //set icon
            Sprite iconImg = ResourceMgr.Instance.LoadAssetType<Sprite>(mTechData.icon) as Sprite;
            if (null != iconImg)
                mTecIcon.sprite = iconImg;
            //set technology name
            mTecName.text = sdMgr.GetTextByID(mTechData.tecName);

            string curLvlEffect = sdMgr.GetTextByID("sociaty_nowlevel");
            string nextLvlEffect = sdMgr.GetTextByID("pet_detail_skill_next_level");
            //set current level info
            mTecCurLvl.text = "LVL" + lvl.ToString();
            mFunctionDescript.text = sdMgr.GetTextByID(mTechData.tecDescript);
            if (mTechData.sociatyLevel >= 0 && mTechData.sociatyLevel < sociatyLvl)
            {
                //not open
                mCurLvlDescript.text = string.Format(sdMgr.GetTextByID("sociaty_startlevel"), mTechData.sociatyLevel);
                mNextLvlDescript.text = string.Empty;
            }
            else
            {
                mContributeConsumValue.text = mTechData.levelUp.ToString();
                SetLvlInfoInternal(curLvlEffect, mTechData, ref mCurLvlDescript);
                //set next level info
                if (nextTechData == null)
                {
                    //no next level means max level 
                    mNextLvlDescript.text = string.Empty;
                    mContributeConsumValue.gameObject.SetActive(false);
                    mContributeConsumIcon.gameObject.SetActive(false);
                    mTecLvlUp.gameObject.SetActive(false);
                    mMaxLvlBackImg.gameObject.SetActive(true);
                    mMaxLvlText.gameObject.SetActive(true);
                    mMaxLvlText.text = sdMgr.GetTextByID("sociaty_maxlevel");
                }
                else
                {
                    mContributeConsumValue.gameObject.SetActive(true);
                    mContributeConsumIcon.gameObject.SetActive(true);
                    mTecLvlUp.gameObject.SetActive(true);
                    mMaxLvlBackImg.gameObject.SetActive(false);
                    mMaxLvlText.gameObject.SetActive(false);
                    SetLvlInfoInternal(nextLvlEffect, nextTechData, ref mNextLvlDescript);
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    private void SetLvlInfoInternal(string prefix, SociatyTechnologyData techData, ref Text curText)
    {
        StaticDataMgr sdMgr = StaticDataMgr.Instance;
        //if (techData != null)
        {
            string lvlInfo = string.Empty;
            string itemName = string.Empty;
            int curCount = 0;
            SociatyTecEnum curType = (SociatyTecEnum)techData.type;
            switch (curType)
            {
                case SociatyTecEnum.Sociaty_Tec_Lvl:
                    lvlInfo = sdMgr.GetTextByID("sociaty_leveltipsmiaoshu1");
                    curText.text = prefix + string.Format(lvlInfo, techData.level);
                    break;
                case SociatyTecEnum.Sociaty_Tec_Member:
                    lvlInfo = sdMgr.GetTextByID("sociaty_peopletipsmiaoshu1");
                    string.Format(lvlInfo, techData);
                    curText.text = prefix + string.Format(lvlInfo, techData.gainPeople);
                    break;
                case SociatyTecEnum.Sociaty_Tec_Coin:
                    lvlInfo = "+{0:F0}%";
                    curText.text = prefix + string.Format(lvlInfo, techData.gainCoin * 100);
                    break;
                case SociatyTecEnum.Sociaty_Tec_Exp:
                    GetItemNameCount(techData.gainExp, out itemName, out curCount);
                    lvlInfo = "{0}*{1}";
                    curText.text = prefix + string.Format(lvlInfo, itemName, curCount);
                    break;
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    private void GetItemNameCount(string reward, out string name, out int count)
    {
        string[] itemInfo = reward.Split('_');

        if (itemInfo.Length < 3)
            Logger.LogError("technology reward config error!");

        ItemStaticData item = StaticDataMgr.Instance.GetItemData(itemInfo[1]);
        if (item == null)
            Logger.LogError("technology reward config error!");

        count = int.Parse(itemInfo[2]);
        name = item.NameAttr;
    }
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
}
