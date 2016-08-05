using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WpIcon : MonoBehaviour
{
    //正常状态类别
    public enum NormalStateType
    {
        BeneficialForSelf = 0,//对本体有加成
        HarmfulForEnemy,//对地方有压制
        ProtectedWpForSelf,//保护弱点
        HightInjuryRatio,//受伤比高
        LowInjuryRatio,//受伤比低
        NormalInjuryRatio//受伤比正常
    }
    public Image iconImage;
    public Image deadMask;
    public UIProgressbar progressBar;
    public Image buttonImage;

    private bool isArmor = false;//是否是铠甲
    private WeakPointRuntimeData wpRealData;
    // Use this for initialization
    void Start ()
    {
        EventTriggerListener.Get(buttonImage.gameObject).onEnter = OnTouchEnter;
        EventTriggerListener.Get(buttonImage.gameObject).onExit = OnTouchExit;
	}

    public  string GetWpId()
    {
        if(null != wpRealData)
        {
            return wpRealData.id;
        }
        return null;
    }

    public  void    RefreshWithWp(WeakPointRuntimeData wpRealData)
    {
        this.wpRealData = wpRealData;
        progressBar.gameObject.SetActive(false);
        deadMask.gameObject.SetActive(false);

        isArmor = false;
        string wpIconname = null;
        switch (wpRealData.wpState)
        {
            case WeakpointState.Ice:
                return;
            case WeakpointState.Dead:
                deadMask.gameObject.SetActive(true);
                return;
            case WeakpointState.Hide:
                wpIconname = "ruodianIcon_hide";
                break;
            case WeakpointState.Normal1:
                wpIconname = GetWpIconName(wpRealData.staticData.stat1WpType);
                isArmor = wpRealData.staticData.stat1WpType >= 0 && wpRealData.staticData.stat1WpType <= 2;
                break;
            case WeakpointState.Normal2:
                wpIconname = GetWpIconName(wpRealData.staticData.state2WpType);
                isArmor = wpRealData.staticData.state2WpType >= 0 && wpRealData.staticData.state2WpType <= 2;
                break;
        }
        Sprite iconSp = ResourceMgr.Instance.LoadAssetType<Sprite>(wpIconname);
        if(null != iconSp)
        {
            iconImage.sprite = iconSp;
        }

        progressBar.gameObject.SetActive(isArmor);
        if(isArmor)
        {
            RefreshProgress(0);
        }
    }

    string GetWpIconName(int type)
    {
        return string.Format("ruodianIcon_{0}", (int)type);
    }

    public void RefreshProgress(int oldHp)
    {
        float ratio = (float)wpRealData.HpAttr / (float)wpRealData.maxHp;
        progressBar.SetTargetRatio(ratio);
    }

    public  void    ShowoffIcon(bool showoff)
    {
        if(showoff)
        {
            iconImage.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        else
        {
            iconImage.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    void OnTouchEnter(GameObject go)
    {
        string tips = null;
        switch(wpRealData.wpState)
        {
            case WeakpointState.Hide:
                tips = wpRealData.staticData.state0Tips;
                break;
            case WeakpointState.Normal1:
                tips = wpRealData.staticData.state1Tips;
                break;
            case WeakpointState.Normal2:
                tips = wpRealData.staticData.state2Tips;
                break;
        }
        if(!string.IsNullOrEmpty(tips))
        {
            ShowoffIcon(true);
            BattleController.Instance.GetUIBattle().wpUI.ShowTips(this,StaticDataMgr.Instance.GetTextByID(tips));
            wpRealData.showoffEffect.Show(true);
        }
    }

    void OnTouchExit(GameObject go)
    {
        ShowoffIcon(false);
        BattleController.Instance.GetUIBattle().wpUI.HideTips();
        wpRealData.showoffEffect.Show(false);
        //test 3fat
       //BattleController.Instance.GetUIBattle().wpUI.ChangeBatch(2.0f);
    }

    
}
