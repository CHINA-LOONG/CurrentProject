using UnityEngine;
using System.Collections;

public class PetRightParamBase
{
    public GameUnit unit; 
}

public class PetDetailRightBase : MonoBehaviour {

    private UIPetDetail parentNode;
    public UIPetDetail ParentNode
    {
        get
        {
            if (parentNode == null)
            {
                parentNode = transform.GetComponentInParent<UIPetDetail>();
            }
            return parentNode;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="param"></param>
    virtual public void ReloadData(PetRightParamBase param)
    {

    }
}
