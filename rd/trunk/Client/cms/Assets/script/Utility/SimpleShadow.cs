using UnityEngine;
using System.Collections;

public class SimpleShadow : MonoBehaviour {
    public Transform mShadowPos;
    public Transform mShadowProjectPos;
    private GameObject mShadowObj;

    //---------------------------------------------------------------------------------------------
    // Update is called once per frame
    void Update ()
    {
        if (mShadowObj != null)
        {
            mShadowObj.transform.position = new Vector3(
                mShadowPos.position.x,
                mShadowProjectPos.position.y,
                mShadowPos.position.z
                );
        }
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
    public void SetShadowVisible(bool visible)
    {
        if (mShadowObj != null)
        {
            mShadowObj.SetActive(visible);
        }
        else if (visible ==  true)
        {
            mShadowObj = ResourceMgr.Instance.LoadAsset("simpleShadow");
        }

    }
    //---------------------------------------------------------------------------------------------
    public void DestroyShadowObj()
    {
        if (mShadowObj != null)
        {
            ResourceMgr.Instance.DestroyAsset(mShadowObj);
            mShadowObj = null;
        }
    }
    //---------------------------------------------------------------------------------------------

}
