using UnityEngine;
using System;
using System.Collections;

//---------------------------------------------------------------------------------------------
public class SwitchLevelEventArgs : EventArgs
{
    public EnterInstanceParam enterParam;
}

public class SwitchLevelMgr : MonoBehaviour
{
    //---------------------------------------------------------------------------------------------

    static SwitchLevelMgr mInst = null;
    //---------------------------------------------------------------------------------------------
    public static SwitchLevelMgr Instance
    {
        get
        {
            if (mInst == null)
            {
                GameObject go = new GameObject("SwitchLevelMgr");
                mInst = go.AddComponent<SwitchLevelMgr>();
            }
            return mInst;
        }
    }
    //---------------------------------------------------------------------------------------------
}
