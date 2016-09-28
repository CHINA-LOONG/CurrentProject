using UnityEngine;
using System.Collections;

public enum SelectableGroupType
{
    Select_Group_Hole,
    Select_Group_Tower,
    Select_Group_Summon,

    Num_Select_Group_Type
}
public class SelectableObjGroup : MonoBehaviour
{
    public SelectableGroupType mType;
    public SelectableObj[] mSelectObjList;
    public Collider mGroupCollider;
    public Material mNormalMat;
    public Material mSelectedMat;
    public Transform mCameraPos;
    public SelectableObjState CurState
    {
        get { return mCurState; }
    }

    private int mSelectObjCount;
    private SelectableObjState mCurState;
    private Renderer mCurRenderer;
    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        mCurRenderer = gameObject.GetComponent<Renderer>();
        mCurState = 0;
        mSelectObjCount = mSelectObjList.Length;
        for (int i = 0; i < mSelectObjCount; ++i)
        {
            mSelectObjList[i].SetLockByGroup(true);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetState(SelectableObjState state, bool ignoreDisable = false)
    {
        if (ignoreDisable == false && mCurState == SelectableObjState.State_Disabled)
            return;

        if (mCurState != state)
        {
            mCurState = state;
            if (mCurState == SelectableObjState.State_Selected)
                mCurRenderer.material = mSelectedMat;
            else
                mCurRenderer.material = mNormalMat;

            for (int i = 0; i < mSelectObjCount; ++i)
            {
                if (mCurState == SelectableObjState.State_Selected)
                    mSelectObjList[i].SetState(SelectableObjState.State_Selected, false);
                else
                    mSelectObjList[i].SetState(SelectableObjState.State_Normal, false);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void SetLookAtState(bool lookat)
    {
        mGroupCollider.enabled = lookat ? false : true;
        for (int i = 0; i < mSelectObjCount; ++i)
        {
            mSelectObjList[i].SetLockByGroup(lookat == false);
        }
    }
    //---------------------------------------------------------------------------------------------
}
