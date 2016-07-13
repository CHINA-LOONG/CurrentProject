using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

	private Transform topPanelTransform;


	static UIMgr mInst = null;
	public static UIMgr Instance
	{
		get
		{
			if (mInst == null)
			{
				GameObject ui = GameObject.Find("/UIRoot");
				//ui.name = "UIMgr";
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

		UICamera.Instance.Init ();
		canVas.worldCamera = UICamera.Instance.CameraAttr;

		GameObject topGo = Util.FindChildByName(gameObject,"topPanel");
		topPanelTransform = topGo.transform;

	}

	public GameObject OpenUI(string uiName)
	{
		GameObject ui = ResourceMgr.Instance.LoadAsset(uiName);
		if (null == ui) 
		{
			return null;
		}
		//GameObject ui = Instantiate(r) as GameObject;

		RectTransform rt = ui.transform as RectTransform;

		rt.localScale = Vector3.one;
		rt.localEulerAngles = Vector3.zero;
		ui.name = uiName;

		UIBase vb = ui.GetComponent<UIBase>();
		if (vb.ViewTypeAttr == UIBase.ViewType.VT_POPUP)
		{
			rt.SetParent (topPanelTransform ,false);
		}
		else 
		{
			rt.SetParent (transform ,false);
			topPanelTransform.SetSiblingIndex (ui.transform.GetSiblingIndex() + 1);
		}

        vb.OnOpenUI();
        return ui;
	}

	public void CloseUI(UIBase vb)
	{
		//Destroy(vb.gameObject);
		ResourceMgr.Instance.DestroyAsset(vb.gameObject);
	}

	public void CloseUI(string name)
	{
        UIBase[] vbs = GetComponentsInChildren<UIBase>();
		foreach (UIBase vb in vbs)
		{
			if (vb.GetType().Name == name)
			{
                
				//Destroy(vb.gameObject);
                vb.OnCloseUI();
				ResourceMgr.Instance.DestroyAsset(vb.gameObject);
			}
		}
	}
}
