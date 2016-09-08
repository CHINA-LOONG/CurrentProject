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
        GameDataMgr.Instance.SociatyDataMgrAttr.GetJidiBaseMonstersAsyn(OnRequestDataFinish);
    }
    void OnRequestDataFinish(List<PB.AllianceBaseMonster> listData)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        Clear();
        List<PB.AllianceBaseMonster> listMonster = GameDataMgr.Instance.SociatyDataMgrAttr.allianceBaseMonster;
        PB.AllianceBaseMonster subMonster = null;
        for(int i = 0;i< listMonster.Count; ++i)
        {
            subMonster = listMonster[i];
            SociatyJidiMemberItem subItem = SociatyJidiMemberItem.CreateWith(subMonster);
            listMembers.Add(subItem);
            scrollView.AddElement(subItem.gameObject);
        }
    }
	
}
