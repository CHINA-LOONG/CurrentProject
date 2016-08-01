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

    //private float mDistanceList;
    private float mCurYawAngle = 0.0f;
    //private Matrix4x4 mLtoW = new Matrix4x4();
    //private Matrix4x4 mLTransform = new Matrix4x4();
    //private float mRadius;
    private float mMaxNormalAngle;
    private float mMinNormalAngle;
    private SelectableObj mCurrentSelectedObj;
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
        mMaxNormalAngle = mMaxYawAngle - mBoundAngle;
        mMinNormalAngle = mMinYawAngle + mBoundAngle;
        Camera.main.transform.SetParent(mRecPos, false);
        ResetCameraPos(0.0f);
    }
    //---------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update ()
    {
        bool isMouseOnUI = false;
#if UNITY_ANDROID
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            float moveLen = Input.GetTouch(0).deltaPosition.x;
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
        else
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
#endif

#if UNITY_IPHONE
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            float moveLen = Input.GetTouch(0).deltaPosition.x;
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
        else
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
#endif

#if UNITY_STANDALONE_WIN
        if (Input.GetMouseButton(0))
        {
            isMouseOnUI = EventSystem.current.IsPointerOverGameObject();
            if (isMouseOnUI == false)
            {
                float moveLen = Input.GetAxis("Mouse X");
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

                //raycast 3d objects
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    mCurrentSelectedObj = hit.collider.gameObject.GetComponent<SelectableObj>();
                    if (mCurrentSelectedObj != null)
                    {
                        mCurrentSelectedObj.SetSelected(true);
                    }
                }
            }
        }
        else
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

            if (mCurrentSelectedObj != null)
            {
                mCurrentSelectedObj.SetSelected(false);
                isMouseOnUI = EventSystem.current.IsPointerOverGameObject();
                if (isMouseOnUI == false)
                {
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
                }
                mCurrentSelectedObj = null;
            }
        }
#endif
    }
    //---------------------------------------------------------------------------------------------
    public void OnSelectableObjClicked(SelectableObj selectedObj)
    {
        if (selectedObj != null)
        {
            if (selectedObj.mSelectType == SelectableObjType.Select_Instance_Entry)
            {
                UIBuild uiBuild = UIMgr.Instance.GetUI(UIBuild.ViewName) as UIBuild;
                if (uiBuild !=  null)
                {
                    //uiBuild.OpenInstanceUI();
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    private void ResetCameraPos(float yawAngle)
    {
        //Quaternion rot = Quaternion.AngleAxis(yawAngle, Vector3.up);
        //mLTransform.SetTRS(new Vector3(mRadius, 0.0f, 0.0f), rot, Vector3.one);
        //Camera.main.transform.RotateAround(mCentrePos.position, Vector3.up, yawAngle);
        if (yawAngle != 0.0f)
        {
            mRecPos.transform.RotateAround(mCentrePos.position, Vector3.up, -yawAngle);
        }
    }
    //---------------------------------------------------------------------------------------------

}
