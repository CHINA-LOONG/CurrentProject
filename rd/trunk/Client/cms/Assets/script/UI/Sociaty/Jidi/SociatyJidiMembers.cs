using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class SociatyJidiMembers : MonoBehaviour
{
    public ScrollView scrollView;

    private List<SociatyJidiMemberItem> listMembers = new List<SociatyJidiMemberItem>();

    private static SociatyJidiMembers instance = null;
    public static SociatyJidiMembers Instance
    {
        get
        {
            if(null == instance)
            {
                GameObject go = ResourceMgr.Instance.LoadAsset("SociatyJidiMembers");
                instance = go.GetComponent<SociatyJidiMembers>();
            }
            return instance;
        }
    }
	// Use this for initialization
	void Start ()
    {
	
	}
    public void Clear()
    {
        SociatyJidiMemberItem subItem = null;
        for(int i = 0;i< listMembers.Count;++i)
        {
            subItem = listMembers[i];
            subItem.Clear();
            ResourceMgr.Instance.DestroyAsset(subItem.gameObject);
        }
        listMembers.Clear();
    }

    public void RequestData()
    {
        RefreshUI();
    }
    
    private void RefreshUI()
    {
        Clear();
        //test
        for(int i = 0;i< 60;++i)
        {
            SociatyJidiMemberItem subItem = SociatyJidiMemberItem.CreateWith();
            listMembers.Add(subItem);
            scrollView.AddElement(subItem.gameObject);
        }
    }
	
}
