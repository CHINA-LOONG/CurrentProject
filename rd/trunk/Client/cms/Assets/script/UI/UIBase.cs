using UnityEngine;
using System.Collections;

public class UIBase : MonoBehaviour 
{

	public enum ViewType
	{
		VT_NORMAL,
		VT_POPUP,
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

    public virtual void OnOpenUI()
    {

    }

    public virtual void OnCloseUI()
    {

    }
}
