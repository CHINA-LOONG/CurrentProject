using UnityEngine;
using System.Collections;

public enum SelectableObjType
{
    Select_Instance_Entry,
    Select_Mail,
    Select_Quest,
    
    Num_Select_Type
}
public enum SelectableObjState
{
    State_Normal,
    State_Selected,
    State_Disabled,

    Num_State
}

public class SelectableObj : MonoBehaviour {

    public const string ClickEvent = "SelectedObjClicked";
    public Material mNormalMat;
    public Material mSelectedMat;
    public Color mNormalColor = Color.white;
    public Color mDisableColor = Color.gray;
    public SelectableObjType mSelectType;
    
    //0:noram 1:selected 2:disabed
    public SelectableObjState CurState
    {
        get { return mCurState; }
    }
    private SelectableObjState mCurState = 0;
    private Renderer mCurRenderer;
    
    private bool mLockByGroup = false;

    //---------------------------------------------------------------------------------------------
    void Start()
    {
        mCurRenderer = gameObject.GetComponentInChildren<Renderer>();
        //mCurState = 0;
    }
    //---------------------------------------------------------------------------------------------
    public void SetLockByGroup(bool isLock, bool updateState = true)
    {
        if (mLockByGroup != isLock)
        {
            mLockByGroup = isLock;
            if (updateState == true)
            {
                UpdateStateInternal(mCurState);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void UpdateStateInternal(SelectableObjState state)
    {
        if (mCurRenderer != null)
        {
            switch (state)
            {
                case SelectableObjState.State_Normal:
                    {
                        mCurRenderer.material = mNormalMat;
                        mCurRenderer.material.color = mNormalColor;
                        break;
                    }
                case SelectableObjState.State_Selected:
                    {
                        mCurRenderer.material = mSelectedMat;
                        break;
                    }
                case SelectableObjState.State_Disabled:
                    {
                        mCurRenderer.material = mNormalMat;
                        if (mLockByGroup == true)
                            mCurRenderer.material.color = mNormalColor;
                        else
                            mCurRenderer.material.color = mDisableColor;

                        break;
                    }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetState(SelectableObjState state, bool saveState = true, bool forceRefresh = false)
    {
        if (mLockByGroup == false && mCurState == SelectableObjState.State_Disabled && forceRefresh == false)
            return;
        
        //if (mCurState != state)
        //always update since, the state may not save
        {
            if (saveState == true)
            {
                mCurState = state;
            }
            UpdateStateInternal(state);
        }
    }
    //---------------------------------------------------------------------------------------------
}
