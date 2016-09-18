using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LoadUIEventArgs : System.EventArgs
{
    public LoadUIEventArgs(
        AssetLoadedCallBack assetCallBack,
        string uiName,
        bool cache
        )
    {
        this.assetCallBack = assetCallBack;
        this.uiName = uiName;
        this.cache = cache;
    }

    public bool cache;
    public string uiName;
    public AssetLoadedCallBack assetCallBack;
}

public class UIBaseData
{
    public UIBase uiItem;
    public int uiIndex;
}

public class UIMgr : MonoBehaviour
{
    [SerializeField]
    RectTransform m_rootRectTransform;
    public RectTransform RootRectTransform
    {
        get
        {
            return m_rootRectTransform;
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
    private HashSet<string> mRequestUIList = new HashSet<string>();

    public MainStageController MainstageInstance
    {
        set { mMainstageController = value; }
        get { return mMainstageController; }
    }
    private MainStageController mMainstageController;

    public float curParkTime;
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

    static string chineseTxt = null;
    public UnityEngine.Font baseFont;

    public static bool IsUIDestroyed()
    {
        return mInst == null;
    }

    Dictionary<string, UIBase> uiList = new Dictionary<string, UIBase>();
    List<UIBase> popupList = new List<UIBase>();

    public List<UIBase> stackList = new List<UIBase>();
    public List<UIBase> uiLayerList = new List<UIBase>();

    void Start()
    {
        m_rootRectTransform = transform as RectTransform;
    }

    public void Init()
    {
        canVas = gameObject.GetComponent<Canvas>();

        UICamera.Instance.Init();
        canVas.worldCamera = UICamera.Instance.CameraAttr;
        InitUIRootData(uiRootNormal, ref uiNormalData);
        InitUIRootData(uiRootTop, ref uiTopData);
    }

    public void SetModelView(Transform model, int index)
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
    public void ChangeRoot(UIBase ui, bool isTop)
    {
        if (isTop == true)
        {
            if (ui.ViewTypeAttr == UIBase.ViewType.VT_POPUP)
            {
                UIUtil.SetParentReset(ui.transform, uiTopData.topPanelTransform);
            }
            else
            {
                UIUtil.SetParentReset(ui.transform, uiTopData.uiPanelTransform);
            }
        }
        else
        {
            if (ui.ViewTypeAttr == UIBase.ViewType.VT_POPUP)
            {
                UIUtil.SetParentReset(ui.transform, uiNormalData.topPanelTransform);
            }
            else
            {
                UIUtil.SetParentReset(ui.transform, uiNormalData.uiPanelTransform);
            }
        }
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
    public UIBase OpenUI_(string uiName, bool cache = true)
    {
        UIBase uiItem = null;
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
            GameObject ui = ResourceMgr.Instance.LoadAsset(uiName);
            if (null == ui)
            {
                return null;
            }
            SetUIInternal(ref uiItem, ui, uiName, cache);
        }
        uiItem.transform.SetAsLastSibling();
        uiItem.Init();
        //显示管理
        if (!string.Equals(uiName, UINetRequest.ViewName))
        {
            AddedToStack(uiItem);
        }

        if (cache)
        {
            OnUILayerOpen(uiItem);
        }
        return uiItem;
    }

    private void SetUIInternal(ref UIBase uiItem, GameObject ui, string uiName, bool cache)
    {
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
    }
    
    private void OnUILayerOpen(UIBase uiItem)
    {
        if (
            uiItem.IgnorePreviousHide == false && 
            uiItem.ViewTypeAttr != UIBase.ViewType.VT_POPUP &&
            uiItem.ViewTypeAttr != UIBase.ViewType.VT_POPUPTOP
            )
        {
            int count = uiLayerList.Count;
            for (int i = 0; i < count; ++i)
            {
                if (uiLayerList[i] == uiItem)
                {
                    uiLayerList.Remove(uiItem);
                    break;
                }
            }

            uiLayerList.Add(uiItem);
            if (uiItem.HidePreviousUI == true)
            {
                count = uiLayerList.Count;
                if (count > 1)
                {
                    HidePreviousUIRecurs(count - 2);
                    //UIBase previousUI = uiLayerList[count - 2];
                    //if (previousUI != null)
                    //{
                    //    previousUI.gameObject.SetActive(false);
                    //}
                    //else
                    //{
                    //    Debug.LogError("null previous ui");
                    //}
                }
            }
        }
    }

    private void HidePreviousUIRecurs(int index)
    {
        if (index >= 0 && index < uiLayerList.Count)
        {
            UIBase previousUI = uiLayerList[index];
            if (previousUI != null)
            {
                previousUI.gameObject.SetActive(false);
                if (previousUI.HidePreviousUI == false)
                {
                    HidePreviousUIRecurs(index - 1);
                }
            }
            else
            {
                Debug.LogError("null previous ui");
            }
        }
        else
        {
            Debug.LogError(string.Format("invalidate hide previous ui index {0}", index));
        }
    }

    private void ShowPreviousUIRecurs(int index)
    {
        if (index >= 0 && index < uiLayerList.Count)
        {
            UIBase previousUI = uiLayerList[index];
            if (previousUI != null)
            {
                previousUI.gameObject.SetActive(true);
                previousUI.RefreshOnPreviousUIHide();
                if (previousUI.HidePreviousUI == false)
                {
                    ShowPreviousUIRecurs(index - 1);
                }
            }
            else
            {
                Logger.LogError("null previous ui");
            }
        }
        else
        {
            Logger.LogError(string.Format("invalidate show previous ui index {0}", index));
        }
    }
    private void OnUILayerClose(UIBase uiItem)
    {
        if (
            uiItem.IgnorePreviousHide == false &&
            uiItem.ViewTypeAttr != UIBase.ViewType.VT_POPUP &&
            uiItem.ViewTypeAttr != UIBase.ViewType.VT_POPUPTOP
            )
        {
            int count = uiLayerList.Count;
            for (int i = 0; i < count; ++i)
            {
                if (uiLayerList[i] == uiItem)
                {
                    uiLayerList.Remove(uiItem);
                    break;
                }
            }

            count = uiLayerList.Count;
            if (count > 0)
            {
                ShowPreviousUIRecurs(count - 1);
                //UIBase previousUI = uiLayerList[count - 1];
                //if (previousUI != null)
                //{
                //    previousUI.gameObject.SetActive(true);
                //}
                //else
                //{
                //    Debug.LogError("null previous ui");
                //}
            }
        }
    }

    public void ClearUILayerList()
    {
        uiLayerList.Clear();
    }

    public void OpenUICallback(GameObject ui, System.EventArgs args)
    {
        LoadUIEventArgs uiEventArgs = args as LoadUIEventArgs;
        if (null != ui && uiEventArgs != null)
        {
            UIBase uiItem = null;
            SetUIInternal(ref uiItem, ui, uiEventArgs.uiName, uiEventArgs.cache);
            uiItem.transform.SetAsLastSibling();
            uiItem.Init();
            //显示管理
            if (string.Equals(uiEventArgs.uiName, UINetRequest.ViewName) == false)
            {
                AddedToStack(uiItem);
            }

            if (uiEventArgs.assetCallBack != null)
            {
                uiEventArgs.assetCallBack(ui, args);
            }

            mRequestUIList.Remove(uiEventArgs.uiName);

            if (uiEventArgs.cache == true)
            {
                OnUILayerOpen(uiItem);
            }
        }
    }

    public void OpenUIAsync(string uiName, bool cache = true, AssetLoadedCallBack callback = null)
    {
        UIBase uiItem = null;
        if (cache)
        {
            uiItem = GetUI(uiName);
            if (uiItem != null)
            {
                uiItem.gameObject.SetActive(true);
                uiItem.transform.SetAsLastSibling();
                uiItem.Init();
                //显示管理
                if (string.Equals(uiName, UINetRequest.ViewName) == false)
                {
                    AddedToStack(uiItem);
                }

                OnUILayerOpen(uiItem);
            }
        }
        if (uiItem == null)
        {
            if (mRequestUIList.Contains(uiName) == false)
            {
                mRequestUIList.Add(uiName);
                AssetRequest requestUI = new AssetRequest(uiName);
                requestUI.assetCallBack = OpenUICallback;
                requestUI.args = new LoadUIEventArgs(callback, uiName, cache);
                ResourceMgr.Instance.LoadAssetAsyn(requestUI);
            }
        }
    }

    public void CloseUI_(string uiName)
    {
        CloseUI_(GetUI(uiName));
    }
    public void CloseUI_(UIBase uiItem)
    {
        if (uiItem != null)
        {
            OnUILayerClose(uiItem);
            if (uiList.ContainsValue(uiItem))
            {
                uiItem.gameObject.SetActive(false);
                RemoveFromStack(uiItem);
            }
            else
            {
                DestroyUI(uiItem);
            }

        }
    }

    public void DestroyUI(UIBase uiItem)
    {
        if (uiItem != null)
        {
            OnUILayerClose(uiItem);
            RemoveFromStack(uiItem);
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
    }
    public void DestroyAllPopup()
    {
        for (int i = popupList.Count - 1; i >= 0; i--)
        {
            if (popupList[i] == null)
                continue;
            RemoveFromStack(popupList[i]);
            ResourceMgr.Instance.DestroyAsset(popupList[i].gameObject);
        }
        popupList.Clear();
    }

    public void ShowUI(UIBase uiItem)
    {
        if (!uiItem.gameObject.activeSelf)
        {
            uiItem.gameObject.SetActive(true);
            //AddedToStack(uiItem);
        }
    }
    public void HideUI(UIBase uiItem)
    {
        if (uiItem.gameObject.activeSelf)
        {
            uiItem.gameObject.SetActive(false);
            //RemoveFromStack(uiItem);
        }
    }


    public void AddedToStack(UIBase uiItem)
    {
        if (stackList.Contains(uiItem))
        {
            stackList.Remove(uiItem);
        }
        stackList.Add(uiItem);
            //Logger.LogError("add ：" + uiItem.gameObject.name);

    }
    public void RemoveFromStack(UIBase uiItem)
    {
        if (stackList.Contains(uiItem))
        {
            stackList.Remove(uiItem);
            //Logger.LogError("remove ：" + uiItem.gameObject.name);
        }
    }
    public UIBase GetCurrentUI()
    {
        if (stackList.Count > 0)
        {
            return stackList[stackList.Count - 1];
        }
        return null;
        //for (int i = stackList.Count - 1; i >= 0; i++)
        //{
        //    if (stackList[i]!=null)
        //    {
        //        return stackList[i];
        //    }
        //}
        //return null;
    }


    public void OpenTowerAdjustTeam()
    {
        string nextInstance;
        int nextFloor;
        GameDataMgr.Instance.GetNextTowerFloor(out nextInstance, out nextFloor);
        if (nextInstance != null)
        {
            UIAdjustBattleTeam.OpenWith(nextInstance, 0, false,InstanceType.Tower, nextFloor);
        }
    }

    public void OnKickPlayer(ProtocolMessage msg)//顶号
    {
        PB.HSKickPlayer result = msg.GetProtocolBody<PB.HSKickPlayer>();
        if (result.reason == 1 )
        {
        }

        SocketClient.Logout();
        curParkTime = 0.0f;
        MsgBox.PromptMsg.Open(
         MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("login_dinghao_001"), SetKickPlayre);
    }
    void Update()
    {
        if (GameMain.Instance.IsCurModule<BattleModule>())
        {
            curParkTime = 0f;
        }
        if (curParkTime != 0)
        {
            if (Time.time - curParkTime >= 10.0f)
            {
                MsgBox.PromptMsg.Open(
                MsgBox.MsgBoxType.Conform, StaticDataMgr.Instance.GetTextByID("login_duankai_001"), SetKickPlayre);
                curParkTime = 0f;
            }
        }
    }

    public void SetKickPlayre(MsgBox.PrompButtonClick state)
    {
        if (state == MsgBox.PrompButtonClick.OK)
        {
            curParkTime = 0.0f;
            //重置数据
            GameDataMgr.Instance.ClearAllData();
            //destroy all ui
            DestroyAllPopup();
            int count = uiList.Count;
            List<UIBase> tmpList = new List<UIBase>();
            tmpList.AddRange(uiList.Values);
            for (int i = count -1; i >= 0; --i)
            {
                RemoveFromStack(tmpList[i]);
                tmpList[i].Clean();
                ResourceMgr.Instance.DestroyAsset(tmpList[i].gameObject);
            }
            tmpList.Clear();
            uiList.Clear();
            ClearUILayerList();

            //跳转到登录
            GameMain.Instance.ChangeModule<LoginModule>();
        }
    }
    //------------------------------------------------------------------------------------------------------
    void OnEnable()
    {
        BindListener();
    }
    //------------------------------------------------------------------------------------------------------
    void OnDisable()
    {
        UnBindListener();
    }
    //------------------------------------------------------------------------------------------------------
    void BindListener()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.KICKOUT_S.GetHashCode().ToString(), OnKickPlayer);
    }
    //------------------------------------------------------------------------------------------------------
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.KICKOUT_S.GetHashCode().ToString(), OnKickPlayer);
    }
    //------------------------------------------------------------------------------------------------------
    public void FixBrokenWord()
    {
        return;
        if (chineseTxt == null)
        {
            TextAsset txt = Resources.Load("commonText") as TextAsset;
            chineseTxt = txt.ToString();
        }

        baseFont.RequestCharactersInTexture(chineseTxt);
    }
    //------------------------------------------------------------------------------------------------------
}
