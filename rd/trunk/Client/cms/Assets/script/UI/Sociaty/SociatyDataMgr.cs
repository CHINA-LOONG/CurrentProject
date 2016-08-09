using UnityEngine;
using System.Collections;

public class SociatyDataMgr : MonoBehaviour
{
    public int allianceID = 0;
	// Use this for initialization
	void Start ()
    {
	
	}
	
    //打开公会
    public  void    OpenSociaty()
    {
        if(allianceID < 1)
        {
            SociatyList.OpenWith(null);
        }
        else
        {
            SociatyMain.OpenWith();
        }
    }
}
