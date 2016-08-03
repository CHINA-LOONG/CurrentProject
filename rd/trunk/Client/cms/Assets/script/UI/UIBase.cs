using UnityEngine;
using System.Collections;

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

    //初始化界面操作，创建或激活后配置初始状态
    public virtual void Init()
    {
 
    }
    //删除界面，对子对象的清理操作
    public virtual void Clean()
    {
 
    }

    //public void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {


    //        Debug.Log("Name:" + gameObject.name + "\t" + transform.GetSiblingIndex());

    //    }
    //}
}
