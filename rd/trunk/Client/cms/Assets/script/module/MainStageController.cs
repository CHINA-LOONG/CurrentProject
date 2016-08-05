using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainStageController : MonoBehaviour
{
    public Transform mRecPos;
    public Transform mCentrePos;
    //public float mInitAngle = 0.0f;
    public float mMaxYawAngle = 90.0f;
    public float mMinYawAngle = -90.0f;
    public float mBoundAngle = 10.0f;
    public float mMoveSpeed = 1.0f;
    public float mBoundsSpeed = 100.0f;

    //list all clickable objects
    public SelectableObj mInstanceObj;
    public SelectableObj mTowerShilianObj;
    public SelectableObj mTowerJuewangObj;
    public SelectableObj mTowerSiwangObj;
    public SelectableObj mHoleJinbiObj;
    public SelectableObj mHoleJingyanObj;
    //lise all group
    public SelectableObjGroup mTowerGroup;
    public SelectableObjGroup mHoleGroup;

    //ui ref
    private UIHoleEntry mUIHoleEntry;
    private UIHole mUIHole;
    private UITowerEntry mUITowerEntry;
    private UITower mUITower;

    //private float mDistanceList;
    private float mCurYawAngle = 0.0f;
    //private Matrix4x4 mLtoW = new Matrix4x4();
    //private Matrix4x4 mLTransform = new Matrix4x4();
    //private float mRadius;
    private float mMaxNormalAngle;
    private float mMinNormalAngle;
    private SelectableObj mCurrentSelectedObj;
    private SelectableObjGroup mCurrentSelectedObjGroup;
    private bool mBeginDrag;
    private float mRotAngle = 0.0f;
    private bool mDisableMove = false;
    //---------------------------------------------------------------------------------------------
    void OnEnable()
    {
        GameEventMgr.Instance.AddListener(GameEventList.DailyRefresh, OnDailyRefresh);
    }
    //---------------------------------------------------------------------------------------------
    public void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener(GameEventList.DailyRefresh, OnDailyRefresh);

        if (UIMgr.IsUIDestroyed() == false)
        {
            UIMgr mgr = UIMgr.Instance;
            if (mgr != null)
            {
                mgr.DestroyUI(mUITowerEntry);
                mgr.DestroyUI(mUIHoleEntry);
                mgr.DestroyUI(mUIHole);
                mgr.DestroyUI(mUITower);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnDailyRefresh()
    {
        RefreshHoleState();
    }
    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Start ()
    {
        //create camera axis
        //Vector3 xDis = mRecPos.position - mCentrePos.position;
        ////y is the up axis, so ignore the y deleta
        //Vector3 xAxis = new Vector3(xDis.x, 0.0f, xDis.z);
        //xAxis.Normalize();
        //Vector3 yAxis = new Vector3(0.0f, 1.0f, 0.0f);
        //Vector3 zAxis = Vector3.Cross(xAxis, yAxis);
        //mLtoW.SetColumn(0, xAxis);
        //mLtoW.SetColumn(1, yAxis);
        //mLtoW.SetColumn(2, zAxis);
        //mLtoW.SetColumn(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        //mRadius = Vector3.Distance(mRecPos.position, mCentrePos.position);
        mRotAngle = 0.0f;
        mMaxNormalAngle = mMaxYawAngle - mBoundAngle;
        mMinNormalAngle = mMinYawAngle + mBoundAngle;
        Camera.main.transform.SetParent(mRecPos, false);
        ResetCameraPos(GameDataMgr.Instance.mainStageRotAngle);
        mBeginDrag = false;

        BuildModule curModule = GameMain.Instance.GetBuildModule();
        if (curModule != null)
        {
            if (GameDataMgr.Instance.curInstanceType == (int)InstanceType.Hole)
            {
                SetCurrentSelectGroup((int)InstanceType.Hole);
            }
            else if (GameDataMgr.Instance.curInstanceType == (int)InstanceType.Tower)
            {
                SetCurrentSelectGroup((int)InstanceType.Tower);
                mUITower = UITower.OpenTower((int)GameDataMgr.Instance.curTowerType);
                if (
                    curModule.CurrentInitState == (int)ExitInstanceType.Exit_Instance_Next ||
                    curModule.CurrentInitState == (int)ExitInstanceType.Exit_Instance_Retry
                    )
                {

                }
            }
        }

        RefreshHoleState();
        RefreshTowerState();
        //test only
        //mTowerSiwangObj.SetState(SelectableObjState.State_Disabled);
    }
    //---------------------------------------------------------------------------------------------
    public void RefreshHoleState()
    {
        mHoleJingyanObj.SetState(SelectableObjState.State_Normal);
        mHoleJinbiObj.SetState(SelectableObjState.State_Normal);
        int count = GameDataMgr.Instance.mHoleStateList.Count;
        for (int i = 0; i < count; ++i)
        {
            PB.HoleState curHoleState = GameDataMgr.Instance.mHoleStateList[i];
            if (curHoleState.isOpen == false)
            {
                if (curHoleState.holeId == (int)HoleType.Hole_Exp)
                {
                    mHoleJingyanObj.SetState(SelectableObjState.State_Disabled);
                }
                else if (curHoleState.holeId == (int)HoleType.Hole_Jingbi)
                {
                    mHoleJinbiObj.SetState(SelectableObjState.State_Disabled);
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void RefreshTowerState()
    {
        mTowerShilianObj.SetState(SelectableObjState.State_Normal);
        mTowerSiwangObj.SetState(SelectableObjState.State_Normal);
        mTowerJuewangObj.SetState(SelectableObjState.State_Normal);

        if (IsTowerOpen(TowerType.Tower_Shilian) == false)
        {
            mTowerShilianObj.SetState(SelectableObjState.State_Disabled);
        }

        if (IsTowerOpen(TowerType.Tower_Juewang) == false)
        {
            mTowerJuewangObj.SetState(SelectableObjState.State_Disabled);
        }

        if (IsTowerOpen(TowerType.Tower_Siwang) == false)
        {
            mTowerSiwangObj.SetState(SelectableObjState.State_Disabled);
        }
    }
    //---------------------------------------------------------------------------------------------
    private bool IsTowerOpen(TowerType towerType)
    {
        TowerData curTower = StaticDataMgr.Instance.GetTowerData((int)towerType);
        if (curTower != null)
        {
            return GameDataMgr.Instance.PlayerDataAttr.LevelAttr >= curTower.level;
        }
        return false;
    }
    //---------------------------------------------------------------------------------------------
    void OnDestroy()
    {
    }
    //---------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update ()
    {
        bool isMouseOnUI = false;

#if UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonDown(0))
        {
            isMouseOnUI = EventSystem.current.IsPointerOverGameObject();
            if (isMouseOnUI == false)
            {
                mBeginDrag = true;
                //raycast 3d objects
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    SelectableObjGroup currentSelectedGroup = hit.collider.gameObject.GetComponent<SelectableObjGroup>();
                    if (currentSelectedGroup != null)
                    {
                        mCurrentSelectedObjGroup = currentSelectedGroup;
                        mCurrentSelectedObjGroup.SetState(SelectableObjState.State_Selected);
                        //mBeginDrag = false;
                    }
                    else
                    {
                        mCurrentSelectedObj = hit.collider.gameObject.GetComponent<SelectableObj>();
                        if (mCurrentSelectedObj != null)
                        {
                            mCurrentSelectedObj.SetState(SelectableObjState.State_Selected);
                            //mBeginDrag = false;
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButton(0) && mDisableMove == false)
        {
            if (mBeginDrag == true)
            {
                float moveLen = Input.GetAxis("Mouse X");
                if (moveLen != 0.0f)
                {
                    float curRotAngle = moveLen * mMoveSpeed * Time.deltaTime;
                    mCurYawAngle += curRotAngle;
                    if (mCurYawAngle > mMaxYawAngle)
                    {
                        curRotAngle = mMaxYawAngle - (mCurYawAngle - curRotAngle);
                        mCurYawAngle = mMaxYawAngle;
                    }
                    else if (mCurYawAngle < mMinYawAngle)
                    {
                        curRotAngle = mMinYawAngle - (mCurYawAngle - curRotAngle);
                        mCurYawAngle = mMinYawAngle;
                    }
                    ResetCameraPos(curRotAngle);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mBeginDrag = false;

            if (mCurrentSelectedObj != null)
            {
                mCurrentSelectedObj.SetState(SelectableObjState.State_Normal);
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    SelectableObj curUpSelectedObj = hit.collider.gameObject.GetComponent<SelectableObj>();
                    if (curUpSelectedObj == mCurrentSelectedObj)
                    {
                        OnSelectableObjClicked(mCurrentSelectedObj);
                    }
                }
                mCurrentSelectedObj = null;
            }

            if (mCurrentSelectedObjGroup != null && mDisableMove == false)
            {
                mCurrentSelectedObjGroup.SetState(SelectableObjState.State_Normal);
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    SelectableObjGroup curGroup = hit.collider.gameObject.GetComponent<SelectableObjGroup>();
                    if (curGroup == mCurrentSelectedObjGroup)
                    {
                        EnterSelectGroup(ref mCurrentSelectedObjGroup);
                    }
                    else
                    {
                        mCurrentSelectedObjGroup = null;
                    }
                }
                else
                {
                    mCurrentSelectedObjGroup = null;
                }
            }
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            isMouseOnUI = EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            if (isMouseOnUI == false)
            {
                mBeginDrag = true;
                //raycast 3d objects
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    SelectableObjGroup currentSelectedGroup = hit.collider.gameObject.GetComponent<SelectableObjGroup>();
                    if (currentSelectedGroup != null)
                    {
                        mCurrentSelectedObjGroup = currentSelectedGroup;
                        mCurrentSelectedObjGroup.SetState(SelectableObjState.State_Selected);
                        //mBeginDrag = false;
                    }
                    else
                    {
                        mCurrentSelectedObj = hit.collider.gameObject.GetComponent<SelectableObj>();
                        if (mCurrentSelectedObj != null)
                        {
                            mCurrentSelectedObj.SetState(SelectableObjState.State_Selected);
                            //mBeginDrag = false;
                        }
                    }
                }
            }
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            if (mBeginDrag == true)
            {
                float moveLen = Input.GetTouch(0).deltaPosition.x;
#if UNITY_ANDROID
                moveLen *= 0.5f;
#endif
#if UNITY_IPHONE
                moveLen *= 0.1f;
#endif
                if (moveLen != 0.0f)
                {
                    //Vector3 movePos = new Vector3(moveLen * Time.deltaTime * mMoveSpeed, 0.0f, 0.0f);
                    //Camera.main.transform.localPosition += movePos;

                    float curRotAngle = moveLen * mMoveSpeed * Time.deltaTime;
                    mCurYawAngle += curRotAngle;
                    if (mCurYawAngle > mMaxYawAngle)
                    {
                        curRotAngle = mMaxYawAngle - (mCurYawAngle - curRotAngle);
                        mCurYawAngle = mMaxYawAngle;
                    }
                    else if (mCurYawAngle < mMinYawAngle)
                    {
                        curRotAngle = mMinYawAngle - (mCurYawAngle - curRotAngle);
                        mCurYawAngle = mMinYawAngle;
                    }
                    ResetCameraPos(curRotAngle);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mBeginDrag = false;

            if (mCurrentSelectedObj != null)
            {
                mCurrentSelectedObj.SetState(SelectableObjState.State_Normal);
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    SelectableObj curUpSelectedObj = hit.collider.gameObject.GetComponent<SelectableObj>();
                    if (curUpSelectedObj == mCurrentSelectedObj)
                    {
                        OnSelectableObjClicked(mCurrentSelectedObj);
                    }
                }
                mCurrentSelectedObj = null;
            }

            if (mCurrentSelectedObjGroup != null && mDisableMove == false)
            {
                mCurrentSelectedObjGroup.SetState(SelectableObjState.State_Normal);
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    SelectableObjGroup curGroup = hit.collider.gameObject.GetComponent<SelectableObjGroup>();
                    if (curGroup == mCurrentSelectedObjGroup)
                    {
                        EnterSelectGroup(ref mCurrentSelectedObjGroup);
                    }
                    else
                    {
                        mCurrentSelectedObjGroup = null;
                    }
                }
                else
                {
                    mCurrentSelectedObjGroup = null;
                }
            }
        }

#endif
        if (mBeginDrag == false && mDisableMove == false)
        {
            //bounds back if necessary
            if (mCurYawAngle < mMinNormalAngle)
            {
                //0.3 simulate 1/3 fingle move
                float adjustRotAngle = 0.3f * mBoundsSpeed * Time.deltaTime;
                mCurYawAngle += adjustRotAngle;
                if (mCurYawAngle > mMinNormalAngle)
                {
                    adjustRotAngle = mMinNormalAngle - (mCurYawAngle - adjustRotAngle);
                    mCurYawAngle = mMinNormalAngle;
                }
                ResetCameraPos(adjustRotAngle);
            }
            else if (mCurYawAngle > mMaxNormalAngle)
            {
                //0.3 simulate 1/3 fingle move
                float adjustRotAngle = -0.3f * mBoundsSpeed * Time.deltaTime;
                mCurYawAngle += adjustRotAngle;
                if (mCurYawAngle < mMaxNormalAngle)
                {
                    adjustRotAngle = mMaxNormalAngle - (mCurYawAngle - adjustRotAngle);
                    mCurYawAngle = mMaxNormalAngle;
                }
                ResetCameraPos(adjustRotAngle);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SaveCurrentRot()
    {
        GameDataMgr.Instance.mainStageRotAngle = mRotAngle;
    }
    //---------------------------------------------------------------------------------------------
    private void OnSelectableObjClicked(SelectableObj selectedObj)
    {
        if (selectedObj != null)
        {
            if (selectedObj.CurState == SelectableObjState.State_Disabled)
            {
                UIIm.Instance.ShowSystemHints("function locked", (int)PB.ImType.PROMPT);
                return;
            }

            //普通副本
            if (selectedObj == mInstanceObj)
            {
                UIBuild uiBuild = UIMgr.Instance.GetUI(UIBuild.ViewName) as UIBuild;
                if (uiBuild != null)
                {
                    uiBuild.OpenInstanceUI();
                }
            }
            //死亡之塔
            else if (selectedObj == mTowerSiwangObj)
            {
                GameDataMgr.Instance.curTowerType = TowerType.Tower_Siwang;
                mUITower = UITower.OpenTower((int)TowerType.Tower_Siwang);
            }
            //试炼之塔
            else if (selectedObj == mTowerShilianObj)
            {
                GameDataMgr.Instance.curTowerType = TowerType.Tower_Shilian;
                mUITower = UITower.OpenTower((int)TowerType.Tower_Shilian);
            }
            //绝望之塔
            else if (selectedObj == mTowerJuewangObj)
            {
                GameDataMgr.Instance.curTowerType = TowerType.Tower_Juewang;
                mUITower = UITower.OpenTower((int)TowerType.Tower_Juewang);
            }
            //金币洞穴
            else if (selectedObj == mHoleJinbiObj)
            {
                if (mUIHoleEntry.IsDailyCountFull(HoleType.Hole_Jingbi) == true)
                {
                    UIIm.Instance.ShowSystemHints("no play count", (int)PB.ImType.PROMPT);
                }
                else
                {
                    mUIHole = UIHole.OpenHole((int)HoleType.Hole_Jingbi);
                }
            }
            //经验洞穴
            else if (selectedObj == mHoleJingyanObj)
            {
                if (mUIHoleEntry.IsDailyCountFull(HoleType.Hole_Exp) == true)
                {
                    UIIm.Instance.ShowSystemHints("no play count", (int)PB.ImType.PROMPT);
                }
                else
                {
                    mUIHole = UIHole.OpenHole((int)HoleType.Hole_Exp);
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    private void EnterSelectGroup(ref SelectableObjGroup selectGroup)
    {
        if (selectGroup != null)
        {
            mDisableMove = true;
            if (selectGroup.CurState == SelectableObjState.State_Disabled)
            {
                selectGroup = null;
                return;
            }
            selectGroup.SetLookAtState(true);
            Camera.main.transform.SetParent(selectGroup.mCameraPos, false);

            if (selectGroup == mTowerGroup)
            {
                mUITowerEntry = UIMgr.Instance.OpenUI_("UITowerEntry") as UITowerEntry;
                mUITowerEntry.SetMainStageControl(this);
                RefreshTowerState();
            }
            else if (selectGroup == mHoleGroup)
            {
                mUIHoleEntry = UIMgr.Instance.OpenUI_("UIHoleEntry") as UIHoleEntry;
                mUIHoleEntry.SetMainStageControl(this);
                RefreshHoleState();
            }

            //TODO: set outside instead of deactive
            UIBase uiBuild = UIMgr.Instance.GetUI(UIBuild.ViewName);
            uiBuild.gameObject.SetActive(false);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void QuitSelectGroup()
    {
        if (mCurrentSelectedObjGroup != null)
        {
            mDisableMove = false;
            mCurrentSelectedObjGroup.SetLookAtState(false);

            mRotAngle = 0.0f;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
            Camera.main.transform.SetParent(mRecPos, false);
            //ResetCameraPos(GameDataMgr.Instance.mainStageRotAngle);

            if (mCurrentSelectedObjGroup == mTowerGroup)
            {
                UIMgr.Instance.DestroyUI(mUITowerEntry);
                mUITowerEntry = null;
            }
            else if (mCurrentSelectedObjGroup == mHoleGroup)
            {
                UIMgr.Instance.DestroyUI(mUIHoleEntry);
                mUIHoleEntry = null;
            }

            //TODO: set outside instead of deactive
            UIBase uiBuild = UIMgr.Instance.GetUI(UIBuild.ViewName);
            uiBuild.gameObject.SetActive(true);
        }
    }
    //---------------------------------------------------------------------------------------------
    //NOTE: for return back from battle only
    public void SetCurrentSelectGroup(int instanceType)
    {
        if (instanceType == (int)InstanceType.Hole)
        {
            mCurrentSelectedObjGroup = mHoleGroup;
        }
        else if (instanceType == (int)InstanceType.Tower)
        {
            mCurrentSelectedObjGroup = mTowerGroup;
        }
        EnterSelectGroup(ref mCurrentSelectedObjGroup);
    }
    //---------------------------------------------------------------------------------------------
    private void ResetCameraPos(float yawAngle) 
    {
        //Quaternion rot = Quaternion.AngleAxis(yawAngle, Vector3.up);
        //mLTransform.SetTRS(new Vector3(mRadius, 0.0f, 0.0f), rot, Vector3.one);
        //Camera.main.transform.RotateAround(mCentrePos.position, Vector3.up, yawAngle);
        if (yawAngle != 0.0f)
        {
            mRotAngle += yawAngle;
            mRecPos.transform.RotateAround(mCentrePos.position, Vector3.up, -yawAngle);
            SaveCurrentRot();
        }
    }
    //---------------------------------------------------------------------------------------------

}
