using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SociatyJidiMemberItem : MonoBehaviour
{
    public Text nameText;
    public Text bpText;
    public RectTransform iconParent;

    private MonsterIcon itemIcon = null;

    public static SociatyJidiMemberItem CreateWith()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("SociatyJidiMemberItem");
        SociatyJidiMemberItem memberItem = go.GetComponent<SociatyJidiMemberItem>();
        memberItem.RefreshWith();
        return memberItem;
    }

    // Use this for initialization
	void Start ()
    {
	
	}
    public void RefreshWith()
    {

    }
    
    public void Clear()
    {
        if(null != itemIcon)
        {
            ResourceMgr.Instance.DestroyAsset(itemIcon.gameObject);
        }
    }
}
