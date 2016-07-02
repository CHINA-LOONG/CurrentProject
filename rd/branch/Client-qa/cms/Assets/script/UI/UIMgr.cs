using UnityEngine;
using System.Collections;

public class UIMgr : MonoBehaviour 
{
	[SerializeField]
	RectTransform	m_rootRectTransform;
	public	RectTransform	RootRectTransform
	{
		get
		{
			return	m_rootRectTransform;
		}
	}

	static UIMgr mInst = null;
	public static UIMgr Instance
	{
		get
		{
			if (mInst == null)
			{
				Object r = ResourceMgr.Instance.LoadAsset ("ui/root", "UIRoot"); 
				GameObject ui = Instantiate(r) as GameObject;
				ui.name = "UIMgr";
				mInst = ui.AddComponent<UIMgr>();

			}
			return mInst;
		}
	}
	void Start()
	{
		m_rootRectTransform = transform as RectTransform;
	}

	public void Init()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void OpenUI(string assertName, string uiName)
	{
		Object r = ResourceMgr.Instance.LoadAsset (assertName, uiName);
		if (null == r) 
		{
			return;
		}
		GameObject ui = Instantiate(r) as GameObject;
        ui.transform.SetParent(transform);
		ui.transform.localPosition = Vector3.zero;
		ui.transform.localEulerAngles = Vector3.zero;
		ui.transform.localScale = Vector3.one;
		UIBase vb = ui.GetComponent<UIBase>();

		if (vb.ViewTypeAttr == UIBase.ViewType.VT_POPUP)
		{
			ui.name = uiName;
			ui.layer = LayerMask.NameToLayer("Default");
			ui.transform.SetParent(transform);
			ui.transform.localScale = Vector3.one;
			ui.transform.localPosition = Vector3.zero;

		}
	}

	public void CloseUI(UIBase vb)
	{
		Destroy(vb.gameObject);
	}

	public void CloseUI(string name)
	{
		UIBase[] vbs = GetComponentsInChildren<UIBase>();
		foreach (UIBase vb in vbs)
		{
			if (vb.GetType().Name == name)
			{
				Destroy(vb.gameObject);
			}
		}
	}
}
