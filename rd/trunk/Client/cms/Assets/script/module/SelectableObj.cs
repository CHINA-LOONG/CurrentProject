using UnityEngine;
using System.Collections;

public enum SelectableObjType
{
    Select_Instance_Entry,
    Select_Mail,
    Select_Quest,
    
    Num_Select_Type
}
public class SelectableObj : MonoBehaviour {

    public const string ClickEvent = "SelectedObjClicked";
    public Material mNormalMat;
    public Material mSelectedMat;
    public SelectableObjType mSelectType;

    private bool mSelected;
    private Renderer mCurRenderer;

    //---------------------------------------------------------------------------------------------
    void Start()
    {
        mCurRenderer = gameObject.GetComponentInChildren<Renderer>();
        mSelected = false;
    }
    //---------------------------------------------------------------------------------------------
    public void SetSelected(bool selected)
    {
        if (mSelected != selected)
        {
            mSelected = selected;
            if (mCurRenderer != null)
            {
                mCurRenderer.material = selected ? mSelectedMat : mNormalMat;
            }
        }
    }
    //---------------------------------------------------------------------------------------------
}
