using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

    private struct uiRootData
    {
        public Transform modelTransform;
        public Transform uiPanelTransform;
        public Transform topPanelTransform;
    }
    private uiRootData uiNormalData = new uiRootData();
    private uiRootData uiTopData = new uiRootData();

    private static GameObject uiRootNormal;
    private static GameObject uiRootTop;

	static UIMgr mInst = null;
	public static UIMgr Instance
	{
		get
		{
			if (mInst == null)
			{
                //GameObject ui = GameObject.Find("/UIRoot");
                uiRootNormal = GameObject.Find("/UIRoot");
                uiRootTop = GameObject.Find("/UIRootTop");

                mInst = uiRootNormal.AddComponent<UIMgr>();
                DontDestroyOnLoad(uiRootNormal);
                DontDestroyOnLoad(uiRootTop);
			}
			return mInst;
		}
	}

    Dictionary<string, UIBase> uiList = new Dictionary<string, UIBase>();
    List<UIBase> popupList = new List<UIBase>();

	void Start()
	{
		m_rootRectTransform = transform as RectTransform;
	}

	public void Init()
	{
		canVas = gameObject.GetComponent<Canvas> ();

		UICamera.Instance.Init ();
		canVas.worldCamera = UICamera.Instance.CameraAttr;
        InitUIRootData(uiRootNormal, ref uiNormalData);
        InitUIRootData(uiRootTop, ref uiTopData);
    }

    public void SetModelView(Transform model,int index)
    {
        UIUtil.SetParentReset(model, uiNormalData.modelTransform, new Vector3(1000f * index, 0f, 1000f));
    }

    public UIBase GetUI(string uiName)
    {
        if (string.IsNullOrEmpty(uiName))
        {
            return null;
        }
        UIBase uiItem = null;
        uiList.TryGetValue(uiName, out uiItem);
        return uiItem;
    }

    private void InitUIRootData(GameObject rootObj, ref uiRootData rootData)
    {
        GameObject viewGo = Util.FindChildByName(rootObj, "modelParent");
        rootData.modelTransform = viewGo.transform;
        GameObject uiGo = Util.FindChildByName(rootObj, "uiPanel");
        rootData.uiPanelTransform = uiGo.transform;
        GameObject topGo = Util.FindChildByName(rootObj, "topPanel");
        rootData.topPanelTransform = topGo.transform;
    }

    UIBase CreateUI(string uiName,bool cache=true)
    {
        UIBase uiItem = null;
        GameObject ui = ResourceMgr.Instance.LoadAsset(uiName);
        if (null == ui)
        {
            return null;
        }
        uiItem = ui.GetComponent<UIBase>();
        ui.name = uiName;
        if (uiItem.ViewTypeAttr == UIBase.ViewType.VT_POPUP)
        {
            UIUtil.SetParentReset(ui.transform, uiNormalData.topPanelTransform);
            popupList.Add(uiItem);
        }
        else if (uiItem.ViewTypeAttr == UIBase.ViewType.VT_NORMALTOP)
        {
            UIUtil.SetParentReset(ui.transform, uiTopData.uiPanelTransform);
            if (cache)
            {
                uiList.Add(uiName, uiItem);
            }
        }
        else if (uiItem.ViewTypeAttr == UIBase.ViewType.VT_POPUPTOP)
        {
            UIUtil.SetParentReset(ui.transform, uiTopData.topPanelTransform);
            popupList.Add(uiItem);
        }
        else
        {
            UIUtil.SetParentReset(ui.transform, uiNormalData.uiPanelTransform);
            if (cache)
            {
                uiList.Add(uiName, uiItem);
            }
        }

        return uiItem;
    }

    public UIBase OpenUI_(string uiName,bool cache=true)
    {
        UIBase uiItem=null;
        if (cache)
        {
            uiItem = GetUI(uiName);
            if (uiItem != null)
            {
                uiItem.gameObject.SetActive(true);
            }
        }
        if (uiItem == null)
        {
            uiItem = CreateUI(uiName, cache);
        }
        uiItem.transform.SetAsLastSibling();
        uiItem.Init();
        return uiItem;
    }

    public void CloseUI_(string uiName)
    {
        CloseUI_(GetUI(uiName));
    }

    public void CloseUI_(UIBase uiItem)
    {
        if (uiItem != null)
        {
            if (uiList.ContainsValue(uiItem))
            {
                uiItem.gameObject.SetActive(false);
            }
            else
            {
                DestroyUI(uiItem);
            }
        }
    }

    public void DestroyUI(UIBase uiItem)
    {
        if (uiItem == null)
        {
            return;
        }
        if (uiList.ContainsValue(uiItem))
        {
            string uiName = "";
            foreach (var item in uiList)
            {
                if (item.Value == uiItem)
                {
                    uiName = item.Key;
                    break;
                }
            }
            uiList.Remove(uiName);
        }
        if (popupList.Contains(uiItem))
        {
            popupList.Remove(uiItem);
        }
        uiItem.Clean();
        ResourceMgr.Instance.DestroyAsset(uiItem.gameObject);
    }

    public void DestroyAllPopup()
    {
        for (int i = popupList.Count - 1; i >= 0; i--)
        {
            if (popupList[i] == null)
                continue;
            ResourceMgr.Instance.DestroyAsset(popupList[i].gameObject);
        }
        popupList.Clear();
    }
}
