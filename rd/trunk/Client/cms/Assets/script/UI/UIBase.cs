using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class UIBase : MonoBehaviour 
{

	public enum ViewType
	{
		VT_NORMAL,
		VT_POPUP,
        VT_POPUPTOP,
        VT_NORMALTOP,
    }
	[SerializeField]
	ViewType mViewType =  ViewType.VT_NORMAL;
	public ViewType ViewTypeAttr
	{
		get
		{
			return mViewType;
		}
	}

    [SerializeField]
    protected bool mIgnorePreviousHide = false;
    public bool IgnorePreviousHide
    {
        get
        {
            return mIgnorePreviousHide;
        }
    }

    [SerializeField]
    protected bool mHidePreviouseUI = true;
    public bool HidePreviousUI
    {
        get
        {
            return mHidePreviouseUI;
        }
    }

    [SerializeField]
    protected bool mDontDestroyWhenSwitchScene = false;
    public bool DontDestroyWhenSwitchScene
    {
        get
        {
            return mDontDestroyWhenSwitchScene;
        }
    }

    public List<GuidePositionObject> posObjectList = new List<GuidePositionObject>();

    public Animator uiAnimator;
    private static string outAnimationTriger = "animationOut";
    private static string inAnimationTriger = "animationIn";

    private int inAnimationState = -1;
    private int outAnimationState = -1;

    public string uiViewName;

    //初始化界面操作，创建或激活后配置初始状态
    public virtual void Init(bool forbidGuide = false)
    {
        if(inAnimationState == -1)
        {
            inAnimationState = Animator.StringToHash("uiEnterState");
            outAnimationState = Animator.StringToHash("uiOutState");
        }

        if (null != uiAnimator && uiAnimator.HasState(0, inAnimationState))
        {
            uiAnimator.SetTrigger(inAnimationTriger);
        }
    }
    //关闭界面时调用，隐藏和销毁都会调用
    public virtual void Disable()
    {

    }
    //删除界面，对子对象的清理操作
    public virtual void Clean()
    {
 
    }

    public virtual void RefreshOnPreviousUIHide()
    {
        if (null != uiAnimator && uiAnimator.HasState(0, inAnimationState))
        {
            uiAnimator.SetTrigger(inAnimationTriger);
        }
    }

    public  void    RequestCloseUi(bool withAni = true)
    {
        if(withAni && null != uiAnimator && uiAnimator.HasState(0, outAnimationState))
        {
            uiAnimator.SetTrigger(outAnimationTriger);
        }
        else
        {
            CloseUi();
        }
    }
    protected void OnExitAnimationFinish()
    {
        CloseUi();
    }

    public virtual  void   CloseUi()
    {
        UIMgr.Instance.CloseUI_(this);
    }

    public  void GuideListener(bool yesAddnoRemove)
    {
        if(null != posObjectList)
        {
            for(int i =0;i<posObjectList.Count;++i)
            {
                if(yesAddnoRemove)
                {
                    GameEventMgr.Instance.AddListener<string>(posObjectList[i].Id, OnGuideMessageCallback);
                }
                else
                {
                    GameEventMgr.Instance.RemoveListener<string>(posObjectList[i].Id, OnGuideMessageCallback);
                }
            }
        }
    }

   protected   virtual  void    OnGuideMessageCallback(string message)
    {

    }

    //public void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {


    //        Logger.Log("Name:" + gameObject.name + "\t" + transform.GetSiblingIndex());

    //    }
    //}
}
