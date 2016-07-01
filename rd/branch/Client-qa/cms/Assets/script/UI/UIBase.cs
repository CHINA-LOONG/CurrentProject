using UnityEngine;
using System.Collections;

public class UIBase : MonoBehaviour 
{

	public enum ViewType
	{
		VT_NORMAL,
		VT_SCREEN,
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
}
