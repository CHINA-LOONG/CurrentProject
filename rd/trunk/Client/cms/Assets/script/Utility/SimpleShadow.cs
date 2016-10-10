using UnityEngine;
using System.Collections;

public class SimpleShadow : MonoBehaviour {
    public GameObject mShadowPos;
    public GameObject mShadowProjectPos;
    private float mShadowAutoDistance;
    private float mShadowFixedYPos = 0.0f;
    private bool mShadowAutoMode = true;
    private GameObject mShadowObj;

    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    void Awake ()
    {
        mShadowObj = ResourceMgr.Instance.LoadAsset("simpleShadow");
    }
    //---------------------------------------------------------------------------------------------
    void Start()
    {
        //mShadowPos = Util.FindChildByName(gameObject, "Bip001 Spine");
        //mShadowProjectPos = Util.FindChildByName(gameObject, "Bip001 L Toe0");
        //if (mShadowPos == null || mShadowProjectPos == null)
        //{
        //    Logger.LogError("no Bip001 Spine or Bip001 L Toe0 node");
        //}
        mShadowAutoDistance = mShadowProjectPos.transform.position.y - mShadowPos.transform.position.y;
    }
    //---------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update ()
    {
        if (mShadowObj != null)
        {
            if (mShadowAutoMode == true)
            {
                mShadowObj.transform.position = new Vector3(
                    mShadowPos.transform.position.x,
                    mShadowPos.transform.position.y + mShadowAutoDistance,
                    mShadowPos.transform.position.z
                    );
            }
            else
            {
                mShadowObj.transform.position = new Vector3(
                    mShadowPos.transform.position.x,
                    mShadowFixedYPos,
                    mShadowPos.transform.position.z
                    );
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    //void OnDestroy()
    //{
    //    ResourceMgr.Instance.DestroyAsset(mShadowObj);
    //}
    //---------------------------------------------------------------------------------------------
    public void SetShadowFixedYPos(float yPos)
    {
        mShadowFixedYPos = yPos;
        mShadowAutoMode = false;
    }
    //---------------------------------------------------------------------------------------------
    public void SetShadowScale(Vector3 scale)
    {
        if (mShadowObj != null)
        {
            mShadowObj.transform.localScale = scale;
        }
    }
    //---------------------------------------------------------------------------------------------

}
