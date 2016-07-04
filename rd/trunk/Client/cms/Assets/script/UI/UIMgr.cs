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

	[SerializeField]
	Canvas canVas = null;
	public Canvas CanvasAttr
	{
		get
		{
			return canVas;
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
		canVas = gameObject.GetComponent<Canvas> ();

		//UICamera.Instance.Init ();
		//canVas.worldCamera = UICamera.Instance.CameraAttr;
	}

	public GameObject OpenUI(string assertName, string uiName)
	{
		Object r = ResourceMgr.Instance.LoadAsset (assertName, uiName);
		if (null == r) 
		{
			return null;
		}
		GameObject ui = Instantiate(r) as GameObject;

		RectTransform rt = ui.transform as RectTransform;
		rt.SetParent (transform ,false);
		rt.localScale = Vector3.one;
		rt.localEulerAngles = Vector3.zero;
		//ui.transform.localPosition = Vector3.zero;

		UIBase vb = ui.GetComponent<UIBase>();
		ui.name = uiName;

        return ui;
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
