using UnityEngine;
using System.Collections;

public class UIMgr : MonoBehaviour 
{
	static UIMgr mInst = null;
	public static UIMgr Instance
	{
		get
		{
			if (mInst == null)
			{
				Object r = PrefabLoadMgr.LoadUIPrefab("UIRoot");
				GameObject ui = Instantiate(r) as GameObject;
				ui.name = "UIMgr";
				mInst = ui.AddComponent<UIMgr>();
			}
			return mInst;
		}
	}
	public static int PopUpViewDepth = 50;
	public void Init()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void OpenUI(string uiName)
	{
		Object r = PrefabLoadMgr.LoadUIPrefab (uiName);
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

	/*
	 * Load UIPrefab  临时使用
	 */
	static public GameObject LoadUIPrefab(string name)
	{
		GameObject prefab = Resources.LoadAssetAtPath("Assets/cmsAsset/prefabs/UI/" + name + ".prefab", typeof(GameObject)) as GameObject;
		if (null == prefab) 
		{
			Debug.LogError("Error for load uiprefab " + name);
		}
		return prefab;
	}
}
